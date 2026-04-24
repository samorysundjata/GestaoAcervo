using Acervo.Application.DTOs.Genero;
using Acervo.Application.Interfaces.Repositories;
using Acervo.Application.Services;
using Acervo.Application.ViewModels;
using Acervo.Domain.Entities;
using MapsterMapper;
using Moq;
using Shouldly;

namespace Acervo.Tests.Services;

public class GeneroServiceTests
{
    private readonly Mock<IGeneroRepository> _repoMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly GeneroService _svc;

    public GeneroServiceTests()
    {
        _svc = new GeneroService(_repoMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task CreateAsync_NomeDuplicado_DeveRetornarFailure()
    {
        _repoMock.Setup(r => r.NomeExistsAsync("Tecnologia", null)).ReturnsAsync(true);

        var result = await _svc.CreateAsync(new CreateGeneroDto("Tecnologia"));

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldContain("já cadastrado");
    }

    [Fact]
    public async Task DeleteAsync_ComLivrosVinculados_DeveRetornarFailure_Primary()
    {
        var genero = new Genero("Tecnologia");
        _repoMock.Setup(r => r.GetByIdAsync(genero.Id)).ReturnsAsync(genero);
        _repoMock.Setup(r => r.HasLivrosAsync(genero.Id)).ReturnsAsync(true);

        var result = await _svc.DeleteAsync(genero.Id);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldContain("livros vinculados");
    }

    [Fact]
    public async Task GetAllAsync_DeveRetornarGenerosMapeados()
    {
        var generos = new List<Genero>
        {
            new("Tecnologia"),
            new("Literatura")
        };

        var vms = new List<GeneroViewModel>
        {
            new(generos[0].Id, generos[0].Nome),
            new(generos[1].Id, generos[1].Nome)
        };

        _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(generos);
        _mapperMock.Setup(m => m.Map<IEnumerable<GeneroViewModel>>(It.IsAny<IEnumerable<Genero>>()))
            .Returns(vms);

        var result = await _svc.GetAllAsync();

        result.ShouldBe(vms);
    }

    [Fact]
    public async Task GetByIdAsync_Encontrado_DeveRetornarSuccess()
    {
        var genero = new Genero("Tecnologia");
        var vm = new GeneroViewModel(genero.Id, genero.Nome);

        _repoMock.Setup(r => r.GetByIdAsync(genero.Id)).ReturnsAsync(genero);
        _mapperMock.Setup(m => m.Map<GeneroViewModel>(genero)).Returns(vm);

        var result = await _svc.GetByIdAsync(genero.Id);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(vm);
    }

    [Fact]
    public async Task GetByIdAsync_NaoEncontrado_DeveRetornarFailure()
    {
        _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Genero?)null);

        var result = await _svc.GetByIdAsync(Guid.NewGuid());

        result.IsFailure.ShouldBeTrue();
    }

    [Fact]
    public async Task CreateAsync_NomeUnico_DeveRetornarSuccess()
    {
        var genero = new Genero("Tecnologia");
        var vm = new GeneroViewModel(genero.Id, genero.Nome);

        _repoMock.Setup(r => r.NomeExistsAsync("Tecnologia", null)).ReturnsAsync(false);
        _repoMock.Setup(r => r.AddAsync(It.IsAny<Genero>())).Returns(Task.CompletedTask);
        _mapperMock.Setup(m => m.Map<GeneroViewModel>(It.IsAny<Genero>())).Returns(vm);

        var result = await _svc.CreateAsync(new CreateGeneroDto("Tecnologia"));

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(vm);
    }

    [Fact]
    public async Task UpdateAsync_NaoEncontrado_DeveRetornarFailure()
    {
        _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Genero?)null);

        var result = await _svc.UpdateAsync(Guid.NewGuid(), new UpdateGeneroDto("Novo"));

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldContain("não encontrado");
    }

    [Fact]
    public async Task UpdateAsync_NomeDuplicado_DeveRetornarFailure()
    {
        var genero = new Genero("Tecnologia");
        var dto = new UpdateGeneroDto("Duplicado");

        _repoMock.Setup(r => r.GetByIdAsync(genero.Id)).ReturnsAsync(genero);
        _repoMock.Setup(r => r.NomeExistsAsync(dto.Nome, genero.Id)).ReturnsAsync(true);

        var result = await _svc.UpdateAsync(genero.Id, dto);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldContain("já cadastrado");
    }

    [Fact]
    public async Task UpdateAsync_DadosValidos_DeveRetornarSuccess()
    {
        var genero = new Genero("Tecnologia");
        var dto = new UpdateGeneroDto("Atualizado");
        var vm = new GeneroViewModel(genero.Id, dto.Nome);

        _repoMock.Setup(r => r.GetByIdAsync(genero.Id)).ReturnsAsync(genero);
        _repoMock.Setup(r => r.NomeExistsAsync(dto.Nome, genero.Id)).ReturnsAsync(false);
        _repoMock.Setup(r => r.UpdateAsync(genero)).Returns(Task.CompletedTask);
        _mapperMock.Setup(m => m.Map<GeneroViewModel>(genero)).Returns(vm);

        var result = await _svc.UpdateAsync(genero.Id, dto);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(vm);
        _repoMock.Verify(r => r.UpdateAsync(genero), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_NaoEncontrado_DeveRetornarFailure()
    {
        _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Genero?)null);

        var result = await _svc.DeleteAsync(Guid.NewGuid());

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldContain("não encontrado");
    }

    [Fact]
    public async Task DeleteAsync_ComLivrosVinculados_DeveRetornarFailure()
    {
        var genero = new Genero("Tecnologia");
        _repoMock.Setup(r => r.GetByIdAsync(genero.Id)).ReturnsAsync(genero);
        _repoMock.Setup(r => r.HasLivrosAsync(genero.Id)).ReturnsAsync(true);

        var result = await _svc.DeleteAsync(genero.Id);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldContain("livros vinculados");
    }

    [Fact]
    public async Task DeleteAsync_SemLivros_DeveRetornarSuccess()
    {
        var genero = new Genero("Tecnologia");
        _repoMock.Setup(r => r.GetByIdAsync(genero.Id)).ReturnsAsync(genero);
        _repoMock.Setup(r => r.HasLivrosAsync(genero.Id)).ReturnsAsync(false);
        _repoMock.Setup(r => r.DeleteAsync(genero)).Returns(Task.CompletedTask);

        var result = await _svc.DeleteAsync(genero.Id);

        result.IsSuccess.ShouldBeTrue();
    }
}
