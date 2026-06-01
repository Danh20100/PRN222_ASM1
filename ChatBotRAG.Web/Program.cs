using ChatBotRAG.DataAccess.Data;
using ChatBotRAG.DataAccess.Repositories;
using ChatBotRAG.Services.Implementations;
using ChatBotRAG.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Database context
builder.Services.AddDbContext<ChatBotDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("ChatBotRAG.DataAccess")));

// Repositories
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();

// Services
builder.Services.AddScoped<IDocumentService, DocumentService>();
builder.Services.AddScoped<IChatService, ChatService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Auto apply migrations (optional)
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ChatBotDbContext>();
    try
    {
        dbContext.Database.Migrate();
    }
    catch (Exception ex)
    {
        Console.WriteLine("Warning: Migration failed -> " + ex.Message);
    }
    
    // Ensure seed data exists regardless of migration success
    try
    {
        var seedId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        if (!dbContext.Subjects.Any(s => s.Id == seedId))
        {
            dbContext.Subjects.Add(new ChatBotRAG.DataAccess.Models.Subject
            {
                Id = seedId,
                Name = "General",
                Description = "General Documents",
                CreatedAt = DateTime.UtcNow
            });
            dbContext.SaveChanges();
            Console.WriteLine("Seed Subject created.");
        }
    }
    catch (Exception seedEx)
    {
        Console.WriteLine("Warning: Seed failed -> " + seedEx.Message);
    }
}

app.Run();
