namespace ARIBApp.Services.implementation.Services;
public class DepartmentService(IUnitOfWork unitOfWork) : IDepartmentService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<IReadOnlyList<DepartmentDTO>> GetAllAsync()
    {
        var depts = await _unitOfWork.DepartmentRepository.GetAllAsync();
        return depts.Adapt<IReadOnlyList<DepartmentDTO>>();


    }
    public async Task<DepartmentDTO> GetByIdAsync(int id)
    {
        var dept = await _unitOfWork.DepartmentRepository.GetByIdAsync(id);
        return dept?.Adapt<DepartmentDTO>() ?? throw new KeyNotFoundException($"Department with ID {id} not found.");

    }
    public async Task<IReadOnlyList<DepartmentDTO>> SearchWithStatsAsync(string keyword)
    {
        var depts = await _unitOfWork.DepartmentRepository.SearchWithStatsAsync(keyword);
        return depts.Adapt<IReadOnlyList<DepartmentDTO>>();
    }
    public async Task AddAsync(DepartmentDTO department)
    {
        await _unitOfWork.DepartmentRepository.AddAsync(department.Adapt<Department>());
        await _unitOfWork.CompleteAsync();
    }
    public async Task UpdateAsync(DepartmentDTO department)
    {
        _unitOfWork.DepartmentRepository.Update(department.Adapt<Department>());
        await _unitOfWork.CompleteAsync();
    }
    public async Task DeleteAsync(int id)
    {
        await _unitOfWork.DepartmentRepository.DeleteAsync(id);
        await _unitOfWork.CompleteAsync();
    }


}