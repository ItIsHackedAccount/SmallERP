
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using ERP.Helpers;
using ERP.Models;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using Microsoft.Identity.Client;
using Microsoft.Win32;
using PdfSharp.Drawing;
using PdfSharp.Fonts;
using PdfSharp.Pdf;
using SkiaSharp;
using Microsoft.Win32;

namespace ERP.ViewModels
{
    public class FinanceViewModel : INotifyPropertyChanged
    {
        public decimal TotalRevenue { get; set; } = 100000m;
        public decimal TotalCost { get; set; } = 70000m;
        public decimal Profit { get; set; } = 70000m;

        public ICommand GenerateReportCommand { get; }

        public GeometryModel3D Point3DCollection { get; }
        public GeometryModel3D Point3DCollection2 { get; }
        public GeometryModel3D Point3DCollection3 { get; }

        public GeometryModel3D XAxis { get; }
        public GeometryModel3D YAxis { get; }
        public GeometryModel3D ZAxis { get; }
        private DateTime? _selectedDate1;

        public DateTime? SelectedDate1
        {
            get => _selectedDate1;
            set
            {
                _selectedDate1 = value;
                OnPropertyChanged(nameof(SelectedDate1));
            }
        }

        private DateTime? _selectedDate2;

        public DateTime? SelectedDate2
        {
            get => _selectedDate2;
            set
            {
                _selectedDate2 = value;
                OnPropertyChanged(nameof(SelectedDate2));
            }
        }

        




        public FinanceViewModel()
        {
            GenerateReportCommand = new RelayCommand(_ =>GenerateReport());

            //Point3DCollection = CreateBarModel(0, Colors.LightBlue, 2);
            //Point3DCollection2 = CreateBarModel(10, Colors.Red, 6);
            //Point3DCollection3 = CreateBarModel(4, Colors.Blue, 3);

            //XAxis = CreateAxisLine(new Point3D(0, 0, 0), new Point3D(6, 0, 0), Colors.Gray);
            //YAxis = CreateAxisLine(new Point3D(0, 0, 0), new Point3D(0, 10, 0), Colors.Gray);
            //ZAxis = CreateAxisLine(new Point3D(0, 0, 0), new Point3D(0, 0, 6), Colors.Gray);

            CalculateData();

        }

        private async void CalculateData()
        {

            int Q1Income = 0, Q2Income = 0, Q3Income = 0, Q4Income = 0, Q1Profit = 0, Q2Profit = 0, Q3Profit = 0, Q4Profit = 0, Q1Cost = 0, Q2Cost = 0, Q3Cost = 0, Q4Cost = 0;

            // 当月第一天，保留当前时分秒
            DateTime firstDay = new DateTime(DateTime.Now.Year, 1, 1, 0, 0,0);

            // 当月最后一天，利用 DaysInMonth 自动判断该月有多少天
            int q1DayNumber = DateTime.DaysInMonth(DateTime.Now.Year, 3);
            DateTime lastQ1Day = new DateTime(DateTime.Now.Year, 3, q1DayNumber, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);

            // 当月最后一天，利用 DaysInMonth 自动判断该月有多少天
            int q2DayNumber = DateTime.DaysInMonth(DateTime.Now.Year, 6);
            DateTime lastQ2Day = new DateTime(DateTime.Now.Year, 6, q2DayNumber, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);

            // 当月最后一天，利用 DaysInMonth 自动判断该月有多少天
            int q3DayNumber = DateTime.DaysInMonth(DateTime.Now.Year, 9);
            DateTime lastQ3Day = new DateTime(DateTime.Now.Year, 9, q3DayNumber, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);

            // 当月最后一天，利用 DaysInMonth 自动判断该月有多少天
            int q4DayNumber = DateTime.DaysInMonth(DateTime.Now.Year, 12);
            DateTime lastQ4Day = new DateTime(DateTime.Now.Year, 12, q4DayNumber, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);


            var connStr = "Data Source=MYDB.db";
            using var db = new ERP.Data.DatabaseClient("Microsoft.Data.Sqlite", connStr);
            var sql = $"SELECT id, income,cost,profit,date FROM Finance where datetime (REPLACE(date, '/', '-')) between '{firstDay.ToString("yyyy-MM-dd HH:mm:ss")}' and '{lastQ4Day.ToString("yyyy-MM-dd HH:mm:ss")}' ";

            var rows = await db.ExecuteQueryAsync(sql);

            if (rows.Any())
            {
                foreach (var row in rows)
                {

                    if (Convert.ToDateTime(row["date"]) <= lastQ1Day && Convert.ToDateTime(row["date"]) >= firstDay)
                    {
                        Q1Income += Convert.ToInt32(row["income"]);
                        Q1Cost += Convert.ToInt32(row["cost"]);
                        Q1Profit += Convert.ToInt32(row["profit"]);
                    }
                    else if (Convert.ToDateTime(row["date"]) <= lastQ2Day && Convert.ToDateTime(row["date"]) > lastQ1Day)
                    {
                        Q2Income += Convert.ToInt32(row["income"]);
                        Q2Cost += Convert.ToInt32(row["cost"]);
                        Q2Profit += Convert.ToInt32(row["profit"]);
                    }
                    else if (Convert.ToDateTime(row["date"]) <= lastQ3Day && Convert.ToDateTime(row["date"]) > lastQ2Day)
                    {
                        Q3Income += Convert.ToInt32(row["income"]);
                        Q3Cost += Convert.ToInt32(row["cost"]);
                        Q3Profit += Convert.ToInt32(row["profit"]);
                    }
                    else if (Convert.ToDateTime(row["date"]) <= lastQ4Day && Convert.ToDateTime(row["date"]) > lastQ3Day)
                    {
                        Q4Income += Convert.ToInt32(row["income"]);
                        Q4Cost += Convert.ToInt32(row["cost"]);
                        Q4Profit += Convert.ToInt32(row["profit"]);
                    }
                }
                TotalRevenue = Q1Income + Q2Income + Q3Income + Q4Income;
                TotalCost = Q1Cost + Q2Cost + Q3Cost + Q4Cost;
                Profit = Q1Profit + Q2Profit + Q3Profit + Q4Profit;

                Series[0] =
                    new ColumnSeries<double>
                    {
                        Values = new double[] { Q1Income, Q2Income, Q3Income, Q4Income },
                        Name = "Income",


                    };


                Series[1] =
                    new ColumnSeries<double>
                    {
                        Values = new double[] { Q1Cost, Q2Cost, Q3Cost, Q4Cost },
                        Name = "Cost"

                    };

                Series[2] =
                new ColumnSeries<double>
                {
                    Values = new double[] { Q1Profit, Q2Profit, Q3Profit, Q4Profit },
                    Name = "Profit"

                };



            }
        }


            
             

        private async void GenerateReport()
        {
            // 列宽位置
            int xDate = 20;
            int xIncome = 200;
            int xCost = 300;
            int xProfit = 400;

            // 创建 PDF 文档
            PdfDocument document = new PdfDocument();
            document.Info.Title = "Financial Report";

            // 添加页面
            PdfPage page = document.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);

            // 设置字体
            GlobalFontSettings.UseWindowsFontsUnderWindows = true;
            XFont font = new XFont("Verdana", 12);
            XFont titleFont = new XFont("Verdana", 20);

            int y = 40;

            // 绘制标题
            gfx.DrawString("Financial Report", titleFont, XBrushes.Black,
                new XRect(0, y, page.Width, page.Height), XStringFormats.TopCenter);

            y += 40; // 往下移一点，避免和表头重叠
                     // 绘制表头
            gfx.DrawString("Date", font, XBrushes.Black, new XPoint(xDate, y));
            gfx.DrawString("Income", font, XBrushes.Black, new XPoint(xIncome, y));
            gfx.DrawString("Cost", font, XBrushes.Black, new XPoint(xCost, y));
            gfx.DrawString("Profit", font, XBrushes.Black, new XPoint(xProfit, y));
            y += 20;


            var connStr = "Data Source=MYDB.db";
            using var db = new ERP.Data.DatabaseClient("Microsoft.Data.Sqlite", connStr);

           if (!(SelectedDate1.HasValue&& SelectedDate2.HasValue && SelectedDate1 < SelectedDate2))
            {
                MessageBox.Show("Please select valid date to generate report");
                return;
            }

            var sql = $"SELECT id, income,cost,profit,date FROM Finance where date between '{SelectedDate1}' and '{SelectedDate2}' ";

            var rows = await db.ExecuteQueryAsync(sql);

            if (rows.Any())
            {
                foreach (var row in rows)
                {
                    gfx.DrawString(row["date"]?.ToString() ?? string.Empty, font, XBrushes.Black, new XPoint(xDate, y));
                    gfx.DrawString(row["income"]?.ToString() ?? string.Empty, font, XBrushes.Black, new XPoint(xIncome, y));
                    gfx.DrawString(row["cost"]?.ToString() ?? string.Empty, font, XBrushes.Black, new XPoint(xCost, y));
                    gfx.DrawString(row["profit"]?.ToString() ?? string.Empty, font, XBrushes.Black, new XPoint(xProfit, y));
                    y += 20;
                }
            }
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PDF files (*.pdf)|*.pdf";
            saveFileDialog.Title = "Save Financial Report";
            saveFileDialog.FileName = "FinancialReport.pdf"; // 默认文件名

            if (saveFileDialog.ShowDialog() == true)
            {
                string filePath = saveFileDialog.FileName;
                document.Save(filePath);
                MessageBox.Show($"Report Generated Successfully at: {filePath}");
            }

        }

        public ISeries[] Series { get; set; } = new ISeries[3];
//{
//        new ColumnSeries<double>
//        {
//            Values = new double[] { 10, 20, 30, 40 },Name="Income"
            
//        },
//          new ColumnSeries<double>
//        {
//            Values = new double[] { 10, 20, 30, 40 },Name="Profit"
       
//        },
//            new ColumnSeries<double>
//        {
//            Values = new double[] { 10, 20, 30, 90},Name="Cost"
         
//        }
//    };

        public Axis[] XAxes { get; set; } =
        {
        new Axis { Labels = new string[] { "Q1", "Q2", "Q3", "Q4" } ,Name = "Quarter",  NamePaint = new SolidColorPaint(SKColors.LightBlue) ,LabelsPaint= new SolidColorPaint(SKColors.LightBlue) // 横轴说明
}
    };

        public Axis[] YAxes { get; set; } =
        {
        new Axis { Name = "Number",NamePaint = new SolidColorPaint(SKColors.LightBlue) ,LabelsPaint = new SolidColorPaint(SKColors.LightBlue)    },
   


    };





        private GeometryModel3D CreateBarModel(double x, Color color, double height)
        {
            var mesh = MeshHelper.CreateBar(x, height);
            var material = new MaterialGroup
            {
                Children = {
                    
                new DiffuseMaterial(new SolidColorBrush(color)),
                new SpecularMaterial(new SolidColorBrush(Colors.White), 40)
            }

            };
            var backMaterial = new MaterialGroup
            {
                Children = {
            new DiffuseMaterial(new SolidColorBrush(color)), // 使用一个不同的颜色作为背面
            // 可以为背面也添加高光材质，或者省略
            new SpecularMaterial(new SolidColorBrush(Colors.White), 20) // 背面可以有不同的高光强度
        }
            };
            return new GeometryModel3D { Geometry = mesh, Material = material,BackMaterial=backMaterial };
        }

        //private GeometryModel3D CreateAxisLine(Point3D start, Point3D end, Color color)
        //{
        //    var mesh = MeshHelper.CreateBar(start.X, start.Z, 0.05, 0.05, (end.Y > 0 ? end.Y : end.X > 0 ? end.X : end.Z));
        //    var material = new DiffuseMaterial(new SolidColorBrush(color));
        //    return new GeometryModel3D { Geometry = mesh, Material = material };
        //}



        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}