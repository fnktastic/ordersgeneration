using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using OrdersGenerations.DataAccess;
using OrdersGenerations.Model;
using OrdersGenerations.Repository;
using OrdersGenerations.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Data;
using System.Windows.Input;

namespace OrdersGenerations.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        #region readonly state
        private readonly Context _context = null;
        private readonly IOrderRepository _orderRepository = null;
        private readonly IPositionRepository _positionRepository = null;
        private readonly IProductRepository _productRepository = null;
        #endregion

        #region public properties
        public ObservableCollection<Client> Clients { get; set; }

        private ObservableCollection<Order> _orders;
        public ObservableCollection<Order> Orders
        {
            get { return _orders; }
            set { _orders = value; RaisePropertyChanged("Orders"); }
        }

        private ObservableCollection<Product> _products;
        public ObservableCollection<Product> Products
        {
            get { return _products; }
            set { _products = value; RaisePropertyChanged("Products"); }
        }

        public ObservableCollection<Dimension> Dimensions { get; set; }

        private Order _currentOrder;
        public Order CurrentOrder
        {
            get { return _currentOrder; }
            set
            {
                _currentOrder = value;
                RaisePropertyChanged("CurrentOrder");
                if (value?.ID != -1)
                {
                    if (_currentOrder != null)
                    {
                        SelectedClient = _currentOrder.Client;
                        if (_currentOrder.Positions != null)
                        {
                            SelectedPositions = new ObservableCollection<Position>(_orderRepository.Orders.FirstOrDefault(x => x.ID == _currentOrder.ID)?.Positions);
                            CalcTotalSum();
                        }
                        else
                            SelectedPositions = new ObservableCollection<Position>();
                    }
                    else
                    {
                        SelectedClient = null;
                        SelectedPositions = null;
                    }
                }
            }
        }

        private ObservableCollection<Position> _selectedPositions;
        public ObservableCollection<Position> SelectedPositions
        {
            get { return _selectedPositions; }
            set { _selectedPositions = value; RaisePropertyChanged("SelectedPositions"); }
        }

        private Client _selectedClient;
        public Client SelectedClient
        {
            get { return _selectedClient; }
            set { _selectedClient = value; RaisePropertyChanged("SelectedClient"); }
        }

        private Product _selectedProduct;
        public Product SelectedProduct
        {
            get { return _selectedProduct; }
            set
            {
                if (value != null)
                {
                    ProductID = value.ID;
                    Barcode = value.Barcode;
                    Caption = value.Caption;
                    Price = value.Price;
                }

                _selectedProduct = value;
                RaisePropertyChanged("SelectedProduct");
            }
        }

        private string _barcode;
        public string Barcode
        {
            get { return _barcode; }
            set { _barcode = value; RaisePropertyChanged("Barcode"); }
        }

        private string _caption;
        public string Caption
        {
            get { return _caption; }
            set { _caption = value; RaisePropertyChanged("Caption"); }
        }

        private double _price;
        public double Price
        {
            get { return _price; }
            set { _price = value; RaisePropertyChanged("Price"); }
        }

        private int _productID;
        public int ProductID
        {
            get { return _productID; }
            set { _productID = value; RaisePropertyChanged("ProductID"); }
        }

        private bool _isSavingAllowed;
        public bool IsSavingAllowed
        {
            get { return _isSavingAllowed; }
            set { _isSavingAllowed = value; RaisePropertyChanged("IsSavingAllowed"); }
        }

        private string _filterProductString;
        public string FilterProductString
        {
            get { return _filterProductString; }
            set
            {
                if (Equals(_filterProductString, value))
                    return;
                _filterProductString = value;
                RaisePropertyChanged("FilterProductString");
                FilteredProducts.Refresh();
            }
        }

        private string _filteredOrdersString;
        public string FilteredOrdersString
        {
            get { return _filteredOrdersString; }
            set
            {
                if (Equals(_filteredOrdersString, value))
                    return;
                _filteredOrdersString = value;
                RaisePropertyChanged("FilteredOrdersString");
                FilteredOrders.Refresh();
            }
        }

        private int _selectedTab;
        public int SelectedTab
        {
            get { return _selectedTab; }
            set { _selectedTab = value; RaisePropertyChanged("SelectedTab"); }
        }

        private double _overPercent;
        public double OverPercent
        {
            get { return _overPercent; }
            set
            {
                _overPercent = value;
                RaisePropertyChanged("OverPercent");
            }
        }

        private string _totalOrderSum;
        public string TotalOrderSum
        {
            get { return _totalOrderSum; }
            set { _totalOrderSum = value; RaisePropertyChanged("TotalOrderSum"); }
        }

        public ICollectionView FilteredOrders { get; set; }
        public ICollectionView FilteredProducts { get; set; }
        #endregion

        #region constructor
        public MainViewModel()
        {
            Database.SetInitializer<Context>(new Initializer());
            _context = new Context();
            _orderRepository = new OrderRepository(_context);
            _positionRepository = new PositionRepository(_context);
            _productRepository = new ProductRepository(_context);
            Clients = new ObservableCollection<Client>(_context.Clients);
            Orders = new ObservableCollection<Order>(_orderRepository.Orders);
            Products = new ObservableCollection<Product>(_context.Products);
            Dimensions = new ObservableCollection<Dimension>(_context.Dimensions);
            InitFilters();
            IsSavingAllowed = false;
            CurrentOrder = null;
            OverPercent = 0;
            SelectedTab = 0;
        }
        #endregion

        #region methods
        private void CalcTotalSum()
        {
            if (SelectedPositions != null && SelectedPositions.Count > 0)
                TotalOrderSum = string.Format("Всього {0} грн.", SelectedPositions.Select(x => x.TotalPrice).Sum());
        }

        private void InitCollections()
        {
            Products = new ObservableCollection<Product>(_productRepository.Products);
        }

        private void InitFilters()
        {
            FilteredOrders = CollectionViewSource.GetDefaultView(Orders);
            FilteredOrders.Filter = OnOrdersFilter;
            FilteredOrders.Refresh();

            FilteredProducts = CollectionViewSource.GetDefaultView(Products);
            FilteredProducts.Filter = OnProductsFilter;
            FilteredProducts.Refresh();
        }

        private bool OnProductsFilter(object product)
        {
            if (string.IsNullOrWhiteSpace(_filterProductString))
                return true;
            Product o = product as Product;
            return o.Barcode.ToLower().Contains(_filterProductString.ToLower())
                || o.Caption.ToLower().Contains(_filterProductString.ToLower())
                || o.Price.ToString("0.00").ToLower().Contains(_filterProductString.ToLower());
        }

        private bool OnOrdersFilter(object order)
        {
            if (string.IsNullOrWhiteSpace(_filteredOrdersString))
                return true;
            Order o = order as Order;
            return o.Client.Description.ToLower().Contains(_filteredOrdersString.ToLower())
                || o.CreatedDate.ToShortDateString().ToLower().Contains(_filteredOrdersString.ToLower());
        }

        private void AddProductIntoPositions(Product product)
        {
            var products = SelectedPositions.Select(x => x.Product);
            if (products.Contains(product))
            {
                Position position = SelectedPositions.FirstOrDefault(p => p.Product == product);
                position.ProductQuantity++;
                position.TotalPrice = position.Product.Price * position.ProductQuantity;
            }

            else
            {
                SelectedPositions.Add(new Position()
                {
                    Product = product,
                    ProductQuantity = 1,
                    DimensionID = 1,
                    Dimension = _context.Dimensions.First()
                });
            }

            CalcTotalSum();
            IsSavingAllowed = true;
        }

        private Order GetCurrentOrder(int orderID)
        {
            return _orderRepository.Orders.FirstOrDefault(x => x.ID == orderID);
        }
        #endregion

        #region relay commands
        private RelayCommand<KeyEventArgs> _checkForScannerCommand;
        public RelayCommand<KeyEventArgs> CheckForScannerCommand
        {
            get
            {
                return _checkForScannerCommand ?? (_checkForScannerCommand = new RelayCommand<KeyEventArgs>((keyEvent) =>
                {
                    if (keyEvent.Key == Key.Return)
                    {
                        Product product = _productRepository.Products.FirstOrDefault(x => x.Barcode == _filterProductString);
                        if (product != null)
                        {
                            AddProductIntoPositions(product);
                            FilterProductString = string.Empty;
                        }
                    }
                }));
            }
        }

        private RelayCommand<Position> _reportPreviewCommand;
        public RelayCommand<Position> ReportPreviewCommand
        {
            get
            {
                return _reportPreviewCommand ?? (_reportPreviewCommand = new RelayCommand<Position>((position) =>
                {
                    PrintUtil.Preview(_currentOrder);
                    Thread.Sleep(100);
                    SelectedTab = 1;
                }));
            }
        }

        private RelayCommand<Position> _reportPrintCommand;
        public RelayCommand<Position> ReportPrintCommand
        {
            get
            {
                return _reportPrintCommand ?? (_reportPrintCommand = new RelayCommand<Position>((position) =>
                {
                    PrintUtil.Print();
                }));
            }
        }

        private RelayCommand<Position> _removePositionCommand;
        public RelayCommand<Position> RemovePositionCommand
        {
            get
            {
                return _removePositionCommand ?? (_removePositionCommand = new RelayCommand<Position>((position) =>
                {
                    _positionRepository.RemovePosition(position.ID);
                    SelectedPositions.Remove(position);
                }));
            }
        }

        private RelayCommand _createOrderCommand;
        public RelayCommand CreateOrderCommand
        {
            get
            {
                return _createOrderCommand ?? (_createOrderCommand = new RelayCommand(() =>
                {
                    CurrentOrder = null;
                    IsSavingAllowed = true;
                    CurrentOrder = new Order();
                }));
            }
        }

        private RelayCommand _copyOrderCommand;
        public RelayCommand CopyOrderCommand
        {
            get
            {
                return _copyOrderCommand ?? (_copyOrderCommand = new RelayCommand(() =>
                {
                    var positions = _selectedPositions
                    .Select(x => new Position()
                    {
                        DimensionID = x.DimensionID,
                        ProductQuantity = x.ProductQuantity,
                        TotalPrice = x.TotalPrice,
                        Product = x.Product,
                        Dimension = x.Dimension,
                    });

                    var order = new Order()
                    {
                        CreatedDate = DateTime.Now,
                        Client = _selectedClient,
                        Positions = new List<Position>(positions)
                    };

                    CurrentOrder = new Order() { ID = -1 };
                    SelectedClient = order.Client;
                    SelectedPositions = new ObservableCollection<Position>(order.Positions);
                    IsSavingAllowed = true;
                }));
            }
        }

        private RelayCommand _saveOrderCommand;
        public RelayCommand SaveOrderCommand
        {
            get
            {
                return _saveOrderCommand ?? (_saveOrderCommand = new RelayCommand(() =>
                {
                    Order orderToSave = new Order()
                    {
                        CreatedDate = DateTime.Now,
                        Client = _selectedClient,
                        Positions = _selectedPositions.ToList<Position>(),
                    };
                    _orderRepository.SaveOrder(orderToSave);
                    Orders = new ObservableCollection<Order>(_orderRepository.Orders);
                    CurrentOrder = _orders.Last();
                    IsSavingAllowed = false;

                    InitFilters();
                }));
            }
        }

        private RelayCommand<Product> _addNewPositionCommand;
        public RelayCommand<Product> AddNewPositionCommand
        {
            get
            {
                return _addNewPositionCommand ??
                  (_addNewPositionCommand = new RelayCommand<Product>(product =>
                  {
                      AddProductIntoPositions(product);
                  }));
            }
        }

        private RelayCommand _saveNewProductCommand;
        public RelayCommand SaveNewProductCommand
        {
            get
            {
                return _saveNewProductCommand ??
                  (_saveNewProductCommand = new RelayCommand(() =>
                  {
                      Product product = new Product()
                      {
                          ID = ProductID,
                          Caption = Caption,
                          Barcode = Barcode,
                          Price = Price
                      };

                      _productRepository.SaveProduct(product);
                      SelectedProduct = new Product() { Caption = string.Empty, Barcode = string.Empty };
                      //InitCollections();
                  }));
            }
        }

        private RelayCommand _makeBlankProductCommand;
        public RelayCommand MakeBlankProductCommand
        {
            get
            {
                return _makeBlankProductCommand ??
                  (_makeBlankProductCommand = new RelayCommand(() =>
                  {
                      SelectedProduct = new Product()
                      {
                          ID = 0,
                          Caption = "",
                          Barcode = "",
                          Price = 0
                      };
                  }));
            }
        }

        private RelayCommand _reCalculateTotlaPricesCommand;
        public RelayCommand ReCalculateTotlaPricesCommand
        {
            get
            {
                return _reCalculateTotlaPricesCommand ??
                  (_reCalculateTotlaPricesCommand = new RelayCommand(() =>
                  {
                      double.TryParse(_overPercent.ToString(), out double percent);
                      if (percent >= 0 && percent < 100)
                      {
                          foreach (var position in SelectedPositions)
                          {
                              var currentPrice = position.ProductQuantity * position.Product.Price;
                              var newPrice = currentPrice + ((currentPrice / 100) * percent);
                              position.TotalPrice = newPrice;
                          }
                      }

                      CalcTotalSum();
                  }));
            }
        }

        #endregion
    }
}