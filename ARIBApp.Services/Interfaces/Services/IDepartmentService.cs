
namespace ARIBApp.Core.Interfaces.Services;
public interface IDepartmentService
{
    Task<IReadOnlyList<DepartmentDTO>> GetAllAsync();
    Task<DepartmentDTO> GetByIdAsync(int id);
    Task AddAsync(DepartmentDTO dto);
    Task UpdateAsync(DepartmentDTO dto);
    Task DeleteAsync(int id);
    Task<IReadOnlyList<DepartmentDTO>> SearchWithStatsAsync(string keyword);
}
