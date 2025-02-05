
using Tizpusoft;
using Tizpusoft.Reporting;
using Tizpusoft.Reporting.Options;

Aid.ConfigureAppStart();

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.Load(builder.Environment);
builder.Host.ConfigureLogger(builder.Configuration);
Serilog.Log.Information("{ProductName} v{AppInformationalVersion} started.", Aid.ProductName, Aid.AppInformationalVersion);

builder.Services.ProvideConfigs(builder.Configuration);
builder.WebHost.ConfigWebHost(builder.Configuration.GetSection(WebHostingOptions.ConfigName)?.Get<WebHostingOptions>());

builder.Services.ProvideServices(builder.Configuration);

var app = builder.Build();

app.Services.WarmUp();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();
app.MapEndpoints();
await app.RunAsync();

Serilog.Log.Information("{ProductName} v{AppInformationalVersion} stopped.", Aid.ProductName, Aid.AppInformationalVersion);
await Serilog.Log.CloseAndFlushAsync();
