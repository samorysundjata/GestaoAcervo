using Acervo.Application.DTOs.Livro;
using Acervo.Application.Interfaces.Repositories;
using Acervo.Application.Services;
using Acervo.Application.ViewModels;
using Acervo.Domain.Entities;
using MapsterMapper;
using Moq;
using Shouldly;

namespace Acervo.Tests.Services;

public class LivroServiceTests
{
    private readonly Mock<ILivroRepository> _livroRepoMock = new();
    private readonly Mock<IAutorRepository> _autorRepoMock = new();
    private readonly Mock<IGeneroRepository> _generoRepoMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly LivroService _svc;

    public LivroServiceTests()
    {
        _svc = new LivroService(
            _livroRepoMock.Object,
            _autorRepoMock.Object,
            _generoRepoMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_DeveRetornarLivrosMapeados()
    {
        var livros = new List<Livro>
        {
            new("Clean Code", "9780132350884", 2008, Guid.NewGuid(), Guid.NewGuid()),
            new("DDD", "9780321125217", 2003, Guid.NewGuid(), Guid.NewGuid())
        };

        var vms = new List<LivroViewModel>
        {
            new(livros[0].Id, livros[0].Titulo, livros[0].ISBN, livros[0].AnoPublicacao, livros[0].AutorId, livros[0].GeneroId),
            new(livros[1].Id, livros[1].Titulo, livros[1].ISBN, livros[1].AnoPublicacao, livros[1].AutorId, livros[1].GeneroId)
        };

        _livroRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(livros);
        _mapperMock.Setup(m => m.Map<IEnumerable<LivroViewModel>>(It.IsAny<IEnumerable<Livro>>()))
            .Returns(vms);

        var result = await _svc.GetAllAsync();

        result.ShouldBe(vms);
    }

    [Fact]
    public async Task GetByIdAsync_NaoEncontrado_DeveRetornarFailure()
    {
        _livroRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Livro?)null);

        var result = await _svc.GetByIdAsync(Guid.NewGuid());

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldContain("Livro não encontrado");
    }

    [Fact]
    public async Task GetByIdAsync_Encontrado_DeveRetornarSuccess()
    {
        var livro = new Livro("Clean Code", "9780132350884", 2008, Guid.NewGuid(), Guid.NewGuid());
        var vm = new LivroDetalheViewModel(
            livro.Id,
            livro.Titulo,
            livro.ISBN,
            livro.AnoPublicacao,
            livro.AutorId,
            "Autor",
            livro.GeneroId,
            "Genero");

        _livroRepoMock.Setup(r => r.GetByIdAsync(livro.Id)).ReturnsAsync(livro);
        _mapperMock.Setup(m => m.Map<LivroDetalheViewModel>(livro)).Returns(vm);

        var result = await _svc.GetByIdAsync(livro.Id);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(vm);
    }

    [Fact]
    public async Task CreateAsync_IsbnDuplicado_DeveRetornarFailure()
    {
        _livroRepoMock.Setup(r => r.IsbnExistsAsync("9780132350884", null)).ReturnsAsync(true);

        var dto = new CreateLivroDto("Clean Code", "9780132350884", 2008, Guid.NewGuid(), Guid.NewGuid());
        var result = await _svc.CreateAsync(dto);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldContain("ISBN já cadastrado");
    }

    [Fact]
    public async Task CreateAsync_AutorNaoEncontrado_DeveRetornarFailure()
    {
        _livroRepoMock.Setup(r => r.IsbnExistsAsync(It.IsAny<string>(), null)).ReturnsAsync(false);
        _autorRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Autor?)null);

        var dto = new CreateLivroDto("Clean Code", "9780132350884", 2008, Guid.NewGuid(), Guid.NewGuid());
        var result = await _svc.CreateAsync(dto);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldContain("Autor não encontrado");
    }

    [Fact]
    public async Task CreateAsync_GeneroNaoEncontrado_DeveRetornarFailure()
    {
        var autorId = Guid.NewGuid();

        _livroRepoMock.Setup(r => r.IsbnExistsAsync(It.IsAny<string>(), null)).ReturnsAsync(false);
        _autorRepoMock.Setup(r => r.GetByIdAsync(autorId)).ReturnsAsync(new Autor("Autor", "autor@test.com"));
        _generoRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Genero?)null);

        var dto = new CreateLivroDto("Clean Code", "9780132350884", 2008, autorId, Guid.NewGuid());
        var result = await _svc.CreateAsync(dto);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldContain("Gênero não encontrado");
    }

    [Fact]
    public async Task CreateAsync_DadosValidos_DeveRetornarSuccess()
    {
        var autorId = Guid.NewGuid();
        var generoId = Guid.NewGuid();
        var autor = new Autor("Robert Martin", "uncle.bob@test.com");
        var genero = new Genero("Tecnologia");
        var vm = new LivroViewModel(Guid.NewGuid(), "Clean Code", "9780132350884", 2008, autorId, generoId);

        _livroRepoMock.Setup(r => r.IsbnExistsAsync(It.IsAny<string>(), null)).ReturnsAsync(false);
        _autorRepoMock.Setup(r => r.GetByIdAsync(autorId)).ReturnsAsync(autor);
        _generoRepoMock.Setup(r => r.GetByIdAsync(generoId)).ReturnsAsync(genero);
        _livroRepoMock.Setup(r => r.AddAsync(It.IsAny<Livro>())).Returns(Task.CompletedTask);
        _mapperMock.Setup(m => m.Map<LivroViewModel>(It.IsAny<Livro>())).Returns(vm);

        var dto = new CreateLivroDto("Clean Code", "9780132350884", 2008, autorId, generoId);
        var result = await _svc.CreateAsync(dto);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.ISBN.ShouldBe("9780132350884");
    }

    [Fact]
    public async Task UpdateAsync_NaoEncontrado_DeveRetornarFailure()
    {
        _livroRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Livro?)null);

        var dto = new UpdateLivroDto("Clean Code", "9780132350884", 2008, Guid.NewGuid(), Guid.NewGuid());
        var result = await _svc.UpdateAsync(Guid.NewGuid(), dto);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldContain("Livro não encontrado");
    }

    [Fact]
    public async Task UpdateAsync_IsbnDuplicado_DeveRetornarFailure()
    {
        var livro = new Livro("Clean Code", "9780132350884", 2008, Guid.NewGuid(), Guid.NewGuid());
        var dto = new UpdateLivroDto("Clean Code", "9780132350884", 2008, livro.AutorId, livro.GeneroId);

        _livroRepoMock.Setup(r => r.GetByIdAsync(livro.Id)).ReturnsAsync(livro);
        _livroRepoMock.Setup(r => r.IsbnExistsAsync(dto.ISBN, livro.Id)).ReturnsAsync(true);

        var result = await _svc.UpdateAsync(livro.Id, dto);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldContain("ISBN já cadastrado");
    }

    [Fact]
    public async Task UpdateAsync_AutorNaoEncontrado_DeveRetornarFailure()
    {
        var livro = new Livro("Clean Code", "9780132350884", 2008, Guid.NewGuid(), Guid.NewGuid());
        var dto = new UpdateLivroDto("Clean Code", "9780132350884", 2008, Guid.NewGuid(), livro.GeneroId);

        _livroRepoMock.Setup(r => r.GetByIdAsync(livro.Id)).ReturnsAsync(livro);
        _livroRepoMock.Setup(r => r.IsbnExistsAsync(dto.ISBN, livro.Id)).ReturnsAsync(false);
        _autorRepoMock.Setup(r => r.GetByIdAsync(dto.AutorId)).ReturnsAsync((Autor?)null);

        var result = await _svc.UpdateAsync(livro.Id, dto);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldContain("Autor não encontrado");
    }

    [Fact]
    public async Task UpdateAsync_GeneroNaoEncontrado_DeveRetornarFailure()
    {
        var livro = new Livro("Clean Code", "9780132350884", 2008, Guid.NewGuid(), Guid.NewGuid());
        var dto = new UpdateLivroDto("Clean Code", "9780132350884", 2008, livro.AutorId, Guid.NewGuid());

        _livroRepoMock.Setup(r => r.GetByIdAsync(livro.Id)).ReturnsAsync(livro);
        _livroRepoMock.Setup(r => r.IsbnExistsAsync(dto.ISBN, livro.Id)).ReturnsAsync(false);
        _autorRepoMock.Setup(r => r.GetByIdAsync(dto.AutorId)).ReturnsAsync(new Autor("Autor", "autor@test.com"));
        _generoRepoMock.Setup(r => r.GetByIdAsync(dto.GeneroId)).ReturnsAsync((Genero?)null);

        var result = await _svc.UpdateAsync(livro.Id, dto);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldContain("Gênero não encontrado");
    }

    [Fact]
    public async Task UpdateAsync_DadosValidos_DeveRetornarSuccess()
    {
        var livro = new Livro("Clean Code", "9780132350884", 2008, Guid.NewGuid(), Guid.NewGuid());
        var dto = new UpdateLivroDto("Clean Code Atualizado", "9780132350884", 2008, livro.AutorId, livro.GeneroId);
        var vm = new LivroViewModel(livro.Id, dto.Titulo, dto.ISBN, dto.AnoPublicacao, dto.AutorId, dto.GeneroId);

        _livroRepoMock.Setup(r => r.GetByIdAsync(livro.Id)).ReturnsAsync(livro);
        _livroRepoMock.Setup(r => r.IsbnExistsAsync(dto.ISBN, livro.Id)).ReturnsAsync(false);
        _autorRepoMock.Setup(r => r.GetByIdAsync(dto.AutorId)).ReturnsAsync(new Autor("Autor", "autor@test.com"));
        _generoRepoMock.Setup(r => r.GetByIdAsync(dto.GeneroId)).ReturnsAsync(new Genero("Genero"));
        _livroRepoMock.Setup(r => r.UpdateAsync(livro)).Returns(Task.CompletedTask);
        _mapperMock.Setup(m => m.Map<LivroViewModel>(livro)).Returns(vm);

        var result = await _svc.UpdateAsync(livro.Id, dto);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(vm);
        _livroRepoMock.Verify(r => r.UpdateAsync(livro), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_NaoEncontrado_DeveRetornarFailure()
    {
        _livroRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Livro?)null);

        var result = await _svc.DeleteAsync(Guid.NewGuid());

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldContain("Livro não encontrado");
    }

    [Fact]
    public async Task DeleteAsync_Encontrado_DeveRetornarSuccess()
    {
        var livro = new Livro("Clean Code", "9780132350884", 2008, Guid.NewGuid(), Guid.NewGuid());

        _livroRepoMock.Setup(r => r.GetByIdAsync(livro.Id)).ReturnsAsync(livro);
        _livroRepoMock.Setup(r => r.DeleteAsync(livro)).Returns(Task.CompletedTask);

        var result = await _svc.DeleteAsync(livro.Id);

        result.IsSuccess.ShouldBeTrue();
        _livroRepoMock.Verify(r => r.DeleteAsync(livro), Times.Once);
    }
}
