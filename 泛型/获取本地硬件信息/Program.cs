﻿using HuaTong.General.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace 获取本地硬件信息
{
    class Program
    {
        static void Main(string[] args)
        {
         string d=   MachineNumber.OSInfo();
        }
    }
}
