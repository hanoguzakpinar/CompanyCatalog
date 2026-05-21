using CompanyCatalog.Application.Abstractions.Results;

namespace CompanyCatalog.Api.Extensions;

public static class ResultExtensions
{
    public static IResult ToHttpResult<T>(this Result<T> result, Func<T, IResult>? onSuccess = null)
    {
        if (result.IsSuccess)
        {
            return onSuccess?.Invoke(result.Value) ?? Results.Ok(result.Value);
        }

        return result.Error.Type switch
        {
            ErrorType.NotFound => Results.Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Kaynak bulunamadı",
                detail: result.Error.Message,
                extensions: new Dictionary<string, object?>
                {
                    ["code"] = result.Error.Code
                }
            ),

            ErrorType.Validation => Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Validation Hatası",
                detail: result.Error.Message,
                extensions: new Dictionary<string, object?>
                {
                    ["code"] = result.Error.Code
                }
            ),

            ErrorType.Conflict => Results.Problem(
                statusCode: StatusCodes.Status409Conflict,
                title: "Conflict",
                detail: result.Error.Message,
                extensions: new Dictionary<string, object?>
                {
                    ["code"] = result.Error.Code
                }
            ),

            ErrorType.Unauthorized => Results.Problem(
                statusCode: StatusCodes.Status401Unauthorized,
                title: "Yetkisiz erişim",
                detail: result.Error.Message
            ),

            ErrorType.Forbidden => Results.Problem(
                statusCode: StatusCodes.Status403Forbidden,
                title: "Yasaklı",
                detail: result.Error.Message
            ),

            _ => Results.Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Bir hata meydana geldi.",
                detail: result.Error.Message
            )
        };
    }
}