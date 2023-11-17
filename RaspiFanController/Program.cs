﻿using RaspiFanController.Logic;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
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

var appPathBase = builder.Configuration.GetSection(AppSettings.SectionName).Get<AppSettings>()?.AppPathBase;
if (!string.IsNullOrEmpty(appPathBase)) app.UsePathBase(appPathBase);

if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();
else
    app.UseExceptionHandler("/Error");

app.UseStaticFiles();
app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
app.MapControllers();

app.Run();