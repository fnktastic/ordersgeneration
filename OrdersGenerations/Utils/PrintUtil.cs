using Microsoft.Reporting.WinForms;
using OrdersGenerations.Model;
using OrdersGenerations.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OrdersGenerations.Utils
{
    public static class PrintUtil
    {
        private static MainWindow _mainWindow;

        public static void SetMainWindowContext(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
        }

        public static void Print(Order order)
        {
            List<ReportViewModel> orderBasic = new List<ReportViewModel>()
            {
                new ReportViewModel()
                {
                    ID = order.ID,
                    CreatedDate = order.CreatedDate,
                    ClientAddress = order.Client.Address,
                    ClientDetails = order.Client.Description,
                    ClientFullName = string.Format("{0} {1} {2}", order.Client.FirstName, order.Client.SurnameName, order.Client.LastName),
                    TotalSummary = order.Positions.Select(x=>x.TotalPrice).Sum(),
                    TotalSummaryByWords = PriceToWords(0)
                }
            };

            List<ReportPositionViewModel> orderPositions = new List<ReportPositionViewModel>();
            order.Positions.ForEach(x =>
            {
                orderPositions.Add(new ReportPositionViewModel()
                {
                    Barcode = x.Product.Barcode,
                    Caption = x.Product.Caption,
                    Dimension = x.Dimension.Caption,
                    Price = x.Product.Price,
                    Quantity = x.ProductQuantity,
                    TotalPrice = x.TotalPrice
                });
            });
            
            ReportDataSource reportDataSource = new ReportDataSource
            {
                Name = "DataSet1",
                Value = orderBasic
            };

            ReportDataSource reportDataSource2 = new ReportDataSource
            {
                Name = "DataSet2",
                Value = orderPositions
            };

            string reportPath = "..\\..\\Report1.rdlc";
            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Local;
            reportViewer.LocalReport.ReportPath = reportPath;
            reportViewer.LocalReport.DataSources.Add(reportDataSource);
            reportViewer.LocalReport.DataSources.Add(reportDataSource2);
            _mainWindow.windowsFormsHost1.Child = reportViewer;
            reportViewer.RefreshReport();
        }

        private static string PriceToWords(double price)
        {
            return "test 001";
        }
    }
}
