using Acervo.Application.DTOs.Autor;
using Acervo.Application.ViewModels;
using Acervo.Domain.Common;
using Acervo.Domain.Entities;
using Acervo.Application.Interfaces.Repositories;
using Acervo.Application.Interfaces.Services;
using MapsterMapper;

namespace Acervo.Application.Services;

public class AutorService : IAutorService
{
    private readonly IAutorRepository _autorRepo;
    private readonly IMapper _mapper;

    public AutorService(IAutorRepository autorRepo, IMapper mapper)
    {
        _autorRepo = autorRepo;
        _mapper = mapper;
    }

    public async Task<IEnumerable<AutorViewModel>> GetAllAsync()
    {
        var autores = await _autorRepo.GetAllAsync();
        return _mapper.Map<IEnumerable<AutorViewModel>>(autores);
    }

    public async Task<Result<AutorViewModel>> GetByIdAsync(Guid id)
    {
        var autor = await _autorRepo.GetByIdAsync(id);
        if (autor is null)
            return Result<AutorViewModel>.Failure("Autor não encontrado.");

        return Result<AutorViewModel>.Success(_mapper.Map<AutorViewModel>(autor));
    }

    public async Task<Result<AutorViewModel>> CreateAsync(CreateAutorDto dto)
    {
        if (await _autorRepo.EmailExistsAsync(dto.Email))
            return Result<AutorViewModel>.Failure("E-mail já cadastrado.");

        var autor = new Autor(dto.Nome, dto.Email);
        await _autorRepo.AddAsync(autor);
        return Result<AutorViewModel>.Success(_mapper.Map<AutorViewModel>(autor));
    }

    public async Task<Result<AutorViewModel>> UpdateAsync(Guid id, UpdateAutorDto dto)
    {
        var autor = await _autorRepo.GetByIdAsync(id);
        if (autor is null)
            return Result<AutorViewModel>.Failure("Autor não encontrado.");

        if (await _autorRepo.EmailExistsAsync(dto.Email, id))
            return Result<AutorViewModel>.Failure("E-mail já cadastrado.");

        autor.Update(dto.Nome, dto.Email);
        await _autorRepo.UpdateAsync(autor);
        return Result<AutorViewModel>.Success(_mapper.Map<AutorViewModel>(autor));
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        var autor = await _autorRepo.GetByIdAsync(id);
        if (autor is null)
            return Result.Failure("Autor não encontrado.");

        if (await _autorRepo.HasLivrosAsync(id))
            return Result.Failure("Não é possível excluir um autor com livros vinculados.");

        await _autorRepo.DeleteAsync(autor);
        return Result.Success();
    }
}
