# dotnet-bookstore-api

ASP.NET Core Web API for managing a collection of books.

## Features
- CRUD operations for books (Id, Title, Author, YearPublished)
- Database operations via Entity Framework Core (service layer + DTOs)
- **Service layer added** to encapsulate business logic and keep controllers thin
- **DTOs (Data Transfer Objects)** to separate API layer from database models
- Pagination and sorting support for large datasets
- Export book data to Excel and PDF formats
- Validation using FluentValidation

> ⚠️ Features like pagination, sorting, Excel/PDF export are not yet implemented in this branch.
> 
## Tech Stack
- ASP.NET Core
- C#
- SQL Server
- Entity Framework Core
- FluentValidation

## Getting Started
1. Clone the repository:
   ```bash
   git clone https://github.com/prajwalk0/dotnet-bookstore-api.git
