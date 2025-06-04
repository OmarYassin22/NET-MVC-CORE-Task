using ARIBApp.Core.Enum;

namespace ARIBApp.Core.Contracts;

public record TaskDTO
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TaskState Status { get; set; }
    public int EmployeeId { get; set; }
    public EmployeeDTO? Employee { get; init; } = null!;
    public DateTime CreatedAt { get; set; }
}