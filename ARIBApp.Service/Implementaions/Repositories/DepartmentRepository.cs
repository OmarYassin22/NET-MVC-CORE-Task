using ARIBApp.Core.Persistence;

namespace ARIBApp.Infrastructure.Implementaions.Repositories;
public class DepartmentRepository(ARIBContext context) : IDepartmentRepository
{
    private readonly ARIBContext _context = context;

    public async Task<IReadOnlyList<Department>> GetAllAsync() =>
        await _context.Departments.Include(d => d.Employees).ToListAsync();

    public async Task<Department?> GetByIdAsync(int id) =>
        await _context.Departments.Include(d => d.Employees).FirstOrDefaultAsync(d => d.Id == id);

    public async Task<IReadOnlyList<Department>> SearchWithStatsAsync(string keyword) =>
        await _context.Departments.Include(d => d.Employees)
                                  .Where(d => d.Name.Contains(keyword)).ToListAsync();

    public async Task AddAsync(Department department) =>
        await _context.Departments.AddAsync(department);

    public void Update(Department department) =>
        _context.Departments.Update(department);

    public async Task DeleteAsync(int id)
    {
        var dept = await _context.Departments.FindAsync(id);
        if (dept != null) _context.Departments.Remove(dept);
    }
}
