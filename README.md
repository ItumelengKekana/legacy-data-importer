# LegacyDataImporterAPI

# Running migrations

The migration files (Infrastructure/Migrations) and generated sql script (migration-script.sql) should be available in the project

NB! make sure dotnet ef is installed (dotnet tool install --global dotnet-ef)
If already installed, make sure it's up to date (dotnet tool update --global dotnet-ef)

1. modify the connection string in appsettings.json to match your local db
2. run dotnet ef database update --project Infrastructure --startup-project LegacyDataImporterAPI
3. Your local db should be updated with the relevant tables

# Running instructions

1. Clone the repository to your desired directory
2. Open the project: Double-click the .sln (Solution) or .csproj file to open it in Visual Studio
3.Select the profile: Locate the Run/Play button at the top toolbar and click the small dropdown arrow next to it. Select your project name rather than "IIS Express" for a cleaner terminal-based log experience.Launch: Press F5 (to run with debugging) or Ctrl + F5 (to run without debugging). Make sure to run this in http mode
4.A browser window should automatically open to the API's default homepage or documentation page

# Request/Response Objects

# /Customer/import-legacy-data
Request: File
Response: {
  "batchId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "processed": 0,
  "created": 0,
  "updated": 0,
  "failed": 0
}

# /Customer/create-order
Request:  {
  "customerId": 0,
  "legacyCustomerId": "string",
  "orderDate": "2026-08-21T14:13:38.357Z",
  "currency": "string",
  "status": "string",
  "items": [
    {
      "description": "string",
      "sku": "string",
      "unitPrice": 0,
      "quantity": 0
    }
  ]
}

Response: {
  "success": true,
  "message": "string"
}

# /Customer/GetOrderById
Request: Id
Response: {
  "id": 0,
  "customerId": 0,
  "customerName": "string",
  "orderDate": "2026-08-21T14:13:38.423Z",
  "currency": "string",
  "status": "string",
  "items": [
    {
      "id": 0,
      "description": "string",
      "sku": "string",
      "unitPrice": 0,
      "quantity": 0,
      "lineTotal": 0
    }
  ],
  "totalAmount": 0
}

# /Customer/order-summary
Request: fromDate: string, toDate: string
Response: [
  {
    "customerId": 0,
    "legacyCustomerId": "string",
    "fullName": "string",
    "email": "string",
    "totalOrders": 0,
    "totalSpent": 0
  }
]
