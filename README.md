# Nadwa Event Booking System

Nadwa is a full-stack event booking system that allows users to browse and book events, manage their bookings, and provides an integrated web-based admin panel for event management.

## Prerequisites

To run this project, you will need the following:

1. **RIDER** (Recommended) or **Visual Studio (VS)** with .NET 9.0
2. **Docker Desktop** (Optional, for Redis caching)

## Setup Instructions

### 1. Download Project Files

Clone or download the project files to your local machine.

### 2. Setup and Run the Project

#### For Rider (Recommended):
1. Open the project in Rider.
2. Press `Alt + F12` to open the terminal in Rider.
3. Run the following commands:
   - `dotnet ef migrations add InitialCreate --project "Nadwa"`
   - `dotnet ef database update --project "Nadwa"`

#### For Visual Studio:
1. Open the project in Visual Studio.
2. Go to `Tools > NuGet Package Manager > Package Manager Console`.
3. Run the following commands:
   - `Add-Migration "InitialCreate"`
   - `Update-Database`

### 3. Redis Caching (optional)

### 4. Login

You can log in using the following default accounts:

- **Admin Account:**
  - Email: `admin@areeb.com`
  - Password: `Admin_123`

- **User Account:**
  - Email: `user@areeb.com`
  - Password: `User_123`

Alternatively, you can create a new user account if you prefer.



