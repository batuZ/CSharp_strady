using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace 程序配置文件_AppConfig
{
    class Program
    {
        //更多用法
        //http://blog.sina.com.cn/s/blog_4c2e288b010008c1.html

        static void Main(string[] args)
        {
            //手写配置文件
            /*
                <appSettings>
                    <add key="hehe1" value="ww,ee,vv"/>
                    <add key="hehe2" value="fff,sss,aaa"/>
                </appSettings>
            */

            //使用ConfigurationManager修改配置文件,无法作用到 File 上，只能作用在变量上
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            config.AppSettings.Settings["hehe2"].Value = "v,d,z";
            config.Save(ConfigurationSaveMode.Full);
            ConfigurationManager.RefreshSection("appSettings");//重新加载新的配置文件
            string filPat = config.FilePath;





            //读取配置文件,读的是   appname.exe.config
            string ttt = ConfigurationManager.AppSettings["hehe2"];

        }
    }
}
