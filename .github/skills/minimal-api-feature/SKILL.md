---
name: minimal-api-feature
description: Scaffolds a complete backend domain feature using Minimal APIs and an in-memory Singleton service within the ApiService project.
version: 1.0.0
author: Enterprise Architecture Team
compatibility: Requires dotnet>=10.0
---

# Minimal API Feature Builder

## Overview
This skill generates a standardized backend domain slice in the `EShopCopilot.ApiService` project. It ensures all new entities follow the exact same file structure, dependency injection pattern, and routing conventions without relying on external templates.

## Prerequisites & Inputs
* `domain_name`: The singular name of the entity being created (e.g., `Product`, `Order`).
* `target_project`: Must be `EShopCopilot.ApiService`.

## Process Steps

1. **Model Generation:** 
   Create `Models/{domain_name}.cs`. 
   *Requirement:* The class must be `public` and include a `public Guid Id { get; set; }` property.

2. **Service Generation:** 
   Create `Services/{domain_name}Service.cs`.
   *Requirement:* Implement standard CRUD operations against a private, in-memory `List<{domain_name}>`.
   *Template Structure:*
    ```csharp
       public class {domain_name}Service 
       {
           private readonly List<{domain_name}> _items = new();
           public IEnumerable<{domain_name}> GetAll() => _items;
           public {domain_name}? GetById(Guid id) => _items.FirstOrDefault(x => x.Id == id);
           public void Create({domain_name} item) => _items.Add(item);
           // Add Update and Delete methods...
       }
    ```
        
3. **Endpoint Generation:**
   Create `Endpoints/{domain_name}Endpoints.cs`.
   *Requirement:* Create a static class with an extension method for IEndpointRouteBuilder. Use MapGroup with appropriate tags.
   *Template Structure:*
   ```csharp
   public static class {domain_name}Endpoints
   {
       public static IEndpointRouteBuilder Map{domain_name}Endpoints(this IEndpointRouteBuilder app)
       {
           var group = app.MapGroup("/api/{domain_name}s").WithTags("{domain_name}s");
           group.MapGet("/", ({domain_name}Service service) => service.GetAll());
           // Add MapGet(id), MapPost, MapPut, MapDelete...
           return app;
       }
   }
   ```
  
4. **Wiring & Integration:**
   Provide explicit instructions or code snippets for the user to update `Program.cs`.
   *Requirement:* Must include `builder.Services.AddSingleton<{domain_name}Service>();` and `app.Map{domain_name}Endpoints();`.

## Conditional Routing Rules
If the domain requires relational data: Remind the user that this skill defaults to in-memory lists for prototyping, and a database migration will be required later.

## Verification Checklist
[ ] Endpoints use MapGroup for route prefixing.
[ ] Service is designed to be registered as a Singleton (thread-safety considerations aside for prototyping).
[ ] Model is placed in the correct Models/ namespace.