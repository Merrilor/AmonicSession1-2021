using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Session1
{
    public static class DatabaseHelper
    {

        public static DateTime GetServerTime()
        {
            Session1Entities entities = new Session1Entities();
            entities.Database.Connection.Open();
            var command = entities.Database.Connection.CreateCommand();
            command.CommandText = "Select SYSUTCDATETIME()";
            DateTime ServerTime = (DateTime)command.ExecuteScalar();
            entities.Database.Connection.Close();

            return ServerTime;
        }

    }
}
