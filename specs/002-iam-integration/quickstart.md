# Quickstart: Using Permission-Based Authorization

## Protecting Endpoints

Use the `[RequirePermission]` attribute on Controller Actions or the Controller itself.

```csharp
[ApiController]
[Route("api/[controller]")]
public class MaterialsController : ControllerBase
{
    [HttpPost]
    [RequirePermission("material.materials.create")]
    public async Task<IActionResult> Create(CreateMaterialRequest request)
    {
        // Implementation
    }

    [HttpGet("{id}")]
    [RequirePermission("material.materials.read")]
    public async Task<IActionResult> Get(Guid id)
    {
        // Implementation
    }
}
```

## Critical Operations

For sensitive actions, enable enhanced auditing by setting `IsCritical = true`.

```csharp
[HttpDelete("{id}")]
[RequirePermission("material.materials.delete", IsCritical = true)]
public async Task<IActionResult> Delete(Guid id)
{
    // Implementation
}
```

## Public Endpoints

Endpoints that allow anonymous access (as clarified in the spec) should use `[AllowAnonymous]`.

```csharp
[HttpGet]
[AllowAnonymous] // Public access for reading materials list
public async Task<IActionResult> GetAll()
{
    // Implementation
}
```

## IAM Registration

The system automatically registers 14 permissions and 4 predefined roles with the IAM Service on startup via `MaterialIAMRegistrationService`.

1. **roles.material.admin**: Full access.
2. **roles.material.manager**: Manage materials, inventory, and categories.
3. **roles.material.clerk**: Daily inventory operations.
4. **roles.material.viewer**: Read-only access.

## Testing with Permissions

In integration tests, use the `CreateAuthenticatedClient` method from the factory to simulate a user with specific permissions.

```csharp
var client = _factory.CreateAuthenticatedClient(permissions: new[] { "material.materials.read" });
var response = await client.GetAsync("/material/v1/materials");
```
