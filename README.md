# VehicleMarket - Premium Vehicle Marketplace

VehicleMarket is a full-stack web application for buying and selling premium vehicles. It features a robust ASP.NET Core backend and a modern, high-performance Angular frontend.

## Features

- **Authentication**: Secure JWT-based registration and login.
- **Listings Management**: 
  - Browse listings with advanced filtering and search.
  - Detailed vehicle pages with image galleries.
  - CRUD operations for vehicle sellers.
- **Messaging**: Real-time (polling-based) chat between buyers and sellers.
- **Favourites**: Save your favourite vehicles to your personal dashboard.
- **Admin Dashboard**: 
  - Moderate listings (Approve/Reject).
  - Manage users (Suspend/Reactivate).
  - Comprehensive management tools.
- **Premium Design**: Modern, responsive UI with dark mode support and micro-animations.

## Tech Stack

- **Backend**: ASP.NET Core 8.0, Entity Framework Core, SQLite, JWT, BCrypt.
- **Frontend**: Angular 17/18, RxJS, Vanilla CSS (CSS Variables for theming).
- **Communication**: RESTful API.

## Getting Started

### Prerequisites

- .NET 8 SDK
- Node.js (v18 or later)
- Angular CLI

### Backend Setup

1. Navigate to the backend directory:
   ```bash
   cd backend/VehicleMarketplace.Api
   ```
2. Restore dependencies:
   ```bash
   dotnet restore
   ```
3. Run the application (this will also seed the database):
   ```bash
   dotnet run
   ```
   The API will be available at `http://localhost:5217`.

### Frontend Setup

1. Navigate to the frontend directory:
   ```bash
   cd frontend
   ```
2. Install dependencies:
   ```bash
   npm install
   ```
3. Run the development server:
   ```bash
   npm start
   ```
   The application will be available at `http://localhost:4200`.

## Documentation

- [Architecture Overview](ARCHITECTURE.md)
- [API Documentation](API.md)

## License
MIT
