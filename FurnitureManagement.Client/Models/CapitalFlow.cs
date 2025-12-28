using System;

namespace FurnitureManagement.Client.Models
{
    public class CapitalFlow
    {
        public int FlowId { get; set; }
        public DateTime FlowDate { get; set; }
        public string FlowType { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public string ReferenceType { get; set; }
        public int? ReferenceId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CapitalFlowSummary
    {
        public decimal TotalIncome { get; set; }
        public decimal TotalExpense { get; set; }
        public decimal Balance { get; set; }
    }
}