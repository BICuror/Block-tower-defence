using System.Text;
using UnityEditor;
using System.IO;

namespace CuroLocalization
{    
    public static class TSVImport
    {
        public static bool CreateTableFromCSV(out Table table)
        {
            string[,] tableData;
            table = null;

            if (GiveCSVLines(out string content))
            {
                tableData = ParseCSVTextToDoubleArray(content);
            }
            else return false;

            table = new Table(tableData);
            return true;
        }

        private static bool GiveCSVLines(out string fileContent)
        {
            fileContent = null;
            
            string file = EditorUtility.OpenFilePanel("Select CSV file", "", "csv");
            
            if (!File.Exists(file)) return false;

            fileContent = File.ReadAllText(file);

            return true;
        }

        private static string[,] ParseCSVTextToDoubleArray(string text)
        {
            StringBuilder parsedTextStringBuilder = GetParsedTable(text);
            
            string[] lines = parsedTextStringBuilder.ToString().Split(LocalizationManager.Settings.TableLingSeparatorSymbol);

            string[][] parsedDoubleArray = new string[lines.Length][];
            
            for (int i = 0; i < lines.Length; i++)
            {
                var parsedData = lines[i].Split(LocalizationManager.Settings.TableCellSeparatorSymbol);
                
                parsedDoubleArray[i] = parsedData;
            }

            string[,] parsedTwoDimensionArray = new string[parsedDoubleArray[0].Length, parsedDoubleArray.Length];

            for (int i = 0; i < parsedDoubleArray.Length; i++)
            {
                for (int j = 0; j < parsedDoubleArray[i].Length; j++)
                {
                    parsedTwoDimensionArray[j, i] = parsedDoubleArray[i][j];
                }
            }

            return parsedTwoDimensionArray;
        }

        private static StringBuilder GetParsedTable(string text)
        {
            StringBuilder stringBuilder = new();

            bool insideBrackets = false;
            
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == '\"')
                {
                    if (insideBrackets)
                    {
                        if (i + 1 < text.Length && text[i + 1] == '\"')
                        {
                            stringBuilder.Append('\"');
                            i++;
                        }
                        else insideBrackets = false;
                    }
                    else insideBrackets = true;
                }
                else if (text[i] == ',')
                {
                    if (insideBrackets) stringBuilder.Append(',');
                    else stringBuilder.Append(LocalizationManager.Settings.TableCellSeparatorSymbol);
                }
                else if (text[i] == '\n')
                {
                    if (insideBrackets) stringBuilder.Append("\r\n");
                    else stringBuilder.Append(LocalizationManager.Settings.TableLingSeparatorSymbol);
                }
                else if (text[i] != '\r') stringBuilder.Append(text[i]);
            }
            
            return stringBuilder;
        }
    }
}