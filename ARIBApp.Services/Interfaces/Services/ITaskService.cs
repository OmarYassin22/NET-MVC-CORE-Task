namespace ARIBApp.Core.Interfaces.Services;
public interface ITaskService
{
    Task<ICollection<TaskDTO>> GetTasksForEmployeeAsync(int employeeId);
    Task<ICollection<TaskDTO>> GetTasksForManagerAsync(int managerId);
    Task AssignTaskAsync(TaskDTO task);
    Task UpdateTaskAsync(TaskDTO task);
    Task<TaskDTO> GetTaskByIdAsync(int taskId);

}
