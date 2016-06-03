using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using System.Data;
using System.IO;
using System.Reflection;
using System.Collections.ObjectModel;

namespace LOQ_Script_Gui
{
    class ExcelIO
    {
        static List<string> columns = new List<string>()
        {
            "POSITION", "TRANS", "TRANS_WAIT_FOR",	"SANS", "SANS_WAIT_FOR", "PERIOD", "SAMPLE_IDENTIFICATION_STRING", 
            "THICKNESS", "TEMPERATURE", "TEMPERATURE2", "FIELD", "SHEAR_RATE_1", "SHEAR_RATE_2", "SHEAR_ANGLE_1", "SHEAR_ANGLE_2",
            "PRE_COMMAND", "POST_COMMAND", "RB_NUMBER"
        };

        // The last column used in Excel
        static string LastColumn = "R";

        DataSet dataset = new DataSet();

        public void OpenExcelWorkBook(string filename)
        {
            // Create connection string variable
            String sConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;" +
                "Data Source=" + filename + ";" +
                "Extended Properties='Excel 8.0;HDR=Yes;IMEX=1;'";

            // Create connection object
            OleDbConnection conn = new OleDbConnection(sConnectionString);

            try
            {
                // Open connection
                conn.Open();

                String[] names = GetWorkSheetNames(conn);
                // Create new OleDbCommand to return data from  the first worksheet
                OleDbCommand cmd = new OleDbCommand(@"SELECT * FROM [" + names[0] + "]", conn);

                // Create new OleDbDataAdapter that is used to build a DataSet
                // based on the preceding SQL SELECT statement
                OleDbDataAdapter adapter = new OleDbDataAdapter();

                // Pass the Select command to the adapter
                adapter.SelectCommand = cmd;

                // Fill the DataSet with the information from the worksheet
                adapter.Fill(dataset, "XLData");

            }
            catch (Exception ex)
            {
                dataset = null;
                throw new IOException("Could not open Excel spreadsheet");
            }
            finally
            {
                // Clean up objects.
                conn.Close();
                conn.Dispose();
            }
        }

        public List<Experiment> ExtractExperiments()
        {
            List<Experiment> experiments = new List<Experiment>();

            if (dataset != null)
            {
                if (dataset.Tables != null && dataset.Tables.Count > 0)
                {
                    if (dataset.Tables[0].Rows != null && dataset.Tables[0].Rows.Count > 0)
                    {
                        // Loop through the rows and create an experiment for each
                        foreach (DataRow dr in dataset.Tables[0].Rows)
                        {
                            Experiment exp = new Experiment();

                            // Position
                            if (!string.IsNullOrWhiteSpace(dr[0].ToString()))
                            {
                                exp.Position = dr[0].ToString();
                            }

                            // Trans
                            if (!string.IsNullOrWhiteSpace(dr[1].ToString()))
                            {
                                exp.Trans = CastToDouble(dr[1].ToString());
                            }

                            // Trans wait
                            if (!string.IsNullOrWhiteSpace(dr[2].ToString()))
                            {
                                exp.TransWait = dr[2].ToString();
                            }

                            // Sans
                            if (!string.IsNullOrWhiteSpace(dr[3].ToString()))
                            {
                                exp.Sans = CastToDouble(dr[3].ToString());
                            }

                            // Sans wait
                            if (!string.IsNullOrWhiteSpace(dr[4].ToString()))
                            {
                                exp.SansWait = dr[4].ToString();
                            }

                            // Period
                            if (!string.IsNullOrWhiteSpace(dr[5].ToString()))
                            {
                                exp.Period = dr[5].ToString();
                            }

                            // Sample ID
                            if (!string.IsNullOrWhiteSpace(dr[6].ToString()))
                            {
                                exp.Sample = dr[6].ToString();
                            }

                            // Thickness
                            if (!string.IsNullOrWhiteSpace(dr[7].ToString()))
                            {
                                exp.Thickness = dr[7].ToString();
                            }

                            // Temperature 1
                            if (!string.IsNullOrWhiteSpace(dr[8].ToString()))
                            {
                                exp.Temperature1 = dr[8].ToString();
                            }

                            // Temperature 2
                            if (!string.IsNullOrWhiteSpace(dr[9].ToString()))
                            {
                                exp.Temperature2 = dr[9].ToString();
                            }

                            // Field
                            if (!string.IsNullOrWhiteSpace(dr[10].ToString()))
                            {
                                exp.Field = dr[10].ToString();
                            }

                            // Shear rate 1
                            if (!string.IsNullOrWhiteSpace(dr[11].ToString()))
                            {
                                exp.ShearRate1 = dr[11].ToString();
                            }

                            // Shear rate 2
                            if (!string.IsNullOrWhiteSpace(dr[12].ToString()))
                            {
                                exp.ShearRate2 = dr[12].ToString();
                            }

                            // Shear angle 1
                            if (!string.IsNullOrWhiteSpace(dr[13].ToString()))
                            {
                                exp.ShearAngle1 = dr[13].ToString();
                            }

                            // Shear angle 2
                            if (!string.IsNullOrWhiteSpace(dr[14].ToString()))
                            {
                                exp.ShearAngle2 = dr[14].ToString();
                            }

                            // Pre-command
                            if (!string.IsNullOrWhiteSpace(dr[15].ToString()))
                            {
                                exp.PreCommand = dr[15].ToString();
                            }

                            // Post-command
                            if (!string.IsNullOrWhiteSpace(dr[16].ToString()))
                            {
                                exp.PostCommand = dr[16].ToString();
                            }

                            // RB
                            if (!string.IsNullOrWhiteSpace(dr[17].ToString()))
                            {
                                exp.RbNumber = dr[17].ToString();
                            }

                            experiments.Add(exp);
                        }

                    }
                }
            }

            return experiments;
        }

        private static double CastToDouble(string val)
        {
            double result;
            if (Double.TryParse(val, out result))
            {
                return result;
            }

            return 0.0;
        }

        private static String[] GetWorkSheetNames(OleDbConnection conn)
        {
            System.Data.DataTable dt = null;

            try
            {
                // Get the data table containg the schema guid
                dt = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

                if (dt == null)
                {
                    return null;
                }

                String[] excelSheets = new String[dt.Rows.Count];
                int i = 0;

                // Add the sheet name to the string array
                foreach (DataRow row in dt.Rows)
                {
                    excelSheets[i] = row["TABLE_NAME"].ToString();
                    i++;
                }

                return excelSheets;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                if (dt != null)
                {
                    dt.Dispose();
                }
            }
        }

        public void ExportDataToExcel(ObservableCollection<Experiment> runs)
        {
            Microsoft.Office.Interop.Excel.Application xlApp;
            Microsoft.Office.Interop.Excel.Workbook xlBook;
            Microsoft.Office.Interop.Excel.Worksheet xlSheet;
            Microsoft.Office.Interop.Excel.Range xlRange;

            try
            {
                // Start Excel
                xlApp = new Microsoft.Office.Interop.Excel.Application();
                xlApp.Visible = true;

                // Create a workbook
                xlBook = xlApp.Workbooks.Add(Missing.Value);

                // Set to page
                xlSheet = (Microsoft.Office.Interop.Excel.Worksheet)xlBook.ActiveSheet;

                // Create headings
                for (int i = 0; i < columns.Count; ++i)
                {
                    xlSheet.Cells[1, i + 1] = columns[i];
                }

                xlSheet.get_Range("A1", LastColumn + "1").Font.Bold = true;

                // Enter data
                for (int i = 0; i < runs.Count; ++i)
                {
                    Experiment exp = runs[i];
                    List<object> data = new List<object>()
                    {
                        exp.Position, exp.Trans.ToString(), exp.TransWait, exp.Sans.ToString(), exp.SansWait,
                        exp.Period, exp.Sample, exp.Thickness, exp.Temperature1, exp.Temperature2, exp.Field, 
                        exp.ShearRate1, exp.ShearRate2, exp.ShearAngle1, exp.ShearAngle2, exp.PreCommand,
                        exp.PostCommand, exp.RbNumber
                    };

                    int row = i + 2;
                    var range = xlSheet.get_Range("A" + row, LastColumn + row);
                    range.Value = data.ToArray();
                }

                //Autofit columns
                xlRange = xlSheet.get_Range("A1", LastColumn + "1");
                xlRange.EntireColumn.AutoFit();

                //Detach
                xlApp.UserControl = true;

            }
            catch (Exception err)
            {
                throw err;
            }
        }
    }
}
