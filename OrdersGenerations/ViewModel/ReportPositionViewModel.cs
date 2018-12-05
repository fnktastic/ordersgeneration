using OrdersGenerations.Model;

namespace OrdersGenerations.ViewModel
{
    public class ReportPositionViewModel
    {
        public int Counter { get; set; }
        public string Barcode { get; set; }
        public string Caption { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public double TotalPrice { get; set; }        
        public string Dimension { get; set; }

        public string QuantityString
        {
            get { return string.Format("{0} {1}",Quantity, Dimension); }
        }

        public string PriceString
        {
            get { return string.Format("{0}", Price.ToString("0.00")); }
        }

        public string TotalPriceString
        {
            get { return string.Format("{0}", TotalPrice.ToString("0.00")); }
        }
    }
}
