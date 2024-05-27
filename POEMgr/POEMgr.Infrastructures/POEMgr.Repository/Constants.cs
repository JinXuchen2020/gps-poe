using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace POEMgr.Repository
{
    public static class Constants
    {
        public const string GetUserPartnersSqlString = @"select distinct
                pa.Id PartnerId,
                pa.PartnerName, us.Id, us.Mail PartnerEmail,us.IsDisabled,ro.RoleName RoleName 
                from [dbo].[Sys_User] us join [dbo].[Sys_UserRole] ur on us.Id = ur.UserId 
                join [dbo].[Sys_Role] ro on ur.RoleId = ro.Id 
                join [dbo].[Poe_Partner] pa on pa.Mail like CONCAT('%', us.Mail,'%') 
                where ur.IsDeleted = 0";

        private static DbCommand CreateCommand(DatabaseFacade facade, string sql, out DbConnection connection, params object[] parameters)
        {
            var conn = facade.GetDbConnection();
            connection = conn;
            conn.Open();
            var cmd = conn.CreateCommand();
            if (facade.IsSqlServer())
            {
                cmd.CommandText = sql;
                cmd.Parameters.AddRange(parameters);
            }
            return cmd;
        }

        public static DataTable SqlQuery(this DatabaseFacade facade, string sql, params object[] parameters)
        {
            using var command = CreateCommand(facade, sql, out DbConnection conn, parameters);
            var reader = command.ExecuteReader();
            var dt = new DataTable();
            dt.Load(reader);
            reader.Close();
            conn.Close();
            return dt;
        }

        public static List<T> SqlQuery<T>(this DatabaseFacade facade, string sql, params object[] parameters) where T : class, new()
        {
            var dt = SqlQuery(facade, sql, parameters);
            return dt.ToList<T>();
        }

        public static List<T> ToList<T>(this DataTable dt) where T : class, new()
        {
            var propertyInfos = typeof(T).GetProperties();
            var list = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                var t = new T();
                foreach (PropertyInfo p in propertyInfos)
                {
                    if (dt.Columns.IndexOf(p.Name) != -1 && row[p.Name] != DBNull.Value)
                        p.SetValue(t, row[p.Name], null);
                }
                list.Add(t);
            }
            return list;
        }
    }
}
