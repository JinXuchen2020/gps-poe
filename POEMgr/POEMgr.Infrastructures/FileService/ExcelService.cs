using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace FileService
{
    public class ExcelService : IDisposable
    {
        protected DataFormatter dataFormatter;
        protected IFormulaEvaluator formulaEvaluator;

        protected IWorkbook workbook;

        private readonly Dictionary<int, int> _columnWithMap = new Dictionary<int, int>();
        //protected ISheet sheet;

        public string Read(string path, int sheetIndex = 0)
        {
            using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                stream.Position = 0;
                return this.Read(stream, sheetIndex);
            }
        }

        internal string Read(string path, string sheetName)
        {
            using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                stream.Position = 0;
                return this.Read(stream, sheetName);
            }
        }

        public string Read(IFormFile file, int sheetIndex = 0)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                file.CopyTo(stream);
                return this.Read(stream, sheetIndex);
            }
        }

        internal string Read(IFormFile file, string sheetName)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                file.CopyTo(stream);
                return this.Read(stream, sheetName);
            }
        }

        internal string Read(Stream stream, int index = 0)
        {
            InitializeMembers(stream);
            ISheet sheet = workbook.GetSheetAt(index);
            return this.ReadSheet(sheet);
        }

        internal string Read(Stream stream, string sheetName)
        {
            ISheet sheet;
            InitializeMembers(stream);

            if (string.IsNullOrEmpty(sheetName))
                sheet = workbook.GetSheetAt(0);
            else
                sheet = workbook.GetSheet(sheetName);

            return this.ReadSheet(sheet);
        }

        private string ReadSheet(ISheet sheet)
        {
            DataTable dtTable = new DataTable();
            List<string> rowList = new List<string>();

            IRow headerRow = sheet.GetRow(0);
            int cellCount = headerRow.LastCellNum;

            for (int j = 0; j < cellCount; j++)
            {
                ICell cell = headerRow.GetCell(j);
                if (cell == null || string.IsNullOrWhiteSpace(cell.ToString())) continue;

                dtTable.Columns.Add(GetUnformattedValue(cell));
            }
            for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
            {
                IRow row = sheet.GetRow(i);
                if (row == null) continue;
                if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;

                for (int j = row.FirstCellNum; j < cellCount; j++)
                    rowList.Add(GetUnformattedValue(row.GetCell(j)));

                if (rowList.Count > 0)
                    dtTable.Rows.Add(rowList.ToArray());

                rowList.Clear();
            }

            return JsonConvert.SerializeObject(dtTable);
        }

        //private string Read(int index = 0)
        //{
        //    DataTable dtTable = new DataTable();
        //    List<string> rowList = new List<string>();

        //    ISheet sheet = workbook.GetSheetAt(index);
        //    IRow headerRow = sheet.GetRow(0);
        //    int cellCount = headerRow.LastCellNum;

        //    for (int j = 0; j < cellCount; j++)
        //    {
        //        ICell cell = headerRow.GetCell(j);
        //        if (cell == null || string.IsNullOrWhiteSpace(cell.ToString())) continue;

        //        dtTable.Columns.Add(GetUnformattedValue(cell));
        //    }
        //    for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
        //    {
        //        IRow row = sheet.GetRow(i);
        //        if (row == null) continue;
        //        if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;

        //        for (int j = row.FirstCellNum; j < cellCount; j++)
        //            rowList.Add(GetUnformattedValue(row.GetCell(j)));

        //        if (rowList.Count > 0)
        //            dtTable.Rows.Add(rowList.ToArray());

        //        rowList.Clear();
        //    }

        //    return JsonConvert.SerializeObject(dtTable);
        //}

        public void InitializeMembers(Stream stream)
        {
            workbook = WorkbookFactory.Create(stream);

            if (this.workbook != null)
            {
                this.dataFormatter = new DataFormatter(CultureInfo.InvariantCulture);
                this.formulaEvaluator = WorkbookFactory.CreateFormulaEvaluator(this.workbook);
            }
        }

        protected string GetFormattedValue(ICell cell)
        {
            string returnValue = string.Empty;
            if (cell != null)
            {
                try
                {
                    returnValue = this.dataFormatter.FormatCellValue(cell, this.formulaEvaluator);
                }
                catch
                {
                    if (cell.CellType == CellType.Formula)
                    {
                        switch (cell.CachedFormulaResultType)
                        {
                            case CellType.String:
                                returnValue = cell.StringCellValue;
                                cell.SetCellValue(cell.StringCellValue);
                                break;
                            case CellType.Numeric:
                                returnValue = dataFormatter.FormatRawCellContents(cell.NumericCellValue, 0, cell.CellStyle.GetDataFormatString());
                                cell.SetCellValue(cell.NumericCellValue);
                                break;
                            case CellType.Boolean:
                                returnValue = cell.BooleanCellValue.ToString();
                                cell.SetCellValue(cell.BooleanCellValue);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }

            return (returnValue ?? string.Empty).Trim();
        }

        protected string GetUnformattedValue(ICell cell)
        {
            string returnValue = string.Empty;
            if (cell != null)
            {
                try
                {
                    returnValue = (cell.CellType == CellType.Numeric ||
                    (cell.CellType == CellType.Formula &&
                    cell.CachedFormulaResultType == CellType.Numeric)) ?
                        formulaEvaluator.EvaluateInCell(cell).NumericCellValue.ToString() :
                        this.dataFormatter.FormatCellValue(cell, this.formulaEvaluator);
                }
                catch
                {
                    if (cell.CellType == CellType.Formula)
                    {
                        switch (cell.CachedFormulaResultType)
                        {
                            case CellType.String:
                                returnValue = cell.StringCellValue;
                                cell.SetCellValue(cell.StringCellValue);
                                break;
                            case CellType.Numeric:
                                returnValue = cell.NumericCellValue.ToString();
                                cell.SetCellValue(cell.NumericCellValue);
                                break;
                            case CellType.Boolean:
                                returnValue = cell.BooleanCellValue.ToString();
                                cell.SetCellValue(cell.BooleanCellValue);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }

            return (returnValue ?? string.Empty).Trim();
        }

        private dynamic GetCellValue(ICell cell)
        {
            if (cell == null)
            {
                return string.Empty;
            }
            else
            {
                switch (cell.CellType)
                {
                    case CellType.Blank:
                        return string.Empty;
                    case CellType.String:
                        return cell.StringCellValue;
                    case CellType.Numeric:
                        return cell.NumericCellValue;
                    case CellType.Boolean:
                        return cell.BooleanCellValue;
                    case CellType.Formula:
                        return cell.NumericCellValue;
                    default:
                        return cell.StringCellValue;
                }
            }

        }

        public void Dispose()
        {
            workbook.Close();
        }


        public byte[] ListToStream<T>(List<T> list, bool isXlsx = true, string sheetName = "SheetOne")
        {
            IWorkbook workbook = null;
            workbook = !isXlsx ? new HSSFWorkbook() : new XSSFWorkbook();
            ISheet defaultSheet = workbook.CreateSheet(sheetName);
            _columnWithMap.Clear();
            Type type = typeof(T);
            PropertyInfo[] properties = type.GetProperties();
            IRow header = defaultSheet.CreateRow(0);
            var headerCellStyle = CreateCellStyle(workbook, true, 0, IndexedColors.DarkBlue.Index, IndexedColors.White.Index);
            int i = 0;
            foreach (PropertyInfo pro in properties)
            {
                header.CreateCell(i).SetCellValue(pro.Name);
                header.GetCell(i).CellStyle = headerCellStyle;
                defaultSheet.SetColumnWidth(i, Encoding.UTF8.GetBytes(header.GetCell(i).ToString()).Length * 256);
                _columnWithMap.Add(i, Encoding.UTF8.GetBytes(header.GetCell(i).ToString()).Length * 256);
                i++;
            }
            int RowNumber = 1;
            list.ForEach(item => {
                IRow row = defaultSheet.CreateRow(RowNumber);
                var cellStyle = CreateCellStyle(workbook, false, RowNumber - 1, IndexedColors.LightCornflowerBlue.Index, IndexedColors.Black.Index);
                PropertyInfo[] tempProperties = properties;
                int ColumnNumber = 0;
                foreach (PropertyInfo pro in properties)
                {
                    var cell = row.CreateCell(ColumnNumber);
                    cell.CellStyle = cellStyle;
                    cell.CellStyle.WrapText = false;
                    if (pro.GetValue(item) == null)
                    {
                        cell.SetCellValue("");
                    }
                    else if (pro.PropertyType == typeof(string[]))
                    {
                        cell.CellStyle.WrapText = true;
                        var target = string.Join("\n", (string[])pro.GetValue(item));
                        cell.SetCellValue(target);
                    }
                    else if (pro.PropertyType.Name.StartsWith("List"))
                    {
                        var value = JsonConvert.SerializeObject(pro.GetValue(item));
                        cell.SetCellValue(value);
                    }
                    else
                    {
                        cell.SetCellValue(Convert.ToString(pro.GetValue(item)));
                    }

                    if (!cell.CellStyle.WrapText && Encoding.UTF8.GetBytes(cell.ToString()).Length * 256 > _columnWithMap[ColumnNumber])
                    {
                        _columnWithMap[ColumnNumber] = Encoding.UTF8.GetBytes(cell.ToString()).Length * 256;
                    }


                    ColumnNumber++;
                }
                RowNumber++;
            });

            _columnWithMap.ToList().ForEach(item =>
            {
                defaultSheet.SetColumnWidth(item.Key, item.Value);
            });

            using (MemoryStream stream = new MemoryStream())
            {
                workbook.Write(stream,false);
                return stream.ToArray();
            }
        }

        private static ICellStyle CreateCellStyle(IWorkbook workbook, bool isHeader, int rowIndex, short foregroundColor, short fontColor)
        {
            var cellStyle = workbook.CreateCellStyle();
            cellStyle.VerticalAlignment = VerticalAlignment.Center;
            cellStyle.Alignment = HorizontalAlignment.Left;
            cellStyle.WrapText = false;

            var font = workbook.CreateFont();
            font.IsBold = isHeader;
            font.FontName = "Calibri";

            if (!isHeader)
            {
                cellStyle.BorderBottom = BorderStyle.Thin;
                cellStyle.BorderLeft = BorderStyle.Thin;
                cellStyle.BorderTop = BorderStyle.Thin;
                cellStyle.BorderRight = BorderStyle.Thin;
            }

            if (rowIndex % 2 == 0)
            {
                cellStyle.FillForegroundColor = foregroundColor;
                cellStyle.FillPattern = FillPattern.SolidForeground;
                font.Color = fontColor;
            }

            cellStyle.SetFont(font);
            return cellStyle;
        }
    }
}