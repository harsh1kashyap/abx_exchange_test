using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

class Packet
{
    public string Symbol { get; set; }
    public string Side { get; set; }
    public int Quantity { get; set; }
    public int Price { get; set; }
    public int Sequence { get; set; }
}

class Program
{
    static async Task Main()
    {
        var outputFilePath = "D:\\AbxFile\\abx_output.json";
        const string host = "127.0.0.1";
        const int port = 3000;
        var packets = new Dictionary<int, Packet>();

        // Request Stream All Packets
        using var client = new TcpClient();
        await client.ConnectAsync(host, port);
        using NetworkStream stream = client.GetStream();
        stream.WriteByte(1);  // callType = 1
        stream.WriteByte(0);  // resendSeq = 0 for callType 1

        byte[] buffer = new byte[17];
        while (true)
        {
            int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
            if (bytesRead == 0) break;

            var packet = ParsePacket(buffer);
            packets[packet.Sequence] = packet;
        }

        // Find missing sequences
        int maxSeq = packets.Keys.Max();
        List<int> missing = new();
        for (int i = 1; i <= maxSeq; i++)
        {
            if (!packets.ContainsKey(i))
                missing.Add(i);
        }

        // Request Resend for Missing Packets
        foreach (int seq in missing)
        {
            using var resendClient = new TcpClient();
            await resendClient.ConnectAsync(host, port);
            using NetworkStream resendStream = resendClient.GetStream();
            resendStream.WriteByte(2); // callType = 2
            resendStream.WriteByte((byte)seq);

            int bytesRead = await resendStream.ReadAsync(buffer, 0, buffer.Length);
            if (bytesRead > 0)
            {
                var packet = ParsePacket(buffer);
                packets[packet.Sequence] = packet;
            }

            resendClient.Close();
        }

        // Export to JSON
        var finalList = packets.OrderBy(p => p.Key).Select(p => p.Value).ToList();
        string jsonOutput = JsonSerializer.Serialize(finalList, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(outputFilePath, jsonOutput);
    }

    static Packet ParsePacket(byte[] data)
    {
        string symbol = Encoding.ASCII.GetString(data, 0, 4);
        string side = Encoding.ASCII.GetString(data, 4, 1);
        int quantity = BitConverter.ToInt32(data[5..9].Reverse().ToArray());
        int price = BitConverter.ToInt32(data[9..13].Reverse().ToArray());
        int sequence = BitConverter.ToInt32(data[13..17].Reverse().ToArray());

        return new Packet
        {
            Symbol = symbol,
            Side = side,
            Quantity = quantity,
            Price = price,
            Sequence = sequence
        };
    }
}

