# EuroSkills 2025 Preliminary Round

## Project setup

### Database

To start the development database use the following steps

Requirements: Node

```bash
cd ./Database
npm install
npm start
```

### Backend

#### Development

You can use a Visual Studio instance to open the `./es2025-s09-r1-100.sln` file.

#### Release

```bash
cd ./Backend
dotnet restore
dotnet run -c Release
```

The server will start up on port `:3001`

**Warning!** An instance of database must be running on `:3000` for correct results.

## Configuration

The program can be configured in a JSON configuration file: `./Backend/appsettings.json`. 

## Architecture


```mermaid
---
title: Services
---
%%{ init: { 'flowchart': { 'curve': 'stepBefore' } } }%%
flowchart LR
    user("User")

    subgraph "Backend services :3001"
        import("Importing service")
        dashboard("Dashboard service")
    end

    database("Database :3000")
    
    user-->|"Import data from structured formats"|import
    user-->|"Overview for container usage"|dashboard
    import-->database
    dashboard-->database
  
```

## Development guide

To follow clean architecture the backend and communication as been split into two separate modules: 

`Backend` and `Shared`. Every data that comes in or goes out from the server must be located in `Shared`.

## Integration guide

When started in Development mode the navigation bar will contain a page for API documentation.

## Disclosure

> Data management (Update, delete) would be a separate service sold separately.

> The project requires active internet connection for correct 3D rendering.