using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatBotRAG.DataAccess.Models
{
    [Table("message_citations")]
    public class MessageCitation
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("message_id")]
        public Guid MessageId { get; set; }

        [Column("chunk_id")]
        public Guid ChunkId { get; set; }

        // Navigation
        [ForeignKey("MessageId")]
        public ChatMessage? Message { get; set; }

        [ForeignKey("ChunkId")]
        public DocumentChunk? Chunk { get; set; }
    }
}
