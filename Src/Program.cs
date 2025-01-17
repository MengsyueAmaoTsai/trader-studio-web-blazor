using Microsoft.AspNetCore.HttpOverrides;

using RichillCapital.Infrastructure.Identity;
using RichillCapital.Infrastructure.Logging;
using RichillCapital.Infrastructure.Resources;
using RichillCapital.TraderStudio.Web.Components;
using RichillCapital.TraderStudio.Web.Middlewares;

using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Infrastructure - Logging
builder.WebHost.UseCustomLogger();
builder.Services.AddSerilog();

// Infrastructure - Identity
builder.Services.AddCustomIdentity();

// Infrastructure - Resources
builder.Services.AddResourceApiService();

// Presentation - Razor Components
builder.Services.AddComponents();
builder.Services.AddMiddlewares();

builder.Services.AddCors(builder =>
{
    builder
        .AddDefaultPolicy(policy =>
        {
            policy
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowAnyOrigin();
        });
});

builder.Services
    .Configure<ForwardedHeadersOptions>(options =>
    {
        options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
        options.KnownNetworks.Clear();
        options.KnownProxies.Clear();
    });

var app = builder.Build();

app.UseForwardedHeaders();

app.UseRequestDebuggingMiddleware();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler(
        "/error",
        createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseCookiePolicy();

app.UseRouting();

app.UseCors();

app.UseAntiforgery();
app.UseAuthentication();
app.UseAuthorization();

app.MapComponents<App>();

await app.RunAsync();


public partial class Program;
