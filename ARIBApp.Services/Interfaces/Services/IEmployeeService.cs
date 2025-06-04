namespace ARIBApp.Core.Interfaces.Services;
public interface IEmployeeService
{

    Task<IReadOnlyList<EmployeeDTO>> GetAllAsync();
    Task<EmployeeDTO> GetByIdAsync(int id);
    Task<IReadOnlyList<EmployeeDTO>> SearchAsync(string keyword);
    Task AddAsync(EmployeeDTO dto);
    Task UpdateAsync(EmployeeDTO dto);
    Task DeleteAsync(int id);
    Task<IReadOnlyList<EmployeeDTO>> GetEmployeesByManagerAsync(int managerId);
}
