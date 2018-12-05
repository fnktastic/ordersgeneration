using OrdersGenerations.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrdersGenerations.ViewModel
{
    public class ReportViewModel
    {
        public int ID { get; set; }        
        public int PositionsCount { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ClientFullName { get; set; }
        public string ClientDetails { get; set; }
        public string ClientAddress { get; set; }
        public double TotalSummary { get; set; }
        public string TotalSummaryByWords { get; set; }

        public string TotalCountAndPriceString
        {
            get { return string.Format("Всього найменувань {0} на суму {1} грн.", PositionsCount, TotalSummary); }
        }

        public string TotalSummaryString
        {
            get { return string.Format("Разом: {0} грн.", TotalSummary); }
        }

    }
}
