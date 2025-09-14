namespace Maliev.MaterialService.Data.Constants;

/// <summary>
/// Constants for database column configurations.
/// </summary>
public static class DatabaseConstants
{
    /// <summary>
    /// Precision and scale for general decimal values (18 digits total, 6 decimal places).
    /// </summary>
    public const string DecimalPrecisionScaleGeneral = "decimal(18,6)";

    /// <summary>
    /// Precision and scale for thickness values (10 digits total, 3 decimal places).
    /// </summary>
    public const string DecimalPrecisionScaleThickness = "decimal(10,3)";
}