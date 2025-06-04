using ARIBApp.Core.Persistence;
using Mapster;

namespace ARIBApp.Infrastructure.Implementaions.Repositories;
public class EmployeeRepository(ARIBContext context) : IEmployeeRepository
{
    private readonly ARIBContext _context = context;


    public async Task<IReadOnlyList<EmployeeDTO>> GetAllAsync()
    {
        var employees = await _context.Employees
            .Include(e => e.Department)
            .Include(e => e.Manager)
                .ThenInclude(m => m.Department)
            .AsNoTracking()
            .ToListAsync();



        return employees.Adapt<IReadOnlyList<EmployeeDTO>>();
    }

    public async Task<Employee?> GetByIdAsync(int id) =>
        await _context.Employees.Include(e => e.Department).Include(e => e.Manager).Include(e => e.Tasks).FirstOrDefaultAsync(e => e.Id == id);

    public async Task<List<Employee>> GetByManagerIdAsync(int managerId) =>
        await _context.Employees.Where(e => e.ManagerId == managerId).Include(e => e.Tasks).ToListAsync();

    public async Task<IReadOnlyList<Employee>> SearchAsync(string keyword) =>
        await _context.Employees.Include(e => e.Department).Include(e => e.Manager)
            .Where(e => e.FirstName.Contains(keyword) || e.LastName.Contains(keyword))
        .AsNoTracking()
            .ToListAsync();

    public async Task AddAsync(Employee employee) =>
        await _context.Employees.AddAsync(employee);

    public void Update(Employee employee) =>
        _context.Employees.Update(employee);

    public async Task DeleteAsync(int id)
    {
        var emp = await _context.Employees.FindAsync(id);
        if (emp != null) _context.Employees.Remove(emp);
    }

    public async Task<bool> HasEmployeesInDepartment(int departmentId) =>
        await _context.Employees.AnyAsync(e => e.DepartmentId == departmentId);
}
