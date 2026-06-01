using ChatBotRAG.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatBotRAG.DataAccess.Data
{
    public class ChatBotDbContext : DbContext
    {
        public ChatBotDbContext(DbContextOptions<ChatBotDbContext> options) : base(options) { }

        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<DocumentChunk> DocumentChunks { get; set; }
        public DbSet<ChatSession> ChatSessions { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<MessageCitation> MessageCitations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Subject
            modelBuilder.Entity<Subject>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            });

            // Document
            modelBuilder.Entity<Document>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Status).HasDefaultValue("pending");
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.HasOne(e => e.Subject)
                      .WithMany(s => s.Documents)
                      .HasForeignKey(e => e.SubjectId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            // DocumentChunk
            modelBuilder.Entity<DocumentChunk>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Content).HasColumnType("nvarchar(max)");
                entity.HasOne(e => e.Document)
                      .WithMany(d => d.Chunks)
                      .HasForeignKey(e => e.DocumentId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ChatSession
            modelBuilder.Entity<ChatSession>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            });

            // ChatMessage
            modelBuilder.Entity<ChatMessage>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Content).HasColumnType("nvarchar(max)");
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.HasOne(e => e.Session)
                      .WithMany(s => s.Messages)
                      .HasForeignKey(e => e.SessionId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // MessageCitation
            modelBuilder.Entity<MessageCitation>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Message)
                      .WithMany(m => m.Citations)
                      .HasForeignKey(e => e.MessageId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Chunk)
                      .WithMany(c => c.Citations)
                      .HasForeignKey(e => e.ChunkId)
                      .OnDelete(DeleteBehavior.NoAction);
            });

            // Seed data to prevent foreign key errors when inserting Documents
            modelBuilder.Entity<Subject>().HasData(
                new Subject { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), Name = "General", Description = "General Documents", CreatedAt = DateTime.UtcNow }
            );
        }
    }
}
