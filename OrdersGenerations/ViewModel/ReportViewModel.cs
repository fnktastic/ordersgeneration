using OrdersGenerations.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrdersGenerations.ViewModel
{
    public class ReportViewModel
    {
        public ReportViewModel(Order order)
        {

        }

        public int ID { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ClientFullName { get; set; }
        public string ClientDetails { get; set; }
        public string ClientAddress { get; set; }
        public double TotalSummary { get; set; }
        public double TotalSummaryByWords { get; set; }
    }
}
