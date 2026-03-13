using mu88.Shared.OpenTelemetry;
using RaspiFanController.Logic;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureOpenTelemetry("raspifancontroller", builder.Configuration);

builder.Services.AddHealthChecks();
builder.Services.AddAntiforgery(options => options.Cookie.Path = "/");
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddHostedService<Worker>();
builder.Services.AddSingleton<RaspiTemperatureController>();
builder.Services.AddSingleton<ITaskHelper, TaskHelper>();
builder.Services.AddSingleton<ITaskCancellationHelper, TaskCancellationHelper>();
builder.Services.AddOptions<AppSettings>().Bind(builder.Configuration.GetSection(AppSettings.SectionName));

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddSingleton<ITemperatureProvider, DevTemperatureProvider>();
    builder.Services.AddSingleton<IFanController, DevFanController>();
}
else
{
    builder.Services.AddSingleton<ITemperatureProvider, RaspiTemperatureProvider>();
    builder.Services.AddSingleton<IFanController, RaspiFanController.Logic.RaspiFanController>();
}

var app = builder.Build();

app.UsePathBase("/cool");
app.UseRouting();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<RaspiFanController.Components.App>()
    .AddInteractiveServerRenderMode();
app.MapHealthChecks("/healthz");

await app.RunAsync();
