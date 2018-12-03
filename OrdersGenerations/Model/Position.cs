using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrdersGenerations.Model
{
    public class Position : ViewModelBase
    {
        public int ID { get; set; }

        private int _productQuantity;
        public int ProductQuantity
        {
            get { return _productQuantity; }
            set
            {
                _productQuantity = value;
                RaisePropertyChanged("ProductQuantity");
                if (Product != null)
                    TotalPrice = _productQuantity * Product.Price;
            }
        }

        private Dimension _dimension;

        public int DimensionID { get; set; }
        public virtual Dimension Dimension
        {
            get { return _dimension; }
            set { _dimension = value; RaisePropertyChanged("Dimension"); }
        }

        private double _totalPrice;
        public double TotalPrice
        {
            get { return _totalPrice; }
            set { _totalPrice = value; RaisePropertyChanged("TotalPrice"); }
        }

        public int OrderID { get; set; }
        public virtual Order Order { get; set; }

        public Product Product { get; set; }
    }
}
