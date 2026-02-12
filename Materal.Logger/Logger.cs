using System.Text.RegularExpressions;

namespace Materal.Logger;

/// <summary>
/// 日志
/// </summary>
internal partial class Logger(string categoryName, LoggerOptions options, ILoggerHost loggerHost, ILoggerListener loggerListener) : ILogger
{
    internal LoggerOptions Options { get; set; } = options;
    private readonly LoggerExternalScopeProvider _scopeProvider = new();
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel)) return;
        LoggerScope loggerScope = new(_scopeProvider);
        string message = string.Empty;
        if (state is IReadOnlyList<KeyValuePair<string, object?>> logValues)
        {
            for (int i = 0; i < logValues.Count; i++)
            {
                try
                {
                    KeyValuePair<string, object?> logValue = logValues[i];
                    string key = logValue.Key;
                    if (key == "{OriginalFormat}")
                    {
                        message = logValue.Value?.ToString() ?? string.Empty;
                        continue;
                    }
                    loggerScope.ScopeData[key] = logValue.Value;
                }
                catch
                {
                }
            }
        }
        message = Formatter(message, loggerScope, exception);
        int threadID = Environment.CurrentManagedThreadId;
        Log log = new(Options.Application, logLevel, eventId, categoryName, message, exception, threadID, loggerScope);
        loggerHost.Log(log);
        loggerListener.Log(log);
    }
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => state switch
    {
        LoggerScope => _scopeProvider.Push(state),
        _ => _scopeProvider.Push(new LoggerScope(state)),
    };
    public bool IsEnabled(LogLevel logLevel) => logLevel >= Options.MinLevel && logLevel <= Options.MaxLevel;
    public static string Formatter(string message, LoggerScope loggerScope, Exception? exception)
    {
        string result = Abstractions.Log.ApplyText(message, loggerScope.ScopeData);
#if NET
        Regex regex = ExpressionRegex();
#else
        Regex regex = new(@"\{[^\}]+\}");
#endif
        result = Abstractions.Log.ApplyText(result, regex, 1, loggerScope.ScopeData);
        return result;
    }
#if NET
    /// <summary>
    /// 模版表达式
    /// </summary>
    /// <returns></returns>
    [GeneratedRegex(@"\{[^\}]+\}")]
    private static partial Regex ExpressionRegex();
#endif
}
