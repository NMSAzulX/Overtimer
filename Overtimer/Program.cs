using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using Timeviewer;

namespace Overtimer
{
    class Program
    {
        
        static void Main(string[] args)
        {
            var result = TeamviewOperator.GetInfos("您的ID","密码");
            //RemoveController user = RemoveController.GetUser();
            //Console.WriteLine($"ID:{user.Username}");
            //Console.WriteLine($"Pass:{user.Password}");
            //Console.WriteLine($"User:{user.Holder}");
            Console.ReadKey();
        }
    }
}
