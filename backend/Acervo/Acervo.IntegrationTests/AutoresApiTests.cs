using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Acervo.Application.DTOs.Autor;
using Acervo.Application.ViewModels;
using Shouldly;

namespace Acervo.IntegrationTests;

[Collection(nameof(ApiCollection))]
public sealed class AutoresApiTests
{
    private readonly HttpClient _client;

    public AutoresApiTests(ApiFactory factory) => _client = factory.CreateClient();

    [Fact]
    public async Task Crud_ShouldPersistAutor()
    {
        var suffix = Guid.NewGuid().ToString("N")[..8];
        var create = new CreateAutorDto($"Autor {suffix}", $"autor.{suffix}@test.com");

        var createdResponse = await _client.PostAsJsonAsync("/api/v1/autores", create);
        createdResponse.StatusCode.ShouldBe(HttpStatusCode.Created);
        var created = await createdResponse.Content.ReadFromJsonAsync<ApiEnvelope<AutorViewModel>>(Json);
        created!.Success.ShouldBeTrue();
        created.Data.ShouldNotBeNull();
        created.Data.Email.ShouldBe(create.Email);

        var getResponse = await _client.GetAsync($"/api/v1/autores/{created.Data.Id}");
        getResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

        var update = new UpdateAutorDto($"Autor Edit {suffix}", $"autor.edit.{suffix}@test.com");
        var updateResponse = await _client.PutAsJsonAsync($"/api/v1/autores/{created.Data.Id}", update);
        updateResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
        var updated = await updateResponse.Content.ReadFromJsonAsync<ApiEnvelope<AutorViewModel>>(Json);
        updated!.Data!.Nome.ShouldBe(update.Nome);

        var listResponse = await _client.GetAsync("/api/v1/autores");
        listResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
        var list = await listResponse.Content.ReadFromJsonAsync<ApiEnvelope<List<AutorViewModel>>>(Json);
        list!.Data!.ShouldContain(a => a.Id == created.Data.Id);

        var deleteResponse = await _client.DeleteAsync($"/api/v1/autores/{created.Data.Id}");
        deleteResponse.StatusCode.ShouldBe(HttpStatusCode.NoContent);

        var missing = await _client.GetAsync($"/api/v1/autores/{created.Data.Id}");
        missing.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Create_DuplicateEmail_ShouldReturnConflict()
    {
        var suffix = Guid.NewGuid().ToString("N")[..8];
        var dto = new CreateAutorDto($"Autor {suffix}", $"dup.{suffix}@test.com");

        (await _client.PostAsJsonAsync("/api/v1/autores", dto)).StatusCode.ShouldBe(HttpStatusCode.Created);
        (await _client.PostAsJsonAsync("/api/v1/autores", dto)).StatusCode.ShouldBe(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task GetById_Unknown_ShouldReturnNotFound()
    {
        var response = await _client.GetAsync($"/api/v1/autores/{Guid.NewGuid()}");
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    private static readonly JsonSerializerOptions Json = new() { PropertyNameCaseInsensitive = true };
}
