using GemBox.Spreadsheet;

namespace ReportLayer.Extensions
{
    internal static class SpreadSheetReportCellExtension
    {
        internal static ExcelCell WriteToCell(this ExcelCell excelCell, object value)
        {
            excelCell.Value = value;
            return excelCell;
        }
        internal static ExcelCell SetFontColor(this ExcelCell excelCell, SpreadsheetColor fontColor)
        {
            excelCell.Style.Font.Color = fontColor;
            return excelCell;
        }
        internal static ExcelCell SetHorizontalAlignment(this ExcelCell excelCell, HorizontalAlignmentStyle alignment)
        {
            excelCell.Style.HorizontalAlignment = alignment;
            return excelCell;
        }
        internal static ExcelCell SetItalic(this ExcelCell excelCell, bool italic = true)
        {
            excelCell.Style.Font.Italic = italic;
            return excelCell;
        }
        internal static CellRange WriteToCell(this CellRange cellRange, object value)
        {
            cellRange.Value = value;
            return cellRange;
        }
        internal static CellRange SetFontColor(this CellRange cellRange, SpreadsheetColor fontColor)
        {
            cellRange.Style.Font.Color = fontColor;
            return cellRange;
        }
        internal static CellRange SetItalic(this CellRange cellRange, bool italic = true)
        {
            cellRange.Style.Font.Italic = italic;
            return cellRange;
        }
        internal static ExcelCell SetStrikeout(this ExcelCell excelCell, bool strikeout = true)
        {
            excelCell.Style.Font.Strikeout = strikeout;
            return excelCell;
        }
        internal static CellRange SetStrikeout(this CellRange cellRange, bool strikeout = true)
        {
            cellRange.Style.Font.Strikeout = strikeout;
            return cellRange;
        }
        internal static ExcelCell SetUnderlineStyle(this ExcelCell excelCell, UnderlineStyle underlineStyle)
        {
            excelCell.Style.Font.UnderlineStyle = underlineStyle;
            return excelCell;
        }
        internal static CellRange SetUnderlineStyle(this CellRange cellRange, UnderlineStyle underlineStyle)
        {
            cellRange.Style.Font.UnderlineStyle = underlineStyle;
            return cellRange;
        }
        internal static ExcelCell SetFontWeight(this ExcelCell excelCell, int weight) //ExcelFont.Weight
        {
            excelCell.Style.Font.Weight = weight;
            return excelCell;
        }
        internal static CellRange SetFontWeight(this CellRange cellRange, int weight) //ExcelFont.Weight
        {
            cellRange.Style.Font.Weight = weight;
            return cellRange;
        }
        internal static CellRange SetHorizontalAlignment(this CellRange cellRange, HorizontalAlignmentStyle alignment)
        {
            cellRange.Style.HorizontalAlignment = alignment;
            return cellRange;
        }
        internal static CellRange SetVerticalAlignment(this CellRange cellRange, VerticalAlignmentStyle alignment)
        {
            cellRange.Style.VerticalAlignment = alignment;
            return cellRange;
        }
        internal static CellRange SetBorder(this CellRange cellRange, MultipleBorders multipleBorders)
        {
            cellRange.Style.Borders.SetBorders(multipleBorders, SpreadsheetColor.FromArgb(0,0,0), LineStyle.Thin);
            return cellRange;
        }
        internal static CellRange SetWrapText(this CellRange cellRange, bool isWrap = true)
        {
            cellRange.Style.WrapText = isWrap;
            return cellRange;
        }
        internal static ExcelCell SetWrapText(this ExcelCell cell, bool isWrap = true)
        {
            cell.Style.WrapText = isWrap;
            return cell;
        }
    }
}
