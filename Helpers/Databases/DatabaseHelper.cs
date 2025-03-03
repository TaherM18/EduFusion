using System.Collections;
using System.Data;
using System.Linq.Expressions;
using Helpers.Extensions;
using Helpers.Logs;
using Npgsql;

namespace Helpers.Databases
{
    public class DatabaseHelper
    {
        private readonly NpgsqlConnection _con;

        public DatabaseHelper(NpgsqlConnection connection) => _con = connection;


        #region GetTableCustom
        /// <summary>
        /// Method <c>GetTableAll</c> gives all columns from table.
        /// </summary>
        public async Task<DataTable> GetTableCustom(string query, NpgsqlParameter[] parameters = null)
        {
            DataTable dt = new DataTable();
            try
            {
                await _con.CloseAsync();
                await _con.OpenAsync();
                
                using (NpgsqlCommand cm = new NpgsqlCommand(query, _con))
                {
                    if (parameters != null) cm.Parameters.AddRange(parameters);
                    NpgsqlDataReader reader = await cm.ExecuteReaderAsync();
                    dt.Load(reader);
                }
            }
            catch (Exception ex)
            {
                LogHelper.AppendLog("ERROR", ex.Message);
                throw new Exception("Exception from GetTableCustom :::> \n" + ex.Message + " <::::::>");
            }
            finally
            {
                await _con.CloseAsync();
            }
            return dt;
        }
        #endregion
        

        /// <summary>
        /// Method <c>GetTableAll</c> gives all columns from table.
        /// </summary>
        public async Task<DataTable> GetTableAll(string tablename, string[] array = null)
        {
            DataTable dt = new DataTable();
            try
            {
                string columns = (array == null || array.Length == 0) ? "*" : string.Join(", ", array);
                string query = $"SELECT {columns} FROM {tablename}";
                await _con.CloseAsync();
                await _con.OpenAsync();
                using (NpgsqlCommand cm = new NpgsqlCommand(query, _con))
                {
                    NpgsqlDataReader reader = await cm.ExecuteReaderAsync();
                    dt.Load(reader);
                }
            }
            catch (Exception ex)
            {
                LogHelper.AppendLog("ERROR", ex.Message);
                throw new Exception("Exception from GetTableAll :::> \n" + ex.Message + " <::::::>");
            }
            return dt;
        }

        /// <summary>
        /// Method <c>GetTableOne</c> gives all columns from a table according to the primary key.
        /// </summary>
        public async Task<DataTable> GetTableOne(string tablename, string pk, string pk_val, string[]? array = null)
        {
            DataTable dt = new DataTable();

            try
            {
                string columns = (array == null || array.Length == 0) ? "*" : string.Join(", ", array);

                string query = $"SELECT {columns} FROM {tablename} WHERE {pk} = @pk_val";

                await _con.CloseAsync();
                await _con.OpenAsync();

                using (NpgsqlCommand cm = new NpgsqlCommand(query, _con))
                {
                    cm.Parameters.AddWithValue($"pk_val", pk_val.ToInt());
                    using (NpgsqlDataReader reader = await cm.ExecuteReaderAsync())
                    {
                        dt.Load(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception from GetTableOne :::> \n" + ex.Message + " <::::::>");
            }

            return dt;
        }

        /// <summary>
        /// Method <c>GetTable</c> gives all columns from table by userid.
        /// </summary>
        public async Task<DataTable> GetTableUser(string tablename, string uid, string uid_val, string[] array = null)
        {
            DataTable dt = new DataTable();
            string columns = (array == null || array.Length == 0) ? "*" : string.Join(", ", array);
            string query = $"SELECT {columns} FROM {tablename} WHERE {uid} = @uid_val";
            await _con.CloseAsync();
            await _con.OpenAsync();
            using (NpgsqlCommand cm = new NpgsqlCommand(query, _con))
            {
                cm.Parameters.AddWithValue("uid_val", uid_val.ToInt());
                NpgsqlDataReader reader = await cm.ExecuteReaderAsync();
                dt.Load(reader);
            }
            return dt;
        }

        public async Task<DataTable> GetTableWithCondition(string tableName, Dictionary<string, object> conditions = null, string[] columns = null)
        {
            DataTable dt = new DataTable();
            try
            {
                // Determine selected columns
                string columnNames = (columns == null || columns.Length == 0) ? "*" : string.Join(", ", columns);

                // Build the base query
                string query = $"SELECT {columnNames} FROM {tableName}";

                List<NpgsqlParameter> parameters = new List<NpgsqlParameter>();

                // Add WHERE conditions dynamically
                if (conditions != null && conditions.Count > 0)
                {
                    var whereClauses = new List<string>();
                    foreach (var condition in conditions)
                    {
                        string paramName = $"@{condition.Key}";
                        whereClauses.Add($"{condition.Key} = {paramName}");
                        parameters.Add(new NpgsqlParameter(paramName, condition.Value));
                    }
                    query += " WHERE " + string.Join(" AND ", whereClauses);
                }

                Console.WriteLine($"Generated Query: {query}");

                await _con.CloseAsync();
                await _con.OpenAsync();

                using (NpgsqlCommand cm = new NpgsqlCommand(query, _con))
                {
                    if (parameters.Count > 0)
                        cm.Parameters.AddRange(parameters.ToArray());

                    using (NpgsqlDataReader reader = await cm.ExecuteReaderAsync())
                    {
                        dt.Load(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.AppendLog("ERROR in GetTableWithCondition", ex.Message);
                Console.WriteLine("DatabaseHelper - GetTableWithCondition() :\n"+ex.Message);
            }
            return dt;
        }

        #region InsertOne
        public async Task<int> InsertOne(string tableName, string[] colNames, List<object> values)
        {
            if (colNames.Length != values.Count)
            {
                throw new ArgumentException("Column names and values count must match.");
            }

            string columns = string.Join(", ", colNames);
            string placeholders = string.Join(", ", colNames.Select((_, i) => $"@p{i}"));

            string query = $"INSERT INTO {tableName} ({columns}) VALUES ({placeholders})";

            Console.WriteLine("DBHelper - InsertOne - query:\n" + query);
            Console.WriteLine("DBHelper - InsertOne - values:");

            try
            {
                await using var cmd = new NpgsqlCommand(query, _con);

                for (int i = 0; i < values.Count; i++)
                {
                    Console.Write(values[i] + ", ");
                    cmd.Parameters.AddWithValue($"@p{i}", values[i]);
                }
                Console.WriteLine();

                await _con.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
                return 1;
            }
            catch (Exception ex)
            {
                LogHelper.AppendLog("Error in InsertOne", ex.Message);
                Console.WriteLine("[ERROR] DatabaseHelper - InsertOne() :\n"+ex.Message);
                return 0;
            }
            finally
            {
                await _con.CloseAsync();
            }
        }
        #endregion

        #region InsertGetId
        public async Task<int> InsertGetId(string tableName, string[] colNames, List<object> values, string pk)
        {
            if (colNames.Length != values.Count)
            {
                throw new ArgumentException("Column names and values count must match.");
            }

            string columns = string.Join(", ", colNames);
            string placeholders = string.Join(", ", colNames.Select((_, i) => $"@p{i}"));

            string query = $"INSERT INTO {tableName} ({columns}) VALUES ({placeholders}) RETURNING {pk}";

            Console.WriteLine("DBHelper - InsertGetId - query:\n" + query);
            Console.WriteLine("DBHelper - InsertGetId - placeholders:\n" + placeholders);

            try
            {
                await using var cmd = new NpgsqlCommand(query, _con);

                for (int i = 0; i < values.Count; i++)
                {
                    cmd.Parameters.AddWithValue($"@p{i}", values[i]);
                }

                await _con.OpenAsync();
                object result = await cmd.ExecuteScalarAsync();

                return result != null && int.TryParse(result.ToString(), out int id) ? id : 0;
            }
            catch (Exception ex)
            {
                LogHelper.AppendLog("Error in InsertGetId", ex.ToString());
                Console.WriteLine("[ERROR] DatabaseHelper - InsertGetId() :\n"+ex.Message);
                return 0;
            }
            finally
            {
                await _con.CloseAsync();
            }
        }
        #endregion


        public async Task<int> UpdateOne(string tablename, string[] colnames, ArrayList values, string pk, string pk_val)
        {
            try
            {
                // Constructing the SET clause properly
                string q = string.Join(", ", colnames.Select(e => $"{e} = @{e}"));

                // Corrected SQL query
                string query = @$"
                UPDATE {tablename} 
                SET {q} 
                WHERE {pk} = @pk_val";

                Console.WriteLine("Query==> \n" + query);

                using (NpgsqlCommand cm = new NpgsqlCommand(query, _con))
                {
                    // Correctly add primary key parameter
                    cm.Parameters.AddWithValue("@pk_val", int.Parse(pk_val));

                    // Add column values as parameters
                    for (int i = 0; i < colnames.Length; i++)
                    {
                        object value = values[i] ?? DBNull.Value; // Handle null values properly
                        cm.Parameters.AddWithValue("@" + colnames[i], value);
                        Console.WriteLine($"Param {colnames[i]} ==> {value}");
                    }

                    await cm.ExecuteNonQueryAsync();
                    return 1;
                }
            }
            catch (Exception ex)
            {
                LogHelper.AppendLog("Error in UpdateOne", ex.Message);
                return 0;
            }
        }

    }
}