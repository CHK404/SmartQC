using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using SmartQC.Models;
using System.Windows.Input;
using System.Windows;
using SmartQC.Data;
using System.Windows.Data;

namespace SmartQC.ViewModels
{
    public class ItemListViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<ProductData> Products { get; }
        public ObservableCollection<ProductDetail> Details { get; }
        private ProductData? _selectedProduct;
        public ProductData? SelectedProduct
        {
            get => _selectedProduct;
            set { _selectedProduct = value; OnPropertyChanged(); }
        }
        private ProductData? _editItem;
        public ProductData? EditItem
        {
            get => _editItem;
            set { _editItem = value; OnPropertyChanged(); }
        }
        private bool _isEditPopupOpen;
        public bool IsEditPopupOpen
        {
            get => _isEditPopupOpen;
            set { _isEditPopupOpen = value; OnPropertyChanged(); }
        }
        private bool _isDetailPopupOpen;
        public bool IsDetailPopupOpen
        {
            get => _isDetailPopupOpen;
            set { _isDetailPopupOpen = value; OnPropertyChanged(); }
        }

        public ICommand OpenEditPopupCommand { get; }
        public ICommand SaveEditCommand { get; }
        public ICommand CancelEditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand RowDoubleClickCommand { get; }
        public ICommand CloseDetailPopupCommand { get; }

        public ItemListViewModel()
        {
            Products = new ObservableCollection<ProductData>(LoadProducts());
            Details = new ObservableCollection<ProductDetail>();

            OpenEditPopupCommand = new RelayCommand(
                o =>
                {
                    if (o is ProductData pd)
                    {
                        EditItem = new ProductData
                        {
                            ProductName = pd.ProductName,
                            Quantity = pd.Quantity,
                            Defective = pd.Defective
                        };
                        IsEditPopupOpen = true;
                    }
                },
                o => o is ProductData
            );

            SaveEditCommand = new RelayCommand(
                o =>
                {
                    if (SelectedProduct != null && EditItem != null)
                    {
                        SelectedProduct.ProductName = EditItem.ProductName;
                        SelectedProduct.Quantity = EditItem.Quantity;
                        SelectedProduct.Defective = EditItem.Defective;
                    }
                    IsEditPopupOpen = false;

                    CollectionViewSource.GetDefaultView(Products).Refresh();
                },
                o => EditItem != null && !string.IsNullOrWhiteSpace(EditItem.ProductName)
            );

            CancelEditCommand = new RelayCommand(
                o => { IsEditPopupOpen = false; }
            );

            DeleteCommand = new RelayCommand(
                o => Delete(o as ProductData),
                o => o is ProductData
            );

            RowDoubleClickCommand = new RelayCommand(
                o => ShowDetails(o as ProductData),
                o => o is ProductData
            );
            CloseDetailPopupCommand = new RelayCommand(
            _ => IsDetailPopupOpen = false
            );
        }
        private IEnumerable<ProductData> LoadProducts()
        {
            return new List<ProductData>
            {
            new ProductData { ProductName = "노트북", Quantity = 15, Defective = 1 },
            new ProductData { ProductName = "무선 마우스", Quantity = 40, Defective = 2 },
            new ProductData { ProductName = "키보드", Quantity = 25, Defective = 0 },
            new ProductData { ProductName = "헤드폰", Quantity = 30, Defective = 3 },
            new ProductData { ProductName = "웹캠", Quantity = 20, Defective = 1 },
            new ProductData { ProductName = "USB 케이블", Quantity = 100, Defective = 5 },
            new ProductData { ProductName = "외장하드", Quantity = 10, Defective = 0 },
            new ProductData { ProductName = "모니터", Quantity = 12, Defective = 1 }
            };
        }
        private void Delete(ProductData? item)
        {
            if (item == null) return;
            if (MessageBox.Show($"[{item.ProductName}] 삭제하시겠습니까?", "확인",
                    MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                Products.Remove(item);
        }
        private void ShowDetails(ProductData? product)
        {
            if (product == null) return;

            SelectedProduct = product;
            Details.Clear();
            for (int i = 1; i <= 7; i++)
            {
                Details.Add(new ProductDetail
                {
                    SerialNumber = $"{product.ProductName?.ToUpper()}-{i:000}",
                    ProductName = product.ProductName,
                    UserName = $"User{i}",
                    Defection = (i % 3 == 0)
                });
            }

            IsDetailPopupOpen = true;
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propName = "")
            => PropertyChanged?.Invoke(this, new(propName));
    }
}
