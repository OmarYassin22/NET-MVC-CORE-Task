using Microsoft.AspNetCore.Identity;

namespace ARIBApp.Data.Model;
public class Employee : IdentityUser<int>
{

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string? ImagePath { get; set; } = string.Empty;

    public decimal Salary { get; set; }

    public int? ManagerId { get; set; }
    public Employee? Manager { get; set; }

    //public ICollection<Employee> Employees { get; set; } = new List<Employee>();

    public string FullName => $"{FirstName} {LastName}";

    public int DepartmentId { get; set; }

    public Department? Department { get; set; }

    public ICollection<EmployeeTask?> Tasks { get; set; }
}

