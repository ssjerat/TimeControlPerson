using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using System.Globalization;
using System.Configuration;

namespace TimeControlPerson
{
    class GoogleSheets
    {
        static string[] Scopes = { SheetsService.Scope.Spreadsheets };
        static string ApplicationName = "PersonTable";
        static string SpreadsheetId = ConfigurationManager.AppSettings["SpreadsheetId"];

        private static UserCredential GetCredential()
        {
            try
            {
                UserCredential credential;
                using (var stream =
                       new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
                {
                    string credPath = "token.json";
                    credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.FromStream(stream).Secrets,
                        Scopes,
                        "user",
                        CancellationToken.None,
                        new FileDataStore(credPath, true)).Result;
                }
                return credential;
            }
            catch (FileNotFoundException ex)
            {
                LogsHandler.LogWriter((LogsHandler.Status)1, ex.Message);
                throw;
            }
        }
        public void WriteRecord(Person person)
        {
            try
            {
                var service = new SheetsService(new BaseClientService.Initializer
                {
                    HttpClientInitializer = GetCredential(),
                    ApplicationName = ApplicationName
                });

                CultureInfo ci = new CultureInfo("ru-Ru");
                DateTimeFormatInfo dtfi = ci.DateTimeFormat;
                string nameSheet = dtfi.GetMonthName(person.DateStamp.Month) + " " + person.DateStamp.Year;
                string letterTable = GetLetterTableByDay(person.DateStamp);
                string tableCell = nameSheet + "!" + letterTable + person.TableRow;

                string range = tableCell;

                SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum valueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
                SpreadsheetsResource.ValuesResource.AppendRequest.InsertDataOptionEnum insertDataOption = SpreadsheetsResource.ValuesResource.AppendRequest.InsertDataOptionEnum.OVERWRITE;

                var items = new List<object>();
                var record = ReadDataBeforeWrite(tableCell);
                if (record == null)
                    record = person.DateStamp.ToString("HH:mm");
                else
                {
                    ClearDataBeforeWrite(tableCell);
                    record = record.ToString() + "\n" + person.DateStamp.ToString("HH:mm");
                }
                items.Add(record);

                var rangeData = new List<IList<object>> { items };

                var valueRange = new ValueRange
                {
                    Values = rangeData
                };

                SpreadsheetsResource.ValuesResource.AppendRequest request = service.Spreadsheets.Values.Append(valueRange, SpreadsheetId, range);
                request.ValueInputOption = valueInputOption;
                request.InsertDataOption = insertDataOption;

                AppendValuesResponse response = request.Execute();

                LogsHandler.LogWriter(0, $"В таблицу googleSheets в ячейку {tableCell} была добавлена новая информация: \n{record}");
            }
            catch (Exception ex)
            {
                LogsHandler.LogWriter((LogsHandler.Status)1, ex.Message);
            }
        }
        private object ReadDataBeforeWrite(string tableCell)
        {
            try
            {
                var service = new SheetsService(new BaseClientService.Initializer
                {
                    HttpClientInitializer = GetCredential(),
                    ApplicationName = ApplicationName
                });

                string range = tableCell;

                SpreadsheetsResource.ValuesResource.GetRequest request =
                        service.Spreadsheets.Values.Get(SpreadsheetId, range);

                ValueRange response = request.Execute();
                IList<IList<Object>> values = response.Values;
                object currentRecord = null;
                if (values == null || values.Count == 0)
                {
                    return currentRecord;
                }
                foreach (var row in values)
                {
                    currentRecord = row[0];
                }
                if (currentRecord != null)
                    LogsHandler.LogWriter(0, $"До попытки записи ячейка {tableCell} имеет следующие данные: {currentRecord}");
                else
                    LogsHandler.LogWriter(0, $"До попытки записи ячейка {tableCell} пуста");
                return currentRecord;
            }
            catch (Exception ex)
            {
                LogsHandler.LogWriter((LogsHandler.Status)1, ex.Message); 
                return null;
            }
            
        }
        private void ClearDataBeforeWrite(string tableCell)
        {
            try
            {
                var service = new SheetsService(new BaseClientService.Initializer
                {
                    HttpClientInitializer = GetCredential(),
                    ApplicationName = ApplicationName
                });

                string range = tableCell;

                ClearValuesRequest requestBody = new ClearValuesRequest();

                SpreadsheetsResource.ValuesResource.ClearRequest request = service.Spreadsheets.Values.Clear(requestBody, SpreadsheetId, range);

                ClearValuesResponse response = request.Execute();

                LogsHandler.LogWriter(0, $"Перед записью новыми данными ячейка {tableCell} была очищенна");
            }
            catch (Exception ex)
            {
                LogsHandler.LogWriter((LogsHandler.Status)1, ex.Message);
            }     
        }       
        private string GetLetterTableByDay(DateTime day)
        {
            switch (day.Day)
            {
                case 1: return "D";
                case 2: return "H";
                case 3: return "L";
                case 4: return "P";
                case 5: return "T";
                case 6: return "X";
                case 7: return "AB";
                case 8: return "AF";
                case 9: return "AJ";
                case 10: return "AN";
                case 11: return "AR";
                case 12: return "AV";
                case 13: return "AZ";
                case 14: return "BD";
                case 15: return "BH";
                case 16: return "BL";
                case 17: return "BP";
                case 18: return "BT";
                case 19: return "BX";
                case 20: return "CB";
                case 21: return "CF";
                case 22: return "CJ";
                case 23: return "CN";
                case 24: return "CR";
                case 25: return "CV";
                case 26: return "CZ";
                case 27: return "DD";
                case 28: return "DH";
                case 29: return "DL";
                case 30: return "DP";
                case 31: return "DT";
                default: return "";
            }
        }
    }
}
