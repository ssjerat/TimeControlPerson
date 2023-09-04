using System;
using System.IO;

namespace TimeControlPerson
{
    class Program
    {
        private static int currentCountRecords;
        public static int CountRecords { get; private set; }
        static void Main(string[] args)
        {
            try
            {
                SQLconnect firebird = new SQLconnect();
                ReadCountRecordTxt();
                currentCountRecords = firebird.GetCountRecord();
                while (CountRecords < currentCountRecords)
                {
                    Person person = firebird.RunQuery(++CountRecords);
                    if (person == null || person.Unidentified) continue;
                    Person.ParseID(person);
                    GoogleSheets googleSheets = new GoogleSheets();
                    googleSheets.WriteRecord(person);
                }
                WriteCountRecordTxt();
                LogsHandler.LogWriter(0, $"Все новые записи из бд обработаны");
            }
            catch (Exception ex)
            {
                LogsHandler.LogWriter((LogsHandler.Status)1, ex.Message);
            }
        }

        static void WriteCountRecordTxt()
        {
            try
            {
                SQLconnect firebird = new SQLconnect();
                using (StreamWriter writer = new StreamWriter(Directory.GetCurrentDirectory() + @"\save\lastRecord.txt", false))
                {
                    writer.WriteLineAsync(currentCountRecords.ToString());
                }
                LogsHandler.LogWriter(0, $"Запись в {Directory.GetCurrentDirectory()}\\save\\lastRecord.txt порядкового номера последней записи завершена: {currentCountRecords}");
            }
            catch (Exception ex)
            {
                LogsHandler.LogWriter((LogsHandler.Status)1, ex.Message);
            }      
        }

        static void ReadCountRecordTxt()
        {
            try
            {
                using (StreamReader reader = new StreamReader(Directory.GetCurrentDirectory() + @"\save\lastRecord.txt"))
                {
                    CountRecords = Int32.Parse(reader.ReadToEnd());                    
                }
                LogsHandler.LogWriter(0, $"Считывания из {Directory.GetCurrentDirectory()}\\save\\lastRecord.txt порядкового номера последней записи завершена: {CountRecords}");
            }
            catch (Exception ex)
            {
                LogsHandler.LogWriter((LogsHandler.Status)1, ex.Message);
            }        
        }
    }
}
