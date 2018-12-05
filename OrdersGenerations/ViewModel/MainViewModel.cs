using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using OrdersGenerations.DataAccess;
using OrdersGenerations.Model;
using OrdersGenerations.Repository;
using OrdersGenerations.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;

namespace OrdersGenerations.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        #region readonly state
        private readonly Context _context = null;
        private readonly IOrderRepository _orderRepository = null;
        private readonly IPositionRepository _positionRepository = null;
        #endregion

        #region public properties
        public ObservableCollection<Client> Clients { get; set; }

        private ObservableCollection<Order> _orders;
        public ObservableCollection<Order> Orders
        {
            get { return _orders; }
            set { _orders = value; RaisePropertyChanged("Orders"); }
        }

        public ObservableCollection<Product> Products { get; set; }
        public ObservableCollection<Dimension> Dimensions { get; set; }

        private Order _currentOrder;
        public Order CurrentOrder
        {
            get { return _currentOrder; }
            set
            {
                _currentOrder = value;
                RaisePropertyChanged("CurrentOrder");
                if (_currentOrder != null)
                {
                    SelectedClient = _currentOrder.Client;
                    if (_currentOrder.Positions != null)
                        SelectedPositions = new ObservableCollection<Position>(_orderRepository.Orders.FirstOrDefault(x => x.ID == _currentOrder.ID)?.Positions);
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

        private bool _isSavingAllowed;
        public bool IsSavingAllowed
        {
            get { return _isSavingAllowed; }
            set { _isSavingAllowed = value; RaisePropertyChanged("IsSavingAllowed"); }
        }

        private string _productSearch;
        public string ProductSearch
        {
            get { return _productSearch; }
            set
            {
                _productSearch = value;
                RaisePropertyChanged("ProductSearch");
            }
        }
        #endregion

        #region constructor
        public MainViewModel()
        {
            Database.SetInitializer<Context>(new Initializer());
            _context = new Context();
            _orderRepository = new OrderRepository(_context);
            _positionRepository = new PositionRepository(_context);


            Clients = new ObservableCollection<Client>(_context.Clients);
            Orders = new ObservableCollection<Order>(_orderRepository.Orders);
            Products = new ObservableCollection<Product>(_context.Products);
            Dimensions = new ObservableCollection<Dimension>(_context.Dimensions);
            IsSavingAllowed = false;
            CurrentOrder = null;
        }
        #endregion

        #region methods
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
                    ProductQuantity = 1
                });
            }

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
                        Product product = _positionRepository.Positions.FirstOrDefault(x => x.Product.Barcode == _productSearch)?.Product;
                        if (product != null)
                        {
                            AddProductIntoPositions(product);
                            ProductSearch = string.Empty;
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

                    CurrentOrder = null;
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
        #endregion
    }
}