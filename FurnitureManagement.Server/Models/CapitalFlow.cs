using System;using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FurnitureManagement.Server.Models
{
    public class CapitalFlow
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FlowId { get; set; }

        [Required]
        public DateTime FlowDate { get; set; }

        [Required]
        [StringLength(10)]
        public string? FlowType { get; set; }

        [Required]
        [Column(TypeName = "decimal(12,2)")]
        public decimal Amount { get; set; }

        [StringLength(200)]
        public string? Description { get; set; }

        [Required]
        [StringLength(10)]
        public string? ReferenceType { get; set; }

        public int? ReferenceId { get; set; }

        public int? CreatedBy { get; set; }

        [ForeignKey("CreatedBy")]
        public User? User { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}