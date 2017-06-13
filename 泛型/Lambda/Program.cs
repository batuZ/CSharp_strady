using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lambda //匿名方法，用来实现委托
{
    class Program
    {

        delegate string nRnC(int x, float y);

        static void Main(string[] args)
        {
            // delegat
            nRnC mod_1 = new nRnC(func);
            nRnC mod_2 = func;

            //to lambda 演化过程
            nRnC moc_3 = new nRnC(delegate (int x, float y) { return ""; });

            nRnC mod_4 = new nRnC((int x, float y) => { return ""; });

            nRnC mod_5 = new nRnC((x, y) => { return ""; });

            nRnC mod_6 = (x, y) => "";


            //由框架提供的委托类型
            //无返的
            Action act0 = () => Console.WriteLine("无参无返");
            Action<int> act1 = x => Console.WriteLine("1 参无返");
            Action<int, int> act2 = (x, y) => Console.WriteLine("2 参无返");
            //有返的,最后一个参数就是返回值
            Func<string> func0 = () => "无参有返";
            Func<int, string> func1 = x => "1 参有返";
            Func<int, double, string> func2 = (x, y) => "多参有返";

        }
        private static string func(int x, float y)
        {
            return "";
        }
    }
}
