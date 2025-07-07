using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SmartQC.Models
{
    public class ProductData
    {
        private string _productName = "";
        [Key]
        public string ProductName
        {
            get => _productName;
            set
            {
                if(_productName != value){
                    _productName = value;
                    OnPropertyChanged();
                }
            }
        }
        private int _quantity;
        public int Quantity
        {
            get => _quantity;
            set
            {
                if(_quantity != value){
                    _quantity = value;
                    OnPropertyChanged();
                }
            }
        }
        private int _defective;
        public int Defective
        {
            get => _defective;
            set
            {
                if(_defective != value){
                    _defective = value;
                    OnPropertyChanged();
                }
            }
        }
        private string? _productInfo;
        public string? ProductInfo
        {
            get => _productInfo;
            set
            {
                if(_productInfo != value){
                    _productInfo = value;
                    OnPropertyChanged();
                }
            }
        }
        private DateTime? _deliveryDueDate;
        public DateTime? DeliveryDueDate
        {
            get => _deliveryDueDate;
            set
            {
                if (_deliveryDueDate != value)
                {
                    _deliveryDueDate = value;
                    OnPropertyChanged();
                }
            }
        }
        private int _requiredQuantity;
        public int RequiredQuantity
        {
            get => _requiredQuantity;
            set
            {
                if (_requiredQuantity != value)
                {
                    _requiredQuantity = value;
                    OnPropertyChanged();
                }
            }
        }
        public int DeliverableQuantity => Quantity - Defective;

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propName = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    }
}
