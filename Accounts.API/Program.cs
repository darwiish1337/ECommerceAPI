using Accounts.API.DependencyInjection;
using Accounts.API.JsonConverters;
using Accounts.API.Middlewares;
using Accounts.API.Swagger;
using Accounts.Application.DependencyInjection;
using Accounts.Infrastructure.Configuration;
using Accounts.Infrastructure.DependencyInjection;
using Accounts.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));


// -----------------------------
// 🪵 Logging Serilog 
// -----------------------------
builder.AddLogging(); 

// -----------------------------
// Infrastructure & Persistence
// -----------------------------
builder.Services.AddInfrastructureServices(builder.Configuration);

// -----------------------------
// Application Layer (Services, Mapping, etc.)
// -----------------------------
builder.Services.AddApplicationServices();

//
builder.Services.AddMapping();


//
builder.Services.AddEmailServices(builder.Configuration);

// -----------------------------
// API Layer (e.g. versioning, filters, etc.)
// -----------------------------
builder.Services.AddApiServices();

// -----------------------------
// EF Core Fallback (for Migrations / Default Connection)
// -----------------------------
builder.Services.AddDbContext<AccountsDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("TenantA")); // TEMP only
});

// -----------------------------
// Validation (FluentValidation)
// -----------------------------
builder.Services.AddValidationLayer(); // Registers validators (Scoped), no AutoValidation

// -----------------------------
// Controllers & Behavior config
// -----------------------------
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.SuppressModelStateInvalidFilter = true;
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new EmailJsonConverter());
    });

// -----------------------------
// Swagger + Tenant Header Support
// -----------------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "Accounts API", Version = "v1" });
    options.OperationFilter<AddTenantHeaderOperationFilter>();
});


// -----------------------------
// HTTP Context (for Tenant Provider, etc.)
// -----------------------------
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// -----------------------------
// Middleware Pipeline
// -----------------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Accounts API v1");
        options.RoutePrefix = "swagger";
    });
}

// -----------------------------
// Tenant Middleware
// -----------------------------
app.UseMiddleware<TenantMiddleware>();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<ValidationExceptionMiddleware>();
app.UseMiddleware<LogDividerMiddleware>();

app.MapControllers();
app.Run();