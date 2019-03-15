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
        private readonly IClientRepository _clientRepository = null;
        #endregion

        #region public properties
        private ObservableCollection<Client> _clients;
        public ObservableCollection<Client> Clients
        {
            get { return _clients; }
            set { _clients = value; RaisePropertyChanged("Clients"); }
        }

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

        private Position _selectedPosition;
        public Position SelectedPosition
        {
            get { return _selectedPosition; }
            set { _selectedPosition = value; RaisePropertyChanged("SelectedPosition"); }
        }

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

        private Client _selectedClientInTab;
        public Client SelectedClientInTab
        {
            get { return _selectedClientInTab; }
            set
            {
                if (value != null)
                {
                    ClientID = value.ID;
                    FirstName = value.FirstName;
                    LastName = value.LastName;
                    SurName = value.SurnameName;
                    Address = value.Address;
                    Description = value.Description;

                }

                _selectedClientInTab = value;
                RaisePropertyChanged("SelectedClientInTab");
            }
        }
        private int _clientID;
        public int ClientID
        {
            get { return _clientID; }
            set { _clientID = value; RaisePropertyChanged("ClientID"); }
        }

        private string _firstName;
        public string FirstName
        {
            get { return _firstName; }
            set { _firstName = value; RaisePropertyChanged("FirstName"); }
        }

        private string _surName;
        public string SurName
        {
            get { return _surName; }
            set { _surName = value; RaisePropertyChanged("SurName"); }
        }

        private string _lastName;
        public string LastName
        {
            get { return _lastName; }
            set { _lastName = value; RaisePropertyChanged("LastName"); }
        }

        private string _address;
        public string Address
        {
            get { return _address; }
            set { _address = value; RaisePropertyChanged("Address"); }
        }

        private string _description;
        public string Description
        {
            get { return _description; }
            set { _description = value; RaisePropertyChanged("Description"); }
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
                IsProductsExanded = true;
                RaisePropertyChanged("FilterProductString");
                FilteredProducts.Refresh();
                if (string.IsNullOrWhiteSpace(_filterProductString))
                    IsProductsExanded = false;
            }
        }

        private bool _isProductsExanded;
        public bool IsProductsExanded
        {
            get { return _isProductsExanded; }
            set
            {
                if (Equals(_isProductsExanded, value))
                    return;
                _isProductsExanded = value;
                RaisePropertyChanged("IsProductsExanded");
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
            _clientRepository = new ClientRepository(_context);
            Clients = new ObservableCollection<Client>(_clientRepository.Clients);
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
            bool barcode = false, caption = false, price = false;
            if (o.Barcode != null)
                barcode = o.Barcode.ToLower().Contains(_filterProductString.ToLower());
            if (o.Caption != null)
                caption = o.Caption.ToLower().Contains(_filterProductString.ToLower());
            price = o.Price.ToString("0.00").ToLower().Contains(_filterProductString.ToLower());

            if (barcode || caption || price)
                return true;

            return false;
        }

        private bool OnOrdersFilter(object order)
        {
            if (string.IsNullOrWhiteSpace(_filteredOrdersString))
                return true;
            Order o = order as Order;
            return o.CreatedDate.ToShortDateString().ToLower().Contains(_filteredOrdersString.ToLower());
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

        private RelayCommand _labelPrintCommand;
        public RelayCommand LabelPrintCommand
        {
            get
            {
                return _labelPrintCommand ?? (_labelPrintCommand = new RelayCommand(() =>
                {
                    var labels = new List<ItemLabelViewModel>();
                    var positionsWithBarcode = _currentOrder.Positions.Where(x => x.Product != null);
                    foreach (var position in positionsWithBarcode)
                    {
                        if (string.IsNullOrEmpty(position.Product.Barcode))
                        {
                            Random rnd = new Random();
                            string newBarcode = rnd.Next(10000, 99000).ToString();
                            _selectedPosition.Product.Barcode = newBarcode;
                            _productRepository.SaveProduct(_selectedPosition.Product);
                        }

                        for (int i = 0; i < position.ProductQuantity; i++)
                        {
                            if (position.Product.Barcode.Length != 5)
                                labels.Add(new ItemLabelViewModel()
                                {
                                    Barcode = string.Empty,
                                    BarcodeNumber = string.Empty,
                                    Name = position.Product.Caption
                                });

                            if (position.Product.Barcode.Length == 5)
                                labels.Add(new ItemLabelViewModel()
                                {
                                    Barcode = string.Format("*{0}*", position.Product.Barcode),
                                    BarcodeNumber = position.Product.Barcode,
                                    Name = position.Product.Caption
                                });
                        }
                    }

                    PrintUtil.PrintLabels(labels);

                    SelectedTab = 1;
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

        private RelayCommand _makeBlankClientCommand;
        public RelayCommand MakeBlankClientCommand
        {
            get
            {
                return _makeBlankClientCommand ??
                  (_makeBlankClientCommand = new RelayCommand(() =>
                  {
                      SelectedClientInTab = new Client()
                      {
                          ID = 0,
                          FirstName = "",
                          LastName = "",
                          SurnameName = "",
                          Description = "",
                          Address = ""
                      };
                  }));
            }
        }

        private RelayCommand _saveNewClientCommand;
        public RelayCommand SaveNewClientCommand
        {
            get
            {
                return _saveNewClientCommand ??
                  (_saveNewClientCommand = new RelayCommand(() =>
                  {
                      Client client = new Client()
                      {
                          ID = ClientID,
                          Address = Address,
                          Description = Description,
                          FirstName = FirstName,
                          SurnameName = SurName,
                          LastName = LastName
                      };

                      _clientRepository.SaveClient(client);
                      SelectedClientInTab = new Client()
                      {
                          ID = 0,
                          FirstName = "",
                          LastName = "",
                          SurnameName = "",
                          Description = "",
                          Address = ""
                      };

                      Clients = new ObservableCollection<Client>(_clientRepository.Clients);
                      //InitCollections();
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

        private RelayCommand _importCommand;
        public RelayCommand ImportCommand
        {
            get
            {
                return _importCommand ??
                  (_importCommand = new RelayCommand(() =>
                  {
                      var fileLines = System.IO.File.ReadAllLines("Items2.txt");
                      foreach (var singleLine in fileLines)
                      {
                          var lineArr = singleLine.Split(';');
                          if (lineArr.Length > 5)
                          {
                              double price;
                              double.TryParse(lineArr[5], out price);

                              var product = new Product() //1 2 5
                              {
                                  Barcode = lineArr[1],
                                  Caption = lineArr[2],
                                  Price = price
                              };

                              _productRepository.SaveProduct(product);
                          }
                      }
                  }));
            }
        }
        #endregion
    }
}