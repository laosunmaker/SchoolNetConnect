using System.Text;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System;
using System.Collections.Generic;
using System.Windows.Documents;

namespace SchoolNetConnect
{
    public class IniFile
    {
        string Path;
        string EXE = Assembly.GetExecutingAssembly().GetName().Name!;
        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        static extern long WritePrivateProfileString(string Section, string Key, string Value, string FilePath);
        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        static extern int GetPrivateProfileString(string Section, string Key, string Default, StringBuilder RetVal, int Size, string FilePath);
        public IniFile(string IniPath = null!)
        {
            Path = new FileInfo(IniPath ?? EXE + ".ini").FullName;
        }
        public string Read(string Key, string Section = null!)
        {
            var RetVal = new StringBuilder(255);
            GetPrivateProfileString(Section ?? EXE, Key, "", RetVal, 255, Path);
            return RetVal.ToString();
        }
        public void Write(string Key, string Value, string Section = null!)
        {
            WritePrivateProfileString(Section ?? EXE, Key, Value, Path);
        }
        public void DeleteKey(string Key, string Section = null!)
        {
            Write(Key, null!, Section ?? EXE);
        }
        public void DeleteSection(string Section = null!)
        {
            Write(null!, null!, Section ?? EXE);
        }
        public bool KeyExists(string Key, string Section = null!)
        {
            return Read(Key, Section).Length > 0;
        }
        /// <summary>
        /// 找到当前路径下文件扩展名相同的文件
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns></returns>
        public List<string> GetIniFile(string path)
        {
            // 获取文件夹的文件
            DirectoryInfo fdir = new DirectoryInfo(path);
            FileInfo[] file = fdir.GetFiles();
            List<string> Inilist = new List<string>(500);
            if (file.Length > 0)
            {
                foreach (var f in file)
                {
                    string ext = ".ini";
                    if (ext.ToLower().IndexOf(f.Extension.ToLower()) >= 0)
                    {
                        string fileName = System.IO.Path.GetFileName(f.FullName);
                        Inilist.Add(fileName);
                    }
                }
            }

            return Inilist;
        }
        public void IniFileinit(String OP)
        {
            if(OP=="Write")
            {
                Write("username", "", "UserConfig");
                Write("password", "", "UserConfig");
                Write("ISP", "", "UserConfig");
                Write("UseCustomIp", "False", "UserConfig");
                Write("CustomIp", "", "UserConfig");
                Write("OnConnect", "false", "RunningDate");
                Write("SetIP", "false", "UserConfig");
                Write("SetReset", "false", "UserConfig");
                Write("Resettime", "1", "UserConfig");
                Write("Resetcount", "1", "UserConfig");
                Write("Driver", "PC", "UserConfig");
                Write("AutoRun", "false", "UserConfig");
            }
           
            
        }

        public void ConfigRead(IniFile Config,
            out string OnConnect,
            out string AutoRun, 
            out string Driver, 
            out string UserName, 
            out string Password, 
            out string CustomIp, 
            out string Resettime, 
            out string Resetcount, 
            out string ISP, 
            out string SetIP, 
            out string SetReset, 
            out string NowIp
            )
        {
            AutoRun = Config.Read("AutoRun", "UserConfig");
            Driver = Config.Read("Drivers", "UserConfig");
            UserName = Config.Read("username", "UserConfig");
            Password = Config.Read("password", "UserConfig");
            CustomIp = Config.Read("CustomIp", "UserConfig");
            Resettime = Config.Read("Resettime", "UserConfig");
            Resetcount = Config.Read("Resetcount", "UserConfig");
            ISP = Config.Read("ISP", "UserConfig");
            SetIP = Config.Read("SetIP", "UserConfig");
            SetReset = Config.Read("SetReset", "UserConfig");
            NowIp = GetSystemData.GetIP();
            OnConnect = Config.Read("Onconnect", "RunningDate");
        }
    }
}
