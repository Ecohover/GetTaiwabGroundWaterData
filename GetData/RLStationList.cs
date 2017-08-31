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
    class RLStationList
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(RLStationList));
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
        public RLStationList()
        {
            Initialize();
        }

        public void Initialize()
        {
            strUrlBase = @"http://" + ConfigurationManager.AppSettings["StationListHttpPath"].Trim();
            Station = new Dictionary<string, string>();
        }

        public void GetSTList(string Basin)
        {
            XmlDocument doc = new XmlDocument();
            try
            {
                FileBasePath = AppDomain.CurrentDomain.BaseDirectory + @"StationList";
                if (!Directory.Exists(FileBasePath))
                {
                    Directory.CreateDirectory(FileBasePath);
                }
                strUrl = UrlBase + "&AreaItem=" + Basin;
                strFileName = FileBasePath + "\\" + @"StationList_" + Basin + ".xml";
                new WebClient().DownloadFile(strUrl, FileBasePath + "\\" + @"StationListtemp_" + Basin + ".xml");
                using (StreamReader reader = new StreamReader(FileBasePath + "\\" + @"StationListtemp_" + Basin + ".xml", System.Text.Encoding.UTF8))
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
                if (doc.SelectSingleNode("//DI//Basin") != null)
                {
                    var nodeList = doc.SelectNodes("//DI//Basin");
                    for (int i = 0; i < nodeList.Count; i++)
                    {
                        string key = nodeList[i]["st_no"].InnerText;
                        string value = nodeList[i]["name_c"].InnerText;
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
