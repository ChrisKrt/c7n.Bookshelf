---
applyTo: "**/*.{cs,csproj,sln,slnx}"
---

# C#
- frontend code should be in its own project (a web api is also a frontend)
- the application code should be in its own project
- the domain code should be put in a "Core" module inside the application project
- the infrastructure code should be put inside its own project
- the aspects code should be put inside its own project
- aspects are cross cutting concerns like logging, caching, authentication, authorization, error handling, notifications
- So, we have at least four projects: frontend, application, infrastructure, and aspects
- the api of the application services should be put in a "api" module inside the application project
- the spi (Service Provider Interface) the application services want to access should be put in a "spi" module inside the application project
- the frontend code only accesses the api module of the application project
- the infrastructure code implements the spi module of the application project
- the application services implement the api module of the application project
- the frontend code only knows the api module of the application project
- the application project does not know the frontend project
- the application project does not know the infrastructure project
- the infrastructure project only knows the spi module of the application project (solved via friendly assemblies)
- we use dependency injection (the built in DI container of .NET is sufficient)
- each module add its own services to the DI container
- the composition root is in the frontend project
- use LINQ whenever possible
- pass an ILogger to the constructor of a class that needs logging
- classes are per default sealed, unless they are designed for inheritance


