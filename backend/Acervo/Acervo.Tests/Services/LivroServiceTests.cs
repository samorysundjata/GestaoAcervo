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
}
