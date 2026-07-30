using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using RotaCerta.API.Middlewares;
using RotaCerta.Extensions;
using RotaCerta.Infraestructure.Context;
using Scalar.AspNetCore;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // enums aparecem como string no JSON
        options.JsonSerializerOptions.Converters
            .Add(new JsonStringEnumConverter());
    });

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource
        .AddService(
            serviceName: "RotaCerta.API",
            serviceVersion: "1.0.0"))
    .WithTracing(tracing => tracing
        .AddAspNetCoreInstrumentation(opts =>
        {
            opts.Filter = ctx =>
                !ctx.Request.Path.StartsWithSegments("/health");
        })
        .AddEntityFrameworkCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddConsoleExporter()
        .AddOtlpExporter(otlp =>
        {
            var baseEndpoint = builder.Configuration["OpenTelemetry:Endpoint"]!;
            otlp.Endpoint = new Uri(baseEndpoint + "/v1/traces");
            otlp.Headers = builder.Configuration["OpenTelemetry:Headers"];
            otlp.Protocol = OtlpExportProtocol.HttpProtobuf;
        }))
    .WithMetrics(metrics => metrics
        .AddAspNetCoreInstrumentation()
        .AddRuntimeInstrumentation()
        .AddOtlpExporter(otlp =>
        {
            var baseEndpoint = builder.Configuration["OpenTelemetry:Endpoint"]!;
            otlp.Endpoint = new Uri(baseEndpoint + "/v1/metrics");
            otlp.Headers = builder.Configuration["OpenTelemetry:Headers"];
            otlp.Protocol = OtlpExportProtocol.HttpProtobuf;
        }));

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, ct) =>
    {
        document.Components ??= new();
        document.Components.SecuritySchemes = new Dictionary<string, IOpenApiSecurityScheme>
        {
            ["Bearer"] = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Description = "Informe o token JWT no campo abaixo."
            }
        };
        return Task.CompletedTask;
    });
});

builder.Services.AddContextAppRotaCerta(builder.Configuration);
builder.Services.AddInjectionsDepedency(builder.Configuration);
builder.Services.AddJwtAuthetication(builder.Configuration);
builder.Services.AddSupabaseStorage(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddPolicy("RotaCertaFront", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:3000",
                "http://localhost:3001",
                "https://app-gestao-motorista-front.vercel.app" )   // ← portas do Next.js
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});


var app = builder.Build();

//Teste OpenTelemetry
var endpoint = builder.Configuration["OpenTelemetry:Endpoint"];
var headers = builder.Configuration["OpenTelemetry:Headers"];
Console.WriteLine($"OTel Endpoint: {endpoint}");
Console.WriteLine($"OTel Headers: {headers?.Substring(0, 20)}...");

app.UseMiddleware<GlobalExceptionMiddleware>();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider
        .GetRequiredService<DbRotaCertaContext>();
    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.Title = "RotaCerta API";
        options.Theme = ScalarTheme.DeepSpace;
        options.DefaultHttpClient = new(ScalarTarget.CSharp, ScalarClient.HttpClient);
        options.Authentication = new ScalarAuthenticationOptions
        {
            PreferredSecurityScheme = "Bearer"
        };
    });
}

var portaRender = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrWhiteSpace(portaRender))
{
    builder.WebHost.UseUrls($"http://+:{portaRender}");
}

// ... resto das configurações ...

app.UseCors("RotaCertaFront");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run(); // ← só isso no final

