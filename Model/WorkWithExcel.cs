using System;
using System.IO;
using Microsoft.Office.Interop.Excel;

namespace ModelPolosin;

public class WorkWithExcel
{
    private readonly Application excel;
    private string filePath;
    private Workbook workbook;
    private Worksheet worksheet;

    public WorkWithExcel()
    {
        excel = new Application();
    }

    public bool Open(string fileName)
    {
        try
        {
            if (File.Exists(fileName))
                workbook = excel.Workbooks.Open(fileName);
            else
                workbook = excel.Workbooks.Add();
            filePath = fileName;
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public void Save()
    {
        workbook.SaveAs(filePath);
        workbook.Close(true);
        excel.Quit();
    }

    public void SetData(string column, int row, string data)
    {
        ((Worksheet)excel.ActiveSheet).Cells[row, column] = data;
    }

    public void DrawInExcel(int number)
    {
        worksheet = (Worksheet)workbook.Worksheets.get_Item(1);
        var chartObjects = (ChartObjects)worksheet.ChartObjects(Type.Missing);
        var myChart = (ChartObject)chartObjects.Add(150, 80, 400, 250);
        var chartPage = myChart.Chart;

        var seriesCollection = (SeriesCollection)chartPage.SeriesCollection(Type.Missing);

        var series = seriesCollection.NewSeries();
        series.XValues = worksheet.get_Range("A7", "A" + (6 + number));
        series.Values = worksheet.get_Range("B7", "B" + (6 + number));
        chartPage.ChartType = XlChartType.xlLine;
    }
}