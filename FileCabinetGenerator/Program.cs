using System;
using FileCabinetApp;
using System.IO;
using System.Collections.Generic;

namespace FileCabinetGenerator
{
    /// <summary>
    /// Generates csv or xml files with random records.
    /// Cmd arguments:
    /// --output-type - xml or csv
    /// --output - path to file
    /// --records-amount
    /// --start-id
    /// </summary>
    public static class Program
    {
        private const int gradesAmount = (int)'Z' - (int)'A';
        private const int minHeight = 100;
        private const int maxHeight = 220;

        private const int defaultRecordsAmount = 5000;
        private const int defaultStartId = 1;
        private const int defaultFileFormat = 1;

        private static string fileName = null;
        private static int recordsAmount = -1;
        private static int startId = -1;
        private static int chosenFileFormat = -1;
        private static List<string> firstNames = new List<string>();
        private static List<string> lastNames = new List<string>();

        private static Tuple<string, string, Action<string>>[] commandLineArguments = new Tuple<string, string, Action<string>>[]
        {
            new Tuple<string, string, Action<string>>("--output-type", "-t", SetOutputType),
            new Tuple<string, string, Action<string>>("--output", "-o", SetOutputFile),
            new Tuple<string, string, Action<string>>("--records-amount", "-a", SetAmountOfRecords),
            new Tuple<string, string, Action<string>>("--start-id", "-i", SetStartingId),
        };

        private static string[][] fileFormats = new string[][]
        {
            new string[]{"csv","RandomData.csv" },
            new string[]{"xml","RandomData.xml" },
        };

        private static Tuple<string, bool>[] yesNoStatements = new Tuple<string, bool>[]
        {
            new Tuple<string, bool>("y", true),
            new Tuple<string, bool>("n", false),
            new Tuple<string, bool>("yes", true),
            new Tuple<string, bool>("no", false),
        };

        /// <summary>
        /// Generates csv or xml files with random records.
        /// </summary>
        /// <param name="args">
        /// --output-type - xml or csv;
        /// --output - path to file;
        /// --records-amount;
        /// --start-id.
        /// </param>
        public static void Main(string[] args)
        {
            ProcessCommandLineArgs(args);
            if (!ValidateArguments())
            {
                return;
            }

            FillNamesList();

            List<FileCabinetRecord> recordsList = new List<FileCabinetRecord>();
            for (int id = startId; id < startId + recordsAmount; id++)
            {
                Random random = new Random();

                DateTime date = new DateTime(1950, 1, 1);
                int range = (DateTime.Today - date).Days;
                date = date.AddDays(random.Next(range));

                FileCabinetRecord record = new FileCabinetRecord()
                {
                    Id = id,
                    FirstName = firstNames[random.Next(firstNames.Count)],
                    LastName = lastNames[random.Next(lastNames.Count)],
                    DateOfBirth = date,
                    Salary = (decimal)(random.NextDouble() * random.Next(10000)),
                    Grade = (char)(random.Next(gradesAmount) + (int)'A'),
                    Height = (short)random.Next(minHeight, maxHeight),
                };
                recordsList.Add(record);
            }

            try
            {
                using (StreamWriter writer = new StreamWriter(fileName))
                {
                    FileCabinetServiceSnapshot snapshot = new FileCabinetServiceSnapshot(recordsList.ToArray());
                    switch (chosenFileFormat)
                    {
                        case 0:
                            snapshot.SaveToCSV(writer);
                            break;
                        case 1:
                            snapshot.SaveToXML(writer);
                            break;
                        default:
                            System.Console.WriteLine("Failed to create file");
                            return;
                    }
                }
            }
            catch (DirectoryNotFoundException)
            {
                System.Console.WriteLine("Failed to create file, no such directory");
            }
        }

        private static void FillNamesList()
        {
            using (StreamReader reader = new StreamReader("First.txt"))
            {
                while (reader.Peek() >= 0)
                {
                    firstNames.Add(reader.ReadLine());
                }
            }
            using (StreamReader reader = new StreamReader("Last.txt"))
            {
                while (reader.Peek() >= 0)
                {
                    lastNames.Add(reader.ReadLine());
                }
            }
        }

        private static bool ValidateArguments()
        {
            if (recordsAmount <= 0)
            {
                recordsAmount = defaultRecordsAmount;
                Console.WriteLine($"Invalid records amount, set to default = {defaultRecordsAmount}");
            }
            if (startId <= 0 || startId > int.MaxValue - recordsAmount)
            {
                startId = defaultStartId;
                Console.WriteLine($"Invalid start id, set to default = {defaultStartId}");
            }
            if (chosenFileFormat <= 0)
            {
                chosenFileFormat = defaultFileFormat;
                Console.WriteLine($"Invalid file format, set to default: {fileFormats[defaultFileFormat][0]}");
            }
            if (fileName is null)
            {
                fileName = fileFormats[chosenFileFormat][1];
                Console.WriteLine($"Invalid name of file, set to default: {fileFormats[defaultFileFormat][1]}");
            }

            if (!fileName.EndsWith(fileFormats[chosenFileFormat][0]))
            {
                fileName = fileFormats[chosenFileFormat][1];
                Console.WriteLine($"Name of file should end with .{fileFormats[chosenFileFormat][0]}, changed to default: {fileFormats[chosenFileFormat][1]}");
            }

            if (File.Exists(fileName))
            {
                int index;
                do
                {
                    System.Console.WriteLine($"File exists - rewrite {fileName} [Y/n]");
                    var input = Console.ReadLine();
                    index = Array.FindIndex(yesNoStatements, i => i.Item1.Equals(input, StringComparison.OrdinalIgnoreCase));
                }
                while (index < 0);

                if (!yesNoStatements[index].Item2)
                {
                    return false;
                }
            }

            return true;
        }

        private static void ProcessCommandLineArgs(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                int index;
                string functionArguments = string.Empty;
                if (args[i][1] == '-')
                {
                    var splitedFullArgument = args[i].Split('=', 2);
                    index = Array.FindIndex(commandLineArguments, n => n.Item1.Equals(splitedFullArgument[0], StringComparison.OrdinalIgnoreCase));
                    if (splitedFullArgument.Length >= 2)
                    {
                        functionArguments = splitedFullArgument[1];
                    }
                }
                else
                {
                    index = Array.FindIndex(commandLineArguments, n => n.Item2.Equals(args[i], StringComparison.OrdinalIgnoreCase));
                    if (args.Length > i + 1)
                    {
                        functionArguments = args[i + 1];
                    }

                    i++;
                }

                if (index >= 0 && !string.IsNullOrEmpty(functionArguments))
                {
                    commandLineArguments[index].Item3(functionArguments);
                }
            }
        }

        private static void SetOutputType(string fileFormat)
        {
            var index = Array.FindIndex(fileFormats, i => i[0].Equals(fileFormat, StringComparison.OrdinalIgnoreCase));
            if (index < 0)
            {
                System.Console.WriteLine($"{fileFormat} is not proper file format");
                return;
            }

            chosenFileFormat = index;
        }

        private static void SetOutputFile(string argumentFileName)
        {
            fileName = argumentFileName;
        }

        private static void SetAmountOfRecords(string argumentAmountOfRecords)
        {
            int parsedAmountOfRecords;
            if (int.TryParse(argumentAmountOfRecords, out parsedAmountOfRecords))
            {
                recordsAmount = parsedAmountOfRecords;
            }
        }

        private static void SetStartingId(string argumentStartingId)
        {
            int parsedStartingId;
            if (int.TryParse(argumentStartingId, out parsedStartingId))
            {
                startId = parsedStartingId;
            }
        }
    }
}
