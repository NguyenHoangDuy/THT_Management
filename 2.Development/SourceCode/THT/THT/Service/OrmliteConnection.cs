using ServiceStack.OrmLite;
using ServiceStack.OrmLite.SqlServer;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace THT.Service
{
    public class OrmliteConnection
    {
        public IDbConnection openConn()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;            
            var dbFactory = new OrmLiteConnectionFactory(connectionString,SqlServerOrmLiteDialectProvider.Instance);
            IDbConnection dbConn = dbFactory.OpenDbConnection();
            OrmLiteConfig.DialectProvider.UseUnicode = true;
            return dbConn;
        }
        public class BasicConnection
        {
            public SqlConnection openConnect()
            {
                string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                SqlConnection dbConn = new SqlConnection(connectionString);
                dbConn.Open();
                return dbConn;
            }
        }
    }
}