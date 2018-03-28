using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace 获取本地硬件信息
{
    static class ComputerInfo
    {
        static List<string> strList = new List<string>();
        public static void getInfo()
        {
            strList = new List<string>();
            CPU();
            Memory();
            Display();
            HardDesk();
            File.WriteAllLines(@"d:\ttt.txt", strList.ToArray());
        }


        /// <summary>
        /// CPU
        /// </summary>
        static void CPU()
        {
            Dictionary<string, string> ttt = new Dictionary<string, string>();
            ManagementObjectCollection cpus = new ManagementObjectSearcher("SELECT * FROM Win32_Processor").Get();
            strList.Add("\r\nCPU：数量 " + cpus.Count.ToString());
            foreach (ManagementObject cpu in cpus)
            {
                //Name  --处理器的名称
                //CurrentClockSpeed  --处理器的当前速度，以MHz为单位
                //MaxClockSpeed  --处理器的最大速度，以MHz为单位
                //NumberOfCores  --芯为处理器的当前实例的数目。核心是在集成电路上的物理处理器
                //NumberOfLogicalProcessors  --用于处理器的当前实例逻辑处理器的数量
                //L2CacheSize  --二级缓存大小
                //L3CacheSize  --三级缓存的大小
                string temp = string.Format("{0}，{1}核心{2}线程  {3}GHz -> {4}GHz, L2:{5}KB , L3:{6}MB",
                                            cpu.Properties["Name"].Value,
                                            cpu.Properties["NumberOfCores"].Value,
                                            cpu.Properties["NumberOfLogicalProcessors"].Value,
                                            Convert.ToDouble(cpu.Properties["CurrentClockSpeed"].Value) * 0.001,
                                            Convert.ToDouble(cpu.Properties["MaxClockSpeed"].Value) * 0.001,
                                            cpu.Properties["L2CacheSize"].Value,
                                            cpu.Properties["L3CacheSize"].Value);
                strList.Add(temp);
            }
        }
        /// <summary>
        /// 内存
        /// </summary>
        static void Memory()
        {
            ManagementObjectCollection memo = new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMemory").Get();
            double capacity = 0;
            foreach (ManagementObject item in memo)
                capacity += Convert.ToDouble(item["Capacity"]);

            string temp = string.Format("\r\n内存：{0:F2} GB", capacity / 1024 / 1024 / 1024);
            strList.Add(temp);
        }
        /// <summary>
        /// 可用内存
        /// </summary>
        static void Available()
        {
            ManagementObjectCollection available = new ManagementClass("Win32_PerfFormattedData_PerfOS_Memory").GetInstances();
            double avaiMem = 0;
            foreach (var item in available)
            {
                avaiMem += Convert.ToDouble(item["AvailableBytes"]);
            }
            string outPut = $"可用内存：{avaiMem / 1024 / 1024 / 1024:f3} GB";
        }
        /// <summary>
        /// 显卡
        /// </summary>
        static void Display()
        {
            ManagementObjectCollection display = new ManagementObjectSearcher("SELECT * FROM Win32_VideoController").Get();
            strList.Add("\r\n显卡：数量 " + display.Count.ToString());
            foreach (ManagementObject aDis in display)
            {
                // Caption 型号  AdapterRAM 显存
                string temp = string.Format("{0} , 显存：{1:F2} GB", aDis.Properties["Caption"].Value,
                   Convert.ToDouble(aDis.Properties["AdapterRAM"].Value) / 1024 / 1024 / 1024);
                strList.Add(temp);
            }
        }
        /// <summary>
        /// 硬盘
        /// </summary>
        static void HardDesk()
        {
            DriveInfo[] drives = DriveInfo.GetDrives();
            strList.Add("\r\n磁盘：");
            foreach (DriveInfo drive in drives)
            {
                if (drive.IsReady)
                {
                    double val1 = (double)drive.TotalSize / 1024 / 1024 / 1024;
                    double val2 = (double)drive.TotalFreeSpace / 1024 / 1024 / 1024;
                    string temp = string.Format("{0}({2})  总容量:{3:F2} GB  可用容量:{4:F2} GB  {1}%可用",
                            drive.Name,
                            string.Format("{0:F2}", val2 / val1 * 100),
                            drive.DriveFormat,
                            val1, val2);
                    strList.Add(temp);
                }
            }
        }
    }
}



