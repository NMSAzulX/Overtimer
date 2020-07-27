using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using System.Text.RegularExpressions;

namespace Timeviewer
{
    public class TeamviewOperator
    {
        private static Regex userReg;

        static TeamviewOperator()
        {
            userReg = new Regex(@"\d+ \d+ \d+", RegexOptions.Singleline | RegexOptions.Compiled);
        }
        public TeamviewOperator()
        {
            Username = string.Empty;
            Password = string.Empty;
            Holder = string.Empty;
        }
        internal int _count;
        public string Username;
        public string Password;
        public string Holder;

        public static TeamviewOperator GetUser()
        {
            TeamviewOperator user = new TeamviewOperator();
            IntPtr tvHwnd = WindowsApi.FindWindow(null, "TeamViewer");
            if (tvHwnd != IntPtr.Zero)
            {
                IntPtr winParentPtr = WindowsApi.GetWindow(tvHwnd, GetWindowCmd.GW_CHILD);
                while (winParentPtr != IntPtr.Zero)
                {

                    IntPtr winSubPtr = WindowsApi.GetWindow(winParentPtr, GetWindowCmd.GW_CHILD);
                    while (winSubPtr != IntPtr.Zero)
                    {
                        StringBuilder controlName = new StringBuilder(512);
                        //获取控件类型
                        WindowsApi.GetClassName(winSubPtr, controlName, controlName.Capacity);  

                        if (controlName.ToString() == "Edit")
                        {

                            StringBuilder winMessage = new StringBuilder(512);
                            //获取控件内容 0xD
                            WindowsApi.SendMessage(winSubPtr, 0xD, (IntPtr)winMessage.Capacity, winMessage);
                            string message = winMessage.ToString();
                            if (userReg.IsMatch(message))
                            {
                                user.Username = message;
                                user._count += 1;

                            }else if (user.Password!=string.Empty)
                            {
                                user.Holder = message;
                                user._count += 1;
                            }
                            else
                            {
                                user.Password = message;
                                user._count += 1;
                            }
                            if (user._count==3)
                            {
                                return user;
                            }
                        }

                        //获取当前子窗口中的下一个控件
                        winSubPtr = WindowsApi.GetWindow(winSubPtr, GetWindowCmd.GW_HWNDNEXT);
                    }
                    //获取当前子窗口中的下一个控件
                    winParentPtr = WindowsApi.GetWindow(winParentPtr, GetWindowCmd.GW_HWNDNEXT);
                }
            }
            return user;
        }


        public static Dictionary<string, string> GetInfos(params string[] keys)
        {
            IntPtr tvHwnd = WindowsApi.FindWindow(null, "TeamViewer");
            return CheckPtrInfo(tvHwnd, ImmutableHashSet.CreateRange(keys));
        }

        public static Dictionary<string,string> CheckPtrInfo(IntPtr tvHwnd, ImmutableHashSet<string> keys)
        {
            Dictionary<string, string> result = default;
            if (tvHwnd != IntPtr.Zero)
            {

                tvHwnd = WindowsApi.GetWindow(tvHwnd, GetWindowCmd.GW_CHILD);
                
                if (tvHwnd != IntPtr.Zero)
                {

                    var message = GetMessage(tvHwnd);
                    if (keys.Contains(message))
                    {
                        tvHwnd = WindowsApi.GetWindow(tvHwnd, GetWindowCmd.GW_HWNDNEXT);
                        var text = GetMessage(tvHwnd);
                        if (result == default)
                        {
                            result = new Dictionary<string, string>();
                        }
                        result[message] = text;
                    }

                    while (tvHwnd != IntPtr.Zero)
                    {
                        var tempReuslt = CheckPtrInfo(tvHwnd, keys);
                        if (tempReuslt != default)
                        {
                            if (result == default)
                            {
                                result = tempReuslt;
                            }
                            else
                            {
                                foreach (var item in tempReuslt)
                                {
                                    result[item.Key] = item.Value;
                                }
                            }
                        }
                        tvHwnd = WindowsApi.GetWindow(tvHwnd, GetWindowCmd.GW_HWNDNEXT);
                    }

                }            
            }
               
            return result;
        }

        public static string GetMessage(IntPtr tvHwnd)
        {
            StringBuilder winMessage = new StringBuilder(512);
            WindowsApi.SendMessage(tvHwnd, 0xD, (IntPtr)winMessage.Capacity, winMessage);
            return winMessage.ToString();
        }
    }

}
