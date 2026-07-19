namespace Maliev.MaterialService.Tests.Unit;

/// <summary>
/// Protects the MaterialService container and validation-only workflow contracts.
/// </summary>
public sealed class ContainerWorkflowContractTests
{
    private static readonly string[] EntryWorkflows =
    [
        "pr-validation.yml",
        "ci-develop.yml",
        "ci-staging.yml",
        "ci-main.yml"
    ];

    /// <summary>
    /// Docker restores exact packages without mutating project source files.
    /// </summary>
    [Fact]
    public void Dockerfile_UsesExplicitPrivateAndLocalRestoreStages()
    {
        var dockerfile = ReadRepositoryFile("Maliev.MaterialService.Api", "Dockerfile");

        Assert.Contains("ARG dependency_restore_stage=restore-private", dockerfile, StringComparison.Ordinal);
        Assert.Contains("FROM build-base AS restore-local", dockerfile, StringComparison.Ordinal);
        Assert.Contains("NuGet.PRValidation.Config", dockerfile, StringComparison.Ordinal);
        Assert.Contains("ARG SHARED_LIBRARY_VERSION=1.0.96-alpha", dockerfile, StringComparison.Ordinal);
        Assert.Contains("ARG SERVICE_DEFAULTS_VERSION=1.0.89-alpha", dockerfile, StringComparison.Ordinal);
        Assert.Contains("--no-restore", dockerfile, StringComparison.Ordinal);
        Assert.DoesNotContain("--mount=type=cache", dockerfile, StringComparison.Ordinal);
    }

    /// <summary>
    /// Branch and pull-request workflows delegate only to the reusable validation workflow.
    /// </summary>
    [Theory]
    [MemberData(nameof(GetEntryWorkflows))]
    public void EntryWorkflow_IsValidationOnly(string workflowName)
    {
        var workflow = ReadRepositoryFile(".github", "workflows", workflowName);

        Assert.Contains("uses: ./.github/workflows/_validate.yml", workflow, StringComparison.Ordinal);
        Assert.DoesNotContain("google-github-actions", workflow, StringComparison.Ordinal);
        Assert.DoesNotContain("maliev-gitops", workflow, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("GITOPS_PAT", workflow, StringComparison.Ordinal);
        Assert.DoesNotContain("push: true", workflow, StringComparison.Ordinal);
    }

    /// <summary>
    /// Reusable validation reconstructs exact dependencies and keeps the image local.
    /// </summary>
    [Fact]
    public void ReusableValidation_FailsClosedWithoutDeploymentCredentials()
    {
        var workflow = ReadRepositoryFile(".github", "workflows", "_validate.yml");

        Assert.Contains("Build exact public-source dependencies", workflow, StringComparison.Ordinal);
        Assert.Contains("sha256sum --check SHA256SUMS", workflow, StringComparison.Ordinal);
        Assert.Contains("push: false", workflow, StringComparison.Ordinal);
        Assert.Contains("severity: HIGH,CRITICAL", workflow, StringComparison.Ordinal);
        Assert.Contains("if-no-files-found: error", workflow, StringComparison.Ordinal);
        Assert.Contains("needs: [dependency-packages, build-and-test, container]", workflow, StringComparison.Ordinal);
        Assert.DoesNotContain("credentials_json", workflow, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("GCP_SA_KEY", workflow, StringComparison.Ordinal);
        Assert.DoesNotContain("GITOPS_PAT", workflow, StringComparison.Ordinal);
    }

    /// <summary>
    /// Kubernetes probes, not a missing runtime executable, own container health evaluation.
    /// </summary>
    [Fact]
    public void FinalImage_ReliesOnKubernetesProbesInsteadOfUnavailableCurl()
    {
        var dockerfile = ReadRepositoryFile("Maliev.MaterialService.Api", "Dockerfile");

        Assert.DoesNotContain("HEALTHCHECK", dockerfile, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("curl", dockerfile, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Generated dependency sources, build outputs, and local credentials stay outside the image context.
    /// </summary>
    [Fact]
    public void DockerContext_ExcludesGeneratedAndCredentialFiles()
    {
        var dockerIgnore = ReadRepositoryFile(".dockerignore");

        Assert.Contains(".ci-sources", dockerIgnore, StringComparison.Ordinal);
        Assert.Contains(".ci-nuget", dockerIgnore, StringComparison.Ordinal);
        Assert.Contains("**/bin", dockerIgnore, StringComparison.Ordinal);
        Assert.Contains("**/obj", dockerIgnore, StringComparison.Ordinal);
        Assert.Contains("*.user", dockerIgnore, StringComparison.Ordinal);
    }

    /// <summary>
    /// Supplies every entry workflow to the shared policy assertion.
    /// </summary>
    public static TheoryData<string> GetEntryWorkflows()
    {
        var data = new TheoryData<string>();
        foreach (var workflow in EntryWorkflows)
        {
            data.Add(workflow);
        }

        return data;
    }

    private static string ReadRepositoryFile(params string[] segments)
    {
        var path = Path.GetFullPath(Path.Combine(
            AppContext.BaseDirectory,
            "..",
            "..",
            "..",
            "..",
            Path.Combine(segments)));

        Assert.True(File.Exists(path), $"Could not find source file: {path}");
        return File.ReadAllText(path).Replace("\r\n", "\n", StringComparison.Ordinal);
    }
}
