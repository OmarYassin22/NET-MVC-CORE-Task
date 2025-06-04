namespace ARIBApp.Services.Helper;
using ARIBApp.Core.Contracts;
using ARIBApp.Data.Model;
using Mapster;

public class MapsterConf
{
    public static void Configure()
    {
        TypeAdapterConfig<Employee, EmployeeDTO>.NewConfig()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.FirstName, src => src.FirstName)
            .Map(dest => dest.LastName, src => src.LastName)
            .Map(dest => dest.ImagePath, src => src.ImagePath)
            .Map(dest => dest.Salary, src => src.Salary)
            .Map(dest => dest.ManagerId, src => src.ManagerId)
            .Map(dest => dest.Manager, src => src.Manager, src => src.Manager != null)
             .Map(dest => dest.DepartmentId, src => src.DepartmentId)
            .Map(dest => dest.Department, src => src.Department)
            .Map(dest => dest.Tasks, src => src.Tasks)
            .Map(dest => dest.FullName, src => src.FullName)
             .Map(dest => dest.UserName, src => src.UserName)
            .Map(dest => dest.Email, src => src.Email)
            .MaxDepth(2);

        TypeAdapterConfig<EmployeeDTO, Employee>.NewConfig()
            .Map(dest => dest.FirstName, src => src.FirstName)
            .Map(dest => dest.LastName, src => src.LastName)
            .Map(dest => dest.ImagePath, src => src.ImagePath)
            .Map(dest => dest.Salary, src => src.Salary)
            .Map(dest => dest.ManagerId, src => src.ManagerId)
            .Map(dest => dest.DepartmentId, src => src.DepartmentId)

            .Map(dest => dest.UserName, src => src.UserName)
            .Map(dest => dest.Email, src => src.Email)
            .Ignore(dest => dest.Manager)
             .Ignore(dest => dest.Department)
            .Ignore(dest => dest.Tasks);

        TypeAdapterConfig<Department, DepartmentDTO>.NewConfig()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.Employees, src => src.Employees)

            .MaxDepth(2);

        TypeAdapterConfig<DepartmentDTO, Department>.NewConfig()
            .Map(dest => dest.Name, src => src.Name)
             .Ignore(dest => dest.Employees);

        TypeAdapterConfig<EmployeeTask, TaskDTO>.NewConfig()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Title, src => src.Title)
            .Map(dest => dest.Description, src => src.Description)
            .Map(dest => dest.Status, src => src.Status)
            .Map(dest => dest.EmployeeId, src => src.EmployeeId)
            .Map(dest => dest.Employee, src => src.Employee)
            .Map(dest => dest.CreatedAt, src => src.CreatedAt)
             .MaxDepth(2);

        TypeAdapterConfig<TaskDTO, EmployeeTask>.NewConfig()
            .Map(dest => dest.Title, src => src.Title)
            .Map(dest => dest.Description, src => src.Description)
            .Map(dest => dest.Status, src => src.Status)
            .Map(dest => dest.EmployeeId, src => src.EmployeeId)
            .Map(dest => dest.CreatedAt, src => src.CreatedAt)
             .Ignore(dest => dest.Employee);
    }
}
