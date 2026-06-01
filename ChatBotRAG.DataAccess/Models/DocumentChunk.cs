using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatBotRAG.DataAccess.Models
{
    [Table("document_chunks")]
    public class DocumentChunk
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("document_id")]
        public Guid DocumentId { get; set; }

        [Column("chunk_index")]
        public int ChunkIndex { get; set; }

        [Column("content")]
        public string Content { get; set; } = string.Empty;

        [Column("page_number")]
        public int? PageNumber { get; set; }

        /// <summary>
        /// Embedding vector stored as JSON string (float array)
        /// </summary>
        [Column("vector_db_id")]
        [MaxLength(4000)]
        public string? VectorDbId { get; set; }

        // Navigation
        [ForeignKey("DocumentId")]
        public Document? Document { get; set; }
        public ICollection<MessageCitation> Citations { get; set; } = new List<MessageCitation>();
    }
}
