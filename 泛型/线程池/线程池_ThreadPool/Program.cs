using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

/*
 *  简化多线程使用，发挥主机最大性能
 *  
 *  网论，可以不修改最大线程数，框架会根据实时资源情况启动任务，带内存保守机制
 *  经测试，8 核心时可以达到理想状态，使用 32 核心CPU时，仅发挥CPU（内存）不到50%能力
 *  结论，自己取核心数-1，指定给线程池，内存不足时线程池有保护功能，目前来看不用管内存问题
 *  异步读写，计算量 > 读写量时，异步数可以等于核心数，否则 。。。
 *  
     */


namespace 线程池_ThreadPool
{
    class Program
    {
        static void Main(string[] args)
        {
            //设置最大任务数
            int num = getNumber();
            ThreadPool.SetMaxThreads(num, num);

            string[] files = new string[10000];

            // 1 匿名函数
            for (int i = 0; i < files.Length; i++)
            {
                string filePath = files[i];
                ThreadPool.QueueUserWorkItem((object sender) =>
                {
                    //using (Bitmap map = new Bitmap(filePath))
                    //{
                    //    BitmapData bmpData = map.LockBits(new Rectangle(Point.Empty, map.Size),
                    //        ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                    //    byte[] buffer = new byte[bmpData.Stride * bmpData.Height];
                    //    Marshal.Copy(bmpData.Scan0, buffer, 0, buffer.Length);
                    //    // do something at this map...
                    //    Marshal.Copy(buffer, 0, bmpData.Scan0, buffer.Length);
                    //    map.UnlockBits(bmpData);
                    //    map.Save(filePath);
                    //    map.Dispose();
                    //}
                    //GC.Collect();
                });
            }

            // 2 调外部函数
            for (int i = 0; i < files.Length; i++)
            {
                ThreadPool.QueueUserWorkItem(work, files[i]);
            }
        }

        static void work(object sender)
        {
            string filePath = sender.ToString();
            using (Bitmap map = new Bitmap(filePath))
            {
                BitmapData bmpData = map.LockBits(new Rectangle(Point.Empty, map.Size),
                    ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                byte[] buffer = new byte[bmpData.Stride * bmpData.Height];
                Marshal.Copy(bmpData.Scan0, buffer, 0, buffer.Length);
                // do something at this map...
                Marshal.Copy(buffer, 0, bmpData.Scan0, buffer.Length);
                map.UnlockBits(bmpData);
                map.Save(filePath);
                map.Dispose();
            }
            GC.Collect();
        }
        /// <summary>
        /// 获取核心数
        /// </summary>
        /// <returns></returns>
        static int getNumber()
        {
            ManagementObjectCollection cpus = new ManagementClass("Win32_Processor").GetInstances();
            int num = -1;
            foreach (var cpu in cpus)
            {
                num += Convert.ToInt32(cpu.Properties["NumberOfLogicalProcessors"].Value);
            }
            return num;
        }
    }
}
