using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using log4net;
using log4net.Config;
using System.IO;

namespace GetData
{
    /// <summary>
    /// App.xaml 的互動邏輯
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            XmlConfigurator.Configure(new System.IO.FileInfo(AppDomain.CurrentDomain.BaseDirectory + @"\log4net.config"));
        }
    }
}
