using Microsoft.AspNetCore.Http;

namespace ARIBApp.Core.Contracts;

public record EmployeeDTO
{
    public int Id { get; init; }
    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;
    public string? ImagePath { get; init; } = string.Empty;
    public IFormFile? ImageFile { get; init; }
    public decimal Salary { get; init; }
    public int? ManagerId { get; init; }
    public EmployeeDTO? Manager { get; init; }
    public List<EmployeeDTO> Employees { get; init; } = new List<EmployeeDTO>();
    public int DepartmentId { get; init; }
    public DepartmentDTO? Department { get; init; } = null!;
    public List<TaskDTO> Tasks { get; init; } = new List<TaskDTO>();
    public string FullName => $"{FirstName} {LastName}";

    public string UserName { get; init; } = null!;
    public string? Email { get; init; }
}