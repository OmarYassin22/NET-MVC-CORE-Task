namespace ARIBApp.Core.Contracts;

public record DepartmentDTO
{
    public int Id { get; init; }
    public string Name { get; init; } = null!;
    public List<EmployeeDTO> Employees { get; init; } = new List<EmployeeDTO>();
}