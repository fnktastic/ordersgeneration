using Microsoft.Reporting.WinForms;
using OrdersGenerations.Model;
using OrdersGenerations.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing.Printing;
using System.Linq;
using System.Threading;

namespace OrdersGenerations.Utils
{
    public static class PrintUtil
    {
        private static MainWindow _mainWindow;
        private static ReportViewer _reportViewer;
        private static ReportViewer _labelViewer;

        public static void SetMainWindowContext(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
        }

        public static void PrintLabelsWithoutBarcode(Product product, short copies)
        {
            try
            {
                ItemLabelViewModel itemLabelViewModel = new ItemLabelViewModel()
                {
                    Barcode = string.Format("*{0}*", product.Barcode),
                    BarcodeNumber = product.Barcode,
                    Name = product.Caption
                };
                ReportDataSource labelDataSource1 = new ReportDataSource
                {
                    Name = "DataSet2",
                    Value = new List<ItemLabelViewModel>() { itemLabelViewModel }
                };

                PrinterSettings printerSettings = new PrinterSettings()
                {
                    PrinterName = "Xprinter XP-370B",
                    Copies = copies,
                };

                var ps = new PaperSize("Custom", 130, 80);
                ps.RawKind = (int)PaperKind.Custom;
                PageSettings pageSettings = new PageSettings()
                {
                    PaperSize = ps,
                };

                string reportPath = "..\\..\\Label2.rdlc";
                _labelViewer = new ReportViewer();
                _labelViewer.ProcessingMode = ProcessingMode.Local;
                _labelViewer.LocalReport.ReportPath = reportPath;
                _labelViewer.LocalReport.DataSources.Add(labelDataSource1);
                _mainWindow.windowsFormsHost1.Child = _labelViewer;
                _labelViewer.PrinterSettings = printerSettings;
                _labelViewer.SetPageSettings(pageSettings);
                _labelViewer.SetDisplayMode(DisplayMode.PrintLayout);
                _labelViewer.RefreshReport();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public static void PrintLabels(Product product, short copies)
        {
            try
            {
                ItemLabelViewModel itemLabelViewModel = new ItemLabelViewModel()
                {
                    Barcode = string.Format("*{0}*", product.Barcode),
                    BarcodeNumber = product.Barcode,
                    Name = product.Caption
                };
                ReportDataSource labelDataSource1 = new ReportDataSource
                {
                    Name = "DataSet2",
                    Value = new List<ItemLabelViewModel>() { itemLabelViewModel }
                };

                PrinterSettings printerSettings = new PrinterSettings()
                {
                    PrinterName = "Xprinter XP-370B",
                    Copies = copies,
                };

                var ps = new PaperSize("Custom", 130, 80);
                ps.RawKind = (int)PaperKind.Custom;
                PageSettings pageSettings = new PageSettings()
                {
                    PaperSize = ps,
                };

                string reportPath = "..\\..\\Label1.rdlc";
                _labelViewer = new ReportViewer();
                _labelViewer.ProcessingMode = ProcessingMode.Local;
                _labelViewer.LocalReport.ReportPath = reportPath;
                _labelViewer.LocalReport.DataSources.Add(labelDataSource1);
                _mainWindow.windowsFormsHost1.Child = _labelViewer;
                _labelViewer.PrinterSettings = printerSettings;
                _labelViewer.SetPageSettings(pageSettings);
                _labelViewer.SetDisplayMode(DisplayMode.PrintLayout);
                _labelViewer.RefreshReport();
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public static void Preview(Order order)
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
                    TotalSummaryByWords = NumberByWords.GrnPhrase(Convert.ToDecimal(order.Positions.Select(x=>x.TotalPrice).Sum())),
                    PositionsCount = order.Positions.Count
                }
            };

            int counter = 0;
            List<ReportPositionViewModel> orderPositions = new List<ReportPositionViewModel>();
            order.Positions.ForEach(x =>
            {
                counter++;
                orderPositions.Add(new ReportPositionViewModel()
                {
                    Barcode = x.Product.Barcode,
                    Caption = x.Product.Caption,
                    Dimension = x.Dimension.Caption,
                    Price = x.Product.Price,
                    Quantity = x.ProductQuantity,
                    TotalPrice = x.TotalPrice,
                    Counter = counter
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
            _reportViewer = new ReportViewer();
            _reportViewer.ProcessingMode = ProcessingMode.Local;
            _reportViewer.LocalReport.ReportPath = reportPath;
            _reportViewer.LocalReport.DataSources.Add(reportDataSource);
            _reportViewer.LocalReport.DataSources.Add(reportDataSource2);
            _mainWindow.windowsFormsHost1.Child = _reportViewer;
            _reportViewer.ZoomMode = ZoomMode.Percent;
            _reportViewer.ZoomPercent = 100;
            _reportViewer.RefreshReport();
            _reportViewer.ShowBackButton = false;
            _reportViewer.ShowFindControls = false;
            _reportViewer.SetDisplayMode(DisplayMode.PrintLayout);
        }

        public static void Print()
        {
            _reportViewer.PrintDialog();
        }
    }
}
