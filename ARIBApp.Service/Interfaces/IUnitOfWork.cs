using ARIBApp.Core.Interfaces.Resositories;

namespace ARIBApp.Core.Interfaces;
public interface IUnitOfWork : IDisposable
{
    IEmployeeRepository EmployeeRepository { get; }
    IDepartmentRepository DepartmentRepository { get; }
    ITaskRepository TaskRepository { get; }
    Task<int> CompleteAsync();
}
