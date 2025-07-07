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
        private ProductData? _addItem;
        public ProductData? AddItem
        {
            get => _addItem;
            set { _addItem = value; OnPropertyChanged(); }
        }
        private ProductData? _editItem;
        public ProductData? EditItem
        {
            get => _editItem;
            set { _editItem = value; OnPropertyChanged(); }
        }
        private bool _isAddPopupOpen;
        public bool IsAddPopupOpen
        {
            get => _isAddPopupOpen;
            set { _isAddPopupOpen = value; OnPropertyChanged(); }
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
        public ICommand OpenAddPopupCommand { get; }
        public ICommand SaveAddCommand { get; }
        public ICommand CancelAddCommand { get; }
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

            OpenAddPopupCommand = new RelayCommand(
                _ =>
                {
                    AddItem = new ProductData
                    {
                        ProductName = string.Empty,
                        Quantity = 0,
                        Defective = 0,
                        ProductInfo = string.Empty,
                        DeliveryDueDate = null,
                        RequiredQuantity = 0
                    };
                    IsAddPopupOpen = true;
                },
                _ => true
            );
            SaveAddCommand = new RelayCommand(
                _ =>
                {
                    using var add = new AppDbContext();
                    add.ProductData.Add(AddItem!);
                    add.SaveChanges();

                    Products.Add(AddItem!);
                    IsAddPopupOpen = false;
                },
                _ => AddItem != null && !string.IsNullOrWhiteSpace(AddItem.ProductName)
            );
            CancelAddCommand = new RelayCommand(
                _ => IsAddPopupOpen = false
            );

            OpenEditPopupCommand = new RelayCommand(
                o =>
                {
                    if (o is ProductData pd)
                    {
                        EditItem = new ProductData
                        {
                            ProductName = pd.ProductName,
                            Quantity = pd.Quantity,
                            Defective = pd.Defective,
                            RequiredQuantity = pd.RequiredQuantity,
                            DeliveryDueDate = pd.DeliveryDueDate
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
                        SelectedProduct.RequiredQuantity = EditItem.RequiredQuantity;
                        SelectedProduct.DeliveryDueDate = EditItem.DeliveryDueDate;

                        using var context = new AppDbContext();

                        var entity = context.ProductData
                                .Single(p => p.ProductName == SelectedProduct.ProductName);

                        entity.Quantity = EditItem.Quantity;
                        entity.Defective = EditItem.Defective;
                        entity.RequiredQuantity = EditItem.RequiredQuantity;
                        entity.DeliveryDueDate = EditItem?.DeliveryDueDate;

                        context.SaveChanges();
                    }
                    IsEditPopupOpen = false;

                    CollectionViewSource.GetDefaultView(Products).Refresh();
                },
                o => EditItem != null && !string.IsNullOrWhiteSpace(EditItem.ProductName)
            );

            CancelEditCommand = new RelayCommand(
                _ => CancelEdit(),
                _ => true
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
            using var context = new AppDbContext();

            var list = context.ProductData.Select(p => new ProductData()
            {
                ProductName = p.ProductName,
                Quantity = p.Quantity,
                Defective = p.Defective,
                ProductInfo = p.ProductInfo,
                DeliveryDueDate = p.DeliveryDueDate,
                RequiredQuantity = p.RequiredQuantity,
            }).ToList();

            return list;
        }
        private void Delete(ProductData? item)
        {
            if (item == null)
                return;

            if (MessageBox.Show($"[{item.ProductName}] 삭제하시겠습니까?",
                                "확인", MessageBoxButton.YesNo)
                != MessageBoxResult.Yes)
                return;
            using var context = new AppDbContext();
            var entity = context.ProductData
                                .SingleOrDefault(p => p.ProductName == item.ProductName);
            if (entity != null)
            {
                context.ProductData.Remove(entity);
                context.SaveChanges();
            }
            Products.Remove(item);
        }
        private void ShowDetails(ProductData? product)
        {
            if (product == null) return;

            SelectedProduct = product;
            Details.Clear();
            
            using var context = new AppDbContext();
            var details = context.ProductDetail.Where(d => d.ProductName == product.ProductName).ToList();

            foreach (var detail in details)
            {
                Details.Add(detail);
            }

            IsDetailPopupOpen = true;
        }
        private void CancelEdit()
        {
            IsEditPopupOpen = false;
            EditItem = null;
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propName = "")
            => PropertyChanged?.Invoke(this, new(propName));
    }
}
