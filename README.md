# Microservices Architecture with Azure Container Apps
## 📌 Overview 
This project is a cloud-native application built with ASP.NET technologies, split into several independently deployable services hosted using **Azure Container Apps**. It started as a modular monolith and evolved into a **mini-microservices architecture** for better scalability, CI/CD isolation, and cloud-native readiness.

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
- etc ... (TBD)

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
│ ├── common-domain # Shared domain models, enums, validations
│ ├── common-dto # Shared DTOs across APIs
│ └── common-infra # Caching, logging, db context, etc.
│ └── common-security # Token handling, auth utils, etc.
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

## Future Improvements
Here are some potential enhancements to improve the architecture and operational efficiency:

1. **Centralized Logging**
Use a centralized logging solution like Azure Monitor, Application Insights, or ELK Stack to aggregate logs from all services for better debugging and monitoring.
2. **API Gateway**
Introduce an API Gateway (e.g., Azure API Management or Ocelot) to handle:
Routing requests to the appropriate backend API.
Rate limiting, caching, and request transformation.
Centralized authentication and authorization.
3. **Service-to-Service Communication**
Use Azure Service Bus or Dapr for reliable messaging and service discovery between APIs.
4. **Health Checks**
Implement health check endpoints (e.g., /health) for all services and configure Azure Container Apps to use these for container health monitoring.
5. **Configuration Management**
Use Azure App Configuration or Key Vault to manage environment-specific configurations and secrets securely.
6. **Scalability**
Configure Azure Container Apps for autoscaling based on CPU, memory, or custom metrics to handle varying loads efficiently.
7. **Documentation Enhancements**
Add a sequence diagram or request flow diagram to illustrate how the web app interacts with the APIs.
Include a section on error handling and retry policies for API calls.
8. **Security**
Ensure all APIs are secured with OAuth 2.0 or JWT tokens via Azure Entra ID.
Use HTTPS for all communication, both internally (between services) and externally.
9. **UI Automation Testing**: 
  - Use tools like **Selenium**, **Playwright**, or **Cypress** to automate end-to-end testing of the web app's user interface.
  - Test scenarios should cover:
    - Navigation flows.
    - Form submissions and validations.
    - API integration points (e.g., data displayed from backend APIs).
    - Authentication flows (e.g., login/logout).
  - Store UI test scripts in a dedicated folder, e.g., `/tests/ui`.
10. **CI/CD Enhancements**
Configure GitHub Actions to:
Run tests in parallel with builds.
Push Docker images to Azure Container Registry (ACR).
Deploy to Azure Container Apps automatically after successful builds.
