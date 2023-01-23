using App2.SolidWorksPackage.NodeWork;
using ExcelLibrary.SpreadSheet;
using System.Collections.Generic;
using System.Globalization;


namespace ConsoleApp1.util
{
    public class ExcelWorker
    {

        public static void WriteNodesToExcel(IEnumerable<Node> nodes, string filepath="nodes.xls")
        {
            Workbook workbook = new Workbook();
            Worksheet worksheet = new Worksheet("Sheet");
            worksheet.Cells[0, 0] = new Cell("x");
            worksheet.Cells[0, 1] = new Cell("y");
            worksheet.Cells[0, 2] = new Cell("z");
            int counter = 1;
            foreach (var node in nodes)
            {
                worksheet.Cells[counter, 0] = new Cell($"{node.point.x.ToString("E", CultureInfo.InvariantCulture)}");
                worksheet.Cells[counter, 1] = new Cell($"{node.point.y.ToString("E", CultureInfo.InvariantCulture)}");
                worksheet.Cells[counter, 2] = new Cell($"{node.point.z.ToString("E", CultureInfo.InvariantCulture)}");
                counter++;
            }
            workbook.Worksheets.Add(worksheet);
            workbook.Save(filepath);
        }
        //чтение файла
        //    Workbook book = Workbook.Load(file);
        //Worksheet sheet = book.Worksheets[0];
        //System.Console.WriteLine(sheet.Cells[0, 0]);
    }
}
