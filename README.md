# Microservices Architecture with Azure Container Apps
## 📌 Overview 
This project is a cloud-native application built with ASP.NET technologies, split into several independently deployable services hosted using Azure Container Apps. It started as a modular monolith and evolved into a mini-microservices architecture for better scalability, CI/CD isolation, and cloud-native readiness.

The solution consists of:

- ### Web App  (client-side UI, authenticated with Azure Entra ID)

- ### APIs:
  - todo-api
  - money-manager-api
  - chinook-api

### Secure Dev Practices:
- Authentication & Authorization via Azure Entra ID
- Secure cookie-based session
- Path-based routing or API Gateway integration
- TBA

## 🧱 Architecture Diagram
Here’s a high-level system architecture:


<sup>Note: You may customize this further with App Gateway, Dapr, or API Management as needed.</sup>

```
                ┌────────────────────────┐
                │      Azure Entra ID    │
                └──────────┬─────────────┘
                           │
                           ▼
                ┌────────────────────────┐
                │    Web App (Client)    │ ◄── Secure cookie, auth
                │  (e.g. ASP.NET MVC)    │
                └──────────┬─────────────┘
                           │ Calls secured APIs
            ┌──────────────┼─────────────────────────┐
            ▼              ▼                         ▼
   ┌────────────────┐  ┌───────────────┐       ┌────────────────┐
   │ Container App: │  │ Container App:│       │ Container App: │
   │   ToDo API     │  │ Chinook API   │       │ Money Manager  │
   └────────────────┘  └───────────────┘       └────────────────┘
           ▲                    ▲                        ▲
           │                    │                        │
        CI/CD                CI/CD                    CI/CD
   (GitHub Actions)    (GitHub Actions)         (GitHub Actions)

       [Optional: Azure API Management or Azure Front Door]


```

## 📁 Folder Structure
```
/your-solution
│
├── /infrastructure
│ ├── bicep # IaC for Azure Container Apps, network, etc.
│ └── scripts # Utility scripts for deployment, init, cleanup
│
├── /apps
│ ├── web-app # ASP.NET Razor or MVC web client
│ ├── todo-api # To-Do API (Dockerized ASP.NET Minimal API)
│ ├── chinook-api # Chinook API (EF Core + Docker)
│ └── money-manager-api # Money manager domain API
│
├── /shared
│ ├── Auth # Shared auth middleware, helpers
│ ├── Contracts # DTOs and shared models
│ └── Utils # Logging, caching, helpers
│
├── /docker
│ ├── web-app.Dockerfile
│ ├── todo.Dockerfile
│ ├── chinook.Dockerfile
│ └── money-manager.Dockerfile
│
├── /.github
│ └── workflows # CI/CD pipelines per service (GitHub Actions)
│
└── README.md
```

## 🚀 **Getting Started**

🔧 Prerequisites
- .NET 8 SDK
- Docker
- Azure CLI
- GitHub Actions for CI/CD

▶️ Running Locally (with Docker Compose)

``` 
docker compose up --build
```
## 🧪 Testing Strategy

#### 🔧 **Unit Testing**

- **Framework**: [xUnit](https://xunit.net/) is used for unit testing.
- **Test Libraries**: 
  - [Shouldly](https://shouldly.github.io/) for assertions.
  - Moq for mocking dependencies.
- **Test Coverage**: Focuses on testing core business logic, service methods, and API controllers.
- **Location**: Unit tests are located in the `/tests/unit` folder for each API module (e.g., `todo-api`, `money-manager-api`).

#### 🧑‍💻 **Integration Testing**

- **Framework**: Integration tests are written in **xUnit** as well.
- **Test Containers**: We use [TestContainers](https://testcontainers.org/) to provide real Docker containers for testing environments (e.g., databases).
- **Scope**: Integration tests are used to verify the interaction between services (e.g., API and database).
- **Location**: Integration tests are located in the `/tests/integration` folder.

