# FPT University Admissions Counseling System - C# Microservices

![Microservices](https://img.shields.io/badge/Architecture-Microservices-blue)
![.NET](https://img.shields.io/badge/.NET-9.0-purple)
![MongoDB](https://img.shields.io/badge/Database-MongoDB-green)
![PostgreSQL](https://img.shields.io/badge/Database-PostgreSQL-blue)
![Docker](https://img.shields.io/badge/Container-Docker-blue)
![RabbitMQ](https://img.shields.io/badge/MessageBroker-RabbitMQ-orange)

A FPT University admissions counseling system built with microservices architecture using C# and .NET. The system allows prospective students to schedule consultations with admission counselors, submit application documents, make program-related payments, and track their application process.

## System Architecture

The system is divided into several independent microservices:

- **AuthService**: User authentication and authorization using OpenIddict
- **AppointmentService**: Consultation appointment management with counselors
- **PaymentService**: Application fee and program payment processing
- **DocumentService**: Application documents and record management
- **SystemFeedbackService**: Counseling session ratings and feedback management
- **ScheduleService**: Counselor work schedule management
- **RequestTicketService**: Admission inquiry and support request management

### Reverse Proxy

The system utilizes a Reverse Proxy (implemented with YARP - Yet Another Reverse Proxy) to:
- Provide a unified entry point to all microservices
- Handle routing requests to appropriate services
- Enable service discovery
- Add an additional security layer by hiding internal service structure
- Simplify client interactions with the system

The Reverse Proxy is configured to route traffic based on paths and can be extended to include authentication, rate limiting, and load balancing as needed.

## Technologies Used

- **Backend**: C# (.NET 9.0)
- **Databases**: 
  - MongoDB (NoSQL)
  - PostgreSQL (SQL)
- **Message Broker**: RabbitMQ
- **Container**: Docker
- **API Documentation**: Swagger

## Project Structure

Each microservice is organized according to Clean Architecture pattern:

- **API Layer**: Handles HTTP requests, API endpoints
- **Application Layer**: Business logic, commands and queries (CQRS)
- **Domain Layer**: Entities, value objects, domain services
- **Infrastructure Layer**: Repositories, external services integrations

## System Requirements

- .NET SDK 7.0+
- Docker and Docker Compose
- MongoDB
- PostgreSQL
- RabbitMQ
- Yarp

## Installation and Execution

### Using Docker

```bash
# Clone repository
git clone [repository-url]
cd Counseling-Microservices-CSharp

# Start services with Docker Compose
docker-compose up
```

### Running Individual Services

```bash
# Run AuthService
cd AuthService.API
dotnet run

# Run AppointmentService
cd ../AppointmentService.API
dotnet run

# Similarly for other services
```

## API Endpoints

After the system is started, you can access the API documentation at:

- AuthService: http://localhost:5050/swagger
- AppointmentService: http://localhost:5051/swagger
- PaymentService: http://localhost:5052/swagger
- And other services...

## Configuration

Each service has its own configuration files in the root directory of the service (appsettings.json, appsettings.Development.json). You can adjust configuration parameters according to your deployment environment.
