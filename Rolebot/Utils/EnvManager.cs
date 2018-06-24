using System;
using DotNetEnv;

namespace Rolebot.Utils
{
    static class EnvManager
    {
        public static string Server { private set; get; }
        public static string UserId { private set; get; }
        public static string Password { private set; get; }
        public static string Database { private set; get; }
        public static string Table { private set; get; }
        public static string DiscordToken { private set; get; }

        static EnvManager()
        {
            GetEnv();
        }

        public static void GetEnv()
        {
            try
            {
                Debug.Log("LoadEnv");
                Env.Load();
                Server = Env.GetString("MYSQL_SERVER");
                UserId = Env.GetString("MYSQL_USER_ID");
                Password = Env.GetString("MYSQL_PASSWORD");
                Database = Env.GetString("DATABASE");
                Table = Env.GetString("DATATABLE");
                DiscordToken = Env.GetString("DISCORD_TOKEN");
                Debug.Log("Success");
            }
            catch (Exception e)
            {
                Debug.Log($"Failed\n{e}");
                throw;
            }
        }
    }
}
