using Acervo.Application.DTOs.Genero;
using Acervo.Application.Interfaces.Repositories;
using Acervo.Application.Services;
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
    public async Task GetByIdAsync_NaoEncontrado_DeveRetornarFailure()
    {
        _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Genero?)null);

        var result = await _svc.GetByIdAsync(Guid.NewGuid());

        result.IsFailure.ShouldBeTrue();
    }
}
