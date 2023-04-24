using Microsoft.Office.Interop.Excel;
using System;
using System.IO;
using Excel = Microsoft.Office.Interop.Excel;

namespace ModelPolosin;

public class WorkWithExcel
{
    private Application excel;
    private Workbook workbook;
    private string filePath;
    private Worksheet worksheet;

    public WorkWithExcel() {
        excel = new Excel.Application();
        
    }
    public bool Open(string fileName) {
        try {
            if (File.Exists(fileName)) {
                workbook = excel.Workbooks.Open(fileName);
            } else {
                workbook = excel.Workbooks.Add();

            }
            filePath = fileName;
            return true;
        } catch (Exception) {
            return false;
        }
    }

    public void Save() {
        workbook.SaveAs(filePath);
        workbook.Close(true);
        excel.Quit();
    }

    public void SetData(string column, int row, string data) {
        ((Excel.Worksheet)excel.ActiveSheet).Cells[row, column] = data;
    }

    public void DrawInExcel(int number) {
        worksheet = (Excel.Worksheet)workbook.Worksheets.get_Item(1);
        Excel.ChartObjects chartObjects = (Excel.ChartObjects)worksheet.ChartObjects(Type.Missing);
        Excel.ChartObject myChart = (Excel.ChartObject)chartObjects.Add(150, 80, 400, 250);
        Excel.Chart chartPage = myChart.Chart;

        SeriesCollection seriesCollection = (SeriesCollection)chartPage.SeriesCollection(Type.Missing);

        Series series = seriesCollection.NewSeries();
        series.XValues = worksheet.get_Range("A7", "A" + (6 + number).ToString());
        series.Values = worksheet.get_Range("B7", "B" + (6 + number).ToString());
        chartPage.ChartType = XlChartType.xlLine;
    }
}