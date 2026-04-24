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
    public async Task GetAllAsync_DeveRetornarAutoresMapeados()
    {
        var autores = new List<Autor>
        {
            new("Autor 1", "autor1@test.com"),
            new("Autor 2", "autor2@test.com")
        };

        var vms = new List<AutorViewModel>
        {
            new(autores[0].Id, autores[0].Nome, autores[0].Email),
            new(autores[1].Id, autores[1].Nome, autores[1].Email)
        };

        _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(autores);
        _mapperMock.Setup(m => m.Map<IEnumerable<AutorViewModel>>(It.IsAny<IEnumerable<Autor>>()))
            .Returns(vms);

        var result = await _svc.GetAllAsync();

        result.ShouldBe(vms);
    }

    [Fact]
    public async Task GetByIdAsync_Encontrado_DeveRetornarSuccess()
    {
        var autor = new Autor("Autor Test", "test@test.com");
        var vm = new AutorViewModel(autor.Id, autor.Nome, autor.Email);

        _repoMock.Setup(r => r.GetByIdAsync(autor.Id)).ReturnsAsync(autor);
        _mapperMock.Setup(m => m.Map<AutorViewModel>(autor)).Returns(vm);

        var result = await _svc.GetByIdAsync(autor.Id);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(vm);
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
    public async Task UpdateAsync_NaoEncontrado_DeveRetornarFailure()
    {
        _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Autor?)null);

        var result = await _svc.UpdateAsync(Guid.NewGuid(), new UpdateAutorDto("Novo", "novo@test.com"));

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldContain("não encontrado");
    }

    [Fact]
    public async Task UpdateAsync_EmailDuplicado_DeveRetornarFailure()
    {
        var autor = new Autor("Autor Test", "test@test.com");
        var dto = new UpdateAutorDto("Autor Atualizado", "duplicado@test.com");

        _repoMock.Setup(r => r.GetByIdAsync(autor.Id)).ReturnsAsync(autor);
        _repoMock.Setup(r => r.EmailExistsAsync(dto.Email, autor.Id)).ReturnsAsync(true);

        var result = await _svc.UpdateAsync(autor.Id, dto);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldContain("E-mail já cadastrado");
    }

    [Fact]
    public async Task UpdateAsync_DadosValidos_DeveRetornarSuccess()
    {
        var autor = new Autor("Autor Test", "test@test.com");
        var dto = new UpdateAutorDto("Autor Atualizado", "novo@test.com");
        var vm = new AutorViewModel(autor.Id, dto.Nome, dto.Email);

        _repoMock.Setup(r => r.GetByIdAsync(autor.Id)).ReturnsAsync(autor);
        _repoMock.Setup(r => r.EmailExistsAsync(dto.Email, autor.Id)).ReturnsAsync(false);
        _repoMock.Setup(r => r.UpdateAsync(autor)).Returns(Task.CompletedTask);
        _mapperMock.Setup(m => m.Map<AutorViewModel>(autor)).Returns(vm);

        var result = await _svc.UpdateAsync(autor.Id, dto);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(vm);
        _repoMock.Verify(r => r.UpdateAsync(autor), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_NaoEncontrado_DeveRetornarFailure()
    {
        _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Autor?)null);

        var result = await _svc.DeleteAsync(Guid.NewGuid());

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
