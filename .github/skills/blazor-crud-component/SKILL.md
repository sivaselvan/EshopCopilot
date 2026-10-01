---
name: blazor-crud-component
description: Generates standardized Blazor WebAssembly UI components and typed HTTP clients for a specific domain model.
version: 1.0.0
author: Frontend Architecture Team
compatibility: Requires blazor-wasm>=10.0
---

# Blazor Enterprise CRUD Scaffolder

## Overview
This skill implements the frontend presentation layer for a domain entity in the `EShopCopilot.Web` project. It embeds the logic for HTTP communication, loading states, and basic HTML table rendering.

## Prerequisites & Inputs
* `domain_name`: The entity we are building the UI for (e.g., `Product`, `ShoppingCart`).
* `api_route`: The base route for the backend API (e.g., `/api/products`).
* `target_project`: Must be `EShopCopilot.Web`.

## Process Steps

1. **API Client Generation:** 
   Create `Clients/{domain_name}ApiClient.cs`.
   *Requirement:* Inject `HttpClient` via the constructor. Provide asynchronous methods (`GetFromJsonAsync`, `PostAsJsonAsync`, etc.) mapped to the `{api_route}`.
   *Template Structure:*
    ```csharp
       public class {domain_name}ApiClient(HttpClient httpClient)
       {
           public async Task<{domain_name}[]> GetAllAsync() 
               => await httpClient.GetFromJsonAsync<{domain_name}[]>("{api_route}") ?? [];
           // Add GetById, Create, Update, Delete...
       }
    ```    

2. **UI Component Generation:**
   Create `Components/Pages/{domain_name}List.razor`.
   *Requirement:* Handle null states during API fetching.
   *Template Structure:*
   ```html
   @page "/{domain_name}s"
   @inject {domain_name}ApiClient ApiClient

   <h3>{domain_name} Catalog</h3>

   @if (items == null)
   {
       <p><em>Loading...</em></p>
   }
   else
   {
       <table class="table">
           <!-- Generate standard table headers and rows dynamically based on domain properties -->
       </table>
   }

   @code {
       private {domain_name}[]? items;

       protected override async Task OnInitializedAsync()
       {
           items = await ApiClient.GetAllAsync();
       }
   }
   ```
3. **Wiring & Integration:**
   Provide the code snippet to update the `EShopCopilot.Web` Program.cs file.
   *Requirement:* Must include `builder.Services.AddHttpClient<{domain_name}ApiClient>(...)` pointing to the ApiService base address.

## Conditional Routing Rules
If the domain model contains nested objects: Render them as simplified strings in the HTML table or leave a `TODO` comment for the developer to implement a custom formatter.

## Verification Checklist
[ ] `.razor` component contains an `@page` route.
[ ] UI safely handles the asynchronous loading state.
[ ] ApiClient uses constructor injection for the `HttpClient`.
