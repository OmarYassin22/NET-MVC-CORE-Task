namespace ARIBApp.Services.implementation.Services;
public class TaskService(IUnitOfWork unitOfWork) : ITaskService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<ICollection<TaskDTO>> GetTasksForEmployeeAsync(int employeeId)
    {
        var tasks = await _unitOfWork.TaskRepository.GetByEmployeeIdAsync(employeeId);
        return tasks.Adapt<ICollection<TaskDTO>>();
    }

    public async Task<ICollection<TaskDTO>> GetTasksForManagerAsync(int managerId)
    {

        var tasks = await _unitOfWork.TaskRepository.GetByManagerIdAsync(managerId);
        return tasks.Adapt<ICollection<TaskDTO>>();
    }

    public async Task AssignTaskAsync(TaskDTO task)
    {
        await _unitOfWork.TaskRepository.AddAsync(task.Adapt<EmployeeTask>());
        await _unitOfWork.CompleteAsync();
    }

    public async Task UpdateTaskAsync(TaskDTO request)
    {
        var task = await _unitOfWork.TaskRepository.GetByIdAsync(request.Id);
        if (task == null)
        {
            throw new KeyNotFoundException($"Task with ID {request.Id} not found");
        }

         task.Title = request.Title;
        task.Description = request.Description;
        task.Status = request.Status;

         if (task.EmployeeId != request.EmployeeId)
        {
             var newEmployee = await _unitOfWork.EmployeeRepository.GetByIdAsync(request.EmployeeId);
            if (newEmployee == null)
            {
                throw new KeyNotFoundException($"Employee with ID {request.EmployeeId} not found");
            }

             task.EmployeeId = request.EmployeeId;
            task.Employee = newEmployee;
        }

         _unitOfWork.TaskRepository.Update(task);
        await _unitOfWork.CompleteAsync();
    }

    public async Task<TaskDTO> GetTaskByIdAsync(int taskId)
    {
        var task = await _unitOfWork.TaskRepository.GetByIdAsync(taskId);
        return task.Adapt<TaskDTO>();
    }
}
