using Acervo.Application.ViewModels;
using Acervo.Domain.Entities;
using Mapster;

namespace Acervo.Application.Mappings;

public class LivroMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Livro, LivroViewModel>()
            .Map(dest => dest.ISBN, src => src.ISBN);

        config.NewConfig<Livro, LivroDetalheViewModel>()
            .Map(dest => dest.ISBN, src => src.ISBN)
            .Map(dest => dest.AutorNome, src => src.Autor.Nome)
            .Map(dest => dest.GeneroNome, src => src.Genero.Nome);
    }
}