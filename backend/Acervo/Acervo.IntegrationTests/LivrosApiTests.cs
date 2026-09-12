using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Acervo.Application.DTOs.Autor;
using Acervo.Application.DTOs.Genero;
using Acervo.Application.DTOs.Livro;
using Acervo.Application.ViewModels;
using Shouldly;

namespace Acervo.IntegrationTests;

[Collection(nameof(ApiCollection))]
public sealed class LivrosApiTests
{
    private readonly HttpClient _client;

    public LivrosApiTests(ApiFactory factory) => _client = factory.CreateClient();

    [Fact]
    public async Task Crud_ShouldPersistLivro_AndBlockDeletingLinkedAutor()
    {
        var suffix = Guid.NewGuid().ToString("N")[..8];
        var isbn = $"978{suffix}123".PadRight(13, '0')[..13];

        var autorResponse = await _client.PostAsJsonAsync(
            "/api/v1/autores",
            new CreateAutorDto($"Autor Livro {suffix}", $"livro.{suffix}@test.com"));
        autorResponse.StatusCode.ShouldBe(HttpStatusCode.Created);
        var autor = (await autorResponse.Content.ReadFromJsonAsync<ApiEnvelope<AutorViewModel>>(Json))!.Data!;

        var generoResponse = await _client.PostAsJsonAsync(
            "/api/v1/generos",
            new CreateGeneroDto($"Genero Livro {suffix}"));
        generoResponse.StatusCode.ShouldBe(HttpStatusCode.Created);
        var genero = (await generoResponse.Content.ReadFromJsonAsync<ApiEnvelope<GeneroViewModel>>(Json))!.Data!;

        var create = new CreateLivroDto("Mensagem", isbn, 1934, autor.Id, genero.Id);
        var createdResponse = await _client.PostAsJsonAsync("/api/v1/livros", create);
        createdResponse.StatusCode.ShouldBe(HttpStatusCode.Created);
        var created = await createdResponse.Content.ReadFromJsonAsync<ApiEnvelope<LivroViewModel>>(Json);
        created!.Data.ShouldNotBeNull();

        var detailResponse = await _client.GetAsync($"/api/v1/livros/{created.Data.Id}");
        detailResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
        var detail = await detailResponse.Content.ReadFromJsonAsync<ApiEnvelope<LivroDetalheViewModel>>(Json);
        detail!.Data!.AutorNome.ShouldBe(autor.Nome);
        detail.Data.GeneroNome.ShouldBe(genero.Nome);

        var linkedDelete = await _client.DeleteAsync($"/api/v1/autores/{autor.Id}");
        linkedDelete.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);

        var linkedGeneroDelete = await _client.DeleteAsync($"/api/v1/generos/{genero.Id}");
        linkedGeneroDelete.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);

        var update = new UpdateLivroDto("Orpheu", isbn, 1915, autor.Id, genero.Id);
        var updateResponse = await _client.PutAsJsonAsync($"/api/v1/livros/{created.Data.Id}", update);
        updateResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

        var missingAutor = await _client.PostAsJsonAsync(
            "/api/v1/livros",
            new CreateLivroDto("Ghost", $"977{suffix}123".PadRight(13, '0')[..13], 2000, Guid.NewGuid(), genero.Id));
        missingAutor.StatusCode.ShouldBe(HttpStatusCode.NotFound);

        var deleteResponse = await _client.DeleteAsync($"/api/v1/livros/{created.Data.Id}");
        deleteResponse.StatusCode.ShouldBe(HttpStatusCode.NoContent);

        (await _client.GetAsync($"/api/v1/livros/{created.Data.Id}")).StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Create_DuplicateIsbn_ShouldReturnConflict()
    {
        var suffix = Guid.NewGuid().ToString("N")[..8];
        var isbn = $"976{suffix}123".PadRight(13, '0')[..13];

        var autor = (await (await _client.PostAsJsonAsync(
            "/api/v1/autores",
            new CreateAutorDto($"Autor ISBN {suffix}", $"isbn.{suffix}@test.com")))
            .Content.ReadFromJsonAsync<ApiEnvelope<AutorViewModel>>(Json))!.Data!;
        var genero = (await (await _client.PostAsJsonAsync(
            "/api/v1/generos",
            new CreateGeneroDto($"Genero ISBN {suffix}")))
            .Content.ReadFromJsonAsync<ApiEnvelope<GeneroViewModel>>(Json))!.Data!;

        var dto = new CreateLivroDto("First", isbn, 2001, autor.Id, genero.Id);
        (await _client.PostAsJsonAsync("/api/v1/livros", dto)).StatusCode.ShouldBe(HttpStatusCode.Created);
        (await _client.PostAsJsonAsync("/api/v1/livros", dto)).StatusCode.ShouldBe(HttpStatusCode.Conflict);
    }

    private static readonly JsonSerializerOptions Json = new() { PropertyNameCaseInsensitive = true };
}
