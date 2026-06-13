# 🚌 Bus Ticket Management System (Web Bán Vé Xe)

A professional, enterprise-grade full-stack web application designed to streamline intercity bus ticket bookings, trip scheduling, and fleet operations.

---

## 🛡️ Tech Stack & Badges

![C#](https://img.shields.io/badge/C%23-%23239120.svg?style=for-the-badge&logo=c-sharp&logoColor=white)
![.NET Core](https://img.shields.io/badge/.NET%20Core-512BD4?style=for-the-badge&logo=.net&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQL%20Server-CC2927?style=for-the-badge&logo=microsoft-sql-server&logoColor=white)
![Bootstrap](https://img.shields.io/badge/Bootstrap-7952B3?style=for-the-badge&logo=bootstrap&logoColor=white)
![Angular](https://img.shields.io/badge/Angular-DD0031?style=for-the-badge&logo=angular&logoColor=white)

---

## 📝 About The Project

The **Bus Ticket Management System** (Web Bán Vé Xe) is a robust full-stack web application designed to streamline intercity bus ticket booking and fleet management using the MVC architectural pattern. The system delivers a seamless booking experience for passengers and provides administrators with powerful back-office controls to manage scheduling, pricing, and bus configurations. By leveraging a modern Clean Architecture style on the backend and Angular on the frontend, it ensures long-term maintainability, reliability, and high performance.

---

## ✨ Key Features

*   **🔐 Secure Admin Dashboard**: A comprehensive administrative panel featuring robust authentication, authorization checks, and fleet operations metrics.
*   **🛣️ Route Management (Tuyến xe)**: Complete backend CRUD operations allowing administrators to create, update, and manage bus routes, including starting points, destinations, distances, and intermediate stops.
*   **📅 Dynamic Trip Scheduling (Chuyến xe)**: An advanced schedule module to configure daily and custom trips, assign buses, determine departure times, and set ticket pricing dynamically.
*   **💾 Relational Database Management**: A structured database layout optimized for transactional safety, ensuring atomic, consistent, isolated, and durable (ACID) ticket booking queries to prevent double-booking issues.
*   **🗺️ Interactive Seat Mapping**: A fully interactive seat reservation map supporting multi-deck layouts (Floor 1 and Floor 2) for custom bus fleets (VIP, SleepBus, standard).

---

## 🛠️ Tech Stack & Architecture

### Backend (Clean Architecture)
*   **Framework**: ASP.NET Core MVC / Web API (v9.0)
*   **Language**: C# (.NET Core)
*   **ORM / Database Access**: Entity Framework Core
*   **Database**: Microsoft SQL Server
*   **Authentication**: JSON Web Token (JWT) with BCrypt password hashing

### Frontend
*   **Framework**: Angular 19+ (Standalone Components)
*   **Styling**: HTML5, CSS3, JavaScript, Bootstrap

---

## 🚀 Getting Started

Follow the steps below to clone, configure, and launch the application locally.

### Prerequisites

Ensure you have the following installed on your machine:
*   **.NET SDK 9.0**
*   **Node.js** (v18+) & **npm**
*   **Microsoft SQL Server** (Express or LocalDB)
*   **Angular CLI** (`npm install -g @angular/cli`)

### Installation & Local Setup

1.  **Clone the Repository**
    ```bash
    git clone https://github.com/Hungho09/WebBanVeXe.git
    cd WebBanVeXe
    ```

2.  **Configure Database Connection String**
    Open `Backend/src/Api/appsettings.json` and update the connection string to point to your local SQL Server instance:
    ```json
    "ConnectionStrings": {
      "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=BusTicketDb;Trusted_Connection=True;MultipleActiveResultSets=true"
    }
    ```

3.  **Run Entity Framework Core Migrations**
    Apply the database schema using EF Core commands in the terminal:
    ```bash
    cd Backend/src/Api
    dotnet ef database update
    ```
    *Alternative Setup:* You can also run the direct database seeding script located at `Backend/src/Api/Scripts/Initialize_All_Database.sql` directly inside SQL Server Management Studio (SSMS) to create and populate the database automatically.

4.  **Launch the Backend API**
    ```bash
    dotnet restore
    dotnet run
    ```
    The backend services will be hosted at `http://localhost:5048` by default.

5.  **Launch the Frontend Application**
    Open a new terminal session, navigate to the `Frontend` directory, install dependencies, and start the development server:
    ```bash
    cd Frontend
    npm install
    npm run start
    ```
    The frontend client will be available at `http://localhost:4200`.

---

## 📸 Screenshots

| Page / Component | Interface Preview |
| :--- | :--- |
| **Customer Homepage & Ticket Search** | ![Homepage](https://via.placeholder.com/600x300) |
| **Secure Admin Dashboard** | ![Admin Dashboard](https://via.placeholder.com/600x300) |
| **Route Management Interface (Tuyến xe)** | ![Route Management](https://via.placeholder.com/600x300) |
| **Trip Scheduling Control (Chuyến xe)** | ![Trip Scheduling](https://via.placeholder.com/600x300) |

---

## 💼 My Contributions & Agile Workflow

As a **Full-stack Developer** on this project, I was responsible for delivering the core data engine and business services that power the application:

*   **Relational Database Schema Design**: Architected the relational structure in SQL Server, establishing schema configurations for route segments, scheduled trips, bus assets, seats, and transactional ticket bookings.
*   **Core Backend CRUD Operations**: Implemented API endpoints and services for handling Route (Tuyến xe) creation/modification and Trip (Chuyến xe) scheduling logic, utilizing Entity Framework Core.
*   **Agile Development & Jira Tracking**: Followed agile software development practices, using Jira to break down requirements into specific tasks, trace delivery, and maintain milestones, referencing tickets such as:
    *   `[BVX-34]`: Model design and EF Core migrations for bus routes and station relationships.
    *   `[BVX-58]`: Dynamic trip scheduling engine implementation and concurrency resolution for bus seat reservation transactions.
