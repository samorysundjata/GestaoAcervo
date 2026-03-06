using Acervo.Application.ViewModels;
using Acervo.Domain.Entities;
using Mapster;

namespace Acervo.Application.Mappings;

public class GeneroMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Genero, GeneroViewModel>();
    }
}
