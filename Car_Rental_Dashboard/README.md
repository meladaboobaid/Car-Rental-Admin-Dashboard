# Car Rental Dashboard

> **⚠️ Demo / MVP Notice**  
> This project is a demonstration and minimum viable product. Several features are not fully implemented (e.g., vehicle removal, edit-mode saving, rentals/customers/payments/reports tabs are UI placeholders only). It is intended as a starting point, not a production-ready application.

A Windows Forms (.NET 8) desktop application that serves as an administrative panel for managing a car rental business. It communicates with a backend REST API to provide fleet management, rental tracking, payment monitoring, and reporting.

---

## Tech Stack

- **.NET 8.0** (Windows Forms)
- **Guna.UI2 WinForms** — modern UI controls (rounded buttons, text boxes, panels, tab controls)
- **Siticone.NetCore UI** — chart components (donut chart for fleet status)
- **Newtonsoft.Json** — JSON serialization/deserialization
- **WinForms.DataVisualization** — native charting support
- **System.Net.Http** — HTTP client for REST API communication
- **JWT (Bearer)** — token-based authentication with automatic refresh

---

## Architecture

The application follows a three-layer architecture:

```
┌─────────────────────┐
│    UI Forms         │  WinForms (frmLogin, frmAdminDashboard, etc.)
├─────────────────────┤
│   APIs Client       │  HTTP client classes (CarsAPI, UsersAPIs, etc.)
├─────────────────────┤
│   DTOs              │  Data Transfer Objects (Auth, Business)
├─────────────────────┤
│   Backend REST API  │  External ASP.NET Core API (localhost:7035)
└─────────────────────┘
```

The app is a **frontend-only** client. All business logic and data persistence live on the backend API.

---

## Project Structure

```
Car_Rental_Dashboard/
├── Program.cs                  # Application entry point — launches frmLogin
├── Global.cs                   # Static global state (current logged-in user)
├── LoggedInUser.cs             # Model for the authenticated user session
│
├── UI Forms/                   # WinForms UI
│   ├── frmLogin.cs             # Login form with credentials & JWT auth
│   ├── frmAdminDashboard.cs    # Main dashboard (7 tabs, KPIs, charts, grids)
│   ├── frmAddEditVehicle.cs    # Add / Edit vehicle form
│   └── frmRemoveVehicle.cs     # Remove vehicle form (incomplete)
│
├── APIs Client/                # REST API client layer
│   ├── UsersAPIs.cs            # Auth: login, token refresh, HTTP client factory
│   ├── CarsAPI.cs              # Fleet CRUD + fleet queries
│   ├── CarTypesAPIs.cs         # Car category management
│   ├── CompaniesAPIs.cs        # Company management
│   ├── RentalsAPIs.cs          # Rental operations (active rentals, counts)
│   └── PaymentsAPIs.cs         # Payment/revenue queries (growth, totals, refunds)
│
├── DTOs/                       # Data Transfer Objects
│   ├── Auth/                   # Auth-related DTOs
│   │   ├── LoginRequest.cs
│   │   ├── LogoutRequest.cs
│   │   ├── RefreshRequest.cs
│   │   ├── TokenResponse.cs
│   │   └── TokenState.cs
│   └── Business/               # Business entity DTOs
│       ├── CarDTO.cs
│       ├── CarTypeDTO.cs
│       ├── CompanyDTO.cs
│       ├── FleetDTO.cs
│       ├── RentalDTO.cs
│       └── UserDTO.cs
│
├── Properties/                 # Assembly resources & settings
├── Resources/                  # Images & icons (dashboard animations, icons)
└── Car_Rental_Dashboard.csproj # Project file
```

---

## Features

### Authentication
- Login with email/password
- JWT access token + refresh token flow
- Automatic token refresh on 401 responses (via `SendWithAutoRefreshAsync`)
- Token state persisted in-memory via `Global.CurrentUser`

### Dashboard (Main Tab)
- **KPI cards**: fleet size, active rentals (this week vs last week), availability rate, monthly/total revenue, growth percentage, refund rate
- **Fleet status donut chart** — visual breakdown of Available / Rented / Maintenance / Reserved vehicles
- **Top 5 active rentals** — latest rental activity in a DataGridView

### Fleet Management
- View full fleet in a filterable, sortable DataGridView
- Filter by vehicle status (Available, Rented, Maintenance, Reserved)
- Search by brand, model, or license plate
- Add new vehicles via a dedicated form
- Edit existing vehicle details

### Additional Tabs (UI placeholders)
- Rentals, Customers, Payments, Reports, Settings — tab switching is wired up with UI panels ready for implementation

### UI Polish
- Rounded window corners via Win32 `CreateRoundRectRgn`
- Rounded DataGridView corners
- Custom-colored donut chart
- Password visibility toggle
- Modern flat design with Guna UI controls

---

## Authentication Flow

1. User enters credentials → `POST /api/Auth/login`
2. Server returns `{ accessToken, refreshToken }`
3. All subsequent API calls include `Authorization: Bearer <accessToken>`
4. If a call returns `401 Unauthorized`:
   - Client sends `POST /api/Auth/refresh` with the refresh token
   - On success, new tokens are stored and the original request is retried
   - On failure, the user is prompted to re-login

---

## Setup & Running

### Prerequisites
- .NET 8 SDK
- Backend REST API running at `https://localhost:7035/` (configurable in `UsersAPIs.BaseUrl`)

### Run the application
```bash
cd Car_Rental_Dashboard
dotnet run
```

### Connection
The URL is defined in `APIs Client/UsersAPIs.cs:17`:
```csharp
public const string BaseUrl = "https://localhost:7035/";
```

The HTTP client is pre-configured to accept self-signed certificates for local development.

---

## Notes

- `frmRemoveVehicle` is a skeleton — the removal API call is not yet wired up
- `frmAddEditVehicle` edit mode reads vehicle data but saving edits is not yet implemented
- The app does not persist any data locally; all operations require the backend API
