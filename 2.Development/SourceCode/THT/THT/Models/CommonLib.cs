using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using ServiceStack.OrmLite;
using ServiceStack.OrmLite.SqlServer;
using THT.Service;
using System.Web.Mvc;

namespace THT.Models
{
    public class CommonLib
    {
        public List<ActiveStatus> GetActiveStatus()
        {
            IDbConnection dbConn = new OrmliteConnection().openConn();
            var list = new List<ActiveStatus>();
            try
            {
                list = dbConn.Select<ActiveStatus>("SELECT StatusValue, StatusName,IsAllMerchant,IsAllDistric FROM vw_List_Active_Status");                
            }
            catch (Exception)
            {                
                throw;
            }
            finally { dbConn.Close(); }
            return list;
        }
    }

    public class ActiveStatus
    {
        public bool StatusValue { get; set; }
        public string StatusName { get; set; }
        public string IsAllMerchant { get; set; }
        public string IsAllDistric { get; set; }
    }
   
}