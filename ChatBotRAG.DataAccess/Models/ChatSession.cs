using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatBotRAG.DataAccess.Models
{
    [Table("chat_sessions")]
    public class ChatSession
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("title")]
        [MaxLength(500)]
        public string? Title { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public ICollection<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
    }
}
