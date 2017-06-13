using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 泛型
{
    class Program
    {
        static void Main(string[] args)
        {
            //普通写法
            showPutIn(445);
            showPutIn("happy");
            long s = 999999;
            showPutIn(s);
            //完整写法
            showPutIn<string>("I'm a string");
            //         <T>      (T parm)




        }
        // T 是一个类型参数，只有在函数被调用时才能确定
        // 没有类型的转换过程，比用object装箱拆箱效率高
        static void showPutIn<T>(T parm)
        {
            Console.WriteLine("{0} is {1}", parm, parm.GetType());
        }
        
        //泛型约束
        //不约束时，SUN可以是任何类型，且parm里只有object的方法
        //通过where SUN : Parent约束后,则只能传入Parent或Parent的子类
        //并且SUN有了Parent的方法和属性
        static void whoRU<SUN>(SUN parm) where SUN : Parent
        {
            parm.name = "haha";
        }
    }

    class Parent { public string name; }
    class sunA : Parent { }
    class sunB : Parent { }
}
