using FluentValidation;
using Maliev.MaterialService.Api.DTOs.Bulk;

namespace Maliev.MaterialService.Api.Validators.Bulk;

/// <summary>
/// Validator for BulkImportRequest
/// </summary>
public class BulkImportRequestValidator : AbstractValidator<BulkImportRequest>
{
    /// <summary>
    /// Initializes a new instance of BulkImportRequestValidator
    /// </summary>
    public BulkImportRequestValidator()
    {
        RuleFor(x => x.Materials)
            .NotEmpty().WithMessage("Materials list cannot be empty")
            .Must(m => m.Count <= 1000).WithMessage("Bulk import is limited to 1000 materials at a time");
    }
}
