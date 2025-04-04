# ✅ Developer Pathways - Titanic API Unit Tests

This repository contains the **unit tests** for the [Developer Pathways - Titanic API](https://github.com/RafaPach/TitanicProjectVSTests).  
The tests validate the core functionality of the **CQRS handlers**, **repositories**, and **domain logic** implemented in the main project.

---

## 📝 Overview

The goal of this test suite is to ensure the **reliability**, **accuracy**, and **maintainability** of the Titanic API by covering:

- **Command Handlers** (Create, Update, Delete operations)
- **Query Handlers**
- **Domain Logic**
- **Validation with FluentValidation**
- **Repository methods** (using mock data)

Testing follows **AAA (Arrange-Act-Assert)** principles and uses **mocking** where appropriate to isolate dependencies.

---

## ⚙️ Tech Stack

| Technology        | Purpose            |
|-------------------|--------------------|
| **.NET 8**        | Target framework   |
| **xUnit**         | Unit testing       |
| **Moq**           | Mocking framework  |
| **FluentAssertions** | Better assertions |
| **MSTest** (Optional) | Additional testing framework |

---

## ✅ What’s Covered?

- **CQRS Handlers**  
  - Commands: CreatePassenger, UpdatePassenger, DeletePassenger  
  - Queries: GetPassengerByClass, GetSurvivalRates  
- **FluentValidation** rules  
- **Repository Pattern** logic  
- **Edge cases & exceptions**  
- **Async operations**  
- **Mocked database context (EF Core InMemory / Moq)**

---

## 🛠️ Run Tests Locally

1. Clone the repo  
   ```bash
   git clone https://github.com/RafaPach/TitanicProjectVSTests.git
   cd TitanicProjectVSTests
   ```
   2 Run the project:
    ```bash
    dotnet test
    ```
3. View test results in the Test Explorer or console output.
