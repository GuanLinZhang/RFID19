using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using ReaderB;
using shoppingCartLib;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using Microsoft.Win32;
using System.Windows.Media.Imaging;
using System.Reflection;
using System;
using System.Windows.Threading;
using System.Threading;
using System.Windows.Input;

namespace RFID915
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        #region MEMBERS
        
        //private static readonly string connStr = System.Configuration.ConfigurationSettings.AppSettings["connect"].ToString();
        private static readonly string ConnStr = "Server=localhost;Database=super;Trusted_Connection=True;";

        private AutoResetEvent runEvent = new AutoResetEvent(false);

        private DispatcherTimer timer;
        private int timerCount; 
        private readonly int TIMES = 20; //up to 5 times

        private ObservableCollection<ReadEPCData> listview_Control;
        private ObservableCollection<CheckOutData> listbox_CheckOut;

        private Product theProduct;
        private DataTable DTlistView;
        private DataTable DTlistBox;
        
        
        //private bool fAppClosed; //在测试模式下响应关闭应用程序
        private byte scantime = 0x30; //default for 3 sec
        private bool InventoryFinish = true;  //disable button

        private byte fComAdr = 0xFF; //当前操作的ComAdr
        private byte fBaud;  //波特率      
        private int fCmdRet = 30; //所有执行指令的返回值
        //private int ferrorcode;
        private bool ComOpen = false; //串口打开标识
        //private bool breakflag = false;

        //private double fdminfre;
        //private double fdmaxfre;

        //private byte Maskadr;
        //private byte MaskLen;
        //private byte MaskFlag;        

        private int fOpenComIndex; //打开的串口索引号
        private int frmcomportindex; //？返回串口句柄索引号

        //public bool fIsInventoryScan; //协议6C
        //public bool fisinventoryscan_6B; //协议6B

        private byte[] fOperEPC = new byte[36];
        private byte[] fPassWord = new byte[4];
        private byte[] fOperID_6B = new byte[8];

        //private int CardNum1 = 0;
                      
        //private bool fTimer_6B_ReadWrite;
        private string fInventory_EPC_List; //存贮询查列表（如果读取的数据没有变化，则不进行刷新）

        //ArrayList list = new ArrayList();

        //private double x_z;
        //private double y_f;

        #endregion

        #region WINDOW BUSINESS METHODS

        public MainWindow()
        {
            InitializeComponent();
            WindowInit();
        }

        private void WindowInit()
        {
            //BtnOpenPort.Click += BtnOpenPort_Click;
            //BtnClosePort.Click += BtnClosePort_Click;
            //ToggleSwitch Toggle = new ToggleSwitch();
            Toggle.IsChecked = ComOpen;
            Toggle.IsCheckedChanged += Toggle_IsCheckedChanged;
            RFID915.Closing += RFID915_Closing;


            BtnIntervalSet.Click += BtnIntervalSet_Click;
            BtnInventory.Click += BtnInventory_Click;
            BtnClearListViewEPC.Click += BtnClearListViewEPC_Click;

            
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 100); //100ms
            timer.Tick += new EventHandler(Inventory_Timer_Elapsed); //Inventory_Timer_Elapsed: make count

            theProduct = new Product();
            
            listview_Control = new ObservableCollection<ReadEPCData>();
            listViewEPC.ItemsSource = listview_Control;  //Binding DataSource to ListView

            DTlistView = new DataTable();
            listViewProduct.ItemsSource = DTlistView.DefaultView; //Binding DataSource to ListView

            DTlistBox = new DataTable();
            //listbox_CheckOut = new ObservableCollection<CheckOutData>();
            ListBox_DisplayItems.ItemsSource = DTlistBox.DefaultView;  //Binding DataSource to ListBox
            //ListBox_DisplayItems.ItemsSource = listbox_CheckOut;
            ListBox_ItemsInOrder.ItemsSource = DTlistBox.DefaultView;


            BtnDataBaseRead.Click += BtnDataBaseRead_Click;
            BtnUpLoadImage.Click += BtnUpLoadImage_Click;
            BtnDataBaseInsert.Click += BtnDataBaseInsert_Click;

            TbProductID.DataContext = theProduct;
            TbName.DataContext = theProduct;
            TbType.DataContext = theProduct;
            TbUnitPrice.DataContext = theProduct;
            TbStore.DataContext = theProduct;
            ImgDefault.DataContext = theProduct;

            listViewProduct.SelectionChanged += ListViewProduct_SelectionChanged;

            BtnCheckOut.MouseLeftButtonDown += BtnCheckOut_MouseLeftButtonDown;
            Btn_Pay.MouseLeftButtonDown += Btn_Pay_MouseLeftButtonDown;
            Btn_Cancel.MouseLeftButtonDown += Btn_Cancel_MouseLeftButtonDown;
            Btn_Help.MouseLeftButtonDown += Btn_Help_MouseLeftButtonDown;

        }
        
        private void RFID915_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //throw new System.NotImplementedException();
            int closePortResult = 30;
            closePortResult = StaticClassReaderB.CloseSpecComPort(fOpenComIndex);
            Toggle.IsChecked = false;
            ComOpen = false;
        }

        #endregion

        #region TOOLS METHODS

        private string GetReturnCodeDesc(int cmdRet)
        {
            switch (cmdRet)
            {
                case 0x00:
                    return "操作成功";
                case 0x01:
                    return "询查时间结束前返回";
                case 0x02:
                    return "指定的询查时间溢出";
                case 0x03:
                    return "本条消息之后，还有消息";
                case 0x04:
                    return "读写模块存储空间已满";
                case 0x05:
                    return "访问密码错误";
                case 0x09:
                    return "销毁密码错误";
                case 0x0a:
                    return "销毁密码不能为全0";
                case 0x0b:
                    return "电子标签不支持该命令";
                case 0x0c:
                    return "对该命令，访问密码不能为全0";
                case 0x0d:
                    return "电子标签已经被设置了读保护，不能再次设置";
                case 0x0e:
                    return "电子标签没有被设置读保护，不需要解锁";
                case 0x10:
                    return "有字节空间被锁定，写入失败";
                case 0x11:
                    return "不能锁定";
                case 0x12:
                    return "已经锁定，不能再次锁定";
                case 0x13:
                    return "参数保存失败,但设置的值在读写模块断电前有效";
                case 0x14:
                    return "无法调整";
                case 0x15:
                    return "询查时间结束前返回";
                case 0x16:
                    return "指定的询查时间溢出";
                case 0x17:
                    return "本条消息之后，还有消息";
                case 0x18:
                    return "读写模块存储空间已满";
                case 0x19:
                    return "电子不支持该命令或者访问密码不能为0";
                case 0xFA:
                    return "有电子标签，但通信不畅，无法操作";
                case 0xFB:
                    return "无电子标签可操作";
                case 0xFC:
                    return "电子标签返回错误代码";
                case 0xFD:
                    return "命令长度错误";
                case 0xFE:
                    return "不合法的命令";
                case 0xFF:
                    return "参数错误";
                case 0x30:
                    return "通讯错误";
                case 0x31:
                    return "CRC校验错误";
                case 0x32:
                    return "返回数据长度有错误";
                case 0x33:
                    return "通讯繁忙，设备正在执行其他指令";
                case 0x34:
                    return "繁忙，指令正在执行";
                case 0x35:
                    return "端口已打开";
                case 0x36:
                    return "端口已关闭";
                case 0x37:
                    return "无效句柄";
                case 0x38:
                    return "无效端口";
                case 0xEE:
                    return "返回指令错误";
                default:
                    return "";
            }
        }

        private string GetErrorCodeDesc(int cmdRet)
        {
            switch (cmdRet)
            {
                case 0x00:
                    return "其它错误";
                case 0x03:
                    return "存储器超限或不被支持的PC值";
                case 0x04:
                    return "存储器锁定";
                case 0x0b:
                    return "电源不足";
                case 0x0f:
                    return "非特定错误";
                default:
                    return "";
            }
        }

        private byte[] HexStringToByteArray(string s)
        {
            s.Replace(" ", "");
            byte[] buffer = new byte[s.Length / 2];
            for(int i = 0; i < s.Length; i += 2)
            {
                buffer[i / 2] = System.Convert.ToByte(s.Substring(i, 2), 16);
            }
            return buffer;
        }

        private string ByteArrayToHexString(byte[] data)
        {
            StringBuilder sb = new StringBuilder(data.Length * 3);
            foreach (byte b in data)
                sb.Append(System.Convert.ToString(b, 16).PadLeft(2, '0'));
            return sb.ToString().ToUpper();

        }

        private DataTable ToDataTable<T>(ObservableCollection<T> list)
        {
            //创建属性的集合    
            List<PropertyInfo> pList = new List<PropertyInfo>();
            //获得反射的入口    
            Type type = typeof(T);
            DataTable dt = new DataTable();
            //把所有的public属性加入到集合 并添加DataTable的列    
            Array.ForEach<PropertyInfo>(type.GetProperties(), p => { pList.Add(p); dt.Columns.Add(p.Name, p.PropertyType); });
            foreach (var item in list)
            {
                //创建一个DataRow实例    
                DataRow row = dt.NewRow();
                //给row 赋值    
                pList.ForEach(p => row[p.Name] = p.GetValue(item, null));
                //加入到DataTable    
                dt.Rows.Add(row);
            }
            return dt;
        }
        
        #endregion

        #region BUSINESS METHODS

        private void BtnOpenPort_Click(object sender, RoutedEventArgs e)
        {
            int port = 0;
            fComAdr = System.Convert.ToByte("FF", 16);
            //fBaud = System.Convert.ToByte('5');  //5 - 576000
            fBaud = 5;
            int openResultCode = 30;
            //string openResultStr = "Nothing Happened";

            try
            {
                openResultCode = StaticClassReaderB.AutoOpenComPort(ref port, ref fComAdr, fBaud, ref fOpenComIndex);
                //fOpenComIndex = frmcomportindex;                

                if (openResultCode == 0)
                {
                    ComOpen = true;
                }
                else if((fCmdRet == 0x35) | (fCmdRet == 0x30)) //0011 1001 | 0011 0000
                {
                    StaticClassReaderB.CloseSpecComPort(fOpenComIndex);
                    ComOpen = false;
                }
            }
            finally
            {


                //LbStatInfo.Content = System.Convert.ToString(GetReturnCodeDesc(openResultCode));
                //LbStatPort.Content = "COM:" + System.Convert.ToString(port);
                //LbStatBaud.Content = "Baud:" + System.Convert.ToString(fBaud);
                //LbStatReaderType.Content = "Type:";
                //LbStatResultCode.Content = "RCode:" + System.Convert.ToString(openResultCode);
                //LbStatAddr.Content = "Addr:" + System.Convert.ToString(fComAdr);
                //LbStatPower.Content = "Pow:";

                //LbReturn.Content =
                //    "COM = " + System.Convert.ToString(port) + "\n"
                //    + "fComAdr = " + System.Convert.ToString(fComAdr) + "\n"
                //    + "fOpenComIndex = " +System.Convert.ToString(fOpenComIndex) + "\n"
                //    + "openResultCode = " + System.Convert.ToString(openResultCode);

                //openResultStr = GetReturnCodeDesc(openResultCode);
                //MessageBox.Show(openResultStr);
            }

        }

        private void BtnClosePort_Click(object sender, RoutedEventArgs e)
        {
            if (ComOpen)
            {
                int closePortResult = 30;
                closePortResult = StaticClassReaderB.CloseSpecComPort(fOpenComIndex);
                ComOpen = false;

                //LbStatInfo.Content = "$" + System.Convert.ToString(GetReturnCodeDesc(closePortResult));
                //LbStatPort.Content = "";
                //LbStatBaud.Content = "";
                //LbStatReaderType.Content = "";
                //LbStatResultCode.Content = "";
                //LbStatAddr.Content = "";
                //LbStatPower.Content = "";
            }
            else
            {
                //LbStatInfo.Content = "端口尚未打开";
            }
        }

        private void Toggle_IsCheckedChanged(object sender, System.EventArgs e)
        {
            //throw new System.NotImplementedException();
            int port = 0;
            fComAdr = System.Convert.ToByte("FF", 16);
            //fBaud = System.Convert.ToByte('5');  //5 - 576000
            fBaud = 5;
            int openResultCode = 30;
            //string openResultStr = "Nothing Happened";
            
            if (!ComOpen)
            {
                
                try
                {
                    openResultCode = StaticClassReaderB.AutoOpenComPort(ref port, ref fComAdr, fBaud, ref fOpenComIndex);
                    //fOpenComIndex = frmcomportindex;                

                    if (openResultCode == 0)
                    {
                        ComOpen = true;
                    }
                    else if ((fCmdRet == 0x35) | (fCmdRet == 0x30)) //0011 1001 | 0011 0000
                    {
                        StaticClassReaderB.CloseSpecComPort(fOpenComIndex);
                        ComOpen = false;
                        Toggle.IsChecked = false;
                        return;
                    }
                }
                finally
                {
                    LbStatus.Content = System.Convert.ToString(GetReturnCodeDesc(openResultCode));
                    LbCOM.Content = System.Convert.ToString(port);
                    LbfBaud.Content = System.Convert.ToString(fBaud);
                    LbRcode.Content =  System.Convert.ToString(openResultCode);
                    LbfComAdr.Content = System.Convert.ToString(fComAdr);
                    LbComOpen.Content = System.Convert.ToString(ComOpen);

                }
            }
            else
            {
                int closePortResult = 30;
                closePortResult = StaticClassReaderB.CloseSpecComPort(fOpenComIndex);
                ComOpen = false;

                LbStatus.Content = "端口关闭" + System.Convert.ToString(GetReturnCodeDesc(openResultCode));
                LbCOM.Content = "";
                LbfBaud.Content = "";
                LbRcode.Content = "";
                LbfComAdr.Content = "";
                LbComOpen.Content = System.Convert.ToString(ComOpen);

            }
        }

        private void BtnIntervalSet_Click(object sender, RoutedEventArgs e)
        {
            //throw new System.NotImplementedException();
            if (!ComOpen) {
                LbStatus.Content = "端口还未打开";
                return;
            }            
            
            switch (comboBox.SelectedIndex)
            {
                case 0: scantime = 0x0a; break;  //10
                case 1: scantime = 0x14; break;
                case 2: scantime = 0x1e; break;
                case 3: scantime = 0x28; break;
                case 4: scantime = 0x32; break;
                default: return;
            }
            fCmdRet = StaticClassReaderB.WriteScanTime(ref fComAdr, ref scantime, fOpenComIndex);
            if (fCmdRet == 0)
                LbStatus.Content = "询查时间设置成功";
            else if (fCmdRet == 0xEE)
                LbStatus.Content = "询查时间返回指令错误?";
            else
            {
                LbStatus.Content = "询查时间设置失败";
            }
        }

        private void Inventory()
        {
            byte[] EPC = new byte[5000];
            int TotalLen = 0, CardNum = 0;
            byte TIDFlag = 0x00, AdrTID = 0x00, LenTID = 0x00;
            

            ListViewItem listViewItem = new ListViewItem();
            bool isOnListView;


            fCmdRet = StaticClassReaderB.Inventory_G2(
                  ref fComAdr,
                  AdrTID,
                  LenTID,
                  TIDFlag,
                  EPC,    //EPClen & EPC = 1Byte + (1+EPC) + (1+EPC) +...
                  ref TotalLen,
                  ref CardNum,
                  fOpenComIndex);

            LbStatus.Content = System.Convert.ToString(GetReturnCodeDesc(fCmdRet));
                        

            if ((fCmdRet == 1) | (fCmdRet == 2) | (fCmdRet == 3) | (fCmdRet == 4) |
                (fCmdRet == 0xFB))
            {
                
                byte[] buffer = new byte[TotalLen];
                for (int i = 0; i < TotalLen; i++) buffer[i] = EPC[i];

                fInventory_EPC_List = ByteArrayToHexString(buffer);
                Tb.Text = fInventory_EPC_List;
                LbCardNum.Content = System.Convert.ToString(CardNum);
                //InventoryFinish = true;

                string subEPC;
                int ptr = 0, subEPCLen;                

                for (int i = 0; i < CardNum; i++)
                {
                    subEPCLen = buffer[ptr];
                    subEPC = fInventory_EPC_List.Substring(ptr * 2 + 2, subEPCLen * 2);
                    ptr += subEPCLen + 1;

                    if (subEPC.Length != subEPCLen * 2) return; //Timer tick again?

                    isOnListView = false;
                    foreach(var item in listview_Control)
                    {
                        if (item.EPCstring.Equals(subEPC))
                        {
                            isOnListView = true;
                            //item.Times++;
                            break;
                        }
                    }
                    if (!isOnListView)
                    {
                        ReadEPCData data = new ReadEPCData(listViewEPC.Items.Count + 1, subEPC);
                        listview_Control.Add(data);
                        
                    }
                    
                }
                                
            }
            else
            {
                LbStatus.Content = "Unknow Error.";
            }
            
        }

        private void Inventory_Timer_Elapsed(object sender, EventArgs e)
        {
            
            DispatcherTimer t = (DispatcherTimer)sender;
            if(timerCount-- <= 0)
            {
                t.Stop();
                MessageBox.Show("STOP");
                return;
            }
            Inventory();
           
        }

        private void Run(object stateInfo)
        {
            //delegate 
            timerCount = TIMES;
            timer.Start();
            ((AutoResetEvent)stateInfo).Set();

            MessageBox.Show("Run() Finish");
            
            
            //while (timerCount != TIMES) ;
        }

        private void ListViewControl_to_DB()
        {
            //string deleteQuery = "DELETE FROM dbo.epc";
            string deleteQuery = "TRUNCATE TABLE dbo.epc";
            string nonQuery = "SELECT * FROM dbo.epc WHERE 1=2";
            DataTable dtEPC = new DataTable();

            using (SqlConnection conn = new SqlConnection(ConnStr))
            {
                SqlCommand command = new SqlCommand();
                command.CommandText = deleteQuery;
                command.Connection = conn;

                conn.Open();

                //DELETE FROM DBO.EPC
                try
                {
                    if (command.ExecuteNonQuery() > 0)
                    {

                    }

                }
                catch (System.Exception ex)
                {

                }
                finally
                {
                    conn.Close();
                }

            }

            //get table structure
            //command.CommandText = nonQuery;
            //SqlDataAdapter adapter = new SqlDataAdapter(command.CommandText, conn);
            //adapter.Fill(dtEPC);

            dtEPC = ToDataTable<ReadEPCData>(listview_Control);

            //upload
            SqlBulkCopy copy = new SqlBulkCopy(ConnStr, SqlBulkCopyOptions.UseInternalTransaction);
            copy.DestinationTableName = "dbo.epc";
            copy.WriteToServer(dtEPC);

        }
        
        private void BtnInventory_Click(object sender, RoutedEventArgs e)
        {
            //throw new System.NotImplementedException();
            if (!ComOpen)
            {
                LbStatus.Content = "端口还未打开";
                return;
            }

            //New Round
            listview_Control.Clear();

            //trigger ? times
            Inventory();
            

            //toDB
            ListViewControl_to_DB();

        }

        private void BtnDataBaseRead_Click(object sender, RoutedEventArgs e)
        {

            
            using(SqlConnection conn = new SqlConnection(ConnStr))
            {
                SqlCommand sqlCommand = new SqlCommand("SELECT * FROM dbo.product ORDER BY productID");
                SqlDataAdapter adapter = new SqlDataAdapter(sqlCommand.CommandText, conn);

                DTlistView.Clear();
                adapter.Fill(DTlistView);

                
            }
        }

        private void BtnUpLoadImage_Click(object sender, RoutedEventArgs e)
        {
            Stream ms;
            byte[] imageByte;

            OpenFileDialog openFileDialog = new OpenFileDialog {

                Filter = "图像文件|*.jpg;*.png;*.jpeg;*.bmp;*.gif|所有文件|*.*"

            };
            openFileDialog.InitialDirectory = @"C:\Users\Administrator\Desktop";

            if ((bool)openFileDialog.ShowDialog())
            {
                if((ms = openFileDialog.OpenFile()) != null)
                {
                    ImgDefault.Source = new BitmapImage(new System.Uri(openFileDialog.FileName));

                    imageByte = new byte[ms.Length];
                    ms.Position = 0;

                    //stream into byte[]
                    ms.Read(imageByte, 0, System.Convert.ToInt32(ms.Length));
                    theProduct.Image = imageByte;
                }
                else
                {
                    MessageBox.Show("Image UpLoad Failed.");
                }
            }
            else
            {
                return;
            }
        }

        private void BtnDataBaseInsert_Click(object sender, RoutedEventArgs e)
        {
            using(SqlConnection conn = new SqlConnection(ConnStr))
            {

                SqlCommand commandInsert = new SqlCommand();
                commandInsert.CommandText = "INSERT INTO dbo.product(productID,name,type,unitPrice,store,image) VALUES(@PRODUCTID,@NAME,@TYPE,@UNITPRICE,@STORE,@IMAGE)";
                commandInsert.Connection = conn;


                theProduct.ProductID = TbProductID.Text.Trim();
                theProduct.Name = TbName.Text.Trim();
                theProduct.Type = TbType.Text.Trim();
                theProduct.UnitPrice = System.Convert.ToDouble(TbUnitPrice.Text.Trim());
                theProduct.Store = System.Convert.ToInt32(TbStore.Text.Trim());

                commandInsert.Parameters.AddWithValue("@PRODUCTID", theProduct.ProductID);
                commandInsert.Parameters.AddWithValue("@NAME", theProduct.Name);
                commandInsert.Parameters.AddWithValue("@TYPE", theProduct.Type);
                commandInsert.Parameters.AddWithValue("@UNITPRICE", theProduct.UnitPrice);
                commandInsert.Parameters.AddWithValue("@STORE", theProduct.Store);
                commandInsert.Parameters.AddWithValue("@IMAGE", theProduct.Image);

                if(theProduct.Image == null)
                {
                    ImgDefault.Focus();
                    MessageBox.Show("Image!!", "WARNNING");
                    return;
                }

                conn.Open();
                                
                try
                {
                    if (commandInsert.ExecuteNonQuery() > 0)
                    {
                        SqlCommand commandQuery = new SqlCommand("SELECT * FROM dbo.product ORDER BY productID");
                        SqlDataAdapter adapter = new SqlDataAdapter(commandQuery.CommandText, conn);

                        DTlistView.Clear();
                        adapter.Fill(DTlistView);
                        

                        UIElementCollection children = theGrid.Children;
                        foreach (UIElement ui in children)
                        {
                            if (ui is TextBox)
                            {
                                (ui as TextBox).Text = string.Empty;
                            }
                        }

                        ImgDefault.Source = null;
                        listViewProduct.SelectedIndex = -1;
                    }

                }
                catch (System.FormatException ex)
                {

                    MessageBox.Show(ex.Message);
                }                
                finally
                {
                    conn.Close();
                }


            }
        }

        private void BtnBindingTest_Click(object sender, RoutedEventArgs e)
        {
            theProduct.ProductID = "fwef";

            
        }

        private void ListViewProduct_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataRowView drView = (DataRowView)listViewProduct.SelectedItem;
            //MessageBox.Show(theProduct.ProductID);
            if (drView != null)
            {
                DataRow datarow = drView.Row;

                theProduct.ProductID = (string)datarow["productID"];
                theProduct.Name = (string)datarow["name"];
                theProduct.Type = (string)datarow["type"];
                theProduct.UnitPrice = System.Convert.ToDouble(datarow["unitPrice"]);
                theProduct.Store = System.Convert.ToInt32(datarow["store"]);
                theProduct.Image = (byte[])datarow["image"];

            }
            else
            {
                return;
            }

        }

        private void BtnClearListViewEPC_Click(object sender, RoutedEventArgs e)
        {
            //dummy
        }

        private void BtnCheckOut_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!ComOpen)
            {
                MessageBox.Show("端口还未打开", "WARNNING");
                return;
            }
            //New Round
            listview_Control.Clear();
            DTlistBox.Clear();

            //trigger ? times
            for(int i = 1; i <= 20; i++)
            {
                Inventory();
            }
            
            //Run(5);
            //ThreadPool.QueueUserWorkItem(new WaitCallback(Run), runEvent);
            //runEvent.WaitOne();

            ListViewControl_to_DB();

            using (SqlConnection conn = new SqlConnection(ConnStr))
            {


                string joinQuery = "SELECT product.productID, product.name, product.unitPrice, product.image" + " " +
                                    "INTO joinQuery" + " " +
                                    "FROM product, epc, mapping" + " " +
                                    "WHERE (epc.epc = mapping.epc) AND (product.productID = mapping.productID)";


                string distinctQuery = "SELECT productID, COUNT(*) AS count" + " " +
                                        "INTO distinctQuery" + " " +
                                        "FROM joinQuery" + " " +
                                        "GROUP BY productID";

                string query = "SELECT product.productID, product.name, product.unitPrice, product.image, distinctQuery.count" + " " +
                               //"INTO query" + " " +
                               "FROM product, distinctQuery" + " " +
                               "WHERE (product.productID=distinctQuery.productID)";
                               //+ "ORDER BY product.productID";
                
                string calculateQuery = "SELECT SUM(unitPrice * count) AS total, COUNT(DISTINCT productID) AS typeNum, SUM(count) AS totalNum FROM (" + query +") AS R";

                string dropQuery = "DROP TABLE joinQuery, distinctQuery";

                DataTable dt = new DataTable();

                SqlCommand command = new SqlCommand();
                command.Connection = conn;

                SqlDataAdapter adapter = new SqlDataAdapter(command);

                //joinQuery
                adapter.SelectCommand.CommandText = joinQuery;
                adapter.Fill(dt);


                //distinctQuery
                adapter.SelectCommand.CommandText = distinctQuery;
                adapter.Fill(dt);

                //query -> DTlistBox
                //if SELECT ... INTO， Fill nothing， return 
                adapter.SelectCommand.CommandText = query;
                adapter.Fill(DTlistBox);

                //calculate
                adapter.SelectCommand.CommandText = calculateQuery;
                adapter.Fill(dt);

                DataRow dr = dt.Rows[0];

                Tbk_TOTAL.Text = dr["total"].ToString();
                Tbk_product_typeNum.Text = dr["typeNum"].ToString();
                Tbk_product_totalNum.Text = dr["totalNum"].ToString();


                //dropQuery
                adapter.SelectCommand.CommandText = dropQuery;
                adapter.Fill(dt);





            }
        }

        private void Btn_Pay_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Set the message box title and content
            txtBlock_MsgBox_Title.Text = "支付";
            Grid_MsgBox.Tag = "支付";
            txtBlock_MsgBox_Content.Text = "";

            //Hidden YES NO Button
            //Btn_MsgBox_No.Visibility = Visibility.Hidden;
            //Btn_MsgBox_Yes.Visibility = Visibility.Hidden;

            //<Image>
            txtBlock_MsgBox_Content.Visibility = Visibility.Collapsed;
            ImgURL.Visibility = Visibility.Visible;


            // Set the message box visible
            Grid_MsgBox.Visibility = Visibility.Visible;
        }
        
        private void Btn_Cancel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Set the message box title and content
            txtBlock_MsgBox_Title.Text = "取消";
            Grid_MsgBox.Tag = "取消";
            txtBlock_MsgBox_Content.Text = "确定要取消此订单么?";
            txtBlock_MsgBox_Content.Visibility = Visibility.Visible;
            ImgURL.Visibility = Visibility.Collapsed;

            // Set the message box visible
            Grid_MsgBox.Visibility = Visibility.Visible;
        }

        private void Btn_MsgBox_No_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            
            Grid_MsgBox.Visibility = Visibility.Collapsed;  //it is the same
        }

        private void Btn_MsgBox_Yes_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Check what the message box is for
            switch (Grid_MsgBox.Tag.ToString())
            {
                case "取消":

                    //New Round
                    listview_Control.Clear();
                    DTlistBox.Clear();

                    Tbk_TOTAL.Text = String.Empty;
                    Tbk_product_typeNum.Text = String.Empty;
                    Tbk_product_totalNum.Text = String.Empty;

                    break;

                case "支付":

                    //New Round
                    listview_Control.Clear();
                    DTlistBox.Clear();

                    //Clear TOTAL
                    Tbk_TOTAL.Text = String.Empty;
                    Tbk_product_typeNum.Text = String.Empty;
                    Tbk_product_totalNum.Text = String.Empty;

                    //1. weixin business
                    //.....
                    //start require Server

                    //2. Click to continue(test)
                    this.ShowMessageAsync("支付成功", "请带好您所购买的商品，从右侧方向闸机离开");


                    break;

            }

            // Set the message box invisible
            Grid_MsgBox.Visibility = Visibility.Collapsed;
        }

        private void Btn_Help_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.TbItm_CheckOut.IsSelected = true;
            
        }


        #endregion


    }
}
