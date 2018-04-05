using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace WindowsFormsApplication1
{
    public class MyConn
    {
        String server = "50.62.209.151";
        String username = "colossusadmin";
        String password = "ciTc298^";
        String database = "gssbca_colossus";
        String connectionString;
        MySqlConnection con = null;

        public MyConn()
        {
            connectionString = "SERVER=" + server + ";" + "DATABASE=" + database + ";" + "UID=" + username + ";" + "PASSWORD=" + password;
            con = new MySqlConnection(connectionString);
        }

        public MySqlConnection CON
        {
            get
            {
                return con;
            }
        }
    }
}
