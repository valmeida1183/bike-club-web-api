using BikeClub.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureAuthentication();
builder.ConfigureCompression();
builder.ConfigureCors();
builder.ConfigureDataContext();
builder.ConfigureSwagger();
builder.ConfigureOutputCache();
builder.ConfigureServices();

var app = builder.Build();

app.LoadSettings();

if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseResponseCompression();
app.UseResourceStaticFiles();
app.UseDefaultCors();
app.UseDefaultOutputCache();
app.MapControllers();
app.UseSwaggerUi();

app.Run();
