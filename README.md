# 🌦️ Weather Data Web API

A robust **RESTful API** designed for managing and querying weather data.  
Built with an **ASP.NET** backend and powered by a **MongoDB** database, this project ensures efficient data handling alongside secure, token-based authentication.

---

## ✨ Features

### 🔍 Data Operations
- **CRUD Functionality:** Create, Read, Update, and Delete weather records.
- **Advanced Queries:** Filter by:
  - Date
  - Location
  - Temperature range
  - And more!

### 🔒 Authentication
- **Secure Endpoints:** Users authenticate using API keys.
- **Access Levels:** Role-based access control defines endpoint permissions.

### 💾 Database
- Powered by **MongoDB**, optimized for handling large weather datasets and user accounts.

### 📄 Documentation
- Comprehensive API documentation available via **Swagger**.  
- View it here: [API Docs](https://pgnw.github.io/Weather-data-web-API/)

---

## 🛠️ Tech Stack
- **Framework:** ASP.NET
- **Database:** MongoDB
- **Authentication:** API keys/tokens with role-based access control
- **Documentation Tool:** Swagger

---

## 🚀 Getting Started

### ✅ Prerequisites
- **ASP.NET Core SDK** (v6.0.100 or higher)
- **MongoDB** (v3.6 or higher)

### ⚙️ Installation

1. **Clone the Repository**
   ```bash
   git clone https://github.com/pgnw/Weather-data-web-API.git
   cd Weather-data-web-API
   
2. **Configure Database**

Start your MongoDB server/cluster.

Open `appsettings.json` and update the `ConnectionStrings:MongoDb` entry to your MongoDB URI.

## Run the Application

```bash
dotnet run
```
Open your browser to http://localhost:5000/api/swagger to view and test all endpoints via Swagger UI.
