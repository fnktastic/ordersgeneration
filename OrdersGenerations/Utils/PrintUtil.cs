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
    public enum ReportTypesEnum
    {
        Order,
        Label
    }

    public static class PrintUtil
    {
        private static MainWindow _mainWindow;
        private static ReportViewer _reportViewer;
        private static ReportViewer _labelViewer;
        private static ReportTypesEnum currentType;

        public static void SetMainWindowContext(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
        }

        public static void PrintLabels(List<ItemLabelViewModel> itemLabelViewModels)
        {
            try
            {
                ReportDataSource labelDataSource1 = new ReportDataSource
                {
                    Name = "DataSet2",
                    Value = itemLabelViewModels
                };

                PrinterSettings printerSettings = new PrinterSettings()
                {
                    PrinterName = "Xprinter XP-370B",
                    Copies = 1,
                };

                var ps = new PaperSize("Custom", 130, 80);
                ps.RawKind = (int)PaperKind.Custom;
                PageSettings pageSettings = new PageSettings()
                {
                    PaperSize = ps,
                };

                string reportPath = "..\\..\\Label1.rdlc";
                _labelViewer = new ReportViewer();
                currentType = ReportTypesEnum.Label;
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

        public static void Preview(Order order)
        {
            List<ReportViewModel> orderBasic = new List<ReportViewModel>()
            {
                new ReportViewModel()
                {
                    ID = order.ID,
                    CreatedDate = order.CreatedDate,
                    ClientAddress = order.Client?.Address,
                    ClientDetails = order.Client?.Description,
                    ClientFullName = string.Format("{0} {1} {2}", order.Client?.FirstName, order.Client?.SurnameName, order.Client?.LastName),
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
            currentType = ReportTypesEnum.Order;
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
            if (currentType == ReportTypesEnum.Label)
                _labelViewer.PrintDialog();

            if (currentType == ReportTypesEnum.Order)
                _reportViewer.PrintDialog();
        }
    }
}
