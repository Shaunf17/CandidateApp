# CandidateApp

CandidateApp is a web application designed to manage candidate information, including personal details and skills. The application is built using Angular for the client-side and .NET 9 for the server-side API.

## Design Choices

### Frontend

- **Framework**: Angular was chosen for its powerful features, including two-way data binding, dependency injection, and a rich ecosystem of libraries and tools.
- **UI Components**: Angular Material is used for UI components to provide a consistent and modern look and feel.
- **Reactive Forms**: Reactive forms are used for form handling to provide better control over form validation and state management.

### Backend

- **Framework**: .NET 9 is used for the backend API to leverage its performance, scalability, and robust ecosystem.
- **Database**: SQL Server is used as the database to store candidate information and skills.
- **Repository Pattern**: The repository pattern is used to abstract data access logic and promote separation of concerns.

## Getting Started

Follow the steps below to get both the backend and frontend up and running.

### 1. Clone the Repository

```bash
git clone https://github.com/Shaunf17/CandidateApp.git
cd CandidateApp
```

### 2. Setup Backend (.NET Web API)

```bash
cd CandidateApp.API
dotnet restore
dotnet build
dotnet run
```

### 3. Setup Frontend (Angular)

```bash
cd CandidateApp.Client
npm install
ng serve

(o + enter to open browser)
```

### 4. Connect Frontend to Backend
Ensure that the Angular frontend is configured to call the correct Web API endpoint. This can be found in `environment.ts`. This default value uses the port declared in `appsettings.json` within `CandidateAppAPI`

```
export const environment = {
  production: false,
  apiUrl: 'https://localhost:7294/api'
};
```
