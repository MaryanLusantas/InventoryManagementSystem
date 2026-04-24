# Inventory Management System

A **CLI-based Inventory Management System** built with **C# (.NET 8)** using Object-Oriented Programming principles.

## Features

| Feature | Description |
|---|---|
| Login System | Multi-user authentication with role-based access |
| Category Management | Add and view product categories |
| Supplier Management | Add and view suppliers |
| Product Management | Add, view, search, update, and delete products |
| Stock Operations | Restock products or deduct stock |
| Reports | Low stock alerts, total inventory value, summary |
| Transaction History | Full audit trail of all inventory changes |
| User Management | Admin can manage system users |

## Project Structure

```
InventoryManagementSystem/
├── Models/
│   ├── Category.cs          # Category model
│   ├── Supplier.cs          # Supplier model
│   ├── Product.cs           # Product model with stock logic
│   ├── User.cs              # User model with role-based access
│   └── TransactionRecord.cs # Audit trail model
├── Services/
│   ├── InventoryService.cs  # Core business logic layer
│   └── UIHelper.cs          # Console UI formatting utilities
├── Program.cs               # Entry point & all menu-driven CLI logic
└── InventoryManagementSystem.csproj
```

## OOP Concepts Used

- **Classes & Objects** — 5 models + 2 service classes
- **Constructors** — All models use parameterized constructors
- **Properties** — Public/private properties with getters and setters
- **Encapsulation** — Private backing fields, controlled access via methods
- **Access Modifiers** — `public`, `private`, `static`, `readonly`
- **Methods** — Business logic separated into focused methods
- **Exception Handling** — `try-catch` blocks with custom messages throughout

## Default Login Credentials

| Username | Password | Role |
|---|---|---|
| admin | admin123 | Admin |
| manager1 | manager123 | Manager |

## How to Run

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download)

### Steps

```bash
# Clone the repository
git clone https://github.com/YOUR_USERNAME/InventoryManagementSystem.git
cd InventoryManagementSystem

# Run the project
dotnet run
```

## Menu Overview

```
Main Menu
├── 1. Category Management
│   ├── View All Categories
│   └── Add New Category
├── 2. Supplier Management
│   ├── View All Suppliers
│   └── Add New Supplier
├── 3. Product Management
│   ├── View All Products
│   ├── Add New Product
│   ├── Search Products
│   ├── Update Product
│   └── Delete Product
├── 4. Stock Operations
│   ├── Restock Product
│   └── Deduct Stock
├── 5. Reports & Analytics
│   ├── Low Stock Alert
│   ├── Total Inventory Value
│   └── Inventory Summary
├── 6. Transaction History
├── 7. User Management (Admin only)
└── 0. Logout
```

## Developer
Maryan Lusantas
BS Information Technology
