using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FurnitureManagement.Server.Models
{
    public class SaleOrder
    {
        [Key]
        [Column("sale_id")]
        public int SaleId { get; set; }

        [Column("customer_name")]
        public string CustomerName { get; set; } = string.Empty;

        [Column("customer_phone")]
        public string CustomerPhone { get; set; } = string.Empty;

        [Column("sale_date")]
        public DateTime SaleDate { get; set; }

        [Column("total_amount")]
        public decimal TotalAmount { get; set; }

        [Column("discount")]
        public decimal Discount { get; set; } = 0.00m;

        [Column("final_amount")]
        public decimal FinalAmount { get; set; }

        [Column("status")]
        public string Status { get; set; } = "pending";

        [Column("created_by")]
        public int? CreatedBy { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation property
        public List<SaleDetail> SaleDetails { get; set; } = new List<SaleDetail>();
    }
}