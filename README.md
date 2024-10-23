
# PriceQueryAPI

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
  - [Connect to the SignalR WebSocket using Postman](#connect-to-the-signalr-websocket-using-postman)
  - [Running Unit Tests](#running-unit-tests)
- [Management of WebSocket over 1000 Subscribers](#management-of-websocket-over-1000-subscribers)
- [Contact](#contact)


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
   git clone https://github.com/ayodeko/PriceQueryAPI.git
   ```
4. Navigate into the project directory:
   ```
   cd PriceQueryAPI
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
   dotnet run --project PriceQuery.API
   ```
   This will start the web server. You should see output indicating that the application is running and listening on a port.

2. **Access the Swagger UI**:
   Open your web browser and navigate to \`https://localhost:7181/swagger/index.html` to view and test the API endpoints.

### Connect to the SignalR WebSocket using Postman

To connect to the WebSocket using Postman, follow the steps outlined in the [WebSocketWithPostman.md](PriceQueryAPI/CodeDocumentation/WebSocketWithPostman.md) guide.

### Running Unit Tests

1. **Run all tests**:
   In the integrated terminal, run:
   ```
   dotnet test
   ```
   This will execute all unit tests and display the results in the terminal.

## Management of WebSocket over 1000 Subscribers

The system is designed to efficiently manage over 1,000 WebSocket subscribers using the following strategies:

1. **Concurrent Subscription Management**:
    - The `SubscriptionManager` class uses a `ConcurrentDictionary` to manage subscriptions, ensuring thread-safe operations.

2. **Single WebSocket Connection**:
    - Maintains a single connection to the data provider in `SubscriptionManager`.

3. **Event-Driven Broadcasting**:
    - The `BroadcastPrice` method in `SubscriptionManager` uses events to efficiently notify all subscribers of price updates.

4. **Background Message Processing**:
    - Messages from the data provider are processed in the background by `SubscriptionManager`.

### Scaling with SignalR and Redis

SignalR is chosen for WebSocket communication due to its scalability features. It can be easily integrated with Redis for horizontal scaling to support millions of users.

- **Redis Backplane**:
    - Using Redis with SignalR allows messages to be distributed across multiple servers, ensuring all clients receive updates.

Refer to the code comments in `SubscriptionManager.cs` for more details.

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
- 
## Contact   
✉️

For any questions or inquiries, please reach out to the project maintainer at [akindekoayooluwa@gmail.com](mailto:akindekoayooluwa@gmail.com).

Happy coding! 😊
