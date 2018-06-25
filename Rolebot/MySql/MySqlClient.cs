using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using MySql.Data.MySqlClient;
using Rolebot.Utils;

namespace Rolebot.MySql
{
    class MySqlClient
    {
        private readonly string Table;
        private readonly string MySqlConnectionString;
        internal MySqlClient(string server, string database, string table, string userId, string password)
        {
            MySqlConnectionString = new MySqlConnectionStringBuilder()
            {
                Server = server,
                Database = database,
                UserID = userId,
                Password = password,
                SslMode = MySqlSslMode.None
            }.ToString();
            Table = table;
        }

        internal ulong[] GetRoles(ulong userId, ulong guildId)
        {
            DataTable dataTable = new DataTable();
            ulong[] result;
            using (var conneciton = new MySqlConnection(MySqlConnectionString))
            {
                using (var command = conneciton.CreateCommand())
                {
                    try
                    {
                        conneciton.Open();
                        command.CommandText = $"select role from {Table} where user={userId} and guild={guildId}";
                        Debug.Log($"SendCommand {command.CommandText}");
                        var adapter = new MySqlDataAdapter(command);
                        adapter.Fill(dataTable);
                        result = (from DataRow row in dataTable.Rows select (ulong)row["role"]).ToArray();
                        Debug.Log($"Success Result:[{string.Join(",", result)}]");
                    }
                    catch (Exception e)
                    {
                        Debug.Log($"Failed\n{e}");
                        throw;
                    }
                }
            }

            return result;
        }
        internal void InsertRole(ulong userId, ulong guildId, ulong roleId) => MySqlCommandNonQuery($"insert into {Table} (user,guild,role) values({userId},{guildId},{roleId})");
        internal void InsertRoles(ulong userId, ulong guildId, IEnumerable<ulong> roleIds)
        {
            string values = string.Join(",", roleIds.Select(ul => $"({userId},{guildId},{ul})"));
            MySqlCommandNonQuery($"insert into {Table} (user,guild,role) values {values}");
        }
        internal void DeleteRole(ulong userId, ulong guildId, ulong roleId) => MySqlCommandNonQuery($"delete from {Table} where user={userId} and guild={guildId} and role={roleId}");
        internal void DeleteAllRoles(ulong userId, ulong guildId) => MySqlCommandNonQuery($"delete from {Table} where user={userId} and guild={guildId}");
        internal void DeleteRoles(ulong userId, ulong guildId, IEnumerable<ulong> roleIds)
        {
            string roles = string.Join(" or ", roleIds.Select(ul => $"role={ul}"));
            MySqlCommandNonQuery($"delete from {Table} where user={userId} and guild={guildId} and ({roles})");
        }
         

        internal void UpdateRoles(ulong userId, ulong guildId, IEnumerable<ulong> after)
        {
            //UPDATEするわけではない
            var before = GetRoles(userId, guildId);
            var addedRoles = after.Except(before);
            var removedRoles = before.Except(after);
            if (addedRoles.Any()) InsertRoles(userId, guildId, addedRoles);
            if (removedRoles.Any()) DeleteRoles(userId, guildId, removedRoles);
        }
        private void MySqlCommandNonQuery(string commandText)
        {
            using (var connection = new MySqlConnection(MySqlConnectionString))
            {
                using (var command = connection.CreateCommand())
                {
                    try
                    {
                        connection.Open();
                        command.CommandText = commandText;
                        Debug.Log($"SendCommand {command.CommandText}");
                        command.ExecuteNonQuery();
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
    }
}
