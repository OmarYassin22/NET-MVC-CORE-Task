using ARIBApp.Core.Enum;

namespace ARIBApp.Infrastructure.DTOs;


public class TaskEditModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TaskState Status { get; set; }
    public int EmployeeId { get; set; }
    public DateTime CreatedAt { get; set; }
}
