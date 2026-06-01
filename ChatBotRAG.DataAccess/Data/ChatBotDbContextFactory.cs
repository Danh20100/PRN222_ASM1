using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ChatBotRAG.DataAccess.Data
{
    public class ChatBotDbContextFactory : IDesignTimeDbContextFactory<ChatBotDbContext>
    {
        public ChatBotDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ChatBotDbContext>();
            optionsBuilder.UseSqlServer("Server=LAPTOP-L554J6O;Database=Chat_Bot_Study;User Id=sa;Password=123;TrustServerCertificate=True;");

            return new ChatBotDbContext(optionsBuilder.Options);
        }
    }
}
