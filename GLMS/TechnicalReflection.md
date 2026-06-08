\# GLMS Technical Reflection Report

\## TechMove Global Logistics Management System - Part 3

\### PROG7311 - 2026



\---



\## 1. Automated Testing in CI/CD Pipelines



\### Why Automated Testing is Critical



In a CI/CD (Continuous Integration / Continuous Deployment) pipeline,

automated testing acts as a mandatory quality gate. Every time a

developer pushes code to the GitHub repository, the pipeline

automatically triggers the full test suite before any deployment

is allowed to proceed. If even one test fails, the deployment is

blocked and the developer is notified immediately.



\### How Automated Testing Prevents Bugs Reaching Production



Without automated testing, a developer would need to manually click

through every feature of the GLMS system after every single code

change. This is slow, inconsistent, and impossible to scale across

a development team working simultaneously.



The GLMS test suite covers two layers:



\*\*Unit Tests (Part 2):\*\*

\- Currency conversion math - verifies that USD to ZAR calculations

&#x20; are mathematically precise at various exchange rates

\- File validation - verifies that only PDF files are accepted and

&#x20; that .exe or .docx files are correctly rejected

\- Workflow rules - verifies that service requests cannot be created

&#x20; against Expired or On Hold contracts



\*\*Integration Tests (Part 3):\*\*

\- POST /api/auth/login returns a valid JWT token for correct credentials

\- POST /api/auth/login returns 401 Unauthorized for wrong credentials

\- GET /api/contracts returns 200 OK with a valid JSON array when authenticated

\- GET /api/contracts returns 401 Unauthorized without a token

\- GET /api/clients returns 200 OK with a valid JSON array when authenticated

\- GET /api/servicerequests/rate returns live exchange rate data without auth



In a real DevOps pipeline such as GitHub Actions, these tests run

automatically on every push. A breaking change such as accidentally

removing JWT authentication or changing the contracts endpoint

response structure would be caught immediately, preventing the bug

from ever reaching the test or production environment.



\---



\## 2. Docker and the "Works on My Machine" Problem



\### The Classic Problem



Before containerisation, deploying the GLMS system required:

\- Installing the correct version of .NET 8 on the target server

\- Installing and configuring SQL Server 2022

\- Setting environment variables manually

\- Ensuring port configurations match

\- Managing dependency conflicts between applications on the same server



A developer running Windows 11 with SQL Server 2022 might build and

run GLMS perfectly. But when deployed to a Ubuntu Linux server in the

cloud, the application fails because of OS differences, missing

dependencies, or configuration mismatches. This is the "it works on

my machine" problem.



\### How Docker Solves It



Docker packages the entire application - the compiled code, the .NET

runtime, all dependencies, and the base operating system layer - into

a portable image. This image behaves identically regardless of what

host machine it runs on.



The GLMS docker-compose.yml orchestrates three containers:



\*\*Container 1 - sql-server-db:\*\*

Runs Microsoft SQL Server 2022 in complete isolation. The database

password, port, and configuration are defined once in the compose file

and applied consistently across all environments.



\*\*Container 2 - glms-backend-api:\*\*

Runs the GLMS REST API built with ASP.NET Core 8. It connects to

sql-server-db using the internal Docker service name rather than

an IP address, making the networking environment-independent.



\*\*Container 3 - glms-frontend-web:\*\*

Runs the GLMS MVC application. It calls glms-backend-api using

the internal Docker network name, never connecting to the database

directly.



\### Environment Consistency



The docker-compose.yml passes all sensitive configuration through

environment variables at runtime. The database connection string,

JWT secret key, API base URL, and environment name are all injected

externally. This means the exact same Docker images can be deployed

to a developer laptop, a QA test server, and an AWS production

environment by simply changing the environment variable values.

No code changes are required between environments.



\### Internal Docker Networking



The glms-network bridge network allows containers to communicate

using service names as hostnames. The MVC app calls

http://glms-backend-api:80 and Docker's internal DNS resolves

this to the correct container IP automatically. This eliminates

hardcoded IP addresses and makes the entire system portable.



\---



\## 3. Service-Oriented Architecture Transition



The GLMS system was deliberately refactored from a monolithic

architecture in Part 2 to a Service-Oriented Architecture in Part 3.



\*\*Part 2 (Monolith):\*\*

The MVC controllers connected directly to SQL Server via Entity

Framework Core. Business logic, data access, and presentation were

tightly coupled in the same application.



\*\*Part 3 (SOA):\*\*

The MVC controllers no longer reference the database at all. Every

data operation is performed by calling the REST API via HttpClient.

The API is the single source of truth for all business logic and

data access.



This separation provides significant benefits:

\- The API can be scaled independently on a more powerful server

&#x20; if database load increases without scaling the frontend

\- The MVC frontend can be replaced with a React app or mobile app

&#x20; without changing the backend API

\- JWT authentication is centralised at the API layer, providing

&#x20; a single security boundary for all clients

\- Each service can be deployed, updated, and rolled back

&#x20; independently without affecting the other



\---



\*PROG7311 Programming 3A - Part 3 Submission\*

\*Student: ST10440943\*

\*Date: 2026\*

