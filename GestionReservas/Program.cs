using GestionReservas.data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Register SoapClient as a singleton service
builder.Services.AddSingleton<SoapClient>();

// Configurar la conexión a MySQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 36))
    ));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers(); // Configura los controladores

    // Imprime todas las rutas configuradas
    var routeBuilder = endpoints.DataSources.SelectMany(ds => ds.Endpoints);
    foreach (var endpoint in routeBuilder)
    {
        if (endpoint is RouteEndpoint routeEndpoint)
        {
            Console.WriteLine($"Route: {routeEndpoint.RoutePattern.RawText}");
        }
    }
});

app.UseAuthorization();

app.MapRazorPages();

app.Run();