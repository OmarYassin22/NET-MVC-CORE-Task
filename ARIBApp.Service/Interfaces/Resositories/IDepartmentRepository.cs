namespace ARIBApp.Core.Interfaces.Resositories;
public interface IDepartmentRepository
{
    Task<IReadOnlyList<Department>> GetAllAsync();
    Task<Department?> GetByIdAsync(int id);
    Task<IReadOnlyList<Department>> SearchWithStatsAsync(string keyword);
    Task AddAsync(Department department);
    void Update(Department department);
    Task DeleteAsync(int id);
}
