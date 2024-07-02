using Asp.Versioning;
using BackgroundWorker;
using DocumentProcessing.API.Filter;
using DocumentProcessing.API.Service.EditPdf;
using DocumentProcessing.API.Service.OfficeToPdf;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using OfficeToPdf.API.Service;
using Swashbuckle.AspNetCore.Filters;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
var apiVersionBuilder = builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ReportApiVersions = true;
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
});
apiVersionBuilder.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var logLevel = Environment.GetEnvironmentVariable("LOG_LEVEL") ?? "Information";
if (Enum.TryParse<LogLevel>(logLevel, true, out var level))
{
    builder.Logging.SetMinimumLevel(level);
}


builder.Services.AddSwaggerGen(swagger =>
{
    swagger.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory,
       $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"), includeControllerXmlComments: true);
    swagger.ExampleFilters();
    swagger.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = JwtBearerDefaults.AuthenticationScheme
    });

    swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id=JwtBearerDefaults.AuthenticationScheme
                },
                Scheme = "Oauth2",
                Name = JwtBearerDefaults.AuthenticationScheme,
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });

});

builder.Services.AddSwaggerExamplesFromAssemblies(Assembly.GetEntryAssembly());

var licenseKey = Environment.GetEnvironmentVariable("SYNCFUSION_LICENSE_KEY");
if (!string.IsNullOrEmpty(licenseKey))
{
    Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(licenseKey);
}

string host = Environment.GetEnvironmentVariable("PGHOST") ?? "localhost";
string port = Environment.GetEnvironmentVariable("PGPORT") ?? "5432";
string user = Environment.GetEnvironmentVariable("PGUSER") ?? "postgres";
string password = Environment.GetEnvironmentVariable("PGPASSWORD") ?? "root@24";
string database = Environment.GetEnvironmentVariable("PGDATABASE") ?? "testdb";
string workerPoolSize = Environment.GetEnvironmentVariable("WORKER_POOL_SIZE") ?? "3";
string connectionString = $"Server={host};Port={port};User Id={user};Password={password};Database={database}";


builder.Services.AddScoped<IJobStorageService>(service =>
new JobStorageServiceSql(connectionString, "OfficeToPDF"));

// Register services

builder.Services.AddScoped<JWTAuthentication>();
builder.Services.AddScoped<IJobExecutor, JobExecutor>();
builder.Services.AddScoped<IWordToPdfService, WordToPdfService>();
builder.Services.AddScoped<IExcelToPdfService, ExcelToPdfService>();
builder.Services.AddScoped<IPowerpointToPdfService, PowerpointToPdfService>();
builder.Services.AddScoped<IMergePdfService, MergePdfService>();
builder.Services.AddScoped<ISplitPdfService, SplitPdfService>();
builder.Services.AddScoped<IFileStorageService, FileStorageService>();
builder.Services.AddScoped<IRotatePdfService, RotatePdfService>();
builder.Services.AddScoped<IDeletePdfService, DeletePdfServices>();
builder.Services.AddScoped<ICompressPdfService, CompressPdfService>();
builder.Services.AddScoped<IFlattenPdfService, FlattenPdfservice>();
builder.Services.AddScoped<IHtmlToPdfService, HtmlToPdfService>();

builder.Services.AddHttpClient();
builder.Services.AddHostedService(service =>
new BackgroundRunner(workerPoolSize,
service.GetRequiredService<IServiceScopeFactory>(),
service.GetRequiredService<ILogger<BackgroundRunner>>()));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

}

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
