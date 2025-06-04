
namespace ARIBApp.Infrastructure.Validators;

public class EmployeeTaskDTOValidator : AbstractValidator<TaskDTO>
{
    public EmployeeTaskDTOValidator()
    {
        RuleFor(t => t.Title)
            .NotEmpty().WithMessage("Task title is required")
            .MaximumLength(200).WithMessage("Task title cannot exceed 200 characters");

        RuleFor(t => t.Description)
            .MaximumLength(1000).WithMessage("Task description cannot exceed 1000 characters")
            .When(t => !string.IsNullOrEmpty(t.Description));

        RuleFor(t => t.Status)
            .IsInEnum().WithMessage("Invalid task status");

        RuleFor(t => t.EmployeeId)
            .GreaterThan(0).WithMessage("Employee ID is required");
         

        RuleFor(t => t.CreatedAt)
            .NotEmpty().WithMessage("Created date is required");
    }
}