using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Net.WebRequestMethods;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Windows.Interop;
using System.Threading;
using HandyControl;
using System.IO;
using System.Collections;
using HandyControl.Controls;
using MessageBox = HandyControl.Controls.MessageBox;
namespace SchoolNetConnect
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        
        public MainWindow()//主窗口
        {
            InitializeComponent();
            StartMain();//启动主程序
            
            
        }

        private void IniSaveButton_Click(object sender, RoutedEventArgs e)//保存按钮被调用
        {
            SaveConfigINI();
        }
        private async void ConnectButton_Click(object sender, RoutedEventArgs e)//连接按钮被调用
        {
            await ConnectNet();

        }
        private void AutoRunMode(object sender, RoutedEventArgs e)//自动模式按钮被调用
        {
            var Config = new IniFile("./config.ini");
            string AutoRun = Config.Read("AutoRun", "UserConfig");
            ChangeAutoRun(Config, AutoRun); 
        }
        private async void SmsSandButtonClick(object sender, RoutedEventArgs e)//密码找回按钮被调用
        {
            string Phone = PhoneInput.Text;
            await SendSms(Phone);
        }



        /// <summary>
        /// 主界面逻辑
        /// </summary>
        private async void StartMain()
        {
            //判断是否连接成功
            var Config = new IniFile("./config.ini"); 
            if (!Config.KeyExists("username", "UserConfig"))//若没有初始配置文件，则初始化
            {
                Config.IniFileinit("Write");
                EchoLogs("成功初始化配置文件");
            }
            List<string> IniFileList = Config.GetIniFile("./");
            foreach (string IniFileName in IniFileList)
            {
                INISelect.Items.Add(IniFileName);
                INIReadSelect.Items.Add(IniFileName); 
            }
            INISelect.SelectedIndex = INIReadSelect.Items.IndexOf("config.ini");
            INIReadSelect.SelectedIndex = INIReadSelect.Items.IndexOf("config.ini");
            Config.ConfigRead(Config,//以下是输出参数
                out string OnConnect,//连接状态
                out string AutoRun,//是否启用自动模式
                out string Driver,//设备信息
                out string UserName,//用户名
                out string Password,//密码
                out string CustomIp,//自定义IP
                out string Resettime,//重连时间
                out string Resetcount,//重连次数
                out string ISP,//运营商
                out string SetIP,//是否使用自定义IP
                out string SetReset,//是否启用重连
                out string NowIp//当前IP
                );
            EchoLogs("成功读取配置文件！");
            IniTranslate(//对获取的信息进行基本处理
                OnConnect,
                Driver,
                UserName,
                Password,
                CustomIp,
                Resettime,
                Resetcount,
                ISP,
                SetIP,
                SetReset,
                NowIp);//转换参数
            if (AutoRun == "true")//判断是否为自动连接
            {

                EchoLogs("启动自动连接......");
                await ConnectNet();
            }
        }

        private void IniTranslate(//用来解析显示INI文件的函数
            string OnConnect,
            string Driver,
            string UserName,
            string Password,
            string CustomIp,
            string Resettime,
            string Resetcount,
            string ISP,
            string SetIP,
            string SetReset,
            string NowIp)
        {
            SetipTranslate(CustomIp, SetIP, NowIp);//检查ip
            ResetTranslate(Resettime, Resetcount, SetReset);//检查自动重置
            ChackConnecting(OnConnect);//检查是否已连接
            try
            {
                UsernameInput.Text = UserName;
                PasswordInput.Text = Password;
            }
            catch (Exception)
            {

                throw;
            }
            DriverTranslate(Driver);
            IspTranslate(ISP);
        }

        private void SetipTranslate(string CustomIp, string SetIP, string NowIp)//对IP进行处理
        {
            if (SetIP == "true")
            {
                UseCustomIp.IsChecked = true;
                CustomIpInput.Text = CustomIp;
            }
            else
            {
                UseCustomIp.IsChecked = false;
                CustomIpInput.Text = NowIp;
            }
        }

        private void ResetTranslate(string Resettime, string Resetcount, string SetReset)//对重连进行处理
        {
            if (SetReset == "true")
            {
                UseReset.IsChecked = true;
                ResetTimeInput.Text = Resettime;
                ResetCountInput.Text = Resetcount;
            }
            else
            {
                UseReset.IsChecked = false;
                ResetTimeInput.Text = "0";
                ResetCountInput.Text = "1";
            }
        }

        private void ChackConnecting(string OnConnect)//检查是否已连接
        {
            if (OnConnect == "true")
            {
                ConnectButton.Content = "断开连接";
                ConnectButton.SetResourceReference(StyleProperty, "ButtonDanger");
            }
        }

        private void IspTranslate(string ISP)//ISP选项翻译
        {
            if (ISP == "CMCC")//中国移动
            {
                ISPSelect.SelectedIndex = 0;
            }
            else if (ISP == "CUCC")//中国联通
            {
                ISPSelect.SelectedIndex = 1;
            }
            else if (ISP == "CTCM")//中国电信
            {
                ISPSelect.SelectedIndex = 2;
            }
            else if (ISP == "School")//内网模式
            {
                ISPSelect.SelectedIndex = 3;

            }
        }

        private void DriverTranslate(string Driver)//设备选项翻译
        {
            if (Driver == "PC")
            {
                DriverPC.IsChecked = true;
                DriverMobile.IsChecked = false;
            }
            else if (Driver == "Mobile")
            {
                DriverMobile.IsChecked = true;
                DriverPC.IsChecked = false;
            }
        }


        /*保存配置文件*/


        
        /// <summary>
        /// 自动模式
        /// </summary>
        /// <param name="Config">Config</param>
        /// <param name="AutoRun">表述是否启用的真假值</param>
        private void ChangeAutoRun(IniFile Config, string AutoRun)
        {
            if (AutoRun == "true")
            {
                Config.Write("AutoRun", "false", "UserConfig");
                EchoLogs("已关闭自动模式");
            }
            else if (AutoRun == "false")
            {
                Config.Write("AutoRun", "true", "UserConfig");
                EchoLogs("已开启自动模式");
            }
        }
        /// <summary>
        /// 保存至INI文件
        /// </summary>
        private void SaveConfigINI()//保存至INI文件
        {
            try
            {
                var UserName = UsernameInput.Text;
                var Password = PasswordInput.Text;
                var CustomIp = CustomIpInput.Text;
                var ResetTime = ResetTimeInput.Text;
                var ResetCount = ResetCountInput.Text;
                var ISPNum = ISPSelect.SelectedIndex;
                var ISP = "";
                var Drivers = "";
                var ConfigFileName = INISelect.SelectedItem.ToString();
                var Config = new IniFile("./" + ConfigFileName);
                if (ISPNum == 0)
                {
                    ISP = "CMCC";
                }
                else if (ISPNum == 1)
                {
                    ISP = "CUCC";
                }
                else if (ISPNum == 2)
                {
                    ISP = "CTCM";
                }
                else if (ISPNum == 3)
                {
                    ISP = "School";
                }
                if (DriverPC.IsChecked == true)
                {
                    Drivers = "PC";
                }
                else if (DriverMobile.IsChecked == true)
                {
                    Drivers = "Mobile";
                }
                Config.Write("username", UserName, "UserConfig");
                Config.Write("password", Password, "UserConfig");
                Config.Write("ISP", ISP, "UserConfig");
                Config.Write("CustomIp", CustomIp, "UserConfig");
                Config.Write("Drivers", Drivers, "UserConfig");
                Config.Write("Resettime", ResetTime, "UserConfig");
                Config.Write("Resetcount", ResetCount, "UserConfig");
                if (UseCustomIp.IsChecked == true)
                {
                    Config.Write("SetIP", "true", "UserConfig");
                }
                else
                {
                    Config.Write("SetIP", "false", "UserConfig");
                }
                if (UseReset.IsChecked == true)
                {
                    Config.Write("SetReset", "true", "UserConfig");
                }
                else
                {
                    Config.Write("SetReset", "false", "UserConfig");
                }
                EchoLogs("已写入配置");
            }
            catch (Exception)
            {

                throw;
            }

            
        }




        /// <summary>
        /// 网络连接逻辑
        /// </summary>
        /// <returns></returns>
        private async Task SendSms(string Phone)
        {
            JObject ApiReturnData = await WebData.SandSmsMassage(Phone);
            try
            {
                ChackSmsSandabled(Phone, ApiReturnData);
            }
            catch (NullReferenceException NullError)
            {
                EchoLogs("程序出现错误，报错信息如下：");
                EchoLogs(NullError.ToString());
            }
        }



        private async Task ConnectNet()
        {
            var UserName = UsernameInput.Text;
            var Password = PasswordInput.Text;
            var IP = "";
            var NowIP = GetSystemData.GetIP();
            var CustomIp = CustomIpInput.Text;
            int ISPNum = ISPSelect.SelectedIndex;
            var ISP = "";
            var Drivers = "";
            var ResetTime = ResetTimeInput.Text;
            var ResetCount = ResetCountInput.Text;
            var Maxcount = 1;
            var Mincount = 1;

            string ConfigReadFileName = INIReadSelect.SelectedItem.ToString();

            var Config = new IniFile("./"+ ConfigReadFileName);

            EchoLogs(ConfigReadFileName);
            string OnConnect = Config.Read("Onconnect", "RunningDate");//判断是否连接成功
            IP = ChackIP(NowIP, CustomIp);//判断该使用什么IP
            ISP = ChackISP(ISPNum, ISP);//判断ISP
            Drivers = ChackDriver(Drivers);//判断设备名
            if (OnConnect == "false" && Maxcount >= Mincount)//联网
            {
                EchoLogs("正在连接");
                JObject ApiReturnData = await WebData.GetDataLogin(UserName, ISP, Password, IP, Drivers);
                try
                {
                    string result = ApiReturnData["result"]!.ToString();
                    if (result == "1")
                    {
                        string msg = ApiReturnData["msg"]!.ToString();
                        EchoLogs(msg);
                        ConnectingChangeTrue(Config);

                    }
                    else if (result == "0")
                    {
                        string RetCode = ApiReturnData["ret_code"]!.ToString();
                        if (RetCode == "2")
                        {
                            EchoLogs("未进行重复连接，因为终端ip已在线");

                            if (UseReset.IsChecked == true)
                            {
                                Maxcount = Convert.ToInt32(ResetCount);
                                Mincount++;
                                EchoLogs("准备尝试第" + Mincount.ToString() + "次");
                                EchoLogs("未进行重复连接，因为终端ip已在线");
                                await WebData.GetDataLogout(IP);
                                Thread.Sleep(Convert.ToInt32(ResetTime) * 1000);
                                await ConnectNet();
                            }
                            else
                            {
                                ConnectingChangeTrue(Config);
                            }
                        }
                        else if (RetCode == "1")
                        {
                            EchoLogs("未连接，由于不确定的因素");
                            if (UseReset.IsChecked == true)
                            {
                                Maxcount = Convert.ToInt32(ResetCount);
                                Mincount++;
                                EchoLogs("准备尝试第" + Mincount.ToString() + "次");
                                Thread.Sleep(Convert.ToInt32(ResetTime) * 1000);
                                await ConnectNet();
                            }
                        }
                    }
                }
                catch (NullReferenceException NullError)
                {
                    EchoLogs("无法连接到网络，请检查是否已接入网络环境，报错信息如下：");
                    EchoLogs(NullError.ToString());
                }
            }
            else//断网
            {
                EchoLogs("正在断开连接……");
                JObject ApiReturnData = await WebData.GetDataLogout(IP);
                try
                {
                    string result = ApiReturnData["result"]!.ToString();
                    if (result == "0" || result == "1")
                    {
                        string Msg = ApiReturnData["msg"]!.ToString();
                        if (result == "0")
                        {
                            EchoLogs(Msg + ",用户未登录");
                        }
                        else
                        {
                            EchoLogs(Msg);
                        }
                        ConnectingChangeFalse(Config);
                    }
                    else
                    {
                        EchoLogs(ApiReturnData.ToString());

                    }
                }
                catch(NullReferenceException NullError)
                {
                    EchoLogs("无法连接到网络，请检查是否已接入网络环境，报错信息如下：");
                    EchoLogs(NullError.ToString());
                }
            }
        }

        private string ChackDriver(string Drivers)//检查设备
        {
            if (DriverPC.IsChecked == true)
            {
                Drivers = "0";
            }
            else if (DriverMobile.IsChecked == true)
            {
                Drivers = "1";
            }

            return Drivers;
        }

        private static string ChackISP(int ISPNum, string ISP)//检查ISP
        {
            switch (ISPNum)
            {
                case 0:
                    ISP = "cmcc";
                    break;
                case 1:
                    ISP = "unicom";
                    break;
                case 2:
                    ISP = "telecom";
                    break;
                case 3:
                    ISP = "";
                    break;
            }

            return ISP;
        }

        private string ChackIP(string NowIP, string CustomIp)//检查IP使用
        {
            string IP;
            if (UseCustomIp.IsChecked == true)
            {
                IP = CustomIp;
            }

            else
            {
                IP = NowIP;
            }

            return IP;
        }
        private void ChackSmsSandabled(string Phone, JObject ApiReturnData)//检查手机发送密码是否生效
        {
            string result = ApiReturnData["result"]!.ToString();
            string msg = ApiReturnData["msg"]!.ToString();
            if (result == "fail")
            {
                MessageBox.Error("无法将你的密码通过短信发送至" + Phone + "请核对信息或检查网络后重试", "操作失败");
                EchoLogs(msg);
            }
            else if (result == "ok")
            {
                MessageBox.Success("已将你的密码通过短信发送至" + Phone + "请注意查收！", "操作成功");
            }
        }

        /*网络连接状态切换*/


        private void ConnectingChangeFalse(IniFile Config)//切换至未连接状态
        {
            ConnectButton.SetResourceReference(StyleProperty, "ButtonSuccess");
            ConnectButton.Content = "一键联网";  
            Config.Write("OnConnect", "false", "RunningDate");
            
        }

        private void ConnectingChangeTrue(IniFile Config)//切换至已连接状态
        {
            
            ConnectButton.Content = "断开连接";
            ConnectButton.SetResourceReference(StyleProperty, "ButtonDanger");
            Config.Write("OnConnect", "true", "RunningDate");
        }
        /// <summary>
        /// 向输出框输出日志
        /// </summary>
        /// <param name="Logs">输出的内容</param>
        private void EchoLogs(string Logs)//日志输出
        {
            LogsBox.Text += "\n$"+Logs;
            LogsBox.ScrollToEnd();
        }

        private void INIReadSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var ConfigReadFileName = INIReadSelect.SelectedItem.ToString();
            INISelect.SelectedIndex = INISelect.Items.IndexOf(ConfigReadFileName);
            var Config = new IniFile("./" + ConfigReadFileName);
            Config.ConfigRead(Config,//以下是输出参数
                                out var OnConnect,//连接状态
                                out var AutoRun,//是否启用自动模式
                                out var Driver,//设备信息
                                out var UserName,//用户名
                                out var Password,//密码
                                out var CustomIp,//自定义IP
                                out var Resettime,//重连时间
                                out var Resetcount,//重连次数
                                out var ISP,//运营商
                                out var SetIP,//是否使用自定义IP
                                out var SetReset,//是否启用重连
                                out var NowIp//当前IP
                                );
            EchoLogs("成功读取配置文件！");
            IniTranslate(//对获取的信息进行基本处理
                    OnConnect,
                    Driver,
                    UserName,
                    Password,
                    CustomIp,
                    Resettime,
                    Resetcount,
                    ISP,
                    SetIP,
                    SetReset,
                    NowIp);//转换参数
        }

        private void INISelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var ConfigFileName = INISelect.SelectedItem.ToString();
            INIReadSelect.SelectedIndex = INIReadSelect.Items.IndexOf(ConfigFileName);
            var Config = new IniFile("./" + ConfigFileName);
            Config.ConfigRead(Config,//以下是输出参数
                                out string OnConnect,//连接状态
                                out string AutoRun,//是否启用自动模式
                                out string Driver,//设备信息
                                out string UserName,//用户名
                                out string Password,//密码
                                out string CustomIp,//自定义IP
                                out string Resettime,//重连时间
                                out string Resetcount,//重连次数
                                out string ISP,//运营商
                                out string SetIP,//是否使用自定义IP
                                out string SetReset,//是否启用重连
                                out string NowIp//当前IP
                                );
            EchoLogs("成功读取配置文件！");
            IniTranslate(//对获取的信息进行基本处理
                    OnConnect,
                    Driver,
                    UserName,
                    Password,
                    CustomIp,
                    Resettime,
                    Resetcount,
                    ISP,
                    SetIP,
                    SetReset,
                    NowIp);//转换参数
        
        }


    }
    
}
