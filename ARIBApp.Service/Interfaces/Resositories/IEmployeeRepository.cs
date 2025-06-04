using ARIBApp.Core.Contracts;

namespace ARIBApp.Core.Interfaces.Resositories;
public interface IEmployeeRepository
{
    Task<IReadOnlyList<EmployeeDTO>> GetAllAsync();
    Task<Employee?> GetByIdAsync(int id);
    Task<List<Employee>> GetByManagerIdAsync(int managerId);
    Task<IReadOnlyList<Employee>> SearchAsync(string keyword);
    Task AddAsync(Employee employee);
    void Update(Employee employee);
    Task DeleteAsync(int id);
    Task<bool> HasEmployeesInDepartment(int departmentId);
}
