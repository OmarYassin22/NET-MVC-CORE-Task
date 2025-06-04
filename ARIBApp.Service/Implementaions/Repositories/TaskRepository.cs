using ARIBApp.Core.Persistence;

namespace ARIBApp.Infrastructure.Implementaions.Repositories;
public class TaskRepository(ARIBContext context) : ITaskRepository
{
    private readonly ARIBContext _context = context;

    public async Task<ICollection<EmployeeTask>> GetByEmployeeIdAsync(int employeeId) =>
        await _context.Tasks.Where(t => t.EmployeeId == employeeId).ToListAsync();

    public async Task<ICollection<EmployeeTask>> GetByManagerIdAsync(int managerId)
    {
        var employeeIds = await _context.Employees
            .Where(e => e.ManagerId == managerId)
            .Select(e => e.Id).ToListAsync();
        return await _context.Tasks.Where(t => employeeIds.Contains(t.EmployeeId.Value)).ToListAsync();
    }

    public async Task<EmployeeTask?> GetByIdAsync(int id) =>
        await _context.Tasks.FindAsync(id);

    public async Task AddAsync(EmployeeTask task) =>
        await _context.Tasks.AddAsync(task);

    public void Update(EmployeeTask task) =>
        _context.Tasks.Update(task);
}
