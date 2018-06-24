using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Rolebot.Events;
using Rolebot.MySql;
using Rolebot.Utils;

namespace Rolebot
{
    class Program
    {
        private DiscordSocketClient Client;
        private RoleMonitor RoleMonitor;
        private JoinUserMonitor JoinUserMonitor;
        private MySqlClient MySqlClient;
        static void Main(string[] args)
        {
#if (!DEBUG)
            Console.WriteLine("RolebotStart");
#endif
            Debug.Log("StartUP");
            var program = new Program();
            program.Awake();
            program.MainAsync().GetAwaiter().GetResult();
        }

        public void Awake()
        {
            Client = new DiscordSocketClient();
            MySqlClient = new MySqlClient(EnvManager.Server, EnvManager.Database, EnvManager.Table, EnvManager.UserId, EnvManager.Password);
            RoleMonitor = new RoleMonitor(MySqlClient);
            JoinUserMonitor = new JoinUserMonitor(MySqlClient);
        }

        private async Task DiscordStart()
        {
            await Client.LoginAsync(TokenType.Bot, EnvManager.DiscordToken);
            await Client.StartAsync();
        }

        public async Task MainAsync()
        {
            GetEvent();
            await DiscordStart();
            await Task.Delay(-1);
        }

        private void GetEvent()
        {
            Console.CancelKeyPress += Console_CancelKeyPress;
            Client.Log += Log;
            Client.Ready += Client_Ready;
            Client.GuildMemberUpdated += RoleMonitor.GuildMemberUpdated;
            Client.UserJoined += JoinUserMonitor.UserJoined;
        }

        private void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            Debug.Log("Exit");
        }

        private async Task Client_Ready()
        {

        }



        private Task Log(LogMessage message)
        {
            Debug.Log(message.Message);
            return Task.CompletedTask;
        }
    }
}
