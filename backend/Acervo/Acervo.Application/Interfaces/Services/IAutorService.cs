using Acervo.Application.DTOs.Autor;
using Acervo.Application.ViewModels;
using Acervo.Domain.Common;

namespace Acervo.Application.Interfaces.Services;

public interface IAutorService
{
    Task<IEnumerable<AutorViewModel>> GetAllAsync();
    Task<Result<AutorViewModel>> GetByIdAsync(Guid id);
    Task<Result<AutorViewModel>> CreateAsync(CreateAutorDto dto);
    Task<Result<AutorViewModel>> UpdateAsync(Guid id, UpdateAutorDto dto);
    Task<Result> DeleteAsync(Guid id);
}
