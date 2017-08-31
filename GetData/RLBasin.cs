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
    class RLContry
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(RLContry));
        public Dictionary<string, string> Basin = new Dictionary<string, string>();

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

        public RLContry()
        {
            Initialize();
        }
        public void Initialize()
        {
            strUrlBase = @"http://" + ConfigurationManager.AppSettings["BasinHttpPath"].Trim();
            Basin = new Dictionary<string, string>();
            XmlDocument doc = new XmlDocument();
            try
            {
                FileBasePath = AppDomain.CurrentDomain.BaseDirectory + @"Basin";
                if (!Directory.Exists(FileBasePath))
                {
                    Directory.CreateDirectory(FileBasePath);
                }
                strUrl = UrlBase ;
                strFileName = FileBasePath + "\\" + "Basin.xml";
                new WebClient().DownloadFile(strUrl, FileBasePath + "\\" + "Basintemp.xml");
                using (StreamReader reader = new StreamReader(FileBasePath + "\\" + "Basintemp.xml", System.Text.Encoding.UTF8))
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
                        string key = nodeList[i]["rv_no"].InnerText;
                        string value = nodeList[i]["rv_name"].InnerText;
                        if (!Basin.ContainsKey(key) && key!="0") Basin.Add(key, value);
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
