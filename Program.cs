using System.Text;
using BikeClub;
using BikeClub.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

ConfigureAuthentication(builder);
ConfigureCompression(builder);
ConfigureCORS(builder);
ConfigureControllers(builder);
ConfigureDataContext(builder);
ConfigureSwagger(builder);

var app = builder.Build();

LoadSettings(app);

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection(); // força o redirecionamento para o https
app.UseAuthentication();
app.UseAuthorization();
app.UseResponseCompression();

// Condig para permitir a WEb api servir arquivos estáticos (ex: imagens) em uma pasta diferente de "wwwroot".
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(app.Environment.ContentRootPath, "Resources")),
    RequestPath = "/Resources"
});

app.UseCors(x => x
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader()
);
app.MapControllers();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Bike Club Api Version: 1");
});

app.Run();

void ConfigureAuthentication(WebApplicationBuilder builder)
{
    // Configs de Autenticação e Jwt
    var secret = builder.Configuration.GetValue<string>("Secret") ?? Guid.NewGuid().ToString();
    var key = Encoding.ASCII.GetBytes(secret);
    builder.Services.AddAuthentication(a =>
    {
        a.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        a.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(j =>
    {
        // j.RequireHttpsMetadata = false;
        // j.SaveToken = true;
        j.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });
}

void ConfigureCompression(WebApplicationBuilder builder)
{
    // Configura o Brotli e compacta todo o response que for do mimeType "application/json"
    builder.Services.AddResponseCompression(opt =>
    {
        opt.EnableForHttps = true;
        opt.Providers.Add<BrotliCompressionProvider>();
        opt.MimeTypes = ResponseCompressionDefaults.MimeTypes
            .Concat(new[] { "application/json; charset=utf-8" });
    });
}

void ConfigureCORS(WebApplicationBuilder builder)
{
    // adiciona o CORS
    builder.Services.AddCors();
}

void ConfigureControllers(WebApplicationBuilder builder)
{
    builder.Services.AddControllers();
}

void ConfigureDataContext(WebApplicationBuilder builder)
{
    // Config do EF com database em memória
    //services.AddDbContext<DataContext>(opt => opt.UseInMemoryDatabase("MyMemoryDatabase"));

    //Config do EF com databse real
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    builder.Services.AddDbContext<DataContext>(opt => opt.UseSqlServer(connectionString));
}

void ConfigureSwagger(WebApplicationBuilder builder)
{
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Bike Club Api",
                Version = "v1"
            });
        });
}

void LoadSettings(WebApplication app)
{
    Settings.Secret = app.Configuration.GetValue<string>("Secret");
    Settings.TokenExpirationHours = app.Configuration.GetValue<int>("TokenExpirationHours");
}
