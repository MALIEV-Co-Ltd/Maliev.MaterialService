# API Authorization Contract

## Authentication
- **Scheme**: Bearer (JWT)
- **Token Source**: External OIDC Provider
- **Required Claims**: 
  - `sub`: User Identity
  - `roles`: List of roles assigned to the user

## Authorization
Access is controlled via granular permissions mapped to the user's roles.

### Error Responses

#### 401 Unauthorized
Returned when the `Authorization` header is missing, invalid, or the token is expired.
```json
{
  "type": "https://tools.ietf.org/html/rfc7235#section-3.1",
  "title": "Unauthorized",
  "status": 401,
  "traceId": "..."
}
```

#### 403 Forbidden
Returned when the authenticated user does not have the required permission for the requested action.
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.3",
  "title": "Forbidden",
  "status": 403,
  "detail": "User does not have permission: material.materials.delete",
  "traceId": "..."
}
```
