# ARIB Employee Management System

[Live Demo](http://hrmangement.runasp.net/)

## Overview
ARIB Employee Management System is an ASP.NET Core 9.0 web application built with Razor Pages that allows organizations to manage employees, departments, and tasks. The application is built on a robust, layered architecture.

## Features
- **User Authentication**: Secure login system with role-based authorization (Admin, Manager, Employee)
- **Employee Management**: Add, edit, and delete employee records
- **Department Management**: Organize employees by departments
- **Task Management**: Assign and track tasks for employees
- **Responsive UI**: Modern user interface with responsive design

## Architecture

The application follows a layered architecture pattern which provides several key benefits:

- **Separation of Concerns**: Each layer has a specific responsibility, making the codebase easier to understand and maintain
- **Scalability**: Layers can be scaled independently based on demand
- **Maintainability**: Changes in one layer have minimal impact on other layers
- **Testability**: Cleaner separation makes unit testing more straightforward
- **Flexibility**: Components can be replaced or upgraded with minimal disruption

### Architectural Layers:
1. **Presentation Layer (ARIBApp.Web)**: 
   - User interface components
   - Controllers handling HTTP requests
   - View models and Razor Pages

2. **Service Layer (ARIBApp.Services)**:
   - Business logic implementation
   - Service interfaces and their implementations
   - Application workflow orchestration

3. **Core Layer (ARIBApp.Core)**:
   - Domain models and entities
   - Business rules and constraints
   - Core interfaces and contracts

4. **Infrastructure Layer**:
   - Data access implementation
   - External service integrations
   - Cross-cutting concerns (logging, caching, etc.)

## Prerequisites
- .NET 9.0 SDK
- SQL Server (local or remote)
- Visual Studio 2022 or similar IDE

## Getting Started

### Setup
1. Clone the repository to your local machine
2. Open the solution in Visual Studio 2022
3. Configure the connection string in `appsettings.json` to point to your SQL Server instance

### Database Configuration
**No manual migrations required!** The application is configured to automatically:
- Apply all database migrations at startup
- Seed initial data including:
  - Default departments (HR, IT, Finance)
  - Admin user (username: admin, password: Admin@123)
  - Sample employees and managers
  - Demo tasks

Simply update the connection string in `appsettings.json` and run the application.

---

## Default Users for Testing

| Username    | Password   | Role     | Department               |
|-------------|------------|----------|--------------------------|
| admin       | Admin@123  | Admin    |                          |
| hrmanager   | Hr@123     | Manager  | HR                       |
| itmanager   | It@123     | Manager  | IT                       |
| emp1        | Emp1@123   | Employee |                          |
| emp2        | Emp2@123   | Employee |                          |

- Managers are assigned to different departments: hrmanager → HR, itmanager → IT.
