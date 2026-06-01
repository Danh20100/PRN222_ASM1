using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatBotRAG.DataAccess.Models
{
    [Table("documents")]
    public class Document
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("subject_id")]
        public Guid? SubjectId { get; set; }

        [Required]
        [Column("title")]
        [MaxLength(500)]
        public string Title { get; set; } = string.Empty;

        [Column("file_type")]
        [MaxLength(50)]
        public string? FileType { get; set; }

        [Column("file_size")]
        public long? FileSize { get; set; }

        [Column("file_url")]
        [MaxLength(1000)]
        public string? FileUrl { get; set; }

        [Column("status")]
        [MaxLength(50)]
        public string Status { get; set; } = "pending"; // pending, processing, completed, failed

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        [ForeignKey("SubjectId")]
        public Subject? Subject { get; set; }
        public ICollection<DocumentChunk> Chunks { get; set; } = new List<DocumentChunk>();
    }
}
