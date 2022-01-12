using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using GemBox.Spreadsheet;
namespace ReportLayer.Bases
{
    public class SpreadSheetReportBase : IDisposable
    {
        public SpreadSheetReportBase(string template, int sheetindex = 0)
        {
            SpreadsheetInfo.SetLicense(ConfigurationManager.AppSettings["SpreadSheetKey"]);
            if (string.IsNullOrEmpty(template))
            {
                WorkBook = new ExcelFile();
            }
            else
            {
                WorkBook = ExcelFile.Load(template);
            }
            WorkSheet = WorkBook.Worksheets[sheetindex];
        }
        
        protected ExcelFile WorkBook { get; set; }
        protected ExcelWorksheet WorkSheet { get; set; }
        protected CellRange Range { get; set; }
        private MemoryStream MemoryStream { get; set; } = new MemoryStream();
        
        public virtual void GenerateReport()
        {
        }

        protected void DuplicateSheet(string name)
        {
            WorkBook.Worksheets.AddCopy(name, WorkBook.Worksheets[0]);
            WorkSheet = WorkBook.Worksheets[WorkBook.Worksheets.Count - 1];
        }
        
        protected void DeleteSheet()
        {
            WorkBook.Worksheets.Remove(0);
        }
        
        protected void CellBorder(int startrow, int startcolumn, int lastrow, int lastcolumn, MultipleBorders multipleBorders, LineStyle lineStyle)
        {
            GetRange(startrow, startcolumn, lastrow, lastcolumn)
                .Style
                .Borders
                .SetBorders(multipleBorders, 
                    SpreadsheetColor.FromName(ColorName.Black), 
                    lineStyle);
        }
        
        protected ExcelCell WriteToCell(string cell, object value)
        {
            WorkSheet.Cells[cell].Value = value;
            return WorkSheet.Cells[cell];
        }

        protected ExcelCell WriteToCell(int row, int column, object value)
        {
            WorkSheet.Cells[row, column].Value = value;
            return WorkSheet.Cells[row, column];
        }

        protected void SetBackgroundColor(int startrow, int startcolumn, int lastrow, int lastcolumn, SpreadsheetColor color)
        {
            GetRange(startrow, startcolumn, lastrow, lastcolumn).Style.FillPattern.SetSolid(color);
        }
        protected void AutofitRow(int row)
        {
            WorkSheet.Rows[row].AutoFit();
        }
        protected void InsertRow(int row, int cnt)
        {
            WorkSheet.Rows.InsertEmpty(row, cnt);
        }
        protected void InsertRowCopy(int row, int cnt)
        {
            WorkSheet.Rows.InsertCopy(row, cnt, WorkSheet.Rows[row-1]);
        }

        protected ExcelCell SetFontColor(string cell, SpreadsheetColor fontColor)
        {
            WorkSheet.Cells[cell].Style.Font.Color = fontColor;
            return WorkSheet.Cells[cell];
        }

        protected ExcelCell SetItalic(string cell, int column, bool italic = true)
        {
            WorkSheet.Cells[cell].Style.Font.Italic = italic;
            return WorkSheet.Cells[cell];
        }

        protected ExcelCell SetStrikeout(string cell, bool strikeout = true)
        {
            WorkSheet.Cells[cell].Style.Font.Strikeout = strikeout;
            return WorkSheet.Cells[cell];
        }

        protected ExcelCell SetUnderlineStyle(string cell, UnderlineStyle underlineStyle)
        {
            WorkSheet.Cells[cell].Style.Font.UnderlineStyle = underlineStyle;
            return WorkSheet.Cells[cell];
        }

        protected ExcelCell SetWeight(string cell, int weight) //ExcelFont.Weight
        {
            WorkSheet.Cells[cell].Style.Font.Weight = weight;
            return WorkSheet.Cells[cell];
        }

        protected CellRange MergeCell(string startCell, string lastCell)
        {
            WorkSheet.Cells.GetSubrange(startCell, lastCell).Merged = true;
            return WorkSheet.Cells.GetSubrange(startCell, lastCell);
        }

        protected void SetRowHeight(int row, double height, LengthUnit lengthUnit = LengthUnit.Centimeter)
        {
            WorkSheet.Rows[row].SetHeight(height, lengthUnit);
        }
        protected CellRange MergeCell(int startrow, int startcolumn, int lastrow, int lastcolumn)
        {
            WorkSheet.Cells.GetSubrangeAbsolute(startrow, startcolumn, lastrow, lastcolumn).Merged = true;
            return WorkSheet.Cells.GetSubrangeAbsolute(startrow, startcolumn, lastrow, lastcolumn);
        }
        protected void DeleteCells(int startrow, int startcolumn, int lastrow, int lastcolumn)
        {
            WorkSheet.Cells.GetSubrangeAbsolute(startrow, startcolumn, lastrow, lastcolumn).Remove(RemoveShiftDirection.Up);
        }
        protected CellRange GetRange(int startrow, int startcolumn, int lastrow, int lastcolumn)
        {
            return WorkSheet.Cells.GetSubrangeAbsolute(startrow, startcolumn, lastrow, lastcolumn);
        }

        public string SaveToPDF()
        {
            return $"data:application/pdf;base64,{Save(new PdfSaveOptions { SelectionType = SelectionType.EntireFile})}";
        }

        public string SaveToExcel()
        {
            return $"data:application/xlsx;base64,{Save(SaveOptions.XlsxDefault)}";
        }

        private string Save(SaveOptions saveOption)
        {
            WorkBook.Save(MemoryStream, saveOption);
            var memStream = MemoryStream.ToArray();
            return Convert.ToBase64String(memStream);
        }

        public virtual void Dispose()
        {
        }
    }
}
