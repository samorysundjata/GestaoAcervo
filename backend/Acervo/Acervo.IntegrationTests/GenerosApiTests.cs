using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Acervo.Application.DTOs.Genero;
using Acervo.Application.ViewModels;
using Shouldly;

namespace Acervo.IntegrationTests;

[Collection(nameof(ApiCollection))]
public sealed class GenerosApiTests
{
    private readonly HttpClient _client;

    public GenerosApiTests(ApiFactory factory) => _client = factory.CreateClient();

    [Fact]
    public async Task Crud_ShouldPersistGenero()
    {
        var suffix = Guid.NewGuid().ToString("N")[..8];
        var create = new CreateGeneroDto($"Genero {suffix}");

        var createdResponse = await _client.PostAsJsonAsync("/api/v1/generos", create);
        createdResponse.StatusCode.ShouldBe(HttpStatusCode.Created);
        var created = await createdResponse.Content.ReadFromJsonAsync<ApiEnvelope<GeneroViewModel>>(Json);
        created!.Data.ShouldNotBeNull();

        var getResponse = await _client.GetAsync($"/api/v1/generos/{created.Data.Id}");
        getResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

        var update = new UpdateGeneroDto($"Genero Edit {suffix}");
        var updateResponse = await _client.PutAsJsonAsync($"/api/v1/generos/{created.Data.Id}", update);
        updateResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

        var deleteResponse = await _client.DeleteAsync($"/api/v1/generos/{created.Data.Id}");
        deleteResponse.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Create_DuplicateNome_ShouldReturnConflict()
    {
        var nome = $"Dup {Guid.NewGuid():N}"[..20];
        var dto = new CreateGeneroDto(nome);

        (await _client.PostAsJsonAsync("/api/v1/generos", dto)).StatusCode.ShouldBe(HttpStatusCode.Created);
        (await _client.PostAsJsonAsync("/api/v1/generos", dto)).StatusCode.ShouldBe(HttpStatusCode.Conflict);
    }

    private static readonly JsonSerializerOptions Json = new() { PropertyNameCaseInsensitive = true };
}
