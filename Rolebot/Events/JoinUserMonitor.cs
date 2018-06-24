using System.Linq;
using System.Threading.Tasks;
using Discord.WebSocket;
using Rolebot.MySql;
using Rolebot.Utils;

namespace Rolebot.Events
{
    class JoinUserMonitor
    {
        private MySqlClient MySqlClient;
        public JoinUserMonitor(MySqlClient mySqlClientr)
        {
            MySqlClient = mySqlClientr;
        }

        public async Task UserJoined(SocketGuildUser user)
        {
            //サーバー入る
            //役職1個目付与
            //役職アップデートイベント取得
            //DB更新
            //役職2個目付与
            //役職アップデートイベント取得
            //DB更新
            //...
            //ってなる
            //仕様は満たしているがなんだかなぁ
            Debug.Log($"{user.Username}:{user.Nickname}:{user.Id} Join {user.Guild.Name}:{user.Guild.Id}");
            ulong[] roleIds = MySqlClient.GetRoles(user.Id, user.Guild.Id);
            if (roleIds.Any())
            {
                var roles = roleIds.Select(user.Guild.GetRole);
                await user.AddRolesAsync(roles);
                var rolesString = string.Join(",", roles.Select(r => $"{r.Name}:{r.Id}"));
                Debug.Log($"{user.Username}:{user.Nickname}:{user.Id} SetRole [{rolesString}]");
            }
        }
    }


}
