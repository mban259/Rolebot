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
        public MySqlClient(string server, string database, string table, string userId, string password)
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

        public ulong[] GetRoles(ulong userId, ulong guildId)
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
                        result = dataTable.AsEnumerable().Select(x => (ulong)x["role"]).ToArray();
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
        public void InsertRole(ulong userId, ulong guildId, ulong roleId) => MySqlCommandNonQuery($"insert into {Table} (user,guild,role) values({userId},{guildId},{roleId})");
        public void InsertRoles(ulong userId, ulong guildId, IEnumerable<ulong> roleIds)
        {
            string values = string.Join(",", roleIds.Select(ul => $"({userId},{guildId},{ul})"));
            MySqlCommandNonQuery($"insert into {Table} (user,guild,role) values {values}");
        }
        public void DeleteRole(ulong userId, ulong guildId, ulong roleId) => MySqlCommandNonQuery($"delete from {Table} where user={userId} and guild={guildId} and role={roleId}");
        public void DeleteAllRoles(ulong userId, ulong guildId) => MySqlCommandNonQuery($"delete from {Table} where user={userId} and guild={guildId}");
        public void DeleteRoles(ulong userId, ulong guildId, IEnumerable<ulong> roleIds)
        {
            string roles = string.Join(" or ", roleIds.Select(ul => $"role={ul}"));
            MySqlCommandNonQuery($"delete from {Table} where user={userId} and guild={guildId} and ({roles})");
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
