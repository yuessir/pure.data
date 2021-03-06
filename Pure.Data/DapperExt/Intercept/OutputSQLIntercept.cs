﻿using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Linq;

namespace Pure.Data
{
    public class OutputSQLIntercept : Singleton<OutputSQLIntercept>, IExecutingInterceptor
    {
        public void OnExecutingCommand(IDatabase database, IDbCommand cmd)
        {
            //
            if (database.Config.EnableDebug)
            {
                database.LastSQL = cmd.CommandText;
                database.LastArgs = cmd.Parameters;// (from IDataParameter parameter in cmd.Parameters select parameter.Value).ToArray();

                string connStr = "    (conn: " + database.Connection?.GetHashCode() + ", status: " + database.Connection?.State + ")";

                if (database.Config.LogWithRawSql == true) //输入RawSQL
                {
                     
                    string ParameterPrefix =  database.SqlGenerator.Configuration.Dialect.ParameterPrefix.ToString();
                    Dictionary<string, object> ps = new Dictionary<string, object>();
                    if (cmd.Parameters != null && cmd.Parameters.Count > 0)
                    {
                        StringBuilder sb = new StringBuilder();
                        foreach (IDataParameter p in cmd.Parameters)
                        {
                            ps.Add(p.ParameterName, p.Value);
                        }
                    }

                    string str = SqlMap.SqlMapStatement.ParseRawSQL(cmd.CommandText,  ps, database.SqlDialectProvider, false, ParameterPrefix, "");

                    database.LogHelper.Debug(str + connStr);

                }
                else
                {

                    StringBuilder sbText = new StringBuilder(cmd.CommandText);
                    if (cmd.Parameters != null && cmd.Parameters.Count > 0)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append("Output Parameters:");

                        foreach (IDataParameter p in cmd.Parameters)
                        {
                            sb.Append(p.ParameterName);
                            sb.Append("=");
                            sb.Append(p.Value);
                            sb.Append("; ");
                        }
                        sbText.AppendLine("");
                        sbText.AppendLine(sb.ToString());
                    }
                    database.LogHelper.Debug(sbText.ToString());
                }


                if (!database.Watch.IsRunning)
                {
                    database.Watch.Reset();
                    database.Watch.Start();
                }
            }

             
        }

        public void OnExecutedCommand(IDatabase database, IDbCommand cmd)
        {
            //
            if (database.Config.EnableDebug)
            {
                
                if (database.Watch.IsRunning)
                {
                    database.Watch.Stop();
                    
                }
                string connStr = "    (conn: " + database.Connection?.GetHashCode() + ", status: " + database.Connection?.State + ")";
                
                database.LogHelper.Debug("Elapsed:" + database.Watch.ElapsedMilliseconds +" ms"+ connStr);
                //database.Watch.Reset();

            }
        }
    }
}
