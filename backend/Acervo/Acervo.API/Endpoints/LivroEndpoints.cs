using Acervo.API.Common;
using Acervo.Application.DTOs.Livro;
using Acervo.Application.Interfaces.Services;

namespace Acervo.API.Endpoints;

public static class LivroEndpoints
{
    public static void MapLivroEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/livros").WithTags("Livros").WithOpenApi();

        group.MapGet("/", async (ILivroService svc) =>
        {
            var result = await svc.GetAllAsync();
            return Results.Ok(ApiResponse<object>.Ok(result));
        });

        group.MapGet("/{id:guid}", async (Guid id, ILivroService svc) =>
        {
            var result = await svc.GetByIdAsync(id);
            return result.IsFailure
                ? Results.NotFound(ApiResponse<object>.Fail(result.Error))
                : Results.Ok(ApiResponse<object>.Ok(result.Value!));
        });

        group.MapPost("/", async (CreateLivroDto dto, ILivroService svc) =>
        {
            var result = await svc.CreateAsync(dto);
            if (result.IsFailure && result.Error.Contains("não encontrado"))
                return Results.NotFound(ApiResponse<object>.Fail(result.Error));
            if (result.IsFailure)
                return Results.Conflict(ApiResponse<object>.Fail(result.Error));
            return Results.Created($"/api/v1/livros/{result.Value!.Id}", ApiResponse<object>.Ok(result.Value, "Livro criado com sucesso."));
        });

        group.MapPut("/{id:guid}", async (Guid id, UpdateLivroDto dto, ILivroService svc) =>
        {
            var result = await svc.UpdateAsync(id, dto);
            if (result.IsFailure && result.Error.Contains("não encontrado"))
                return Results.NotFound(ApiResponse<object>.Fail(result.Error));
            if (result.IsFailure)
                return Results.Conflict(ApiResponse<object>.Fail(result.Error));
            return Results.Ok(ApiResponse<object>.Ok(result.Value!));
        });

        group.MapDelete("/{id:guid}", async (Guid id, ILivroService svc) =>
        {
            var result = await svc.DeleteAsync(id);
            return result.IsFailure
                ? Results.NotFound(ApiResponse.Fail(result.Error))
                : Results.NoContent();
        });
    }
}
