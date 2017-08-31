using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Xml;
using System.IO;
using System.Configuration;
using log4net;
using System.Data;
using System.Data.OleDb;
using System.Reflection;


namespace GetData
{
    class GWStation
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(GWStation));
        private string strUrl = "";
        private string strUrlBase = "";
        private string strSTNO = "";
        private string strSY = "";
        private string strQY = "";
        private string strEY = "";
        private string strQM = "";
        private string strMode = "";
        private string strFileName = "";
        private string strStationNo = "";
        private string strStationName = "";
        //private string strYearsList = "";
        private List<string> listYear = new List<string>();
        private string FileBasePath = "";
        private string OutputBasePath = "";

        #region 屬性
        public string UrlBase
        {
            get { return strUrlBase; }
            set { strUrlBase = value; }
        }

        public string STNO
        {
            get { return strSTNO; }
            set
            {
                strStationNo = value;
                strSTNO = value;
            }
        }

        public string SY
        {
            get { return strSY; }
            set { strSY = value; }
        }

        public string QY
        {
            get { return strQY; }
            set { strQY = value; }
        }

        public string EY
        {
            get { return strEY; }
            set { strEY = value; }
        }

        public string QM
        {
            get { return strQM; }
            set { strQM = value; }
        }

        public string Mode
        {
            get { return strMode; }
            set { strMode = value; }
        }

        public List<string> Years
        {
            get { return listYear; }
            set { listYear = value; }
        }

        #endregion

        public GWStation()
        {
            strUrlBase = @"http://" + ConfigurationManager.AppSettings["GWSTHttpPath"].Trim();
            strSY = "1994";
            strEY = DateTime.Today.Year.ToString();
            OutputBasePath = AppDomain.CurrentDomain.BaseDirectory + @"Output";
        }
        public void DailySavetotext()
        {
            XmlDocument doc = new XmlDocument();
            XmlNode node = null;
            System.Data.DataTable dt = null;
            //DataRow dr = null;
            try
            {
                FileBasePath = AppDomain.CurrentDomain.BaseDirectory + @"Station";
                if (!Directory.Exists(FileBasePath))
                {
                    Directory.CreateDirectory(FileBasePath);
                }
                if (!Directory.Exists(OutputBasePath))
                {
                    Directory.CreateDirectory(OutputBasePath);
                }
                strUrl = UrlBase + "?stno=" + STNO + "&sYY=" + SY + "&qYY=" + QY + "&eYY=" + EY + "&Mode=0";
                Logger.Debug("DailySavetotext URL = " + strUrl);
                strFileName = FileBasePath + "\\" + strStationNo + "_" + QY + ".xml";
                Logger.Debug("strFileName = " + strFileName);
                new WebClient().DownloadFile(strUrl, FileBasePath + "\\temp.xml");
                using (StreamReader reader = new StreamReader(FileBasePath + "\\temp.xml", System.Text.Encoding.UTF8))
                {
                    using (StreamWriter writer = new StreamWriter(strFileName))
                    {
                        writer.Write(reader.ReadToEnd().Replace("><", ">\r\n<"));
                    }
                }

                using (StreamReader reade = new StreamReader(strFileName, System.Text.Encoding.UTF8))
                {
                    doc.LoadXml(reade.ReadToEnd());
                }

                File.Delete(FileBasePath + "\\temp.xml");
                Logger.Debug("Delete path = " + FileBasePath + "\\temp.xml");

                dt = new System.Data.DataTable();
                dt.TableName = STNO;
                dt.Columns.Add("TIME");
                dt.Columns.Add("LV");
                strStationNo = STNO;
                //CreateExcel(OutputBasePath + "\\" + STNO + ".xlsx");

                using (StreamWriter writer = new StreamWriter(OutputBasePath + "\\" + STNO + "_" + QY + ".txt")) 
                {
                    writer.WriteLine("DATE,Level");
                    if (doc.SelectSingleNode("result") != null)
                    {
                        node = doc.SelectSingleNode("result");
                        strStationName = node.Attributes["stName"].Value.ToString();

                        if (doc.SelectSingleNode("//result//LineList//ROW") != null)
                        {
                            var nodeList = doc.SelectNodes("//result//LineList//ROW");
                            for (int i = 0; i < nodeList.Count; i++)
                            {
                                writer.WriteLine(nodeList[i]["DATE"].InnerText + "," + nodeList[i]["L"].InnerText);
                                //dr = dt.NewRow();
                                //dr["TIME"] = nodeList[i]["DATE"].InnerText;
                                //dr["LV"] = nodeList[i]["L"].InnerText;
                                //dt.Rows.Add(dr);
                            }
                        }
                    }
                }
                //AddData(QY, dt, OutputBasePath + "\\" + STNO + ".xlsx");
            }
            catch(Exception ex)
            {
                Logger.Error(ex.Message.ToString());
            }
        }

        public void GetYearsList()
        {
            XmlDocument doc = new XmlDocument();
            //XmlNode node = null;
            try
            {
                strUrl = UrlBase + "?stno=" + STNO + "&sYY=" + SY + "&eYY=" + EY + "&Mode=0";
                Logger.Debug("GetYearsList URL = " + strUrl);
                strFileName = FileBasePath + "\\" + @"Station" + strStationNo + ".xml";
                new WebClient().DownloadFile(strUrl, FileBasePath + "\\" + "temp.xml");
                using (StreamReader reader = new StreamReader(FileBasePath + "\\" + "temp.xml", System.Text.Encoding.UTF8))
                {
                    using (StreamWriter writer = new StreamWriter(strFileName))
                    {
                        writer.Write(reader.ReadToEnd().Replace("><", ">\r\n<"));
                    }
                }
                using (StreamReader readernew = new StreamReader(strFileName, System.Text.Encoding.UTF8))
                {
                    doc.LoadXml(readernew.ReadToEnd());
                }
                File.Delete(FileBasePath + "\\temp.xml");
                Logger.Debug("Delete path = " + FileBasePath + "\\temp.xml");


                if (doc.SelectSingleNode("//result//STDqvYear//yy") != null)
                {
                    var nodeList = doc.SelectNodes("//result//STDqvYear//yy");
                    for (int i = 0; i < nodeList.Count; i++)
                    {
                        listYear.Add(nodeList[i].InnerText);
                    }
                }
                else
                {
                    listYear.Clear();
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message.ToString());
            }
        }

        private void CreateExcel(string strPath)
        {
            Logger.Debug("CreateExcel");
            try
            {
                FileInfo fi = new FileInfo(strPath);
                Application App = new Application();

                App.Visible = false;//不顯示excel程式
                Workbook wb = App.Workbooks.Add(XlWBATemplate.xlWBATWorksheet);
                wb.Application.DisplayAlerts = false;
                wb.Application.AlertBeforeOverwriting = false;
                Worksheet ws = new Worksheet();
                ws.Application.DisplayAlerts = false;
                ws.Application.AlertBeforeOverwriting = false;
                ws.Name = STNO;

                wb.Worksheets.Add(Type.Missing, ws, Type.Missing, Type.Missing);
                wb.SaveAs(strPath, Microsoft.Office.Interop.Excel.XlFileFormat.xlExcel8, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                wb.Close(false, Type.Missing, Type.Missing);
                App.Workbooks.Close();
                App.Quit();
                //刪除 Windows工作管理員中的Excel.exe process，  
                System.Runtime.InteropServices.Marshal.ReleaseComObject(App);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(wb);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(ws);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message.ToString());
            }
        }
        private void AddData(string strYear, System.Data.DataTable dt, string strPath)
        {
            Logger.Debug("AddData");
            try
            {
                Application App = new Application();
                Workbook wb = App.Workbooks.Open(strPath);
                wb.Application.DisplayAlerts = false;
                wb.Application.AlertBeforeOverwriting = false;

                System.IO.FileInfo xlsAttribute = new FileInfo(strPath);
                xlsAttribute.Attributes = FileAttributes.Normal;

                Worksheet ws = new Worksheet();
                ws.Application.DisplayAlerts = false;
                ws.Application.AlertBeforeOverwriting = false;
                ws.Name = strYear;
                ws.Cells[1, 1] = "Date";
                ws.Cells[1, 2] = "Level";
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow dr = dt.Rows[i];
                    ws.Cells[i + 2, 1] = dr["TIME"];
                    ws.Cells[i + 2, 2] = dr["LV"];
                }

                wb.Worksheets.Add(Type.Missing, ws, Type.Missing, Type.Missing);
                wb.SaveAs(strPath, Microsoft.Office.Interop.Excel.XlFileFormat.xlExcel8, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                wb.Close(false, Type.Missing, Type.Missing);
                App.Workbooks.Close();
                App.Quit();
                //刪除 Windows工作管理員中的Excel.exe process，  
                System.Runtime.InteropServices.Marshal.ReleaseComObject(App);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(wb);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(ws);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message.ToString());
            }

        }

    }
}
