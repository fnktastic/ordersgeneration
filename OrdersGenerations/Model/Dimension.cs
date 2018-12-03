using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrdersGenerations.Model
{
    public class Dimension : ViewModelBase
    {
        private int _id;
        public int ID
        {
            get { return _id; }
            set { _id = value; RaisePropertyChanged("ID"); }
        }

        public string Caption { get; set; }
    }
}
