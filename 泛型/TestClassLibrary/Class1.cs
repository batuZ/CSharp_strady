using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestClassLibrary
{
    public interface IClass
    {
        void getName();
        void getSize();
    }
    public class Class1: IClass
    {
        //属性
        public string libName { get; set; }
        public int libSize { get; }

        //方法
        public void getName()
        {
            Console.WriteLine(libName);
        }
        public void getSize()
        {
            Console.WriteLine(libSize);
        }
    }
    public class Class2 : IClass
    {
        public Class1 cls1 = new Class1();
        public float flt = 3.14f;

        public void getName()
        {
            
        }

        public void getSize()
        {
        }

        public void PubFunc() { }
        private void priFunc() { }
    }
}
