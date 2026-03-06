using Acervo.Application.DTOs.Genero;
using Acervo.Application.ViewModels;
using Acervo.Domain.Common;
using Acervo.Domain.Entities;
using Acervo.Application.Interfaces.Repositories;
using Acervo.Application.Interfaces.Services;
using MapsterMapper;

namespace Acervo.Application.Services;

public class GeneroService : IGeneroService
{
    private readonly IGeneroRepository _generoRepo;
    private readonly IMapper _mapper;

    public GeneroService(IGeneroRepository generoRepo, IMapper mapper)
    {
        _generoRepo = generoRepo;
        _mapper = mapper;
    }

    public async Task<IEnumerable<GeneroViewModel>> GetAllAsync()
    {
        var generos = await _generoRepo.GetAllAsync();
        return _mapper.Map<IEnumerable<GeneroViewModel>>(generos);
    }

    public async Task<Result<GeneroViewModel>> GetByIdAsync(Guid id)
    {
        var genero = await _generoRepo.GetByIdAsync(id);
        if (genero is null)
            return Result<GeneroViewModel>.Failure("Gênero não encontrado.");

        return Result<GeneroViewModel>.Success(_mapper.Map<GeneroViewModel>(genero));
    }

    public async Task<Result<GeneroViewModel>> CreateAsync(CreateGeneroDto dto)
    {
        if (await _generoRepo.NomeExistsAsync(dto.Nome))
            return Result<GeneroViewModel>.Failure("Gênero com este nome já cadastrado.");

        var genero = new Genero(dto.Nome);
        await _generoRepo.AddAsync(genero);
        return Result<GeneroViewModel>.Success(_mapper.Map<GeneroViewModel>(genero));
    }

    public async Task<Result<GeneroViewModel>> UpdateAsync(Guid id, UpdateGeneroDto dto)
    {
        var genero = await _generoRepo.GetByIdAsync(id);
        if (genero is null)
            return Result<GeneroViewModel>.Failure("Gênero não encontrado.");

        if (await _generoRepo.NomeExistsAsync(dto.Nome, id))
            return Result<GeneroViewModel>.Failure("Gênero com este nome já cadastrado.");

        genero.Update(dto.Nome);
        await _generoRepo.UpdateAsync(genero);
        return Result<GeneroViewModel>.Success(_mapper.Map<GeneroViewModel>(genero));
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        var genero = await _generoRepo.GetByIdAsync(id);
        if (genero is null)
            return Result.Failure("Gênero não encontrado.");

        if (await _generoRepo.HasLivrosAsync(id))
            return Result.Failure("Não é possível excluir um gênero com livros vinculados.");

        await _generoRepo.DeleteAsync(genero);
        return Result.Success();
    }
}
