
# 🧪 ABX Exchange Client (C#)

This is a C# console application designed to interact with a Node.js-based ABX Mock Exchange Server over TCP. The client requests and collects all stock ticker packets, ensures none are missing by requesting resends, and finally generates a JSON output file with the complete data.

---

## 📦 Requirements

- [.NET SDK 8.0]
- [Node.js v16.17.0 or higher]
- Git (for cloning the repo)

---

## 🚀 Getting Started

### 1. Clone this repository
```bash
git clone https://github.com/harsh1kashyap/abx_exchange_test.git
cd abx_exchange_test
```

### 2. Start the ABX Exchange Server

1. Download and unzip the `abx_exchange_test` file provided to you.
2. Navigate into the extracted folder "nodeserver".
3. Run the following command to start the server:
   ```bash
   node main.js
   ```
4. The server will start on TCP port **3000**.

### 3. Run the C# Client

1. Navigate into the client folder (if not already there):
   ```bash
   cd abx_exchange_test
   ```

2. Build the project:
   ```bash
   dotnet build
   ```

3. Run the client:
   ```bash
   dotnet run
   ```

---

## 📁 Output

Once the program finishes running, it creates a file called:

```
abx_output.json
```

You can replace the file directory with your own.

---

## 📄 JSON Output Format

The final JSON file contains an array of objects with the following format:

```json
[
  {
    "Symbol": "MSFT",
    "Side": "B",
    "Quantity": 100,
    "Price": 123456,
    "Sequence": 1
  },
  {
    "Symbol": "AAPL",
    "Side": "S",
    "Quantity": 50,
    "Price": 654321,
    "Sequence": 2
  }
]
```

---

## 🔧 Features

- ✅ Connects to ABX exchange server via TCP
- ✅ Sends stream-all-packets request
- ✅ Parses binary response with big endian byte order
- ✅ Detects and requests missing packet sequences
- ✅ Exports all packets to well-formatted JSON

---

## 🧠 Notes

- Always start the ABX Node.js server **before** running the client.
- The client gracefully handles disconnections and missing data.
