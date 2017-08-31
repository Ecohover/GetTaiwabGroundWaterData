using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.Xml;
using System.IO;
using log4net;
using System.Threading;
using System.Windows.Forms;

namespace GetData
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// http://gic.wra.gov.tw/gic/Water/Space/Main.aspx
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(MainWindow));
        GWStation objST = null;
        GWCounty objBasin = null;
        GWStationList objSTList = null;

        public MainWindow()
        {
            InitializeComponent();
            Initializeitem();
        }
        private void Initializeitem()
        {
            Logger.Debug("Initializeitem");
            objBasin=new GWCounty();
            cBBasin.SelectedValuePath = "Key";
            cBBasin.DisplayMemberPath = "Value";
            cBBasin.ItemsSource = objBasin.County;
        }

        private void btnRun_Click(object sender, RoutedEventArgs e)
        {
            Logger.Debug("btnRun_Click");
            try
            {
                foreach (string item in lBGetData.Items)
                {
                    objST = new GWStation();
                    objST.QY = item.Split(' ')[1];
                    objST.STNO = item.Split(' ')[0];
                    objST.DailySavetotext();
                }
                System.Windows.Forms.MessageBox.Show("資料讀取成功");
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message.ToString());
                System.Windows.Forms.MessageBox.Show("資料讀取失敗 error : " + ex.Message.ToString());
            }

        }

        private void cBBasin_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Logger.Debug("cBBasin_SelectionChanged");
            try
            {
                //cBST = new System.Windows.Controls.ComboBox();
                objSTList = new GWStationList();
                objST = new GWStation();
                cBST.SelectedValuePath = "Key";
                cBST.DisplayMemberPath = "Value";
                objSTList.GetSTList(cBBasin.SelectedValue.ToString());
                cBST.ItemsSource = objSTList.Station;
                objST.Years.Clear();
                lBYear.ItemsSource = objST.Years;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message.ToString());
            }
        }

        private void cBST_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Logger.Debug("cBST_SelectionChanged");
            try
            {
                //lBYear = new System.Windows.Controls.ListBox();
                if (cBST.SelectedValue != null)
                {
                    objST = new GWStation();
                    objST.STNO = cBST.SelectedValue.ToString();
                    objST.GetYearsList();
                    lBYear.ItemsSource = objST.Years;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message.ToString());
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            Logger.Debug("btnAdd_Click");
            foreach (var item in lBYear.SelectedItems)
            {
                var temp = item;
                temp = cBST.SelectedValue.ToString() + " " + temp;
                lBGetData.Items.Remove(temp);
                lBGetData.Items.Add(temp);
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            Logger.Debug("btnDelete_Click");
            List<string> temp = new List<string>();
            foreach (string item in lBGetData.SelectedItems)
            {
                temp.Add(item);
            }
            foreach (var item in temp)
            {
                lBGetData.Items.Remove(item);
            }
        }

        private void btnAddAll_Click(object sender, RoutedEventArgs e)
        {
            Logger.Debug("btnAddAll_Click");
            foreach (var item in lBYear.Items)
            {
                var temp = item;
                temp = cBST.SelectedValue.ToString() + " " + temp;
                lBGetData.Items.Remove(temp);
                lBGetData.Items.Add(temp);
            }
        }

        private void btnDeleteAll_Click(object sender, RoutedEventArgs e)
        {
            Logger.Debug("btnDeleteAll_Click");
            List<string> temp = new List<string>();
            foreach (string item in lBGetData.Items)
            {
                temp.Add(item);
            }
            foreach (var item in temp)
            {
                lBGetData.Items.Remove(item);
            }
        }

        private void OpenFile()
        {
            OpenFileDialog file = new OpenFileDialog();
            file.Filter = "Text Files (*.txt)|*.txt";
            file.ShowDialog();
            ReadSTText(file.FileName);
        }

        private void ReadSTText(string strPath)
        {
            string temp="";
            try
            {
                using (StreamReader reader = new StreamReader(strPath, System.Text.Encoding.UTF8))
                {
                    while (reader.Peek() >= 0)
                    {
                        temp = reader.ReadLine();
                        if (!temp.Trim().Equals(""))
                        {
                            lBGetData.Items.Remove(temp);
                            lBGetData.Items.Add(temp);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message.ToString());
            }
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFile();

        }
    }
}
