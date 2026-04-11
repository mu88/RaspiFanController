// [LoggerMessage] partial method declarations live here because the source generator
// cannot process declarations inside .razor @code blocks — it runs before the Razor
// compiler emits the partial class. Business logic stays in Index.razor.
namespace RaspiFanController.Components.Pages;

public sealed partial class Index
{
    [LoggerMessage(Level = LogLevel.Warning, SkipEnabledCheck = true, Message = "Failed to parse fan state from '{Value}', defaulting to fan on to avoid overheating")]
    private static partial void LogFanStateParseFailure(ILogger logger, string? value);

    [LoggerMessage(Level = LogLevel.Error, SkipEnabledCheck = true, Message = "Error refreshing UI state")]
    private static partial void LogRefreshError(ILogger logger, Exception ex);
}
