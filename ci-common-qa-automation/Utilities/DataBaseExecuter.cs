using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using Snowflake.Data.Client;

namespace ci_common_qa_automation.Utilities
{
    public static class DataBaseExecuter
    {
        //Set Defaults for DataExecuter timeouts and MaxResultsAccepted on the Return Limits
        private static int QueryTimeOut = 300;
        private static int MaxResultsAccepted = 1000;

        public static void OverrideParameters(int queryTimeout, int maxResultsAccepted)
        {
            QueryTimeOut = queryTimeout;
            MaxResultsAccepted = maxResultsAccepted;
        }
        public static DataTable ExecuteCommandWithReturnLimits(string connectionType, string connectionString, string command)
        {
            if (string.IsNullOrEmpty(command)) return new DataTable();
            Dictionary<string, object> parms = new();
            switch (connectionType.ToLower())
            {
                case "snowflake":
                    return ExecuteSnowflakeCommandWithReturnLimits(connectionString, command, parms);
                default:
                    return ExecuteCommand(connectionString, command, parms);
            }
        }
        public static DataTable ExecuteCommand(string connectionType, string connectionString, string command)
        {
            if (string.IsNullOrEmpty(command)) return new DataTable();
            Dictionary<string, object> parms = new();
            switch (connectionType.ToLower())
            {
                case "snowflake":
                    return ExecuteSnowflakeCommand(connectionString, command, parms);
                default:
                    return ExecuteCommand(connectionString, command, parms);
            }
        }
        public static DataTable ExecuteCommandWithReturnLimits(string connectionType, string connectionString, string command, Dictionary<string, object> parameters)
        {
            if (string.IsNullOrEmpty(command)) return new DataTable();
            switch (connectionType.ToLower())
            {
                case "snowflake":
                    return ExecuteSnowflakeCommandWithReturnLimits(connectionString, command, parameters);
                default:
                    return ExecuteCommand(connectionString, command, parameters);
            }
        }
        public static DataTable ExecuteCommand(string connectionType, string connectionString, string command, Dictionary<string, object> parameters)
        {
            if (string.IsNullOrEmpty(command)) return new DataTable();
            switch (connectionType.ToLower())
            {
                case "snowflake":
                    return ExecuteSnowflakeCommand(connectionString, command, parameters);
                default:
                    return ExecuteCommand(connectionString, command, parameters);
            }
        }
        private static DataTable ExecuteCommand(string connectionString, string command, Dictionary<string, object> parameters)
        {
            DataTable dt = new();
            using SqlConnection sqlConnection = new(connectionString);
            sqlConnection.Open();
            using (SqlCommand sqlCommand = new(command, sqlConnection))
            {
                sqlCommand.CommandTimeout = QueryTimeOut;
                foreach (string key in parameters.Keys)
                {
                    sqlCommand.Parameters.AddWithValue(key, parameters[key]);
                }
                using (SqlDataAdapter sqlAdapter = new())
                {
                    sqlAdapter.SelectCommand = sqlCommand;
                    sqlAdapter.Fill(dt);
                }
            }
            sqlConnection.Close();
            return dt;
        }
        private static DataTable ExecuteSnowflakeCommand(string connectionString, string command, Dictionary<string, object> parameters)
        {
            DataTable dt = new();
            using (SnowflakeDbConnection sfConnection = new())
            {
                sfConnection.ConnectionString = connectionString;
                sfConnection.Open();
                using (DbCommand sfCommand = sfConnection.CreateCommand())
                {
                    sfCommand.CommandTimeout = QueryTimeOut;
                    sfCommand.CommandText = command;
                    foreach (string key in parameters.Keys)
                    {
                        DbParameter parm = sfCommand.CreateParameter();
                        parm.ParameterName = key;
                        parm.Value = parameters[key];
                        sfCommand.Parameters.Add(parm);
                    }

                    DbDataReader result = sfCommand.ExecuteReader();
                    dt.Load(result);
                }
                sfConnection.Close();
            }
            return dt;
        }
        private static DataTable ExecuteSnowflakeCommandWithReturnLimits(string connectionString, string command, Dictionary<string, object> parameters)
        {
            DataTable dt = new();
            DataTable dtLarge = new();
            dtLarge.Columns.Add("Result");
            DataRow rowResult = dtLarge.NewRow();
            rowResult["Result"] = "Result Set is TOO LARGE with over " + MaxResultsAccepted + ".  Please run the query manually to verify the results";
            dtLarge.Rows.Add(rowResult);
            using SnowflakeDbConnection sfConnection = new();
            sfConnection.ConnectionString = connectionString;
            sfConnection.Open();
            using (DbCommand sfCommand = sfConnection.CreateCommand())
            {
                sfCommand.CommandTimeout = QueryTimeOut;
                sfCommand.CommandText = command;
                foreach (string key in parameters.Keys)
                {
                    DbParameter parm = sfCommand.CreateParameter();
                    parm.ParameterName = key;
                    parm.Value = parameters[key];
                    sfCommand.Parameters.Add(parm);
                }

                DbDataReader result = sfCommand.ExecuteReader();
                //dt.Load(result);
                DataTable? dtSchema = result.GetSchemaTable();
                // You can also use an ArrayList instead of List<> 
                List<DataColumn> listCols = new();
                if (dtSchema != null)
                {
                    foreach (DataRow drow in dtSchema.Rows)
                    {
                        string? columnName = Convert.ToString(drow["ColumnName"]);
                        DataColumn column = new(columnName, (Type)(drow["DataType"]));
                        listCols.Add(column);
                        dt.Columns.Add(column);
                    }

                }

                // Read rows from DataReader and populate the DataTable 

                int rowCount = 0;
                while (result.Read())
                {
                    DataRow dataRow = dt.NewRow();
                    for (int i = 0; i < listCols.Count; i++)
                    {
                        dataRow[(listCols[i])] = result[i];
                    }
                    dt.Rows.Add(dataRow);
                    rowCount++;
                    if (rowCount <= MaxResultsAccepted) continue;
                    dt = dtLarge;
                    break;
                }
            }
            sfConnection.Close();
            return dt;
        }
    }
}
