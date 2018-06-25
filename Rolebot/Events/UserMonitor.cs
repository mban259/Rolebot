using System.Linq;
using System.Threading.Tasks;
using Discord.WebSocket;
using Rolebot.MySql;
using Rolebot.Utils;

namespace Rolebot.Events
{
    class UserMonitor
    {
        private MySqlClient MySqlClient;
        internal UserMonitor(MySqlClient mySqlClientr)
        {
            MySqlClient = mySqlClientr;
        }

        internal async Task UserJoined(SocketGuildUser user)
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

        internal async Task UserLeft(SocketGuildUser user)
        {
            Debug.Log($"{user.Username}:{user.Nickname}:{user.Id} Left {user.Guild.Name}:{user.Guild.Id}");
            var roleIds = user.Roles.Where(r => !r.IsEveryone).Select(r => r.Id);
            MySqlClient.UpdateRoles(user.Id, user.Guild.Id, roleIds);
        }
    }


}
