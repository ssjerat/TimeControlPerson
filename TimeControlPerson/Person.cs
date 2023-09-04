using System;
using System.Configuration;
using System.IO;

namespace TimeControlPerson
{
    class Person
    {
        public Person(DateTime date,string id) { DateStamp = date; ID = id; Unidentified = false; }
        public DateTime DateStamp { get; set; }
        public string ID { get; set; }
        public string FullName { get; set; }
        public string TableRow { get; set; }
        public bool Unidentified { get; set; }

        public static void ParseID(Person person)
        {
            if (person != null)
            {
                switch (person.ID)
                {
                    case "1658334019985":
                        person.FullName = ConfigurationManager.AppSettings["Person5"];
                        person.TableRow = "5";
                        break;
                    case "1658334533424":
                        person.FullName = ConfigurationManager.AppSettings["Person6"];
                        person.TableRow = "6";
                        break;
                    case "1658334785178":
                        person.FullName = ConfigurationManager.AppSettings["Person7"];
                        person.TableRow = "7";
                        break;
                    case "1658334851413":
                        person.FullName = ConfigurationManager.AppSettings["Person8"];
                        person.TableRow = "8";
                        break;
                    case "1658334909435":
                        person.FullName = ConfigurationManager.AppSettings["Person9"];
                        person.TableRow = "9";
                        break;
                    case "1658335005990":
                        person.FullName = ConfigurationManager.AppSettings["Person10"];
                        person.TableRow = "10";
                        break;
                    case "1658335053913":
                        person.FullName = ConfigurationManager.AppSettings["Person11"];
                        person.TableRow = "11";
                        break;
                    case "1658335092040":
                        person.FullName = ConfigurationManager.AppSettings["Person12"];
                        person.TableRow = "12";
                        break;
                    case "1658335726061":
                        person.FullName = ConfigurationManager.AppSettings["Person14"];
                        person.TableRow = "14";
                        break;
                    case "1658335132552":
                        person.FullName = ConfigurationManager.AppSettings["Person15"];
                        person.TableRow = "15";
                        break;
                    default: person.Unidentified = true; break;
                }

                LogsHandler.LogWriter(0, $"Запись с ID={person.ID} обработана");
                if (!person.Unidentified)
                {
                    LogsHandler.LogWriter(0, $"Сотрудник идентифицирован: {person.FullName}");
                    WriteRecordEmployee(person);
                }
                else
                    LogsHandler.LogWriter(0, $"Сотрудник неидентифицирован!");
            }
        }
        static void WriteRecordEmployee(Person person)
        {
            try
            {
                SQLconnect firebird = new SQLconnect();
                using (StreamWriter writer = new StreamWriter(Directory.GetCurrentDirectory() + @"\logs\attendance.txt", true))
                {
                    writer.WriteLineAsync(person.FullName + "\t" + person.DateStamp);
                }
            }
            catch (Exception ex)
            {
                LogsHandler.LogWriter((LogsHandler.Status)1, ex.Message);
            }
        }

    }
}
