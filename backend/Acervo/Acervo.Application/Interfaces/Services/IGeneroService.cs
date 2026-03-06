using Acervo.Application.DTOs.Genero;
using Acervo.Application.ViewModels;
using Acervo.Domain.Common;

namespace Acervo.Application.Interfaces.Services;

public interface IGeneroService
{
    Task<IEnumerable<GeneroViewModel>> GetAllAsync();
    Task<Result<GeneroViewModel>> GetByIdAsync(Guid id);
    Task<Result<GeneroViewModel>> CreateAsync(CreateGeneroDto dto);
    Task<Result<GeneroViewModel>> UpdateAsync(Guid id, UpdateGeneroDto dto);
    Task<Result> DeleteAsync(Guid id);
}
