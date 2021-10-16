using System;
using FileCabinetApp;
using System.IO;

namespace FileCabinetGenerator
{
    public static class Program
    {
        private static FileCabinetServiceSnapshot snapshot;
        private static StreamWriter writer;
        private static string fileName = null;
        private static int recordsAmount = -1;
        private static int startId = -1;
        private static int chosenFileFormat = -1;
        private static int defaultRecordsAmount = 100;
        private static int defaultStartId = 1;
        private static int defaultFileFormat = 1;

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


        static void Main(string[] args)
        {
            ProcessCommandLineArgs(args);
            if(!ValidateArguments())
            {
                return;
            }


        }

        private static bool ValidateArguments()
        {
            if (recordsAmount <= 0)
            {
                recordsAmount = defaultRecordsAmount;
                Console.WriteLine($"Invalid records amount, set to default = {defaultRecordsAmount}");
            }
            if (startId <= 0 && startId > int.MaxValue - recordsAmount)
            {
                startId = defaultStartId;
                Console.WriteLine($"Invalid start id, set to default = {defaultStartId}");
            }
            if (chosenFileFormat <= 0)
            {
                chosenFileFormat = defaultFileFormat;
                Console.WriteLine($"Invalid file format, set to default: {fileFormats[0][defaultFileFormat]}");
            }
            if (fileName is null)
            {
                fileName = fileFormats[1][chosenFileFormat];
                Console.WriteLine($"Invalid name of file, set to default: {fileFormats[1][chosenFileFormat]}");
            }

            if(!fileName.EndsWith(fileFormats[0][chosenFileFormat]))
            {
                fileName = fileFormats[1][chosenFileFormat];
                Console.WriteLine($"Name of file should end with .{fileFormats[0][chosenFileFormat]}, changed to default: {fileFormats[1][chosenFileFormat]}");
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

        private static void SetOutputType(string validationRules)
        {

        }

        private static void SetOutputFile(string validationRules)
        {

        }

        private static void SetAmountOfRecords(string validationRules)
        {

        }

        private static void SetStartingId(string validationRules)
        {

        }
    }
}
