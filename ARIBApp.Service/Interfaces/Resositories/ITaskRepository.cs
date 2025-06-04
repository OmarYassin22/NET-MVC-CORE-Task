namespace ARIBApp.Core.Interfaces.Resositories;
public interface ITaskRepository
{
    Task<ICollection<EmployeeTask>> GetByEmployeeIdAsync(int employeeId);
    Task<ICollection<EmployeeTask>> GetByManagerIdAsync(int managerId);
    Task<EmployeeTask?> GetByIdAsync(int id);
    Task AddAsync(EmployeeTask task);
    void Update(EmployeeTask task);
}
