using OrdersGenerations.Model;

namespace OrdersGenerations.ViewModel
{
    public class ReportPositionViewModel
    {
        public string Barcode { get; set; }
        public string Caption { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public double TotalPrice { get; set; }
        public string Dimension { get; set; }
    }
}
