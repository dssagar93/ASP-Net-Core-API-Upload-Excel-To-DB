using ExcelToDatabase.Helpers;
using ExcelToDatabase.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ExcelToDatabase.CustomExceptions;

namespace ExcelToDatabase.Services
{
    public class BrandChannelService : IBrandChannelService
    {
        private readonly BrandChannelDbContext _brandChannelDbContext;

        public int firstCol = 0;
        public int firstRow = 0;

        public int lastCol = 0;
        public int lastRow = 0;
        List<string> colsRequired = new List<string>(new string[] { "Brand", "YYYYMM", "DocNo", "Channel", "TimeBandStart", "TimeBandEnd", "Amount", "ActivityDate", "MWUID", "IsMonitored", "IsDisputed" });

        private readonly string _config;
        public BrandChannelService(BrandChannelDbContext bctxt, IConfiguration config)
        {
            _brandChannelDbContext = bctxt;
            _config = config.GetConnectionString("AppConnectionString");
        }

        public string UploadExcel(IFormFile formFile)
        {
            var ms = new MemoryStream();
            formFile.CopyTo(ms);

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;


            using (ExcelPackage package = new ExcelPackage(ms))
            {
                ExcelWorksheet sheet = package.Workbook.Worksheets[0];

                var start = sheet.Dimension.Start;
                var end = sheet.Dimension.End;

                firstCol = start.Column;
                firstRow = start.Row;

                lastCol = end.Column;
                lastRow = end.Row;

                ValidateColumnNames(sheet);

                SaveToDB(sheet);
            }
        }



        public DataTable GetBrandChannel(string query)
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(_config))
            {

                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = conn;
                    command.CommandText = "SP_FilterBrandChannel";
                    command.Parameters.Add("@FilterQuery", SqlDbType.NVarChar);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters["@FilterQuery"].Value = query;
                    conn.Open();

                    using (var adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(dt);
                    }
                    conn.Close();
                }

            }
            return dt;
        }







        public void ValidateColumnNames(ExcelWorksheet ws)
        {

            List<string> colNames = WorksheetHelper.GetColNamesInExcelSheet(ws,lastCol);
            foreach (var c in colsRequired)
            {
                if (!colNames.Contains(c))
                {
                    throw new ValidationException("Column name " + c + " doesn't exist in the excel file");
                }
            }

        }


        public void SaveToDB(ExcelWorksheet ws)
        {
            
                DataTable dt = WorksheetHelper.WorksheetToDT(ws, colsRequired, firstRow, lastRow, firstCol, lastCol);
                DataTable manipulatedDt = ManipulatedDT(dt);

                using (SqlConnection conn = new SqlConnection(_config))
                {

                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = conn;

                        command.CommandText = "SP_BulkInsertBrandChannel";
                        command.Parameters.Add("@brandchannel", SqlDbType.Structured);
                        command.Parameters["@brandchannel"].TypeName = "UDT_BrandChannels";

                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters["@brandchannel"].Value = manipulatedDt;

                        conn.Open();
                        command.ExecuteNonQuery();
                        conn.Close();

                    }
                }
        }


       


        public DataTable ManipulatedDT(DataTable ogTable)
        {
            DataTable dt_mod = new DataTable();

            Type type = typeof(BrandChannel);
            var properties = type.GetProperties();


            #region Column Addition

            foreach (PropertyInfo info in properties)
            {
                dt_mod.Columns.Add(new DataColumn(info.Name, Nullable.GetUnderlyingType(info.PropertyType) ?? info.PropertyType));
            }
            #endregion


            #region Value Addition With Manipulation

            foreach (DataRow row in ogTable.Rows)
            {
                int hours = 0;
                TimeSpan totalSpan = new TimeSpan();
                DataRow rowNew = dt_mod.Rows.Add();
                string ImpactBase = "";
                foreach (DataColumn col in ogTable.Columns)
                {
                    string colName = col.ColumnName;
                    var cellData = row[col].ToString();

                    if (colName == "YYYYMM")
                    {
                        rowNew["YY_Year"] = cellData.Substring(0, 4);
                        rowNew["MM_Month"] = cellData.Substring(4);
                    }
                    else if (colName == "TimeBandStart" || colName == "TimeBandEnd")
                    {
                        hours = Int32.Parse(cellData.Split(":")[0]);
                        totalSpan = totalSpan + TimeSpan.FromHours(hours);
                        rowNew[colName] = cellData;
                    }
                    else if (colName == "ActivityDate")
                    {
                        var acDate= totalSpan.Days > 1 ? DateTime.Parse(cellData).AddDays(totalSpan.Days) : DateTime.Parse(cellData); 
                        rowNew[colName] = acDate;
                    }
                    else if (colName == "ActivityDate")
                    {
                        var acDate = totalSpan.Days > 1 ? DateTime.Parse(cellData).AddDays(totalSpan.Days) : DateTime.Parse(cellData);
                        rowNew[col.Ordinal] = acDate;
                    }
                    else if (colName == "Amount")
                    {
                        rowNew[colName] = cellData;
                        ImpactBase = double.Parse(cellData)>=1000 ? "Impact":"Base";
                    }
                    else if (colName == "IsMonitored" || colName =="IsDisputed")
                    {
                        rowNew[colName] = Convert.ToBoolean(Convert.ToInt16(cellData));
                    }
                    else
                    {
                        rowNew[colName] = cellData;
                    }

                }

                rowNew[dt_mod.Columns.Count - 1] = ImpactBase;
            }

            #endregion

            DataTable distinctTable = dt_mod.AsEnumerable()
                .GroupBy(x => x.Field<string>("MWUID"))
                .Select(y => y.First())
                .CopyToDataTable();

            return distinctTable;
        }
    }
}
