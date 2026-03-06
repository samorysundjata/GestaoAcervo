using Acervo.API.Common;
using Acervo.Application.DTOs.Genero;
using Acervo.Application.Interfaces.Services;

namespace Acervo.API.Endpoints;

public static class GeneroEndpoints
{
    public static void MapGeneroEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/generos").WithTags("Gêneros").WithOpenApi();

        group.MapGet("/", async (IGeneroService svc) =>
        {
            var result = await svc.GetAllAsync();
            return Results.Ok(ApiResponse<object>.Ok(result));
        });

        group.MapGet("/{id:guid}", async (Guid id, IGeneroService svc) =>
        {
            var result = await svc.GetByIdAsync(id);
            return result.IsFailure
                ? Results.NotFound(ApiResponse<object>.Fail(result.Error))
                : Results.Ok(ApiResponse<object>.Ok(result.Value!));
        });

        group.MapPost("/", async (CreateGeneroDto dto, IGeneroService svc) =>
        {
            var result = await svc.CreateAsync(dto);
            return result.IsFailure
                ? Results.Conflict(ApiResponse<object>.Fail(result.Error))
                : Results.Created($"/api/v1/generos/{result.Value!.Id}", ApiResponse<object>.Ok(result.Value, "Gênero criado com sucesso."));
        });

        group.MapPut("/{id:guid}", async (Guid id, UpdateGeneroDto dto, IGeneroService svc) =>
        {
            var result = await svc.UpdateAsync(id, dto);
            if (result.IsFailure && result.Error.Contains("não encontrado"))
                return Results.NotFound(ApiResponse<object>.Fail(result.Error));
            if (result.IsFailure)
                return Results.Conflict(ApiResponse<object>.Fail(result.Error));
            return Results.Ok(ApiResponse<object>.Ok(result.Value!));
        });

        group.MapDelete("/{id:guid}", async (Guid id, IGeneroService svc) =>
        {
            var result = await svc.DeleteAsync(id);
            if (result.IsFailure && result.Error.Contains("não encontrado"))
                return Results.NotFound(ApiResponse.Fail(result.Error));
            if (result.IsFailure)
                return Results.UnprocessableEntity(ApiResponse.Fail(result.Error));
            return Results.NoContent();
        });
    }
}