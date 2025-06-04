using ARIBApp.Core.Interfaces;
using ARIBApp.Core.Persistence;

namespace ARIBApp.Infrastructure.Implementaions;
public class UnitOfWork(ARIBContext context,
                      IEmployeeRepository empRepo,
                      IDepartmentRepository deptRepo,
                      ITaskRepository taskRepo) : IUnitOfWork
{
    private readonly ARIBContext _context = context;



    public IEmployeeRepository EmployeeRepository => empRepo;

    public IDepartmentRepository DepartmentRepository => deptRepo;

    public ITaskRepository TaskRepository => taskRepo;


    public async Task<int> CompleteAsync() => await _context.SaveChangesAsync();

    public void Dispose() => _context.Dispose();
}
