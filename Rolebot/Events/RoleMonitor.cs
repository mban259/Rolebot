﻿using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Rolebot.Utils;
using Rolebot.MySql;

namespace Rolebot.Events
{
    class RoleMonitor
    {
        private MySqlClient MySqlClient;

        public RoleMonitor(MySqlClient mySqlClient)
        {
            MySqlClient = mySqlClient;
        }

        private string RoleToString(IRole role) => $"{role.Name}:{role.Id}";
        public Task GuildMemberUpdated(SocketGuildUser before, SocketGuildUser after)
        {
            if (!before.Roles.SequenceEqual(after.Roles))
            {
                var beforeRoles = before.Roles.Where(r => !r.IsEveryone);
                var afterRoles = after.Roles.Where(r => !r.IsEveryone);
                var afterRoleIds = afterRoles.Select(r => r.Id);
                var beforeRolesString = string.Join(",", beforeRoles.Select(r => $"{r.Name}:{r.Id}"));
                var afterRoleString = string.Join(",", afterRoles.Select(r => $"{r.Name}:{r.Id}"));
                Debug.Log($"{after.Username}:{after.Nickname}:{after.Id} RoleChange [{beforeRolesString}] -> [{afterRoleString}]");
                var dbRoles = MySqlClient.GetRoles(after.Id, after.Guild.Id);
                var addedRoles = afterRoleIds.Except(dbRoles);
                var removedRoles = dbRoles.Except(afterRoleIds);
                if (addedRoles.Any()) MySqlClient.InsertRoles(after.Id, after.Guild.Id, addedRoles);
                if (removedRoles.Any()) MySqlClient.DeleteRoles(after.Id, after.Guild.Id, removedRoles);
            }
            return Task.CompletedTask;
        }
    }
}