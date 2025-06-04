namespace ARIBApp.Infrastructure.Validators;

public class DepartmentDTOValidator : AbstractValidator<DepartmentDTO>
{
    public DepartmentDTOValidator()
    {
        RuleFor(d => d.Name)
            .NotEmpty().WithMessage("Department name is required")
            .MaximumLength(100).WithMessage("Department name cannot exceed 100 characters");

    }
}