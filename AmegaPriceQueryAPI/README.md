
# AmegaPriceQueryAPI

A .NET 8 application for querying FX and Crypto prices from WebSocket Server
and expose the prices to clients through WebSockets and REST APIs.

## Table of Contents
- [Project Specifications](#project-specifications)
- [Design Patterns Used](#design-patterns-used)
- [Frameworks and Tools](#frameworks-and-tools)
- [Getting Started](#getting-started)
  - [Prerequisites](#prerequisites)
  - [Clone the Repository](#clone-the-repository)
  - [Setting Up the Project](#setting-up-the-project)
  - [Running the Project](#running-the-project)
  - [Running Unit Tests](#running-unit-tests)
- [Contact](#contact)

## Project Specifications

- **.NET Version**: .NET 8
- **Core Features**:
  - Real-time price updates via WebSocket
  - REST API for fetching instruments and prices
  - SignalR for WebSocket communication

## Design Patterns Used 

- **Dependency Injection**: Achieved using built-in .NET Core DI.
- **Singleton Pattern**: Used in service registrations for single-instance services.
- **Observer Pattern**: Implemented with events for real-time updates.

## Frameworks and Tools

- **ASP.NET Core**: For building the web API.
- **xUnit**: For unit testing.
- **SignalR**: For WebSocket communication.
- **Swagger**: For API documentation.
- **Newtonsoft.Json**: For JSON serialization and deserialization.

## Getting Started

### Prerequisites

- [**.NET SDK 8**](https://dotnet.microsoft.com/download/dotnet/8.0).
-  [**Visual Studio Code**](https://code.visualstudio.com/).
- [**Git**](https://git-scm.com/).

### Clone the Repository

1. Open your terminal.
2. Navigate to the directory where you want to clone the project.
3. Run the following command to clone the repository:
   ```
   git clone https://github.com/ayodeko/AmegaPriceQueryAPI.git
   ```
4. Navigate into the project directory:
   ```
   cd AmegaPriceQueryAPI
   ```

### Setting Up the Project

1. **Open the project in VS Code**:
   ```
   code .
   ```

2. **Restore dependencies**:
   Open the integrated terminal in VS Code and run:
   ```
   dotnet restore
   ```

3. **Build the project**:
   ```
   dotnet build
   ```

### Running the Project

1. **Run the project**:
   ```
   dotnet run --project AmegaPriceQuery.API
   ```
   This will start the web server. You should see output indicating that the application is running and listening on a port.

2. **Access the Swagger UI**:
   Open your web browser and navigate to \`https://localhost:7181/swagger/index.html` to view and test the API endpoints.

### Connect to the SignalR WebSocket using Postman

To connect to the WebSocket using Postman, follow the steps outlined in the [WebSocketWithPostman.md](CodeDocumentation/WebSocketWithPostman.md) guide.

### Running Unit Tests

1. **Run all tests**:
   In the integrated terminal, run:
   ```
   dotnet test
   ```
   This will execute all unit tests and display the results in the terminal.

## Contact ✉️

For any questions or inquiries, please reach out to the project maintainer at [akindekoayooluwa@gmail.com](mailto:akindekoayooluwa@gmail.com).

Happy coding! 😊
