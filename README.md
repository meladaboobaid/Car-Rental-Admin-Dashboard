# Car Rental Dashboard

A car rental management dashboard built with .NET 8 WinForms as the client and ASP.NET Core as the backend API.

>This project is a demonstration and learning prototype — not a production-ready application. It was built to practice full-stack development concepts including secure API integration, accessing database, authentication flows, and Windows Forms UI design.

## Functionality

The application provides an admin panel for:

- **Authentication** — login with email/password using JWT access & refresh tokens
- **Dashboard** — displays KPIs (fleet size, active rentals, revenue, growth rate, refund rate)
- **Fleet Management** — view, filter, search, add, and edit vehicles in the fleet
- **Rentals** — view active rentals and top recent rentals
- **Payments** — revenue tracking and refund statistics
- **Visualizations** — donut chart showing fleet status breakdown (Available, Rented, Maintenance, Reserved)

## Tech Stack

### Frontend (Client-Side)
- .NET 8 (Windows Forms)
- Guna.UI2 WinForms & Siticone UI Components

### Backend (Server-Side)
- ASP.NET Core (.NET 8)
- REST API with secure endpoints
- JWT Authentication with auto-refresh
- SQL Server Database
- ADO.NET
- 3-Tier Architecture
- API Authorization & Ownership-Based Access Control
- Rate Limiting
