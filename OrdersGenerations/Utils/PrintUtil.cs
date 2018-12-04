using Microsoft.Reporting.WinForms;
using OrdersGenerations.Model;
using OrdersGenerations.ViewModel;
using System;
using System.Collections.Generic;

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
            List<ReportViewModel> orders = new List<ReportViewModel>()
            {
                new ReportViewModel()
                {
                    ClientAddress = "adres",
                    ClientFullName = "king lord 22",
                    ClientDetails = "azs #4",
                    CreatedDate = DateTime.Now,
                    ID = 454,
                }
            };

            List<ReportPositionViewModel> positions = new List<ReportPositionViewModel>()
            {
                new ReportPositionViewModel() { Barcode = "6756", Caption = "kvas", Dimension = "st.", Price = 4, Quantity = 10, TotalPrice = 40 },
                new ReportPositionViewModel() { Barcode = "0996", Caption = "kvas 2 ", Dimension = "st.", Price = 4, Quantity = 10, TotalPrice = 40 },
                new ReportPositionViewModel() { Barcode = "6898", Caption = "kvas 3", Dimension = "st.", Price = 4, Quantity = 10, TotalPrice = 40 },
            };


            ReportDataSource reportDataSource = new ReportDataSource
            {
                Name = "DataSet1",
                Value = orders
            };

            ReportDataSource reportDataSource2 = new ReportDataSource
            {
                Name = "DataSet2",
                Value = positions
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
    }
}
