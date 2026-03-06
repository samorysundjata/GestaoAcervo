using Acervo.Application.DTOs.Autor;
using Acervo.Application.Interfaces.Repositories;
using Acervo.Application.Services;
using Acervo.Application.ViewModels;
using Acervo.Domain.Entities;
using MapsterMapper;
using Moq;
using Shouldly;

namespace Acervo.Tests.Services;

public class AutorServiceTests
{
    private readonly Mock<IAutorRepository> _repoMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly AutorService _svc;

    public AutorServiceTests()
    {
        _svc = new AutorService(_repoMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task CreateAsync_EmailDuplicado_DeveRetornarFailure()
    {
        _repoMock.Setup(r => r.EmailExistsAsync("test@test.com", null)).ReturnsAsync(true);

        var result = await _svc.CreateAsync(new CreateAutorDto("Autor Test", "test@test.com"));

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldContain("E-mail já cadastrado");
    }

    [Fact]
    public async Task CreateAsync_EmailUnico_DeveRetornarSuccess()
    {
        var autor = new Autor("Autor Test", "test@test.com");
        var vm = new AutorViewModel(autor.Id, autor.Nome, autor.Email);

        _repoMock.Setup(r => r.EmailExistsAsync("test@test.com", null)).ReturnsAsync(false);
        _repoMock.Setup(r => r.AddAsync(It.IsAny<Autor>())).Returns(Task.CompletedTask);
        _mapperMock.Setup(m => m.Map<AutorViewModel>(It.IsAny<Autor>())).Returns(vm);

        var result = await _svc.CreateAsync(new CreateAutorDto("Autor Test", "test@test.com"));

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Email.ShouldBe("test@test.com");
    }

    [Fact]
    public async Task GetByIdAsync_NaoEncontrado_DeveRetornarFailure()
    {
        _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Autor?)null);

        var result = await _svc.GetByIdAsync(Guid.NewGuid());

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldContain("não encontrado");
    }

    [Fact]
    public async Task DeleteAsync_ComLivrosVinculados_DeveRetornarFailure()
    {
        var autor = new Autor("Autor Test", "test@test.com");
        _repoMock.Setup(r => r.GetByIdAsync(autor.Id)).ReturnsAsync(autor);
        _repoMock.Setup(r => r.HasLivrosAsync(autor.Id)).ReturnsAsync(true);

        var result = await _svc.DeleteAsync(autor.Id);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldContain("livros vinculados");
    }

    [Fact]
    public async Task DeleteAsync_SemLivros_DeveRetornarSuccess()
    {
        var autor = new Autor("Autor Test", "test@test.com");
        _repoMock.Setup(r => r.GetByIdAsync(autor.Id)).ReturnsAsync(autor);
        _repoMock.Setup(r => r.HasLivrosAsync(autor.Id)).ReturnsAsync(false);
        _repoMock.Setup(r => r.DeleteAsync(autor)).Returns(Task.CompletedTask);

        var result = await _svc.DeleteAsync(autor.Id);

        result.IsSuccess.ShouldBeTrue();
    }
}
