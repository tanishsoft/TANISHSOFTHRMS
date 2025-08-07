
# .NET Framework SQL Reader

A .NET Framework application that connects to a Microsoft SQL Server database, retrieves data using SQL queries, and displays the results in a simple and extensible format.

## 🚀 Features

- Connects to SQL Server using `System.Data.SqlClient`
- Executes SQL queries
- Displays results in console or UI (depending on implementation)
- Configurable connection string
- Modular and easy to extend

## 🧰 Technologies Used

- .NET Framework (version 4.8)
- C#
- SQL Server
- Visual Studio (recommended)

## 📂 Project Structure

```
/SqlReaderApp
│
├── App.config              # Configuration file with SQL connection string
├── Program.cs              # Main entry point
├── SqlService.cs           # Handles all SQL operations
├── Models/                 # Optional: Contains data models
└── README.md               # This file
```

## ⚙️ Getting Started

### Prerequisites

- Windows OS
- Visual Studio 2019 or later
- .NET Framework (4.7.2 or compatible)
- SQL Server (local or remote)

### Installation

1. **Clone the repository:**

   ```bash
   git clone https://github.com/tanishsoft/TANISHSOFTHRMS
   cd your-repo-name
   ```

2. **Open in Visual Studio:**

   Open the `.sln` file in Visual Studio.

3. **Configure the connection string:**

   In `App.config`, edit the connection string to point to your SQL Server:

   ```xml
   <connectionStrings>
     <add name="DefaultConnection" 
          connectionString="Data Source=.;Initial Catalog=YourDatabase;Integrated Security=True;" 
          providerName="System.Data.SqlClient" />
   </connectionStrings>
   ```

4. **Build and Run:**

   Press `F5` to build and run the application.

## 🧪 Usage

Once the application is running, it will:

- Connect to the database using the connection string
- Execute predefined SQL queries (or accept input)
- Display results in the console or UI

Example SQL Query in `SqlService.cs`:

```csharp
string query = "SELECT * FROM Users";
```

## 🔒 Security Note

**Never hardcode credentials.** Use environment variables or secure app settings when deploying to production.

## 🤝 Contributing

Contributions are welcome! Please:

1. Fork the repo
2. Create your feature branch (`git checkout -b feature/my-feature`)
3. Commit your changes (`git commit -m 'Add some feature'`)
4. Push to the branch (`git push origin feature/my-feature`)
5. Create a pull request

## 📄 License

This project is licensed under the MIT License. 

