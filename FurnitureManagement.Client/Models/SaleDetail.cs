using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FurnitureManagement.Client.Models
{
    /// <summary>
    /// 销售明细模型
    /// </summary>
    public class SaleDetail : INotifyPropertyChanged
    {
        /// <summary>
        /// 明细ID
        /// </summary>
        public int DetailId { get; set; }

        /// <summary>
        /// 销售订单ID
        /// </summary>
        public int SaleId { get; set; }

        /// <summary>
        /// 家具ID
        /// </summary>
        public int FurnitureId { get; set; }

        private int _quantity;
        /// <summary>
        /// 销售数量
        /// </summary>
        public int Quantity
        {
            get { return _quantity; }
            set 
            { 
                _quantity = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TotalPrice));
            }
        }

        private decimal _unitPrice;
        /// <summary>
        /// 销售单价
        /// </summary>
        public decimal UnitPrice
        {
            get { return _unitPrice; }
            set 
            { 
                _unitPrice = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TotalPrice));
            }
        }

        /// <summary>
        /// 销售总价
        /// </summary>
        public decimal TotalPrice
        {
            get { return Quantity * UnitPrice; }
        }

        /// <summary>
        /// 出库仓库ID
        /// </summary>
        public int WarehouseId { get; set; }

        /// <summary>
        /// 家具信息
        /// </summary>
        public Furniture? Furniture { get; set; }

        /// <summary>
        /// 仓库信息
        /// </summary>
        public Warehouse? Warehouse { get; set; }

        #region INotifyPropertyChanged 实现
        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}