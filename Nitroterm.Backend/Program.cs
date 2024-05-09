using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Nitroterm.Backend.Database;
using Nitroterm.Backend.Middleware;
using Nitroterm.Backend.Utilities;

Secrets.Load();

try
{
    using NitrotermDbContext db = new();
    db.Database.Migrate();
}
catch (Exception e)
{
    Console.WriteLine(e);
}

FirebaseApp.Create(new AppOptions()
{
    Credential = GoogleCredential.FromJson(Secrets.Instance.Firebase),
    ProjectId = "nitroterm-be30d"
});

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCors(options =>
{
    options.AddPolicy("cors", policy =>
    {
        policy.AllowAnyOrigin();
        policy.AllowAnyHeader();
        policy.AllowAnyMethod();
    });
});
#if DEBUG
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1",
        new OpenApiInfo
        {
            Title = "Nitroterm.Backend",
            Version = "v1"
        }
    );

    var filePath = Path.Combine(System.AppContext.BaseDirectory, "Nitroterm.Backend.xml");
    c.IncludeXmlComments(filePath);
});

builder.Services.AddSwaggerGen(c =>
{
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "JWT Authentication",
        Description = "Enter JWT Bearer token **_only_**",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer", // must be lower case
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Id = "jwt",
            Type = ReferenceType.SecurityScheme
        }
    };
    
    c.AddServer(new OpenApiServer()
    {
        Description = "Local Server",
        Url = "http://localhost:5168"
    });
    c.AddServer(new OpenApiServer()
    {
        Description = "Production Server",
        Url = "https://services.cacahuete.dev"
    });
    
    c.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {securityScheme, []}
    });
});
#endif

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.UseHttpsRedirection();
app.UseCors("cors");
app.UseMiddleware<BruteforceProtectionMiddleware>();
app.UseMiddleware<JwtMiddleware>();

app.Run();