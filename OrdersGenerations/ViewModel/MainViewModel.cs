using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using OrdersGenerations.DataAccess;
using OrdersGenerations.Model;
using OrdersGenerations.Repository;
using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;

namespace OrdersGenerations.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        #region readonly state
        private readonly Context _context = null;
        private readonly IOrderRepository _orderRepository = null;
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
                SelectedClient = _currentOrder.Client;
                if (_currentOrder.Positions != null)
                    SelectedPositions = new ObservableCollection<Position>(_currentOrder.Positions);
                else
                    SelectedPositions = new ObservableCollection<Position>();
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

        #endregion

        #region constructir
        public MainViewModel()
        {
            Database.SetInitializer<Context>(new Initializer());
            _context = new Context();
            _orderRepository = new OrderRepository(_context);


            Clients = new ObservableCollection<Client>(_context.Clients);
            Orders = new ObservableCollection<Order>(_orderRepository.Orders);
            Products = new ObservableCollection<Product>(_context.Products);
            Dimensions = new ObservableCollection<Dimension>(_context.Dimensions);

            CurrentOrder = Orders.Last();
        }
        #endregion

        #region methods
        #endregion

        #region relay commands
        private RelayCommand _newOrder;
        public RelayCommand NewOrder
        {
            get
            {
                return _newOrder ?? (_newOrder = new RelayCommand(() =>
                {
                    CurrentOrder = new Order();                    
                }));
            }
        }

        private RelayCommand _generateOrderCommand;
        public RelayCommand GenerateOrderCommand
        {
            get
            {
                return _generateOrderCommand ?? (_generateOrderCommand = new RelayCommand(() =>
                {
                    Order orderTooSave = new Order()
                    {
                        ID = CurrentOrder.ID,
                        Client = _selectedClient,
                        Positions = SelectedPositions.ToList<Position>(),
                        CreatedDate = DateTime.Now
                    };
                    _orderRepository.SaveOrder(orderTooSave);
                    Orders = new ObservableCollection<Order>(_orderRepository.Orders);
                    CurrentOrder = _orders.Last();
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
                      var products = SelectedPositions.Select(x => x.Product);
                      if (products.Contains(product))
                      {
                          Position position = SelectedPositions.FirstOrDefault(p => p.Product == product);
                          position.ProductQuantity++;
                          position.TotalPrice = position.Product.Price * position.ProductQuantity;
                      }

                      else
                          SelectedPositions.Add(new Position()
                          {
                              Product = product,
                          });
                  }));
            }
        }
        #endregion
    }
}