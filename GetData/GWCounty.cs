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
    class GWCounty
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(GWCounty));
        public Dictionary<string, string> County = new Dictionary<string, string>();

        private string strUrl = "";
        private string strUrlBase = "";
        private string strFileName = "";
        private string FileBasePath = "";

        #region 屬性
        public string UrlBase
        {
            get { return strUrlBase; }
            set { strUrlBase = value; }
        }
        #endregion

        public GWCounty()
        {
            Initialize();
        }
        public void Initialize()
        {
            strUrlBase = @"http://" + ConfigurationManager.AppSettings["GWCountyHttpPath"].Trim();
            County = new Dictionary<string, string>();
            XmlDocument doc = new XmlDocument();
            try
            {
                FileBasePath = AppDomain.CurrentDomain.BaseDirectory + @"GWCounty";
                if (!Directory.Exists(FileBasePath))
                {
                    Directory.CreateDirectory(FileBasePath);
                }
                strUrl = UrlBase;
                Logger.Debug("URL = " + strUrl);
                strFileName = FileBasePath + "\\" + "GWCounty.xml";
                new WebClient().DownloadFile(strUrl, FileBasePath + "\\" + "GWCounty_temp.xml");
                using (StreamReader reader = new StreamReader(FileBasePath + "\\" + "GWCounty_temp.xml", System.Text.Encoding.UTF8))
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
                File.Delete(FileBasePath + "\\" + "GWCounty_temp.xml");
                Logger.Debug("Delete path = " + FileBasePath + "\\" + "GWCounty_temp.xml");


                if (doc.SelectSingleNode("//DI//Region") != null)
                {
                    var nodeList = doc.SelectNodes("//DI//Region");
                    for (int i = 0; i < nodeList.Count; i++)
                    {
                        string key = nodeList[i]["coun_id"].InnerText;
                        string value = nodeList[i]["coun_na"].InnerText;
                        if (!County.ContainsKey(key) && key!="0") County.Add(key, value);
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
