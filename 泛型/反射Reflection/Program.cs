using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;//                  <------------- 添加引用
using System.Text;
using System.Threading.Tasks;

namespace 反射Reflection
{
    class Program
    {
        static void Main(string[] args)
        {

            //加载DLL，要放在编译目录下
            Assembly ass = Assembly.Load("TestClassLibrary");

            //----------------------- 获取DLL模块 ---------------------

            //获取指定的DLL模块
            Module dll = ass.GetModule("TestClassLibrary.dll");
            //获取全部DLL模块,dlls[0] = TestClassLibrary.dll
            Module[] dlls = ass.GetModules();

            //---------------------- 获取模块中的类 ----------------------

            //获取指定的类
            Type tagT = ass.GetType("TestClassLibrary.Class1");
            string dd = tagT.Name;

            //获取所有类
            Type[] tpyes = ass.GetTypes();

            //---------------------- 创建实例 ----------------------

            //用这个类创建实例
            object aTagObject = Activator.CreateInstance(tagT);
            
            //一般是通过接口创建一个返射模块中的类对象
            //这需要模块和主程序都引用接口

            //---------------------- 获取类的方法 ----------------------

            //获取方法的信息
            MethodInfo[] methods = tagT.GetMethods();
            MethodInfo method_getSize = tagT.GetMethod("getSize");

            //调用无参无返的方法（输入一个此类的实例，无参则给空，或 new object[] { }）
            method_getSize.Invoke(aTagObject, new object[] { });

            //调用无参有返的方法
            //调用方法的一些标志位，这里的含义是Public并且是实例方法，这也是默认的值
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance;
            //GetValue方法的参数放在{}中
            object[] parameters = new object[] { };
            //调用方法，用一个object接收返回值
            object returnValue = method_getSize.Invoke(aTagObject, flag, Type.DefaultBinder, parameters, null);

            //---------------------- 获取类的属性 ----------------------
            MemberInfo[] mem = tagT.GetMembers();
            MemberInfo[] libName = tagT.GetMember("libName");
            
            Console.Write("");
        }
    }
}
