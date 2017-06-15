﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using System.Net;
using System.Management;  //在项目-》添加引用....里面引用System.Management  
using System.Runtime.InteropServices;


namespace FileSplit
{
    //此类用于获取当前主机的相关信息  
    public class MachineInfo
    {
        //用法示例  
        private string example()
        {
            string Info = "";
            MachineInfo info = MachineInfo.I();       //获取主机的对象信息  


            Info = "\r\n主机名:" + Dns.GetHostName(); //获取主机名称  
            Info += "\r\n系统类型:" + info.GetSystemType();
            Info += "\r\n物理内存:" + info.GetPhysicalMemory();


            Info += "\r\n\r\n本地IP:";
            string[] LocalIp = info.GetLocalIpAddress();
            foreach (string ip in LocalIp) Info += "\r\n" + ip;


            Info += "\r\n\r\n外网IP:";
            string[] ExternalIp = info.GetExtenalIpAddress();
            foreach (string ip in ExternalIp) Info += "\r\n" + ip;


            Info += "\r\n\r\nMAC:";
            Info += "\r\n" + info.GetNetCardMACAddress();
            //Info += "\r\n" + info.GetMacAddress(LocalIp[0]);   


            Info += "\r\n";
            Info += "\r\nCPU序列号:" + info.GetCPUSerialNumber();
            //Info += "\r\nCPU编号:" + info.GetCPUID();  
            //Info += "\r\nCPU版本信息:" + info.GetCPUVersion();  
            //Info += "\r\nCPU名称信息:" + info.GetCPUName();  
            //Info += "\r\nCPU制造厂商:" + info.GetCPUManufacturer();  
            //Info += "\r\n主板制造厂商:" + info.GetBoardManufacturer();  


            //Info += "\r\n主板序列号:" + info.GetBIOSSerialNumber();  
            //Info += "\r\n硬盘序列号:" + info.GetHardDiskSerialNumber();  
            //Info += "\r\n显卡PNPDeviceID:\r\n" + info.GetVideoPNPID();  
            //Info += "\r\n声卡PNPDeviceID:\r\n" + info.GetSoundPNPID();  


            return Info;
        }




        static MachineInfo Instance;


        /// <summary>  
                /// 获取当前类对象的一个实例  
                /// </summary>  
        public static MachineInfo I()
        {
            if (Instance == null) Instance = new MachineInfo();
            return Instance;
        }

        /// <summary>  
                /// 获取本地ip地址，多个ip  
                /// </summary>  
        public String[] GetLocalIpAddress()
        {
            string hostName = Dns.GetHostName();                    //获取主机名称  
            IPAddress[] addresses = Dns.GetHostAddresses(hostName); //解析主机IP地址  


            string[] IP = new string[addresses.Length];             //转换为字符串形式  
            for (int i = 0; i < addresses.Length; i++) IP[i] = addresses[i].ToString();


            return IP;
        }




        //从网站"http://1111.ip138.com/ic.asp"，获取本机外网ip地址信息串  
        //"<html>\r\n<head>\r\n<meta http-equiv=\"content-type\" content=\"text/html; charset=gb2312\">\r\n<title>   
        //您的IP地址 </title>\r\n</head>\r\n<body style=\"margin:0px\"><center>您的IP是：[218.104.71.178] 来自：安徽省合肥市 联通</center></body></html>"  


        /// <summary>  
                /// 获取外网ip地址  
                /// </summary>  
        public string[] GetExtenalIpAddress()
        {
            string[] IP = new string[] { "未获取到外网ip", "" };


            string address = "http://1111.ip138.com/ic.asp";
            string str = GetWebStr(address);


            try
            {
                //提取外网ip数据 [218.104.71.178]  
                int i1 = str.IndexOf("[") + 1, i2 = str.IndexOf("]");
                IP[0] = str.Substring(i1, i2 - i1);


                //提取网址说明信息 "来自：安徽省合肥市 联通"  
                i1 = i2 + 2; i2 = str.IndexOf("<", i1);
                IP[1] = str.Substring(i1, i2 - i1);
            }
            catch (Exception) { }


            return IP;
        }


        /// <summary>  
                /// 获取网址address的返回的文本串数据  
                /// </summary>  
        public string GetWebStr(string address)
        {
            string str = "";
            try
            {
                //从网址中获取本机ip数据  
                System.Net.WebClient client = new System.Net.WebClient();
                client.Encoding = System.Text.Encoding.Default;
                str = client.DownloadString(address);
                client.Dispose();
            }
            catch (Exception) { }


            return str;
        }


        /// <summary>  
                /// 获取本机的MAC;  //在项目-》添加引用....里面引用System.Management  
                /// </summary>  
        public string GetLocalMac()
        {
            string mac = null;
            ManagementObjectSearcher query = new ManagementObjectSearcher("SELECT * FROM Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection queryCollection = query.Get();
            foreach (ManagementObject mo in queryCollection)
            {
                if (mo["IPEnabled"].ToString() == "True")
                    mac = mo["MacAddress"].ToString();
            }
            return (mac);
        }


        //只能获取同网段的远程主机MAC地址. 因为在标准网络协议下，ARP包是不能跨网段传输的，故想通过ARP协议是无法查询跨网段设备MAC地址的。  
        [DllImport("Iphlpapi.dll")]
        private static extern int SendARP(Int32 dest, Int32 host, ref Int64 mac, ref Int32 length);
        [DllImport("Ws2_32.dll")]
        private static extern Int32 inet_addr(string ip);
        /// <summary>  
                /// 获取ip对应的MAC地址  
                /// </summary>  
        public string GetMacAddress(string ip)
        {
            Int32 ldest = inet_addr(ip);            //目的ip   
            Int32 lhost = inet_addr("127.0.0.1");   //本地ip   


            try
            {
                Int64 macinfo = new Int64();
                Int32 len = 6;
                int res = SendARP(ldest, 0, ref macinfo, ref len);  //使用系统API接口发送ARP请求，解析ip对应的Mac地址  
                return Convert.ToString(macinfo, 16);
            }
            catch (Exception err)
            {
                Console.WriteLine("Error:{0}", err.Message);
            }
            return "获取Mac地址失败";
        }


        /// <summary>  
                /// 获取主板序列号  
                /// </summary>  
                /// <returns></returns>  
        public string GetBIOSSerialNumber()
        {
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("Select * From Win32_BIOS");
                string sBIOSSerialNumber = "";
                foreach (ManagementObject mo in searcher.Get())
                {
                    sBIOSSerialNumber = mo["SerialNumber"].ToString().Trim();
                }
                return sBIOSSerialNumber;
            }
            catch
            {
                return "";
            }
        }


        /// <summary>  
                /// 获取CPU序列号  
                /// </summary>  
                /// <returns></returns>  
        public string GetCPUSerialNumber()
        {
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("Select * From Win32_Processor");
                string sCPUSerialNumber = "";
                foreach (ManagementObject mo in searcher.Get())
                {
                    sCPUSerialNumber = mo["ProcessorId"].ToString().Trim();
                }
                return sCPUSerialNumber;
            }
            catch
            {
                return "";
            }
        }
        //获取硬盘序列号  
        public string GetHardDiskSerialNumber()
        {
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMedia");
                string sHardDiskSerialNumber = "";
                foreach (ManagementObject mo in searcher.Get())
                {
                    sHardDiskSerialNumber = mo["SerialNumber"].ToString().Trim();
                    break;
                }
                return sHardDiskSerialNumber;
            }
            catch
            {
                return "";
            }
        }


        //获取网卡地址  
        public string GetNetCardMACAddress()
        {
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_NetworkAdapter WHERE ((MACAddress Is Not NULL) AND (Manufacturer <> 'Microsoft'))");
                string NetCardMACAddress = "";
                foreach (ManagementObject mo in searcher.Get())
                {
                    NetCardMACAddress = mo["MACAddress"].ToString().Trim();
                }
                return NetCardMACAddress;
            }
            catch
            {
                return "";
            }
        }




        /// <summary>  
                /// 获得CPU编号  
                /// </summary>  
        public string GetCPUID()
        {
            string cpuid = "";
            ManagementClass mc = new ManagementClass("Win32_Processor");
            ManagementObjectCollection moc = mc.GetInstances();
            foreach (ManagementObject mo in moc)
            {
                cpuid = mo.Properties["ProcessorId"].Value.ToString();
            }
            return cpuid;
        }


        /// <summary>  
                /// 获取硬盘序列号  
                /// </summary>  
        public string GetDiskSerialNumber()
        {
            //这种模式在插入一个U盘后可能会有不同的结果，如插入我的手机时  
            String HDid = "";
            ManagementClass mc = new ManagementClass("Win32_DiskDrive");
            ManagementObjectCollection moc = mc.GetInstances();
            foreach (ManagementObject mo in moc)
            {
                HDid = (string)mo.Properties["Model"].Value;//SerialNumber  
                break;//这名话解决有多个物理盘时产生的问题，只取第一个物理硬盘  
            }
            return HDid;


            /*ManagementClass mc = new ManagementClass("Win32_PhysicalMedia"); 
            ManagementObjectCollection moc = mc.GetInstances(); 
            string str = ""; 
            foreach (ManagementObject mo in moc) 
            { 
                str = mo.Properties["SerialNumber"].Value.ToString(); 
                break; 
            } 
            return str;*/
        }


        /// <summary>  
                /// 获取网卡硬件地址  
                /// </summary>  
        public string GetMacAddress()
        {
            string mac = "";
            ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection moc = mc.GetInstances();
            foreach (ManagementObject mo in moc)
            {
                if ((bool)mo["IPEnabled"] == true)
                {
                    mac = mo["MacAddress"].ToString();
                    break;
                }
            }
            return mac;
        }


        /// <summary>  
                /// 获取IP地址  
                /// </summary>  
        public string GetIPAddress()
        {
            string st = "";
            ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection moc = mc.GetInstances();
            foreach (ManagementObject mo in moc)
            {
                if ((bool)mo["IPEnabled"] == true)
                {
                    //st=mo["IpAddress"].ToString();   
                    System.Array ar;
                    ar = (System.Array)(mo.Properties["IpAddress"].Value);
                    st = ar.GetValue(0).ToString();
                    break;
                }
            }
            return st;
        }


        /// <summary>  
                /// 操作系统的登录用户名  
                /// </summary>  
        public string GetUserName()
        {
            return Environment.UserName;
        }




        /// <summary>  
                /// 获取计算机名  
                /// </summary>  
        public string GetComputerName()
        {
            return Environment.MachineName;
        }


        /// <summary>  
                /// 操作系统类型  
                /// </summary>  
        public string GetSystemType()
        {
            string st = "";
            ManagementClass mc = new ManagementClass("Win32_ComputerSystem");
            ManagementObjectCollection moc = mc.GetInstances();
            foreach (ManagementObject mo in moc)
            {
                st = mo["SystemType"].ToString();
            }
            return st;
        }


        /// <summary>  
                /// 物理内存  
                /// </summary>  
        public string GetPhysicalMemory()
        {
            string st = "";
            ManagementClass mc = new ManagementClass("Win32_ComputerSystem");
            ManagementObjectCollection moc = mc.GetInstances();
            foreach (ManagementObject mo in moc)
            {
                st = mo["TotalPhysicalMemory"].ToString();
            }
            return st;
        }


        /// <summary>  
                /// 显卡PNPDeviceID  
                /// </summary>  
        public string GetVideoPNPID()
        {
            string st = "";
            ManagementObjectSearcher mos = new ManagementObjectSearcher("Select * from Win32_VideoController");
            foreach (ManagementObject mo in mos.Get())
            {
                st = mo["PNPDeviceID"].ToString();
            }
            return st;
        }


        /// <summary>  
                /// 声卡PNPDeviceID  
                /// </summary>  
        public string GetSoundPNPID()
        {
            string st = "";
            ManagementObjectSearcher mos = new ManagementObjectSearcher("Select * from Win32_SoundDevice");
            foreach (ManagementObject mo in mos.Get())
            {
                st = mo["PNPDeviceID"].ToString();
            }
            return st;
        }


        /// <summary>  
                /// CPU版本信息  
                /// </summary>  
        public string GetCPUVersion()
        {
            string st = "";
            ManagementObjectSearcher mos = new ManagementObjectSearcher("Select * from Win32_Processor");
            foreach (ManagementObject mo in mos.Get())
            {
                st = mo["Version"].ToString();
            }
            return st;
        }


        /// <summary>  
                /// CPU名称信息  
                /// </summary>  
        public string GetCPUName()
        {
            string st = "";
            ManagementObjectSearcher driveID = new ManagementObjectSearcher("Select * from Win32_Processor");
            foreach (ManagementObject mo in driveID.Get())
            {
                st = mo["Name"].ToString();
            }
            return st;
        }


        /// <summary>  
                /// CPU制造厂商  
                /// </summary>  
        public string GetCPUManufacturer()
        {
            string st = "";
            ManagementObjectSearcher mos = new ManagementObjectSearcher("Select * from Win32_Processor");
            foreach (ManagementObject mo in mos.Get())
            {
                st = mo["Manufacturer"].ToString();
            }
            return st;
        }


        /// <summary>  
                /// 主板制造厂商  
                /// </summary>  
        public string GetBoardManufacturer()
        {
            SelectQuery query = new SelectQuery("Select * from Win32_BaseBoard");
            ManagementObjectSearcher mos = new ManagementObjectSearcher(query);
            ManagementObjectCollection.ManagementObjectEnumerator data = mos.Get().GetEnumerator();
            data.MoveNext();
            ManagementBaseObject board = data.Current;
            return board.GetPropertyValue("Manufacturer").ToString();
        }


        /// <summary>  
                /// 主板编号  
                /// </summary>  
        public string GetBoardID()
        {
            string st = "";
            ManagementObjectSearcher mos = new ManagementObjectSearcher("Select * from Win32_BaseBoard");
            foreach (ManagementObject mo in mos.Get())
            {
                st = mo["SerialNumber"].ToString();
            }
            return st;
        }


        /// <summary>  
                /// 主板型号  
                /// </summary>  
        public string GetBoardType()
        {
            string st = "";
            ManagementObjectSearcher mos = new ManagementObjectSearcher("Select * from Win32_BaseBoard");
            foreach (ManagementObject mo in mos.Get())
            {
                st = mo["Product"].ToString();
            }
            return st;
        }
    }
}


/* 
 * 在很多情况下，你可能都需要得到微机的硬件信息。比如：你想给你的软件加锁，不让别人随便访问。 
 
 
最有效的办法是获取CPU的序列号，然后让你的软件只能运行在有这样的CPU序列号的机器上。众所周知，CPU序列号是唯一的！因此，这样就可以为你的软件加锁了。powered by 25175.net 
 
 
另外一个需要硬盘信息的例子是：硬盘有几个分区，每个分区各有多少剩余空间。当你正在做一个多媒体应用程序的时候，你可能也需要获得有关声卡、显卡的硬件信息。 
 
 
 
 
 
 
本应用程序另一个精彩的应用是：获取有关系统内存的信息，如内存地址，内存设备等等。  
 
 
首先，你必须知道这个应用程序的功能是使用System.Management这个类得到的。因此，你需要加上下面的这句话： 
 
 
using System.Management; 
 
 
 
 
为了获取硬件信息，你还需要创建一个ManagementObjectSearcher 对象。 
 
 
ManagementObjectSearcher searcher = new ManagementObjectSearcher("select * from " + Key); 
 
 
// 硬件  
Win32_Processor, // CPU 处理器  
Win32_PhysicalMemory, // 物理内存条  
Win32_Keyboard, // 键盘  
Win32_PointingDevice, // 点输入设备，包括鼠标。  
Win32_FloppyDrive, // 软盘驱动器  
Win32_DiskDrive, // 硬盘驱动器  
Win32_CDROMDrive, // 光盘驱动器  
Win32_BaseBoard, // 主板  
Win32_BIOS, // BIOS 芯片  
Win32_ParallelPort, // 并口  
Win32_SerialPort, // 串口  
Win32_SerialPortConfiguration, // 串口配置  
Win32_SoundDevice, // 多媒体设置，一般指声卡。  
Win32_SystemSlot, // 主板插槽 (ISA & PCI & AGP)  
Win32_USBController, // USB 控制器  
Win32_NetworkAdapter, // 网络适配器  
Win32_NetworkAdapterConfiguration, // 网络适配器设置  
Win32_Printer, // 打印机  
Win32_PrinterConfiguration, // 打印机设置  
Win32_PrintJob, // 打印机任务  
Win32_TCPIPPrinterPort, // 打印机端口  
Win32_POTSModem, // MODEM  
Win32_POTSModemToSerialPort, // MODEM 端口  
Win32_DesktopMonitor, // 显示器  
Win32_DisplayConfiguration, // 显卡  
Win32_DisplayControllerConfiguration, // 显卡设置  
Win32_VideoController, // 显卡细节。  
Win32_VideoSettings, // 显卡支持的显示模式。  
 
 
// 操作系统  
Win32_TimeZone, // 时区  
Win32_SystemDriver, // 驱动程序  
Win32_DiskPartition, // 磁盘分区  
Win32_LogicalDisk, // 逻辑磁盘  
Win32_LogicalDiskToPartition, // 逻辑磁盘所在分区及始末位置。  
Win32_LogicalMemoryConfiguration, // 逻辑内存配置  
Win32_PageFile, // 系统页文件信息  
Win32_PageFileSetting, // 页文件设置  
Win32_BootConfiguration, // 系统启动配置  
Win32_ComputerSystem, // 计算机信息简要  
Win32_OperatingSystem, // 操作系统信息  
Win32_StartupCommand, // 系统自动启动程序  
Win32_Service, // 系统安装的服务  
Win32_Group, // 系统管理组  
Win32_GroupUser, // 系统组帐号  
Win32_UserAccount, // 用户帐号  
Win32_Process, // 系统进程  
Win32_Thread, // 系统线程  
Win32_Share, // 共享  
Win32_NetworkClient, // 已安装的网络客户端  
Win32_NetworkProtocol, // 已安装的网络协议  
 
 
 
 
 
 
上面代码的Key是一个将被对应正确的数据填入的值。例如，获取CPU的信息，就需要把Key值设成Win32_Processor。所有Key可能的值，列举如下： 
 
 
Win32_1394Controller 
Win32_1394ControllerDevice 
Win32_Account 
Win32_AccountSID 
Win32_ACE 
Win32_ActionCheck 
Win32_AllocatedResource 
Win32_ApplicationCommandLine 
Win32_ApplicationService 
Win32_AssociatedBattery 
Win32_AssociatedProcessorMemory 
Win32_BaseBoard 
Win32_BaseService 
Win32_Battery 
Win32_Binary 
Win32_BindImageAction 
Win32_BIOS 
Win32_BootConfiguration 
Win32_Bus 
Win32_CacheMemory 
Win32_CDROMDrive 
Win32_CheckCheck 
Win32_CIMLogicalDeviceCIMDataFile 
Win32_ClassicCOMApplicationClasses 
Win32_ClassicCOMClass 
Win32_ClassicCOMClassSetting 
Win32_ClassicCOMClassSettings 
Win32_ClassInfoAction 
Win32_ClientApplicationSetting 
Win32_CodecFile 
Win32_COMApplication 
Win32_COMApplicationClasses 
Win32_COMApplicationSettings 
Win32_COMClass 
Win32_ComClassAutoEmulator 
Win32_ComClassEmulator 
Win32_CommandLineAccess 
Win32_ComponentCategory 
Win32_ComputerSystem 
Win32_ComputerSystemProcessor 
Win32_ComputerSystemProduct 
Win32_COMSetting 
Win32_Condition 
Win32_CreateFolderAction 
Win32_CurrentProbe 
Win32_DCOMApplication 
Win32_DCOMApplicationAccessAllowedSetting 
Win32_DCOMApplicationLaunchAllowedSetting 
Win32_DCOMApplicationSetting 
Win32_DependentService 
Win32_Desktop 
Win32_DesktopMonitor 
Win32_DeviceBus 
Win32_DeviceMemoryAddress 
Win32_DeviceSettings 
Win32_Directory 
Win32_DirectorySpecification 
Win32_DiskDrive 
Win32_DiskDriveToDiskPartition 
Win32_DiskPartition 
Win32_DisplayConfiguration 
Win32_DisplayControllerConfiguration 
Win32_DMAChannel 
Win32_DriverVXD 
Win32_DuplicateFileAction 
Win32_Environment 
Win32_EnvironmentSpecification 
Win32_ExtensionInfoAction 
Win32_Fan 
Win32_FileSpecification 
Win32_FloppyController 
Win32_FloppyDrive 
Win32_FontInfoAction 
Win32_Group 
Win32_GroupUser 
Win32_HeatPipe 
Win32_IDEController 
Win32_IDEControllerDevice 
Win32_ImplementedCategory 
Win32_InfraredDevice 
Win32_IniFileSpecification 
Win32_InstalledSoftwareElement 
Win32_IRQResource 
Win32_Keyboard 
Win32_LaunchCondition 
Win32_LoadOrderGroup 
Win32_LoadOrderGroupServiceDependencies 
Win32_LoadOrderGroupServiceMembers 
Win32_LogicalDisk 
Win32_LogicalDiskRootDirectory 
Win32_LogicalDiskToPartition 
Win32_LogicalFileAccess 
Win32_LogicalFileAuditing 
Win32_LogicalFileGroup 
Win32_LogicalFileOwner 
Win32_LogicalFileSecuritySetting 
Win32_LogicalMemoryConfiguration 
Win32_LogicalProgramGroup 
Win32_LogicalProgramGroupDirectory 
Win32_LogicalProgramGroupItem 
Win32_LogicalProgramGroupItemDataFile 
Win32_LogicalShareAccess 
Win32_LogicalShareAuditing 
Win32_LogicalShareSecuritySetting 
Win32_ManagedSystemElementResource 
Win32_MemoryArray 
Win32_MemoryArrayLocation 
Win32_MemoryDevice 
Win32_MemoryDeviceArray 
Win32_MemoryDeviceLocation 
Win32_MethodParameterClass 
Win32_MIMEInfoAction 
Win32_MotherboardDevice 
Win32_MoveFileAction 
Win32_MSIResource 
Win32_networkAdapter 
Win32_networkAdapterConfiguration 
Win32_networkAdapterSetting 
Win32_networkClient 
Win32_networkConnection 
Win32_networkLoginProfile 
Win32_networkProtocol 
Win32_NTEventlogFile 
Win32_NTLogEvent 
Win32_NTLogEventComputer 
Win32_NTLogEventLog 
Win32_NTLogEventUser 
Win32_ODBCAttribute 
Win32_ODBCDataSourceAttribute 
Win32_ODBCDataSourceSpecification 
Win32_ODBCDriverAttribute 
Win32_ODBCDriverSoftwareElement 
Win32_ODBCDriverSpecification 
Win32_ODBCSourceAttribute 
Win32_ODBCTranslatorSpecification 
Win32_OnBoardDevice 
Win32_OperatingSystem 
Win32_OperatingSystemQFE 
Win32_OSRecoveryConfiguration 
Win32_PageFile 
Win32_PageFileElementSetting 
Win32_PageFileSetting 
Win32_PageFileUsage 
Win32_ParallelPort 
Win32_Patch 
Win32_PatchFile 
Win32_PatchPackage 
Win32_PCMCIAController 
Win32_Perf 
Win32_PerfRawData 
Win32_PerfRawData_ASP_ActiveServerPages 
Win32_PerfRawData_ASPnet_114322_ASPnetAppsv114322 
Win32_PerfRawData_ASPnet_114322_ASPnetv114322 
Win32_PerfRawData_ASPnet_ASPnet 
Win32_PerfRawData_ASPnet_ASPnetApplications 
Win32_PerfRawData_IAS_IASAccountingClients 
Win32_PerfRawData_IAS_IASAccountingServer 
Win32_PerfRawData_IAS_IASAuthenticationClients 
Win32_PerfRawData_IAS_IASAuthenticationServer 
Win32_PerfRawData_InetInfo_InternetInformationServicesGlobal 
Win32_PerfRawData_MSDTC_DistributedTransactionCoordinator 
Win32_PerfRawData_MSFTPSVC_FTPService 
Win32_PerfRawData_MSSQLSERVER_SQLServerAccessMethods 
Win32_PerfRawData_MSSQLSERVER_SQLServerBackupDevice 
Win32_PerfRawData_MSSQLSERVER_SQLServerBufferManager 
Win32_PerfRawData_MSSQLSERVER_SQLServerBufferPartition 
Win32_PerfRawData_MSSQLSERVER_SQLServerCacheManager 
Win32_PerfRawData_MSSQLSERVER_SQLServerDatabases 
Win32_PerfRawData_MSSQLSERVER_SQLServerGeneralStatistics 
Win32_PerfRawData_MSSQLSERVER_SQLServerLatches 
Win32_PerfRawData_MSSQLSERVER_SQLServerLocks 
Win32_PerfRawData_MSSQLSERVER_SQLServerMemoryManager 
Win32_PerfRawData_MSSQLSERVER_SQLServerReplicationAgents 
Win32_PerfRawData_MSSQLSERVER_SQLServerReplicationDist 
Win32_PerfRawData_MSSQLSERVER_SQLServerReplicationLogreader 
Win32_PerfRawData_MSSQLSERVER_SQLServerReplicationMerge 
Win32_PerfRawData_MSSQLSERVER_SQLServerReplicationSnapshot 
Win32_PerfRawData_MSSQLSERVER_SQLServerSQLStatistics 
Win32_PerfRawData_MSSQLSERVER_SQLServerUserSettable 
Win32_PerfRawData_netFramework_netCLRExceptions 
Win32_PerfRawData_netFramework_netCLRInterop 
Win32_PerfRawData_netFramework_netCLRJit 
Win32_PerfRawData_netFramework_netCLRLoading 
Win32_PerfRawData_netFramework_netCLRLocksAndThreads 
Win32_PerfRawData_netFramework_netCLRMemory 
Win32_PerfRawData_netFramework_netCLRRemoting 
Win32_PerfRawData_netFramework_netCLRSecurity 
Win32_PerfRawData_Outlook_Outlook 
Win32_PerfRawData_PerfDisk_PhysicalDisk 
Win32_PerfRawData_Perfnet_Browser 
Win32_PerfRawData_Perfnet_Redirector 
Win32_PerfRawData_Perfnet_Server 
Win32_PerfRawData_Perfnet_ServerWorkQueues 
Win32_PerfRawData_PerfOS_Cache 
Win32_PerfRawData_PerfOS_Memory 
Win32_PerfRawData_PerfOS_Objects 
Win32_PerfRawData_PerfOS_PagingFile 
Win32_PerfRawData_PerfOS_Processor 
Win32_PerfRawData_PerfOS_System 
Win32_PerfRawData_PerfProc_FullImage_Costly 
Win32_PerfRawData_PerfProc_Image_Costly 
Win32_PerfRawData_PerfProc_JobObject 
Win32_PerfRawData_PerfProc_JobObjectDetails 
Win32_PerfRawData_PerfProc_Process 
Win32_PerfRawData_PerfProc_ProcessAddressSpace_Costly 
Win32_PerfRawData_PerfProc_Thread 
Win32_PerfRawData_PerfProc_ThreadDetails_Costly 
Win32_PerfRawData_RemoteAccess_RASPort 
Win32_PerfRawData_RemoteAccess_RASTotal 
Win32_PerfRawData_RSVP_ACSPerRSVPService 
Win32_PerfRawData_Spooler_PrintQueue 
Win32_PerfRawData_TapiSrv_Telephony 
Win32_PerfRawData_Tcpip_ICMP 
Win32_PerfRawData_Tcpip_IP 
Win32_PerfRawData_Tcpip_NBTConnection 
Win32_PerfRawData_Tcpip_networkInterface 
Win32_PerfRawData_Tcpip_TCP 
Win32_PerfRawData_Tcpip_UDP 
Win32_PerfRawData_W3SVC_WebService 
Win32_PhysicalMedia 
Win32_PhysicalMemory 
Win32_PhysicalMemoryArray 
Win32_PhysicalMemoryLocation 
Win32_PNPAllocatedResource 
Win32_PnPDevice 
Win32_PnPEntity 
Win32_PointingDevice 
Win32_PortableBattery 
Win32_PortConnector 
Win32_PortResource 
Win32_POTSModem 
Win32_POTSModemToSerialPort 
Win32_PowerManagementEvent 
Win32_Printer 
Win32_PrinterConfiguration 
Win32_PrinterController 
Win32_PrinterDriverDll 
Win32_PrinterSetting 
Win32_PrinterShare 
Win32_PrintJob 
Win32_PrivilegesStatus 
Win32_Process 
Win32_Processor 
Win32_ProcessStartup 
Win32_Product 
Win32_ProductCheck 
Win32_ProductResource 
Win32_ProductSoftwareFeatures 
Win32_ProgIDSpecification 
Win32_ProgramGroup 
Win32_ProgramGroupContents 
Win32_ProgramGroupOrItem 
Win32_Property 
Win32_ProtocolBinding 
Win32_PublishComponentAction 
Win32_QuickFixEngineering 
Win32_Refrigeration 
Win32_Registry 
Win32_RegistryAction 
Win32_RemoveFileAction 
Win32_RemoveIniAction 
Win32_ReserveCost 
Win32_ScheduledJob 
Win32_SCSIController 
Win32_SCSIControllerDevice 
Win32_SecurityDescriptor 
Win32_SecuritySetting 
Win32_SecuritySettingAccess 
Win32_SecuritySettingAuditing 
Win32_SecuritySettingGroup 
Win32_SecuritySettingOfLogicalFile 
Win32_SecuritySettingOfLogicalShare 
Win32_SecuritySettingOfObject 
Win32_SecuritySettingOwner 
Win32_SelfRegModuleAction 
Win32_SerialPort 
Win32_SerialPortConfiguration 
Win32_SerialPortSetting 
Win32_Service 
Win32_ServiceControl 
Win32_ServiceSpecification 
Win32_ServiceSpecificationService 
Win32_SettingCheck 
Win32_Share 
Win32_ShareToDirectory 
Win32_ShortcutAction 
Win32_ShortcutFile 
Win32_ShortcutSAP 
Win32_SID 
Win32_SMBIOSMemory 
Win32_SoftwareElement 
Win32_SoftwareElementAction 
Win32_SoftwareElementCheck 
Win32_SoftwareElementCondition 
Win32_SoftwareElementResource 
Win32_SoftwareFeature 
Win32_SoftwareFeatureAction 
Win32_SoftwareFeatureCheck 
Win32_SoftwareFeatureParent 
Win32_SoftwareFeatureSoftwareElements 
Win32_SoundDevice 
Win32_StartupCommand 
Win32_SubDirectory 
Win32_SystemAccount 
Win32_SystemBIOS 
Win32_SystemBootConfiguration 
Win32_SystemDesktop 
Win32_SystemDevices 
Win32_SystemDriver 
Win32_SystemDriverPNPEntity 
Win32_SystemEnclosure 
Win32_SystemLoadOrderGroups 
Win32_SystemLogicalMemoryConfiguration 
Win32_SystemMemoryResource 
Win32_SystemnetworkConnections 
Win32_SystemOperatingSystem 
Win32_SystemPartitions 
Win32_SystemProcesses 
Win32_SystemProgramGroups 
Win32_SystemResources 
Win32_SystemServices 
Win32_SystemSetting 
Win32_SystemSlot 
Win32_SystemSystemDriver 
Win32_SystemTimeZone 
Win32_SystemUsers 
Win32_TapeDrive 
Win32_TemperatureProbe 
Win32_Thread 
Win32_TimeZone 
Win32_Trustee 
Win32_TypeLibraryAction 
Win32_UninterruptiblePowerSupply 
Win32_USBController 
Win32_USBControllerDevice 
Win32_UserAccount 
Win32_UserDesktop 
Win32_VideoConfiguration 
Win32_VideoController 
Win32_VideoSettings 
Win32_VoltageProbe 
Win32_WMIElementSetting 
Win32_WMISetting 
 
 
 
 
首先，调用ManagementObjectSearcher实例（在本文中的例子里为searcher ）中的Get()方法，该方法将会把返回信息填在这个实例中。然后，你所要做的就是处理这个实例searcher中的数据。 
 
 
foreach (ManagementObject share in searcher.Get()){// Some Codes ...} 
 
 
每个ManagementObject的对象中都有一些，我们所需要的数据，当然我们可以接着这么处理这些数据： 
 
 
foreach (PropertyData PC in share.Properties){//some codes ...} 
 
 
本文中代码的其它部分只是对于ListView控件的一些操作，很简单，就不再详述了！ 
*/

/* Win32_VideoController, // 显卡细节。 
 AcceleratorCapabilities  --图形和视频控制器的三维阵列的能力
AdapterCompatibility  --用于此控制器与系统比较兼容性一般芯片组
AdapterDACType  --姓名或数字 - 模拟转换器（DAC）芯片的标识符
AdapterRAM  --视频适配器的内存大小
Availability  --可用性和设备的状态
CapabilityDescriptions  --自由形式字符串提供更详细的解释中的任何加速器能力阵列所指示的视频加速器功能
Caption  --对象的简短描述
ColorTableEntries  --尺寸系统的色表
ConfigManagerErrorCode  --Win32的配置管理器错误代码
ConfigManagerUserConfig  --如果为TRUE，该装置是使用用户定义的配置
CreationClassName  --第一个具体类的名称出现在创建实例所使用的继承链
CurrentBitsPerPixel  --使用的比特数以显示每个像素
CurrentHorizontalResolution  --水平像素的当前数量
CurrentNumberOfColors  --在当前的分辨率支持的色彩数目
CurrentNumberOfColumns  --此视频控制器列（如果在字符模式下）编号
CurrentNumberOfRows  --此视频控制器行（如果在字符模式下）编号
CurrentRefreshRate  --频率在该视频控制器刷新监视器的图像
CurrentScanMode  --当前扫描模式
CurrentVerticalResolution  --当前垂直像素数量
Description  --描述
DeviceID  --该视频控制器标识符（唯一的计算机系统）
DeviceSpecificPens  --目前许多设备专用笔。值0xFFFF表示设备不支持笔。
DitherType  --抖动型视频控制器
DriverDate  --当前已安装的视频驱动程序的最后修改日期和时间
DriverVersion  --视频驱动程序的版本号
ErrorCleared  --如果为真，报上一个错误代码属性中的错误现已清除
ErrorDescription  --可能采取的纠正措施字符串提供有关记录在一个错误代码属性错误的详细信息
ICMIntent  --应使用默认三种可能的配色方法或意图中的一个特定值
ICMMethod  --处理ICM方法。对于非ICM的应用程序，这个属性决定了ICM是否已启用对于ICM的应用程序，系统将检查此属性来确定如何处理ICM支持
InfFilename  --视频适配器的路径.inf文件
InfSection  --Windows的视频信息所在的.inf文件
InstallDate  --安装的日期
InstalledDisplayDrivers  --已安装的显示设备驱动程序的名称
LastErrorCode  --报告的逻辑设备上一个错误代码 
MaxMemorySupported  --以字节为单位支持的内存最高限额
MaxNumberControlled  --可支持通过该控制器可直接寻址的实体的最大数量
MaxRefreshRate  --在赫兹视频控制器的最大刷新率
MinRefreshRate   --在赫兹视频控制器的最小刷新率
Monochrome  --如果是TRUE，灰阶用于显示图像。
Name  --标签由该对象是已知的。当子类，该属性可以被覆盖是一个关键属性。
NumberOfColorPlanes  --当前一些颜色平面。如果该值不适用于当前视频的配置，输入0（零）
NumberOfVideoPages  --当前的分辨率和可用内存支持视频页数
PNPDeviceID  --即插即用逻辑设备的播放装置识别符
PowerManagementCapabilities  --逻辑设备的特定功率相关的能力阵列
PowerManagementSupported  --如果为TRUE，该装置可以是电源管理（可以投入挂起模式，等等）
ProtocolSupported  --由控制器使用协议访问“控制”的设备
ReservedSystemPaletteEntries  --系统调色板保留的条目数
SpecificationVersion  --初始化数据规范的版本号（在其上的结构的基础）
Status  --对象的当前状态
StatusInfo  --对象的当前状态详细信息
SystemCreationClassName  --该作用域计算机的创建类别名称属性的值
SystemName  --系统的名称
SystemPaletteEntries  --当前一些系统调色板颜色索引条目
TimeOfLastReset  --该控制器是最后一次复位日期和时间，这可能意味着该控制器被断电或重新初始化
VideoArchitecture  --视频体系结构的类型
VideoMemoryType  --显存类型
VideoMode  --当前视频模式
VideoModeDescription  --当前的分辨率，颜色和视频控制器的扫描模式设置
VideoProcessor  --无格式的字符串描述视频处理器 

   */

/* Win32_Processor, // CPU 处理器 
 AddressWidth  --在32位操作系统，该值是32，在64位操作系统是64。
Architecture  --所使用的平台的处理器架构。
AssetTag  --代表该处理器的资产标签。
Availability  --设备的状态。
Caption  --设备的简短描述
Characteristics  --处理器支持定义的功能
ConfigManagerErrorCode  --Windows API的配置管理器错误代码
ConfigManagerUserConfig  --如果为TRUE，该装置是使用用户定义的配置
CpuStatus  --处理器的当前状态
CreationClassName  --出现在用来创建一个实例继承链的第一个具体类的名称
CurrentClockSpeed  --处理器的当前速度，以MHz为单位
CurrentVoltage  --处理器的电压。如果第八位被设置，位0-6包含电压乘以10，如果第八位没有置位，则位在VoltageCaps设定表示的电压值。 CurrentVoltage时SMBIOS指定的电压值只设置
DataWidth  --在32位处理器，该值是32，在64位处理器是64
Description  --描述
DeviceID  --在系统上的处理器的唯一标识符
ErrorCleared  --如果为真，报上一个错误代码的被清除
ErrorDescription  --错误的代码描述
ExtClock  --外部时钟频率，以MHz为单位
Family  --处理器系列类型
InstallDate  --安装日期
L2CacheSize  --二级缓存大小
L2CacheSpeed  --二级缓存处理器的时钟速度
L3CacheSize  --三级缓存的大小
L3CacheSpeed  --三级缓存处理器的时钟速度
LastErrorCode  --报告的逻辑设备上一个错误代码
Level  --处理器类型的定义。该值取决于处理器的体系结构
LoadPercentage  --每个处理器的负载能力，平均到最后一秒
Manufacturer   --处理器的制造商
MaxClockSpeed  --处理器的最大速度，以MHz为单位
Name  --处理器的名称
NumberOfCores  --芯为处理器的当前实例的数目。核心是在集成电路上的物理处理器
NumberOfEnabledCore  --每个处理器插槽启用的内核数
NumberOfLogicalProcessors  --用于处理器的当前实例逻辑处理器的数量
OtherFamilyDescription  --处理器系列类型
PartNumber  --这款处理器的产品编号制造商所设置
PNPDeviceID  --即插即用逻辑设备的播放设备标识符
PowerManagementCapabilities  --逻辑设备的特定功率相关的能力阵列
PowerManagementSupported  --如果为TRUE，该装置的功率可以被管理，这意味着它可以被放入挂起模式
ProcessorId  --描述处理器功能的处理器的信息
ProcessorType  --处理器的主要功能
Revision  --系统修订级别取决于体系结构
Role  --所述处理器的作用
SecondLevelAddressTranslationExtensions  --如果为True，该处理器支持用于虚拟地址转换扩展
SerialNumber --处理器的序列号
SocketDesignation  --芯片插座的线路上使用的类型
Status  --对象的当前状态
StatusInfo  --对象的当前状态信息
Stepping  --在处理器家族处理器的版本
SystemCreationClassName  --创建类名属性的作用域计算机的价值
SystemName  --系统的名称
ThreadCount  --每个处理器插槽的线程数
UniqueId  --全局唯一标识符的处理器
UpgradeMethod  --CPU插槽的信息
Version  --依赖于架构处理器的版本号
VirtualizationFirmwareEnabled  --如果真，固件可以虚拟化扩展
VMMonitorModeExtensions  --如果为True，该处理器支持Intel或AMD虚拟机监控器扩展。
VoltageCaps  --该处理器的电压的能力 
 */
