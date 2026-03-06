namespace Acervo.API.Common;

public class ApiResponse<T>
{
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;
    public T? Data { get; init; }
    public IEnumerable<string>? Errors { get; init; }

    public static ApiResponse<T> Ok(T data, string message = "Operação realizada com sucesso.") =>
        new() { Success = true, Message = message, Data = data };

    public static ApiResponse<T> Fail(string error) =>
        new() { Success = false, Message = "Não foi possível concluir a operação.", Errors = [error] };

    public static ApiResponse<T> Fail(IEnumerable<string> errors) =>
        new() { Success = false, Message = "Não foi possível concluir a operação.", Errors = errors };
}

public class ApiResponse
{
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;
    public IEnumerable<string>? Errors { get; init; }

    public static ApiResponse Ok(string message = "Operação realizada com sucesso.") =>
        new() { Success = true, Message = message };

    public static ApiResponse Fail(string error) =>
        new() { Success = false, Message = "Não foi possível concluir a operação.", Errors = [error] };
}
