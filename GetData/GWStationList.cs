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

namespace GetData
{
    class GWStationList
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(GWStationList));
        public Dictionary<string, string> Station = new Dictionary<string, string>();

        private string strUrl = "";
        private string strUrlBase = "";
        private string strFileName = "";
        //private string strAreaItem = "";
        private string FileBasePath = "";

        #region 屬性
        public string UrlBase
        {
            get { return strUrlBase; }
            set { strUrlBase = value; }
        }
        #endregion
        public GWStationList()
        {
            Initialize();
        }

        public void Initialize()
        {
            strUrlBase = @"http://" + ConfigurationManager.AppSettings["GWStationListHttpPath"].Trim();
            Station = new Dictionary<string, string>();
        }
        public void GetSTList(string County)
        {
            XmlDocument doc = new XmlDocument();
            try
            {
                FileBasePath = AppDomain.CurrentDomain.BaseDirectory + @"GWStationList";
                if (!Directory.Exists(FileBasePath))
                {
                    Directory.CreateDirectory(FileBasePath);
                }
                strUrl = UrlBase + "&AreaItem=" + County;
                Logger.Debug("URL = " + strUrl);
                strFileName = FileBasePath + "\\" + @"GWStationList_" + County + ".xml";
                new WebClient().DownloadFile(strUrl, FileBasePath + "\\" + @"GWStationList_temp_" + County + ".xml");
                using (StreamReader reader = new StreamReader(FileBasePath + "\\" + @"GWStationList_temp_" + County + ".xml", System.Text.Encoding.UTF8))
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
                File.Delete(FileBasePath + "\\" + @"GWStationList_temp_" + County + ".xml");
                Logger.Debug("Delete path = " + "\\" + @"GWStationList_temp_" + County + ".xml");
                if (doc.SelectSingleNode("//DI//Region") != null)
                {
                    var nodeList = doc.SelectNodes("//DI//Region");
                    for (int i = 0; i < nodeList.Count; i++)
                    {
                        string key = nodeList[i]["wellno"].InnerText;
                        string value = nodeList[i]["well"].InnerText;
                        Station.Add(key, value);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message.ToString());
            }
        }
    }
}
