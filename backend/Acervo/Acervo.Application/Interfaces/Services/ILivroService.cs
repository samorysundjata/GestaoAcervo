using Acervo.Application.DTOs.Livro;
using Acervo.Application.ViewModels;
using Acervo.Domain.Common;

namespace Acervo.Application.Interfaces.Services;

public interface ILivroService
{
    Task<IEnumerable<LivroViewModel>> GetAllAsync();
    Task<Result<LivroDetalheViewModel>> GetByIdAsync(Guid id);
    Task<Result<LivroViewModel>> CreateAsync(CreateLivroDto dto);
    Task<Result<LivroViewModel>> UpdateAsync(Guid id, UpdateLivroDto dto);
    Task<Result> DeleteAsync(Guid id);
}