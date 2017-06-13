using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace 异步_Async
{
    class Program
    {
        static void Main(string[] args)
        {
            Func<double, int> myFunc = x => Convert.ToInt32(x);


            //关于回调方法 ，无返回且带一个参数的委托 , 在异步完成后调用一次
            //由子线程调用，与主线无关
            //public delegate void AsyncCallback(IAsyncResult ar);
            AsyncCallback callback =
                r => Console.WriteLine("I'm CallBack,and{0}", r.AsyncState);//r.AsyncState = "haha"






            //主线程调用
            int res = myFunc.Invoke(66.6);

            //异步调用(委托形参，回调方法或lambda，回调方法参数)
            IAsyncResult asyncRes = myFunc.BeginInvoke(44.3, callback, "haha");
            //用例：
            myFunc.BeginInvoke(2.3, r =>//r 就是 asyncRes，用BeginInvoke的返回值当作参数传入回调方法
            {
                Console.WriteLine("I'm CallBack,and{0}");
                Console.WriteLine(r.AsyncState);
            }, "haha");






            //   重要义意: 在启动异步后 ，异步等待以前 ， 可以做很多事 
            //   这时，主线程闲置，子线程工作
            Console.WriteLine("其它事A");
            Console.WriteLine("其它事B");
            Console.WriteLine("其它事C");
            




            //异步等待，阻止主线程，状态类似于在主线程工作

            //1、 阻止主线程一段时间（毫秒），-1为无限等待
            asyncRes.AsyncWaitHandle.WaitOne(-1);

            //2、
            while (asyncRes.IsCompleted)
            {
                Console.WriteLine("进度，或请等待");
                Thread.Sleep(111);
            }

            //3、阻止主线程等待异步完成，并拿到myFunc的返回值（计算结果）
            int ress = myFunc.EndInvoke(asyncRes);
        }
    }
}
