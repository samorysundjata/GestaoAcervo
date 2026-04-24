using Acervo.Application.Mappings;
using Acervo.Application.ViewModels;
using Acervo.Domain.Entities;
using Mapster;
using MapsterMapper;
using Shouldly;

namespace Acervo.Tests.Mappings;

public class GeneroMappingConfigTests
{
    [Fact]
    public void Register_DeveMapearGeneroParaGeneroViewModel()
    {
        var config = new TypeAdapterConfig();
        new GeneroMappingConfig().Register(config);
        var mapper = new Mapper(config);

        var genero = new Genero("Tecnologia");
        var result = mapper.Map<GeneroViewModel>(genero);

        result.ShouldBe(new GeneroViewModel(genero.Id, genero.Nome));
    }
}