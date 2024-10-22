using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Reflection;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using Microsoft.Extensions.Options;
using ProveedoresCrud.Repositories;
using ProveedoresCrud.Services;
using proveedoresCrud.config;
using ProveedoresCrud.Middlewares;
using proveedoresCrud.models;
using System.Net;
using Swashbuckle.AspNetCore.SwaggerUI;

var builder = WebApplication.CreateBuilder(args);

// Configurar MongoDB
builder.Services.Configure<MongoSettings>(
    builder.Configuration.GetSection("MongoSettings"));

builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var mongoSettings = sp.GetRequiredService<IOptions<MongoSettings>>().Value;
    return new MongoClient(mongoSettings.ConnectionString);
});

builder.Services.AddScoped(sp =>
{
    var mongoSettings = sp.GetRequiredService<IOptions<MongoSettings>>().Value;
    var client = sp.GetRequiredService<IMongoClient>();
    return client.GetDatabase(mongoSettings.DatabaseName);
});

// Inyección de dependencias de Repositorios y Servicios
builder.Services.AddScoped<IProveedorRepository, ProveedorRepository>();
builder.Services.AddScoped<IProveedorService, ProveedorService>();

// Registrar la configuración de JWT
var environment = builder.Environment.IsProduction() ? "Production" : "Development";
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection($"Jwt:{environment}"));
builder.Services.AddSingleton(sp =>
{
    var jwtSettings = builder.Configuration.GetSection($"Jwt:{environment}").Get<JwtSettings>();
    return jwtSettings;
});

// Configuración de JWT
var jwtConfig = builder.Configuration.GetSection($"Jwt:{environment}").Get<JwtSettings>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtConfig.Issuer,
            ValidAudience = jwtConfig.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.SecretKey)),
            ClockSkew = TimeSpan.Zero 
        };

        options.Events = new JwtBearerEvents
        {
            OnChallenge = context =>
            {
                context.HandleResponse();

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                var response = new ApiResponse<string>("Unauthorized. El token de acceso es inválido o no se proporcionó.", false);

                return context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response));
            }
        };
    });

// Agregar controladores
builder.Services.AddControllers();
// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowOrigin",
    builder =>
    {
        builder.WithOrigins("https://pruebatec-hkgbdpgfhgh5bfd6.canadacentral-01.azurewebsites.net", "https://localhost:7296")
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});
// Configurar Swagger con documentación XML
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Proveedores API",
        Version = "v1",
        Description = "API para gestionar proveedores"
    });

    // Incluir los comentarios XML generados
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

    // Configurar Swagger para usar JWT
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingrese 'Bearer' seguido del token JWT."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Proveedores API v1");
    });
}
// Habilitar Swagger tanto en desarrollo como en producción
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Proveedores API v1");
    c.DocExpansion(DocExpansion.None); // Ajustar según tus preferencias
});
app.UseHttpsRedirection();

// Agregar autenticación y autorización
app.UseAuthentication();
app.UseAuthorization();

// Agregar middleware de manejo global de errores
app.UseErrorHandlerMiddleware();

// Mapear los controladores
app.MapControllers();
app.Run();
