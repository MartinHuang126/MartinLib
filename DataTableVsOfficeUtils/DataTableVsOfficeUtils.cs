using System;
using System.Data;
using System.IO;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;

namespace DataTableVsOfficeUtils
{
    public class DataTableVsOfficeUtils
    {
        public static object _lockobj = new object();
        /// <summary>
        /// 将excel中的数据导入到DataTable中
        /// </summary>
        /// <param name="sheetName">excel工作薄sheet的名称</param>
        /// <param name="isFirstRowColumn">第一行是否是DataTable的列名</param>
        /// <param name="fileName">文件路径</param>
        /// <returns>返回的DataTable</returns>
        public static DataTable ExcelToDataTable(string sheetName, bool isFirstRowColumn, string fileName)
        {
            if (string.IsNullOrEmpty(sheetName))
            {
                throw new ArgumentNullException(sheetName);
            }
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException(fileName);
            }
            var data = new DataTable();
            IWorkbook workbook = null;
            FileStream fs = null;
            lock (_lockobj)
            {
                try
                {
                    fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                    if (fileName.IndexOf(".xlsx", StringComparison.Ordinal) > 0)
                    {
                        workbook = new XSSFWorkbook(fs);
                    }
                    else if (fileName.IndexOf(".xls", StringComparison.Ordinal) > 0)
                    {
                        workbook = new HSSFWorkbook(fs);
                    }

                    ISheet sheet = null;
                    if (workbook != null)
                    {
                        //如果没有找到指定的sheetName对应的sheet，则尝试获取第一个sheet
                        sheet = workbook.GetSheet(sheetName) ?? workbook.GetSheetAt(0);
                    }
                    if (sheet == null) return data;
                    var firstRow = sheet.GetRow(0);
                    //一行最后一个cell的编号 即总的列数
                    int cellCount = firstRow.LastCellNum;
                    int startRow;
                    if (isFirstRowColumn)
                    {
                        for (int i = firstRow.FirstCellNum; i < cellCount; ++i)
                        {
                            var cell = firstRow.GetCell(i);
                            var cellValue = cell.StringCellValue;
                            if (cellValue == null) continue;
                            var column = new DataColumn(cellValue);
                            data.Columns.Add(column);
                        }
                        startRow = sheet.FirstRowNum + 1;
                    }
                    else
                    {
                        startRow = sheet.FirstRowNum;
                    }
                    //最后一列的标号
                    var rowCount = sheet.LastRowNum;
                    for (var i = startRow; i <= rowCount; ++i)
                    {
                        var row = sheet.GetRow(i);
                        //没有数据的行默认是null
                        if (row == null) continue;
                        var dataRow = data.NewRow();
                        for (int j = row.FirstCellNum; j < cellCount; ++j)
                        {
                            //同理，没有数据的单元格都默认是null
                            if (row.GetCell(j) != null)
                                dataRow[j] = row.GetCell(j).ToString();
                        }
                        data.Rows.Add(dataRow);
                    }

                    return data;
                }
                catch (IOException ioex)
                {
                    throw new IOException(ioex.Message);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                finally
                {
                    if (fs != null)
                    {
                        fs.Close();
                    }
                }
            }
            
        }

        /// <summary>
        /// 将DataTable数据导入到excel中
        /// </summary>
        /// <param name="data">要导入的数据</param>
        /// <param name="isColumnWritten">DataTable的列名是否要导入</param>
        /// <param name="sheetName">要导入的excel的sheet的名称</param>
        /// <param name="fileName">文件夹路径</param>
        /// <returns>导入数据行数(包含列名那一行)</returns>
        public static int DataTableToExcel(DataTable data, string sheetName, bool isColumnWritten, string fileName)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            if (string.IsNullOrEmpty(sheetName))
            {
                throw new ArgumentNullException(sheetName);
            }
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException(fileName);
            }
            IWorkbook workbook = null;
            if (fileName.IndexOf(".xlsx", StringComparison.Ordinal) > 0)
            {
                workbook = new XSSFWorkbook();
            }
            else if (fileName.IndexOf(".xls", StringComparison.Ordinal) > 0)
            {
                workbook = new HSSFWorkbook();
            }

            FileStream fs = null;
            lock (_lockobj)
            {
                try
                {
                    fs = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    ISheet sheet;
                    if (workbook != null)
                    {
                        sheet = workbook.CreateSheet(sheetName);
                    }
                    else
                    {
                        return -1;
                    }

                    int j;
                    int count;
                    //写入DataTable的列名，写入单元格中
                    if (isColumnWritten)
                    {
                        var row = sheet.CreateRow(0);
                        for (j = 0; j < data.Columns.Count; ++j)
                        {
                            row.CreateCell(j).SetCellValue(data.Columns[j].ColumnName);
                        }
                        count = 1;
                    }
                    else
                    {
                        count = 0;
                    }
                    //遍历循环datatable具体数据项
                    int i;
                    for (i = 0; i < data.Rows.Count; ++i)
                    {
                        var row = sheet.CreateRow(count);
                        for (j = 0; j < data.Columns.Count; ++j)
                        {
                            row.CreateCell(j).SetCellValue(data.Rows[i][j].ToString());
                        }
                        ++count;
                    }
                    //将文件流写入到excel
                    workbook.Write(fs);
                    return count;
                }
                catch (IOException ioex)
                {
                    throw new IOException(ioex.Message);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                finally
                {
                    if (fs != null)
                    {
                        fs.Close();
                        workbook.Close();
                    }
                }
            }
            
        }
    }
}
