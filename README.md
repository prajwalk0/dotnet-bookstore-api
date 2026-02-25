# dotnet-bookstore-api

ASP.NET Core Web API for managing a collection of books.

## Features
- CRUD operations for books (Id, Title, Author, YearPublished)
- Database operations via **Entity Framework Core** (service layer + DTOs)
- **Service layer added** to encapsulate business logic and keep controllers thin
- **DTOs (Data Transfer Objects)** to separate API layer from database models
- Pagination and sorting support for large datasets *(not yet implemented)*
- Export book data to Excel *(implemented) and PDF formats *(not yet implemented)*
- Validation using FluentValidation

> ⚠️ Note: Pagination, sorting, and PDF export are placeholders for future implementation.

## Tech Stack
- ASP.NET Core
- C#
- SQL Server
- Entity Framework Core
- FluentValidation

## Getting Started
1. Clone the repository:
   ```bash
   git clone https://github.com/prajwalk0/dotnet-bookstore-api.git<img width="1322" height="595" alt="books_swagger" src="https://github.com/user-attachments/assets/c8739736-711d-4d90-bb2a-6194239356e9" />
