using FluentValidation;
using Maliev.MaterialService.Api.DTOs.Materials;
using Maliev.MaterialService.Api.Services.External;

namespace Maliev.MaterialService.Api.Validators.Materials;

/// <summary>
/// Validator for UpdateMaterialRequest
/// </summary>
public class UpdateMaterialRequestValidator : AbstractValidator<UpdateMaterialRequest>
{
    /// <summary>
    /// Initializes a new instance of UpdateMaterialRequestValidator
    /// </summary>
    /// <param name="supplierServiceClient">Client for validating supplier existence</param>
    public UpdateMaterialRequestValidator(ISupplierServiceClient supplierServiceClient)
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(200).WithMessage("Name cannot exceed 200 characters");

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Code is required")
            .MaximumLength(100).WithMessage("Code cannot exceed 100 characters");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.PricePerUnit)
            .GreaterThan(0).WithMessage("PricePerUnit must be greater than 0");

        RuleFor(x => x.StockLevel)
            .GreaterThanOrEqualTo(0).WithMessage("StockLevel must be greater than or equal to 0");

        RuleFor(x => x.Version)
            .GreaterThan(0).WithMessage("Version must be greater than 0");

        RuleFor(x => x.SupplierId)
            .MustAsync(async (supplierId, cancellation) =>
            {
                if (!supplierId.HasValue) return true;
                return await supplierServiceClient.ValidateSupplierExistsAsync(supplierId.Value);
            })
            .WithMessage("Supplier with the specified ID does not exist.")
            .When(x => x.SupplierId.HasValue);
    }
}
