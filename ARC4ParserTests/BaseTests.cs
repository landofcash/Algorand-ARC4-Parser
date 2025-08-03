namespace ARC4ParserTests;

using Microsoft.Extensions.Logging;

internal class BaseTests
{
    protected ILogger Logger { get; set; }
        
    [SetUp]
    public void Setup()
    {
        // Configure the logger
        ILoggerFactory _loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Trace);
        });
        Logger = _loggerFactory.CreateLogger<BaseTests>();
    }
}