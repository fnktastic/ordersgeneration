using MahApps.Metro.Controls;
using Microsoft.Reporting.WinForms;
using OrdersGenerations.DataAccess;
using OrdersGenerations.Model;
using OrdersGenerations.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OrdersGenerations
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            ReportViewer_Load();
        }

        private void ReportViewer_Load()
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
            windowsFormsHost1.Child = reportViewer;
            reportViewer.RefreshReport();
        }
    }
}
