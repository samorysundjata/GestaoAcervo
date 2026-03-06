using Acervo.Application.ViewModels;
using Acervo.Domain.Entities;
using Mapster;

namespace Acervo.Application.Mappings;

public class AutorMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Autor, AutorViewModel>();
    }
}
