namespace ARIBApp.Infrastructure.Validators;

public class EmployeeDTOValidator : AbstractValidator<EmployeeDTO>
{
    public EmployeeDTOValidator()
    {
        RuleFor(e => e.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .MaximumLength(50).WithMessage("First name cannot exceed 50 characters");

        RuleFor(e => e.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters");


        RuleFor(e => e.Salary)
            .GreaterThanOrEqualTo(0).WithMessage("Salary must be non-negative");

        RuleFor(e => e.DepartmentId)
            .GreaterThan(0).WithMessage("Department is required");

        RuleFor(e => e.ManagerId)
            .GreaterThan(0).When(e => e.ManagerId.HasValue).WithMessage("Invalid manager ID");

        RuleFor(e => e.UserName)
            .NotEmpty().WithMessage("Username is required")
            .MaximumLength(256).WithMessage("Username cannot exceed 256 characters");

        RuleFor(e => e.Email)
            .EmailAddress().When(e => !string.IsNullOrEmpty(e.Email)).WithMessage("Invalid email address");
    }
}