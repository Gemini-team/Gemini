using UnityEngine;
using System.IO;
using System;

namespace Gemini.EMRS.ScenarioGenerator
{
    public class ReadCSV
    {

        public static T[] readColumnData<T>(string columnName, string dataPath) where T : IConvertible
        {
            string[] data = readCSVColumn(columnName, dataPath);
            T[] dataFloat = new T[data.Length - 1];
            var VarType = typeof(T);

            for (int i = 1; i < data.Length; i++)
            {
                var elem = data[i];
                if(VarType != typeof(string))
                {
                    // for systems with decimal notation ','
                    //elem = elem.Replace(".", ",");
                }
                dataFloat[i - 1] = (T)Convert.ChangeType(elem, typeof(T));
            }
            return dataFloat;
        }

        private static string[] readCSVColumn(string columnName, string dataPath)
        {
            string fileData = File.ReadAllText(dataPath);
            var lines = fileData.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            int index = ColumnIndex(columnName, lines[0]);
            // Remove garbage at end of CSV file
            string[] column = new string[lines.Length - 1];

            for (int i = 0; i < lines.Length - 1; i++)
            {
                var line = lines[i];
                column[i] = line.Split(',')[index];
            }
            return column;
        }

        private static int ColumnIndex(string columnName, string firstRow)
        {
            var firstRowElements = firstRow.Split(',');
            for (int i = 0; i < firstRowElements.Length; i++)
            {
                if (firstRowElements[i] == columnName)
                {
                    return i;
                }
            }
            throw new Exception(columnName + ", was not found as a column name");
        }
    }
}