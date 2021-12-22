using NPOI.HSSF.UserModel;
using NPOI.SS.Formula;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace EasyOC.OrchardCore.Excel.Libs
{
    /// <summary>
    /// Excel 操作类 ，TODO 替换为 Magicodes.IE.Excel ,引用已添加
    /// </summary>

    public class NPOIExcelHelper : IDisposable
    {
        public enum ExcelFileType
        {
            XLS, XLSX
        }
        /// <summary>
        /// 区域拷贝定义
        /// </summary>
        public class CopyRegionSettings
        {
            public ISheet FromSheet { get; set; }
            public int StartCell { get; set; }
            public int StartRow { get; set; }
            public int EndRow { get; set; }

            public int EndCell { get; set; }
            public ISheet targetSheet { get; set; }
            /// <summary>
            /// 目标Sheet 起始行
            /// </summary>
            public int ToRowIndex { get; set; }

            public int ToCellIndex { get; set; }
            public bool CopyAll { get; set; }
            public bool CopyData { get; set; }
            public bool CopyStyle { get; set; }
            public bool CopyFormula { get; set; }
            /// <summary>
            /// 需要避开写入的行
            /// </summary>
            public List<int> SkipRows { get; set; }
            public List<int> SkipCells { get; set; }

        }

        /// <summary>
        /// 删除行
        /// </summary>
        /// <param name="startRow">从第几行开始（0开始）</param>
        /// <param name="delCount">共删除N行</param>
        public void DelRow(int startRow, int delCount = 1)
        {
            //sheet.ShiftRows(startRow + 1, sheet.LastRowNum, -1, false, false);//删除一行（为负数只能为-1）
            for (int i = 0; i < delCount; i++)
            {
                CurrentSheet.ShiftRows(startRow + 1, sheet.LastRowNum, -1);
            }
        }

        public BaseFormulaEvaluator baseFormulaEvaluator;
        /// <summary>
        /// 文件ID SalesportalUploadlogModel.FileID
        /// </summary>
        public Guid FileID { get; set; }
        public IWorkbook workbook;
        private ISheet sheet;
        private readonly Stream stream;
        public NPOIExcelHelper(Stream excelStream, ExcelFileType fileType = ExcelFileType.XLSX)
        {
            stream = excelStream;

            try
            {
                this.fileType = fileType;
                if (fileType == ExcelFileType.XLS)
                {
                    workbook = new HSSFWorkbook(stream);
                    baseFormulaEvaluator = new HSSFFormulaEvaluator(workbook);
                }
                else
                {
                    workbook = new XSSFWorkbook(stream);
                    baseFormulaEvaluator = new XSSFFormulaEvaluator(workbook);
                }
                workbook.SetActiveSheet(0);
                CurrentSheet = workbook.GetSheetAt(0);

            }
            catch (Exception ex)
            {
                if (stream != null)
                {
                    stream.Close();
                }
                throw new Exception("NPOI实例化错误:" + ex);
            }

        }

        public void Reload()
        {
            if (fileType == ExcelFileType.XLS)
            {
                workbook = new HSSFWorkbook(stream);
            }
            else
            {
                workbook = new XSSFWorkbook(stream);
            }
        }
        private ExcelFileType fileType = ExcelFileType.XLSX;
        public NPOIExcelHelper(string templatePath, ExcelFileType fileType = ExcelFileType.XLSX)
        {
            this.fileType = fileType;
            try
            {
                stream = File.OpenRead(templatePath);
                if (fileType == ExcelFileType.XLS)
                {
                    workbook = new HSSFWorkbook(stream);
                }
                else
                {
                    workbook = new XSSFWorkbook(stream);
                }
                workbook.SetActiveSheet(0);

                CurrentSheet = workbook.GetSheetAt(0);
            }
            catch (Exception ex)
            {
                throw new Exception("NPOI实例化错误:" + ex.ToString());
            }
        }

        /// <summary>
        /// 设置/获取当前Sheet
        /// </summary>
        public string CurrentSheetName
        {
            get { return sheet.SheetName; }
            set
            {
                sheet = workbook.GetSheet(value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startColumnIndex">开始列</param>
        /// <param name="endRowIndex">默认为0  </param>
        /// <param name="startRowIndex">开始行</param>
        /// <param name="endColumnIndex">结束列</param>
        /// <returns></returns>
        public Array ReadAsArray(int startRowIndex = 0, int endColumnIndex = 0, int endRowIndex = 0, int startColumnIndex = 0)
        {

            endColumnIndex = endColumnIndex == 0 ? ColumnCount : endColumnIndex;
            endRowIndex = endRowIndex == 0 ? RowCount : endRowIndex;

            Array array = new object[(endRowIndex - startRowIndex + 1), (endColumnIndex - startColumnIndex + 1)];

            for (int i = startRowIndex; i < endRowIndex + 1; i++)
            {
                for (int j = startColumnIndex; j < endColumnIndex + 1; j++)
                {
                    array.SetValue(GetValue(i, j).Trim(), i - startRowIndex, j - startColumnIndex);
                }
            }
            return array;

        }

        public List<Dictionary<string, string>> ReadAsDictionary(int pkIndex = -1, int startRowIndex = 0, int endRowIndex = 0, int startColumnIndex = 0, int endColumnIndex = 0)
        {

            endColumnIndex = endColumnIndex == 0 ? ColumnCount - 1 : endColumnIndex;
            endRowIndex = endRowIndex == 0 ? RowCount : endRowIndex;
            var headers = new Dictionary<int, string>();
            #region Build Headers 

            for (int i = startColumnIndex; i < endColumnIndex + 1; i++)
            {
                var value = GetValue(startRowIndex - 1, i);
                if (string.IsNullOrEmpty(value) || headers.Values.Contains(value))
                {
                    value = ColumnIndexToName(i + 1);
                }
                headers[i] = value.Trim();
            }
            #endregion
            var data = new List<Dictionary<string, string>>();
            for (int i = startRowIndex; i < endRowIndex + 1; i++)
            {

                if (pkIndex != -1 && string.IsNullOrEmpty(GetValue(i, pkIndex).Trim()))
                {
                    break;
                }
                var row = new Dictionary<string, string>();
                row["$index"] = (i + 1).ToString();
                for (int j = startColumnIndex; j < endColumnIndex + 1; j++)
                {
                    row[headers[j]] = GetValue(i, j).Trim();
                }
                data.Add(row);
            }
            return data;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="pkIndex">指定主键列，如果为空则不往下读取-1为忽略</param>
        /// <param name="startColumnIndex">开始列</param>
        /// <param name="startRowIndex">开始行</param>
        /// <param name="endRowIndex"></param>
        /// <param name="endColumnIndex">结束列</param>
        /// <returns></returns>
        public DataTable ReadAsDataTable(DataTable dt = null, int pkIndex = -1, int startRowIndex = 0, int endRowIndex = 0, int startColumnIndex = 0, int endColumnIndex = 0)
        {

            endColumnIndex = endColumnIndex == 0 ? ColumnCount - 1 : endColumnIndex;
            endRowIndex = endRowIndex == 0 ? RowCount : endRowIndex;
            if (dt is null)
            {
                dt = new DataTable();
            }
            #region Build Headers


            for (int i = startColumnIndex; i < endColumnIndex + 1; i++)
            {
                var value = GetValue(startRowIndex - 1, i);
                if (string.IsNullOrEmpty(value) || dt.Columns.Contains(value))
                {
                    value = ColumnIndexToName(i + 1);
                }
                dt.Columns.Add(value);
            }

            #endregion


            for (int i = startRowIndex; i < endRowIndex + 1; i++)
            {
                if (pkIndex != -1 && string.IsNullOrEmpty(GetValue(i, pkIndex).Trim()))
                {
                    break;
                }
                DataRow row = dt.NewRow();
                for (int j = startColumnIndex; j < endColumnIndex + 1; j++)
                {
                    row[j - startColumnIndex] = GetValue(i, j).Trim();
                }
                dt.Rows.Add(row);
            }
            return dt;
        }

        /// <summary>
        /// 设置/获取当前Sheet
        /// </summary>
        public ISheet CurrentSheet
        {
            get { return sheet; }
            set { sheet = value; }
        }
        public IRow GetRow(int rowIndex)
        {

            return sheet.GetRow(rowIndex);
        }

        public int RowCount
        {
            get
            {    //重新定义最后行下标 
                return CleanEmptyRow();
            }
        }

        /// <summary>
        /// 从下往上,从左到右, 找到一个单元格不为空,则取此行下标为当前sheet最大下标
        /// </summary>
        private int CleanEmptyRow()
        {
            for (int i = sheet.LastRowNum; i > 0; i--)
            {
                for (int j = 0; j < sheet.GetRow(i).LastCellNum; j++)
                {
                    if (!string.IsNullOrEmpty(GetValue(i, j)))
                    {
                        return i;
                    }
                }
            }
            return 0;
        }
        /// <summary> 
        /// 取第一行的最后一列的下标,不是最大列数
        /// </summary>
        public int ColumnCount
        {
            get { return sheet.GetRow(0).LastCellNum; }
        }

        public ICell GetCell(IRow row, int cellIndex)
        {
            return row.GetCell(cellIndex) ?? row.CreateCell(cellIndex);

        }
        public ICell GetCell(int rowIndex, int cellIndex)
        {
            IRow row = sheet.GetRow(rowIndex) ?? sheet.CreateRow(rowIndex);
            ICell cell = row.GetCell(cellIndex) ?? row.CreateCell(cellIndex);
            return cell;
        }
        public decimal? GetNullableDecimal(int rowIndex, int cellIndex)
        {
            var cell = GetCell(rowIndex, cellIndex);
            if (!string.IsNullOrEmpty(cell.ToString()))
            {
                return Convert.ToDecimal(cell.NumericCellValue);
            }
            else
            {
                return null;
            }
        }
        public string GetValue(int rowIndex, int cellIndex)
        {
            var cell = GetCell(rowIndex, cellIndex);
            if (cell.CellFormula != "")
            {
                return baseFormulaEvaluator.Evaluate(cell).ToString();
            }
            else
            {
                return cell.ToString();
            }

        }
        public DateTime GetDate(int rowIndex, int cellIndex)
        {

            var cell = GetCell(rowIndex, cellIndex);
            return cell.DateCellValue;
        }
        /// <summary>
        /// 用于excel表格中列号字母转成列索引，从1对应A开始
        /// </summary>
        /// <param name="column">列号</param>
        /// <returns>列索引</returns>
        public int ColumnNameToIndex(string column)
        {
            if (!Regex.IsMatch(column.ToUpper(), @"[A-Z]+"))
            {
                throw new Exception("Invalid parameter");
            }
            int index = 0;
            char[] chars = column.ToUpper().ToCharArray();
            for (int i = 0; i < chars.Length; i++)
            {
                index += ((int)chars[i] - (int)'A' + 1) * (int)Math.Pow(26, chars.Length - i - 1);
            }
            return index - 1;
        }

        /// <summary>
        /// 用于将excel表格中列索引转成列号字母，从A对应1开始
        /// </summary>
        /// <param name="index">列索引</param>
        /// <returns>列号</returns>
        public string ColumnIndexToName(int index)
        {
            if (index < 0)
            {
                throw new Exception("Invalid parameter");
            }
            index -= 1;
            string column = string.Empty;
            do
            {
                if (column.Length > 0)
                {
                    index--;
                }
                column = ((char)(index % 26 + (int)'A')).ToString() + column;
                index = (int)((index - index % 26) / 26);
            } while (index >= 0);
            return column;
        }

        public void SetValue(int rowIndex, int cellIndex, object value)
        {
            IRow row = sheet.GetRow(rowIndex) ?? sheet.CreateRow(rowIndex);
            ICell cell = row.GetCell(cellIndex) ?? row.CreateCell(cellIndex);
            string val = null == value ? string.Empty : value.ToString();
            if (value != null)
                switch (value.GetType().FullName)
                {
                    case "System.String": //字符串类型
                        cell.SetCellValue(val);
                        break;
                    case "System.DateTime": //日期类型
                        DateTime dateV;
                        DateTime.TryParse(val, out dateV);
                        cell.SetCellValue(dateV);
                        break;
                    case "System.Boolean": //布尔型
                        bool boolV;
                        bool.TryParse(val, out boolV);
                        cell.SetCellValue(boolV);
                        break;
                    case "System.Int16": //整型
                    case "System.Int32":
                    case "System.Int64":
                    case "System.Byte":
                        int intV;
                        int.TryParse(val, out intV);
                        cell.SetCellValue(intV);
                        break;
                    case "System.Decimal": //浮点型
                    case "System.Double":
                        double doubV;
                        double.TryParse(val, out doubV);
                        cell.SetCellValue(doubV);
                        break;
                    case "System.DBNull": //空值处理
                        cell.SetCellValue("");
                        break;
                    default:
                        cell.SetCellValue("");
                        break;
                }
        }

        /// <summary>
        /// 设置有效性
        /// </summary>
        /// <param name="colName">列名</param>
        /// <param name="startRowIndex">开始行索引 0起</param>
        /// <param name="validSheetName">有效性 目标sheet</param>
        /// <param name="validateColName">有效性列名</param>
        /// <param name="validateRowStart">有效性开始行 0起</param>
        /// <param name="validateRowEnd">有效性结束行 0起</param>
        public void SetValidation(string colName, int startRowIndex,
            string validSheetName, string validateColName, int validateRowStart, int validateRowEnd)
        {
            var columnIndex = ColumnNameToIndex(colName);
            //设置数据有效性作用域
            var regions = new CellRangeAddressList(startRowIndex, 65535, columnIndex, columnIndex);
            //设置名称管理器管理数据源范围
            var range = workbook.CreateName();
            //                          验证页,              验证列名                  验证开始行
            range.RefersToFormula = validSheetName + "!$" + validateColName + "$" + (validateRowStart + 1) +
                //验证结束列             //验证结束行
                ":$" + validateColName + "$" + (validateRowStart + validateRowEnd);
            range.NameName = "dicRange" + columnIndex;
            //根据名称生成下拉框内容
            DVConstraint constraint = DVConstraint.CreateFormulaListConstraint("dicRange" + columnIndex);
            //绑定下拉框和作用区域
            var dataValidate = new HSSFDataValidation(regions, constraint);
            sheet.AddValidationData(dataValidate);
        }


        /// <summary>
        /// 区域复制函数
        /// </summary>
        /// <param name="region"></param> 
        public static void CopyArea(CopyRegionSettings region)
        {

            int toRowIndex = region.ToRowIndex;

            for (int fromRowIndex = region.StartRow; fromRowIndex <= region.EndRow; fromRowIndex++, toRowIndex++)
            {
                if (region.SkipRows != null && region.SkipRows.Contains(fromRowIndex))
                {
                    continue;
                }
                int toColIndex = region.ToCellIndex;
                for (int fromColIndex = region.StartCell; fromColIndex <= region.EndCell; fromColIndex++, toColIndex++)
                {
                    if (region.SkipCells != null && region.SkipCells.Contains(fromColIndex))
                    {
                        continue;
                    }
                    IRow sourceRow = region.FromSheet.GetRow(fromRowIndex);
                    ICell source = sourceRow.GetCell(fromColIndex);
                    if (sourceRow != null && source != null)
                    {
                        IRow changingRow = null;
                        ICell target = null;
                        changingRow = region.targetSheet.GetRow(toRowIndex);
                        if (changingRow == null)
                            changingRow = region.targetSheet.CreateRow(toRowIndex);
                        target = changingRow.GetCell(toColIndex);
                        if (target == null)
                            target = changingRow.CreateCell(toColIndex);
                        if (region.CopyData)//仅数据
                        {
                            //对单元格的值赋值
                            switch (source.CellType)
                            {
                                case CellType.Unknown:
                                    break;
                                case CellType.Numeric:
                                    target.SetCellValue(source.NumericCellValue);
                                    break;
                                case CellType.String:
                                    target.SetCellValue(source.StringCellValue);
                                    break;
                                case CellType.Formula:
                                    target.SetCellFormula(source.CellFormula);
                                    //target = e.EvaluateInCell(target);

                                    break;
                                case CellType.Blank:
                                    break;
                                case CellType.Boolean:
                                    target.SetCellValue(source.BooleanCellValue);
                                    break;
                                case CellType.Error:
                                    break;
                                default:
                                    target.SetCellValue(source.ToString());
                                    break;
                            }
                        }

                        if (region.CopyStyle)
                        {
                            //单元格的格式
                            target.CellStyle = source.CellStyle;
                        }
                    }
                }
            }
        }
        public void SaveAs(Stream stream)
        {
            //将内容写入数据流,以供下载 

            SetForceFormulaRecalculation(true);
            workbook.Write(stream);
        }
        private void SetForceFormulaRecalculation(bool value)
        {
            if (workbook is HSSFWorkbook)
            {
                ((HSSFWorkbook)workbook).ForceFormulaRecalculation = value;
            }
            else
            {
                ((XSSFWorkbook)workbook).SetForceFormulaRecalculation(value);
            }
        }
        public void SaveAs(string filePath)
        {
            //将内容写入临时文件,以供下载

            using (var fsTemp = File.OpenWrite(filePath))
            {
                SetForceFormulaRecalculation(true);
                workbook.Write(fsTemp);
            }
        }
        public string Save()
        {
            //workbook.ForceFormulaRecalculation = true;
            string filepath = Path.GetTempFileName();
            SaveAs(filepath);
            return filepath;
        }

        public void Dispose()
        {
            stream.Close();//GC.Collect();
        }
    }


}



