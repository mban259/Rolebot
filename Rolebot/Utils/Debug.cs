using System;
using System.IO;

namespace Rolebot.Utils
{
    static class Debug
    {
        public static void Log(string s)
        {
            string writeResult = $"{DateTime.Now} {s}";
            using (StreamWriter sw = new StreamWriter("debug.log", true))
            {
                sw.WriteLine(writeResult);
            }
            Console.WriteLine(writeResult);
        }

        public static void Log(string s, params object[] objects)
        {
            string formated = string.Format(s, objects);
            Log(formated);
        }
    }
}
