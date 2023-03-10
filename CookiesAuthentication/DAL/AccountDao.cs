using System.Data.SqlClient;

namespace CookiesAuthentication.DAL
{
    public class AccountDao
    {
        private readonly IConfiguration _configuration; 
        private readonly string _checkIfMatchQuery = "SELECT COUNT(*) FROM userInfo WHERE userName = @username AND password = @password";
        public AccountDao(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetPermissionByUserName(string userName)
        {
            string result = "";
            using (var conn = new SqlConnection(_configuration.GetConnectionString("MyConn")))
            {
                conn.Open();
                var cmd = new SqlCommand($"select permission from userInfo where username = '{userName}'", conn);
                result = cmd.ExecuteScalar().ToString();
            }
            return result;
        }

        public bool AuthenticationCheck(string username, string password)
        {
            bool result = false;
            using (var conn = new SqlConnection(_configuration.GetConnectionString("MyConn")))
            {
                conn.Open();
                var cmd = new SqlCommand(_checkIfMatchQuery, conn);
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@password", password);
                //varify user credential
                var count = Convert.ToInt32(cmd.ExecuteScalar());
                if(count > 0)
                    result = true;
            }
            return result;
        }
    }
    //public bool AuthorizationCheck(string username, string password string )
    //{
    //    bool result = false;
    //    using (var conn = new SqlConnection(_configuration.GetConnectionString("MyConn")))
    //    {
    //        conn.Open();
    //        var cmd = new SqlCommand(_checkIfMatchQuery, conn);
    //        cmd.Parameters.AddWithValue("@username", username);
    //        cmd.Parameters.AddWithValue("@password", password);
    //        //varify user credential
    //        var count = Convert.ToInt32(cmd.ExecuteScalar());
    //        if (count > 0)
    //            result = true;
    //    }
    //    return result;
    //}
}
