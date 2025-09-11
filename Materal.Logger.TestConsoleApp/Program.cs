using Materal.Logger.Abstractions.Extensions;
using Materal.Logger.ConsoleLogger;
using Materal.Logger.Extensions;
using Materal.Logger.MongoLogger;
using Materal.Utils.Extensions;
using Materal.Utils.MongoDB.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Materal.Logger.TestConsoleApp
{
    internal class Program
    {
        private const string _textFormat = "${DateTime}|${Application}|${Level}|${Scope}\r\n${Message}\r\n${Exception}";
        const string connectionString = "mongodb://admin:Materal%40123456@localhost:27017/";
        public static void Main()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddMateralUtils();
            services.AddMongoUtils();
            services.AddMateralLogger(option =>
            {
                option.AddConsoleTarget("ConsoleLog", _textFormat);
                option.AddMongoTarget("MongoDBLog", connectionString, "Logs", "TestConsoleAppLogs");
                option.AddAllTargetsRule();
            });
            IServiceProvider serviceProvider = services.BuildServiceProvider();
            ILogger<Program> logger = serviceProvider.GetRequiredService<ILogger<Program>>();
            TestModel testModel = new();
            logger.LogInformation("Hello World!_{Arg1}", testModel);
            logger.LogInformation("Hello World!_{Arg1}_{Arg1.Number1}", testModel);
            logger.LogInformation("Hello World!_{Arg1:none}_{Arg1.Number1}", testModel);
            logger.LogInformation("Hello World!_{Arg1:json}_{Arg1.Number1}", testModel);
        }
    }
    public class TestModel
    {
        public int Number1 { get; set; } = 1;
        public int Number2 { get; set; } = 2;
        public string String1 { get; set; } = "String1";
        public string String2 { get; set; } = "String2";
        public DateTime NowTime { get; set; } = DateTime.Now;
        public override string ToString() => "TestModel";
    }
}
