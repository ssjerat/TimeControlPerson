using System;
using System.Data.SqlClient;

namespace TimeControlPerson
{
    class SQLconnect
    {
        string connectionString =
            @"Data Source=.\SQLEXPRESS;"
            + "Integrated Security=true";

        public string GetQueryString(int IDrecord)
        {
            return 
            "select DT as Timestamp ,USR as PersonID from FB...FB_EVN as even "
                + "JOIN FB...FB_USR as users ON even.USR=users.ID "
                + $"where even.ID = '{IDrecord}' "
                + "ORDER BY DT";
        }  
        public int GetCountRecord()
        {
            string query = "select COUNT(id) from FB...FB_EVN";
            int count = 0;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        count = reader.GetInt32(0);
                    }
                    reader.Close();
                    LogsHandler.LogWriter(0, $"Получаем количество записей из бд: {count}");
                    return count;
                }
                catch (Exception ex) 
                {
                    LogsHandler.LogWriter((LogsHandler.Status)1, ex.Message);
                    return Program.CountRecords;
                }
            }         
        }
        public Person RunQuery(int IDrecord)
        {
            Person person = null;
            string queryString = GetQueryString(IDrecord);
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                      person = new Person(DateTime.Parse(reader[0].ToString()),reader[1].ToString());    
                    }
                    reader.Close();
                    LogsHandler.LogWriter(0, $"Запрос на получение записи {IDrecord} был обработан");
                    if (person != null)
                        LogsHandler.LogWriter(0, $"Найден ID сотрудника {person.ID} с явкой {person.DateStamp}");
                    else
                        LogsHandler.LogWriter((LogsHandler.Status)1, $"ID записи из бд неизвестен!");

                    return person;
                }
                catch (Exception ex) 
                {
                    LogsHandler.LogWriter((LogsHandler.Status)1, ex.Message);
                    throw;
                }
            }      
        }           
    }
}
