using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args).Inject();
builder.Host.UseSerilogDefault(config =>
{
    string date = DateTime.Now.ToString("yyyy-MM-dd");//按时间创建文件夹
    string outputTemplate = "{NewLine}【{Level:u3}】{Timestamp:yyyy-MM-dd HH:mm:ss.fff}" +
    "{NewLine}#Msg#{Message:lj}" +
    "{NewLine}#Pro #{Properties:j}" +
    "{NewLine}#Exc#{Exception}" +
    new string('-', 50);//输出模板
    config
         .MinimumLevel.Debug() // 所有Sink的最小记录级别
         .MinimumLevel.Override("Microsoft", LogEventLevel.Fatal)
         .Enrich.FromLogContext()
         .WriteTo.Console(outputTemplate: outputTemplate);
});
var app = builder.Build();
app.Run();