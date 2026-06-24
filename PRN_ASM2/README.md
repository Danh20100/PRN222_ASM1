<h1 align="center">📚 RAG LMS Project - PRN222 Assignment 2</h1>

<p align="center">
  <strong>An advanced Learning Management System (LMS) integrated with Retrieval-Augmented Generation (RAG) capabilities.</strong>
</p>

## 🌟 Overview

This project is built using **ASP.NET Core Razor Pages** and follows a clean **N-Tier Architecture**. It integrates AI capabilities to allow users to interact with course documents, ask questions, and receive AI-generated answers with citations based on the uploaded materials.

The application supports multiple AI models including **Gemini, OpenAI, HuggingFace**, and local models via **Ollama**.

## 🏗️ Architecture

The project is structured into three main layers to ensure separation of concerns and maintainability:

1. **`PRN222_Assignment2` (Presentation Layer)**
   - ASP.NET Core Razor Pages web application.
   - Handles UI, routing, user interactions, and dependency injection configuration.

2. **`BusinessLayer` (Service Layer)**
   - Contains the core business logic.
   - Orchestrates data operations between the Data Access Layer and the Presentation Layer.
   - Handles AI interactions, document chunking, and embedding generation strategies.

3. **`DataAccessLayer` (Data Access Layer)**
   - Entity Framework Core Database Context (`RagLmsDb`).
   - Implements the Repository Pattern for data access.
   - Contains domain entities: `User`, `Subject`, `Document`, `ChatSession`, `ChatHistory`, `ChatCitation`, `AiModel`, `EmbeddingModel`, etc.
   - Handles database migrations.

## ✨ Key Features

- **User & Subject Management**: Manage users, teachers, and subjects.
- **Document Management**: Upload and manage course materials (documents, chapters).
- **RAG Integration**: 
  - Document chunking and vector embeddings indexing.
  - Search and retrieve relevant document chunks.
- **AI Chat Interface**: 
  - Interactive chat sessions with AI.
  - Context-aware responses with citations linking back to original documents.
- **Multiple LLM Support**: Configurable API integrations for Gemini, OpenAI, HuggingFace, and Ollama.

## 🚀 Getting Started

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download) (or compatible version)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) (recommended)
- SQL Server LocalDB (included with Visual Studio)

### Installation & Setup

1. **Clone the repository** (if you haven't already):
   ```bash
   git clone <repository-url>
   cd PRN_ASM2
   ```

2. **Open the Solution**:
   Open `PRN222_Assignment2.sln` using Visual Studio.

3. **Configure API Keys**:
   Navigate to `PRN222_Assignment2/appsettings.json` and update the `ApiKeys` section with your own keys if you plan to use AI features.
   ```json
   "ApiKeys": {
     "Gemini": "YOUR_GEMINI_API_KEY",
     "HuggingFace": "",
     "OpenAI": "",
     "OllamaBaseUrl": "http://localhost:11434"
   }
   ```

4. **Apply Database Migrations**:
   Open the **Package Manager Console** (Tools > NuGet Package Manager > Package Manager Console), set the default project to `DataAccessLayer`, and run:
   ```powershell
   Update-Database
   ```
   *Alternatively, you can run `dotnet ef database update --project DataAccessLayer --startup-project PRN222_Assignment2` from the terminal.*

5. **Run the Application**:
   Press `F5` in Visual Studio or run the following command in the terminal:
   ```bash
   dotnet run --project PRN222_Assignment2/PRN222_Assignment2.csproj
   ```

## 🛠️ Tech Stack

- **Framework**: .NET 8.0 / ASP.NET Core Razor Pages
- **Database ORM**: Entity Framework Core
- **Database**: MS SQL Server (LocalDB)
- **AI & ML**: Custom RAG implementation (Retrieval-Augmented Generation)

## 📝 License

This project is created for educational purposes as part of the PRN222 course.
