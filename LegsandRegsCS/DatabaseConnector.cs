using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using LegsandRegsCS.Special_Data_Types;

namespace LegsandRegsCS
{
    public class DatabaseConnector
    {
        private static SqlDataReader queryDb(string sql, string[] parameters = null)
        {
            try
            {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
                builder.DataSource = "maroun-tc-test-01.database.windows.net";
                builder.UserID = "marounl";
                builder.Password = "G^mT0+Odd*1I65Y9vxJL";
                builder.InitialCatalog = "Legs_and_regs_test";

                SqlConnection connection = new SqlConnection(builder.ConnectionString);

                SqlCommand command = new SqlCommand(sql, connection);

                if (parameters != null)
                {
                    for(int i = 0; i < parameters.Length; i++)
                    {
                        command.Parameters.Add("@value" + (i + 1).ToString(), System.Data.SqlDbType.VarChar).Value = parameters[i];
                    }
                    
                }

                connection.Open();
                return command.ExecuteReader();
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
                return null;
            }
        }
        public static List<Act> getActs()
        {
            SqlDataReader reader = queryDb("SELECT * FROM Acts;");

            List<Act> list = new List<Act>();

            while (reader.Read())
            {
                Console.WriteLine(reader.GetString(0) + " | " + reader.GetString(1) + " | " + reader.GetString(2) + " | " + reader.GetString(3) + " | " + reader.GetDateTime(4).ToString("yyyy-MM-dd"));
                list.Add(new Act(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetDateTime(4).ToString("yyyy-MM-dd")));
            }
            return list;
        }

        public static Act getActById(string uniqueId, string lang)
        {
            String[] arguments = { uniqueId, lang };
            SqlDataReader reader = queryDb("SELECT * FROM Acts WHERE uniqueId = @value1 AND lang = @value2;", arguments);

            List<Act> list = new List<Act>();

            reader.Read();

            return new Act(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetDateTime(4).ToString("yyyy-MM-dd"));
        }
    }
}
