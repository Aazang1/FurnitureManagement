using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FurnitureManagement.Server.Models
{
    public class Warehouse
    {
        [Key]
        [Column("warehouse_id")]
        public int WarehouseId { get; set; }

        [Column("warehouse_name")]
        public string WarehouseName { get; set; } = string.Empty;

        [Column("location")]
        public string? Location { get; set; }

        [Column("capacity")]
        public int? Capacity { get; set; }

        [Column("manager")]
        public string? Manager { get; set; }

        [Column("created_by")]
        public int? CreatedBy { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}