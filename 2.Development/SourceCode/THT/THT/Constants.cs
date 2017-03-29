using System;
using System.Collections.Generic;
using System.Configuration;
using log4net;


namespace THT
{
    public class Constants
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(Constants));
        private static Constants m_Constants;

        #region Setting from App.config
        public string CONNECTION_STRING;
        public string CONNECTION_STRING1;
        public string CONNECTION_STRING2;
        public int CONNECTION_TIMEOUT;
        public int LOG_DB_CALL;

        public string RecaptchaPucblicKey;
        public string RecaptchaSecretKey;
        public string VDCAccount;
        public string VDCACpass;
        public string VDCusername;
        public string VDCpassword;
        #endregion

        public Constants()
        {
            logger.Debug("Get CONNECTION_STRING");
            CONNECTION_STRING = ConfigurationManager.AppSettings["connectionString"].ToString().Trim();
            logger.Debug("Get CONNECTION_TIMEOUT");
            CONNECTION_TIMEOUT = int.Parse(ConfigurationManager.AppSettings["connectionTimeout"].ToString().Trim());
            logger.Debug("Get LOG_DB_CALL");
            LOG_DB_CALL = int.Parse(ConfigurationManager.AppSettings["logDBCall"].ToString().Trim());


            RecaptchaPucblicKey = ConfigurationManager.AppSettings["RecaptchaPucblicKey"].ToString().Trim();
            RecaptchaSecretKey = ConfigurationManager.AppSettings["RecaptchaSecretKey"].ToString().Trim();
            try
            {
                VDCAccount = ConfigurationManager.AppSettings["VDCAccount"].ToString().Trim();
                VDCACpass = ConfigurationManager.AppSettings["VDCACpass"].ToString().Trim();
                VDCusername = ConfigurationManager.AppSettings["VDCusername"].ToString().Trim();
                VDCpassword = ConfigurationManager.AppSettings["VDCpassword"].ToString().Trim();
            }
            catch { }
        }

        public static Constants AllConstants()
        {
            if (m_Constants == null)
            {
                try
                {
                    logger.Debug("Begin initing constants");
                    m_Constants = new Constants();
                }
                catch (Exception ex)
                {
                    logger.Error(ex.Message);
                    System.Environment.Exit(1);
                }
            }
            return m_Constants;
        }

        public static void Refresh()
        {
            if (m_Constants != null)
            {
                logger.Debug("Begin refresh constants");
                m_Constants = null;
                Constants.AllConstants();
                logger.Debug("End refresh constants");
            }
        }
    }
}