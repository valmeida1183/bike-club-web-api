using BikeClub.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureAuthentication();
builder.ConfigureCompression();
builder.ConfigureCors();
builder.ConfigureDataContext();
builder.ConfigureSwagger();
builder.ConfigureOutputCache();
builder.ConfigureServices();
builder.Services.AddEndpoints();
builder.Services.AddFluentValidation();
builder.Services.AddGlobalExceptionHandling();

var app = builder.Build();

app.LoadSettings();

app.UseGlobalExceptionHandling();

if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseResponseCompression();
app.UseResourceStaticFiles();
app.UseDefaultCors();
app.UseDefaultOutputCache();
app.MapEndpoints();
app.MapControllers();
app.UseSwaggerUi();

app.Run();
