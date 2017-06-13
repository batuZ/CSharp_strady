using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 多线程
{
    class Program
    {
        static void Main(string[] args)
        {
            //任务工厂
            TaskFactory taskFactory = new TaskFactory();
            //任务集
            List<Task> taskList = new List<Task>();
            for (int i = 0; i < 33; i++)
            {
                //创建并启动任务
                Task aTask = taskFactory.StartNew(f => Console.WriteLine(f + i.ToString()), "tt");
                //塞入任务集
                taskList.Add(aTask);
            }
            //阻止主线程，等待子线程全部完成
            Task.WaitAll(taskList.ToArray());//给个任务集
            Task.WaitAny(taskList.ToArray());

            //不阻止主，等待子线程全部完成后，调用一个委托（热线程）,有用！
            taskFactory.ContinueWhenAll(
                taskList.ToArray(),         //任务集
                r => Console.WriteLine("任务全部完成啦！"));
        }
    }
}
