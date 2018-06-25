using System;
using DotNetEnv;

namespace Rolebot.Utils
{
    static class EnvManager
    {
        internal static string Server { private set; get; }
        internal static string UserId { private set; get; }
        internal static string Password { private set; get; }
        internal static string Database { private set; get; }
        internal static string Table { private set; get; }
        internal static string DiscordToken { private set; get; }

        static EnvManager()
        {
            GetEnv();
        }

        internal static void GetEnv()
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
