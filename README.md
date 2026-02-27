# dotnet-bookstore-api

ASP.NET Core Web API for managing a collection of books.

## Features
- CRUD operations for books (Id, Title, Author, YearPublished)
- Database operations via **Entity Framework Core** (service layer + DTOs)
- **Service layer added** to encapsulate business logic and keep controllers thin
- **DTOs (Data Transfer Objects)** to separate API layer from database models
- Pagination and sorting support for large datasets *(not yet implemented)*
- Export book data to Excel and PDF formats *(implemented)*
- Validation using FluentValidation

## Tech Stack
- ASP.NET Core
- C#
- SQL Server
- Entity Framework Core
- FluentValidation
- ClosedXML
- QuestPDF

## Getting Started
1. Clone the repositor<img width="1334" height="590" alt="books_swagger" src="https://github.com/user-attachments/assets/84f5269b-099a-468f-b973-58eedd1604f8" />
y:<img width="1327" height="563" alt="books_swagger_sche<img width="1334" height="570" alt="books_pagination_and_sorting" src="https://github.com/user-attachments/assets/cbb9dce4-08da-4b43-aa6c-f979acdfc93e" />
ma" src="htt<img width="1320" height="585" alt="books_data" src="https://github.com/user-attachments/assets/e1c8c202-5f84-4b58-a1b8-e817c7ee5742" />
ps://github.com/user-attachments/assets/884945ff-df02-4761-9cca-3931a32750c7" />

   ```bash
   git clone https://github.com/prajwalk0/dotnet-bookstore-api.git
