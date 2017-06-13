using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 
            //由框架提供的委托类型
            //无返的
            Action act0 = () => Console.WriteLine("无参无返");
            Action<int> act1 = x => Console.WriteLine("1 参无返");
            Action<int, int> act2 = (x, y) => Console.WriteLine("2 参无返");
            //有返的
            Func<string> func0 = () => "无参有返";
            Func<int, string> func1 = x => "1 参有返";
            Func<int, double, string> func2 = (x, y) => "多参有返";

     */

namespace 委托_deleget
{

    //  1、声明委托
    //就声明方法模版加 deleget 修饰
    //就是把方法转义成类，同时指定这个类的使用规则，如 返、参的类型或数量

    public delegate void nRnC();
    public delegate int hRnC();
    delegate string hRhC(string a, int b);


    class Program
    {
        //  2、定义供委托调用的方法
        private static void for_nRnC_1() { Console.WriteLine("for_nRnC_1"); }
        private static void for_nRnC_2() { Console.WriteLine("for_nRnC_2"); }
        private static string for_hRhC(string a, int v)
        {
            string temp = null;
            for (int i = 0; i < v; i++)
            {
                temp += a;
            }
            return temp;
        }


        static void Main(string[] args)
        {
            //  3、实例化委托对象，它包括一个调用方法列表

            //无参无返
            nRnC myDeleget_nRnC = new nRnC(for_nRnC_1);
            //有参有返,
            hRhC myDeleget_hRhC = new hRhC(for_hRhC);

            //  4、使用（执行）委托

            // nRnC
            myDeleget_nRnC.Invoke();

            //hRhC
            string ret = myDeleget_hRhC("batu", 5);
            Console.WriteLine(ret);

            //多波委托,返回值为最后一个方法的返回值
            myDeleget_nRnC += for_nRnC_2;
            myDeleget_nRnC -= for_nRnC_2;

            Console.Read();
        }
    }

    //事件_Event

    class EventTest
    {
        void doSomeThing()
        {
            ForEvent myTest = new ForEvent();
            myTest.shijian += MyTest_shijian;
            //myTest.shijian = MyTest_shijian;
            //myTest.shijian();
            myTest.weituo = MyTest_shijian;
            myTest.weituo();
        }

        private void MyTest_shijian() {; ; ; }
    }

    class ForEvent
    {
        //这是一个属性,可以直接赋值和执行
        //相当于把类内部的方法直接暴露出来
        public nRnC weituo { set; get; }

        //这是一个事件，只能 += 或 -= 且不能从外部调用
        //Event 的用作是，控制对象的权限，
        //在外部只能向这个事件上挂方法或删方法，不能执行，也不能改变原有方法
        public event nRnC shijian;
    }
}