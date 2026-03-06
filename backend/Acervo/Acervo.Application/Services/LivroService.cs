using Acervo.Application.DTOs.Livro;
using Acervo.Application.ViewModels;
using Acervo.Domain.Common;
using Acervo.Domain.Entities;
using Acervo.Application.Interfaces.Repositories;
using Acervo.Application.Interfaces.Services;
using MapsterMapper;

namespace Acervo.Application.Services;

public class LivroService : ILivroService
{
    private readonly ILivroRepository _livroRepo;
    private readonly IAutorRepository _autorRepo;
    private readonly IGeneroRepository _generoRepo;
    private readonly IMapper _mapper;

    public LivroService(
        ILivroRepository livroRepo,
        IAutorRepository autorRepo,
        IGeneroRepository generoRepo,
        IMapper mapper)
    {
        _livroRepo = livroRepo;
        _autorRepo = autorRepo;
        _generoRepo = generoRepo;
        _mapper = mapper;
    }

    public async Task<IEnumerable<LivroViewModel>> GetAllAsync()
    {
        var livros = await _livroRepo.GetAllAsync();
        return _mapper.Map<IEnumerable<LivroViewModel>>(livros);
    }

    public async Task<Result<LivroDetalheViewModel>> GetByIdAsync(Guid id)
    {
        var livro = await _livroRepo.GetByIdAsync(id);
        if (livro is null)
            return Result<LivroDetalheViewModel>.Failure("Livro não encontrado.");

        return Result<LivroDetalheViewModel>.Success(_mapper.Map<LivroDetalheViewModel>(livro));
    }

    public async Task<Result<LivroViewModel>> CreateAsync(CreateLivroDto dto)
    {
        if (await _livroRepo.IsbnExistsAsync(dto.ISBN))
            return Result<LivroViewModel>.Failure("ISBN já cadastrado.");

        if (await _autorRepo.GetByIdAsync(dto.AutorId) is null)
            return Result<LivroViewModel>.Failure("Autor não encontrado.");

        if (await _generoRepo.GetByIdAsync(dto.GeneroId) is null)
            return Result<LivroViewModel>.Failure("Gênero não encontrado.");

        var livro = new Livro(dto.Titulo, dto.ISBN, dto.AnoPublicacao, dto.AutorId, dto.GeneroId);
        await _livroRepo.AddAsync(livro);
        return Result<LivroViewModel>.Success(_mapper.Map<LivroViewModel>(livro));
    }

    public async Task<Result<LivroViewModel>> UpdateAsync(Guid id, UpdateLivroDto dto)
    {
        var livro = await _livroRepo.GetByIdAsync(id);
        if (livro is null)
            return Result<LivroViewModel>.Failure("Livro não encontrado.");

        if (await _livroRepo.IsbnExistsAsync(dto.ISBN, id))
            return Result<LivroViewModel>.Failure("ISBN já cadastrado.");

        if (await _autorRepo.GetByIdAsync(dto.AutorId) is null)
            return Result<LivroViewModel>.Failure("Autor não encontrado.");

        if (await _generoRepo.GetByIdAsync(dto.GeneroId) is null)
            return Result<LivroViewModel>.Failure("Gênero não encontrado.");

        livro.Update(dto.Titulo, dto.ISBN, dto.AnoPublicacao, dto.AutorId, dto.GeneroId);
        await _livroRepo.UpdateAsync(livro);
        return Result<LivroViewModel>.Success(_mapper.Map<LivroViewModel>(livro));
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        var livro = await _livroRepo.GetByIdAsync(id);
        if (livro is null)
            return Result.Failure("Livro não encontrado.");

        await _livroRepo.DeleteAsync(livro);
        return Result.Success();
    }
}