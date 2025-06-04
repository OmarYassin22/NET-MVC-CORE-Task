namespace ARIBApp.Services.implementation.Services;
public class EmployeeService(IUnitOfWork unitOfWork, IFileService fileService) : IEmployeeService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IFileService _fileService = fileService;

    public async Task<IReadOnlyList<EmployeeDTO>> GetAllAsync()
    {
        var emps = await _unitOfWork.EmployeeRepository.GetAllAsync();
        return await _unitOfWork.EmployeeRepository.GetAllAsync();
    }
    public async Task<EmployeeDTO> GetByIdAsync(int id)
    {
        var emp = await _unitOfWork.EmployeeRepository.GetByIdAsync(id);
        emp.Tasks = await _unitOfWork.TaskRepository.GetByEmployeeIdAsync(emp.Id);

        return emp.Adapt<EmployeeDTO>();
    }



    public async Task<IReadOnlyList<EmployeeDTO>> SearchAsync(string keyword)
    {
        var emps = await _unitOfWork.EmployeeRepository.SearchAsync(keyword);
        return emps.Adapt<IReadOnlyList<EmployeeDTO>>();

    }

    public async Task<IReadOnlyList<EmployeeDTO>> GetEmployeesByManagerAsync(int managerId)
    {
        var emps = await _unitOfWork.EmployeeRepository.GetByManagerIdAsync(managerId);


        return emps.Adapt<IReadOnlyList<EmployeeDTO>>();
    }


    public async Task AddAsync(EmployeeDTO employeeDto)
    {
         if (employeeDto.ImageFile is null)
        {
            employeeDto = employeeDto with { ImagePath = await _fileService.SaveImageAsync(employeeDto.ImageFile) };
        }
        else if (string.IsNullOrEmpty(employeeDto.ImagePath))
        {
            employeeDto = employeeDto with { ImagePath = _fileService.GetDefaultImagePath() };
        }

        var employee = employeeDto.Adapt<Employee>();
        await _unitOfWork.EmployeeRepository.AddAsync(employee);
        await _unitOfWork.CompleteAsync();
    }
    public async Task UpdateAsync(EmployeeDTO employeeDto)
    {
         var existingEmployee = await _unitOfWork.EmployeeRepository.GetByIdAsync(employeeDto.Id);
        if (existingEmployee == null)
            throw new KeyNotFoundException($"Employee with ID {employeeDto.Id} not found.");

         if (employeeDto.ImageFile != null)
        {
             if (!string.IsNullOrEmpty(existingEmployee.ImagePath) &&
                existingEmployee.ImagePath != _fileService.GetDefaultImagePath())
            {
                _fileService.DeleteImage(existingEmployee.ImagePath);
            }

            employeeDto = employeeDto with { ImagePath = await _fileService.SaveImageAsync(employeeDto.ImageFile) };
        }

         employeeDto.Adapt(existingEmployee);

     
        await _unitOfWork.CompleteAsync();
    }

    public async Task DeleteAsync(int id)
    {
         var employee = await _unitOfWork.EmployeeRepository.GetByIdAsync(id);
        if (employee != null && !string.IsNullOrEmpty(employee.ImagePath) &&
            employee.ImagePath != _fileService.GetDefaultImagePath())
        {
            _fileService.DeleteImage(employee.ImagePath);
        }

        await _unitOfWork.EmployeeRepository.DeleteAsync(id);
        await _unitOfWork.CompleteAsync();
    }

}
