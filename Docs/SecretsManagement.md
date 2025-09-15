# Secrets Management with Google Secret Manager

This application supports multiple ways to manage secrets:

1. **Local Development**: Using `secrets.yaml` file
2. **Kubernetes Deployment**: Using mounted secrets volume
3. **Production/GCP**: Using Google Secret Manager

## Google Secret Manager Integration

When deploying to Google Cloud Platform, secrets can be automatically made available to the application through environment variables by configuring Google Secret Manager properly.

### How it Works

Google Secret Manager integrates with Google Cloud services to automatically inject secrets as environment variables. The application reads these values directly from the environment without requiring explicit Secret Manager API calls.

### Required Configuration

The application expects the following secrets to be available as environment variables:

- `Jwt__Issuer` - JWT token issuer
- `Jwt__Audience` - JWT token audience
- `Jwt__SecurityKey` - JWT signing key
- `ConnectionStrings__DefaultConnection` - Database connection string
- `Serilog__WriteTo__0__Args__connectionString` - Logging database connection string

Note the double underscore (`__`) notation which is the standard way to represent hierarchical configuration in environment variables.

### Setting up Google Secret Manager

1. Create secrets in Google Secret Manager with the names matching the expected environment variables:
   ```
   gcloud secrets create Jwt__Issuer --data-file=- <<< "your-jwt-issuer"
   gcloud secrets create Jwt__Audience --data-file=- <<< "your-jwt-audience"
   gcloud secrets create Jwt__SecurityKey --data-file=- <<< "your-jwt-security-key"
   gcloud secrets create ConnectionStrings__DefaultConnection --data-file=- <<< "your-database-connection-string"
   ```

2. Grant the appropriate service account access to these secrets:
   ```
   gcloud secrets add-iam-policy-binding Jwt__Issuer \
       --member="serviceAccount:your-service-account@your-project.iam.gserviceaccount.com" \
       --role="roles/secretmanager.secretAccessor"
   ```

3. When deploying to Google Cloud Run or other services, configure the service to automatically mount these secrets as environment variables.

### Environment Variable Format

All configuration values in ASP.NET Core can be overridden using environment variables with the following format:
- Hierarchical sections are separated by double underscores (`__`)
- Arrays use index notation (e.g., `Serilog__WriteTo__0__Name`)

Examples:
- `Jwt__Issuer` maps to configuration path `Jwt:Issuer`
- `ConnectionStrings__DefaultConnection` maps to `ConnectionStrings:DefaultConnection`
- `Serilog__WriteTo__0__Args__connectionString` maps to `Serilog:WriteTo:0:Args:connectionString`

### Local Development with Google Secret Manager

For local development with Google Secret Manager:

1. Install Google Cloud SDK: https://cloud.google.com/sdk/docs/install
2. Authenticate with `gcloud auth login` or `gcloud auth application-default login`
3. Set the project: `gcloud config set project YOUR_PROJECT_ID`
4. Ensure your account has the necessary permissions to access the secrets

The application will automatically pick up environment variables set by the Google Cloud SDK or your deployment environment.