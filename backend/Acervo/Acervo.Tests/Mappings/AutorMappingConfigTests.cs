using Acervo.Application.Mappings;
using Acervo.Application.ViewModels;
using Acervo.Domain.Entities;
using Mapster;
using MapsterMapper;
using Shouldly;

namespace Acervo.Tests.Mappings;

public class AutorMappingConfigTests
{
    [Fact]
    public void Register_DeveMapearAutorParaAutorViewModel()
    {
        var config = new TypeAdapterConfig();
        new AutorMappingConfig().Register(config);
        var mapper = new Mapper(config);

        var autor = new Autor("Autor", "autor@dominio.com");
        var result = mapper.Map<AutorViewModel>(autor);

        result.ShouldBe(new AutorViewModel(autor.Id, autor.Nome, autor.Email));
    }
}