using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCApp
{
    public class TestCleanup
    {
        public static void Init()
        {
            //TEST PURPOSES - Startup Cleanup
            var connInfo = @"Data Source=(LocalDB)\v11.0;AttachDbFilename=D:\GitHub\ImplicitLock\MVCApp\App_Data\TestDB.mdf;Integrated Security=True;Connect Timeout=30";
            var connection = new System.Data.SqlClient.SqlConnection(connInfo);
            var query = "DELETE FROM Locks";
            try
            {
                connection.Open();

                var command = new System.Data.SqlClient.SqlCommand(query, connection);
                command.ExecuteNonQuery();
            }
            finally
            {
                connection.Close();
            }
        }
    }
}