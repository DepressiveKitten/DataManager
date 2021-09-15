using System;
using System.Globalization;

namespace FileCabinetApp
{
    public static class Program
    {
        private const string DeveloperName = "Dmitriy Lopatin";
        private const string HintMessage = "Enter your command, or enter 'help' to get help.";
        private const string OutputDateFormat = "yyyy-MMM-d";
        private const int CommandHelpIndex = 0;
        private const int DescriptionHelpIndex = 1;
        private const int ExplanationHelpIndex = 2;

        private static bool isRunning = true;

        private static Tuple<string, Action<string>>[] commands = new Tuple<string, Action<string>>[]
        {
            new Tuple<string, Action<string>>("help", PrintHelp),
            new Tuple<string, Action<string>>("exit", Exit),
            new Tuple<string, Action<string>>("stat", Stat),
            new Tuple<string, Action<string>>("create", Create),
            new Tuple<string, Action<string>>("list", List),
        };

        private static string[][] helpMessages = new string[][]
        {
            new string[] { "help", "prints the help screen", "The 'help' command prints the help screen." },
            new string[] { "exit", "exits the application", "The 'exit' command exits the application." },
            new string[] { "stat", "shows statistics about service", "The 'stat' command shows statistics about service." },
            new string[] { "create", "create a new record", "The 'create' command create a new record." },
            new string[] { "list", "show all records", "The 'list' command show all records." },
        };

        private static FileCabinetService fileCabinetService;

        public static void Main(string[] args)
        {
            fileCabinetService = new FileCabinetService();
            Console.WriteLine($"File Cabinet Application, developed by {Program.DeveloperName}");
            Console.WriteLine(Program.HintMessage);
            Console.WriteLine();

            do
            {
                Console.Write("> ");
                var inputs = Console.ReadLine().Split(' ', 2);
                const int commandIndex = 0;
                var command = inputs[commandIndex];

                if (string.IsNullOrEmpty(command))
                {
                    Console.WriteLine(Program.HintMessage);
                    continue;
                }

                var index = Array.FindIndex(commands, 0, commands.Length, i => i.Item1.Equals(command, StringComparison.OrdinalIgnoreCase));
                if (index >= 0)
                {
                    const int parametersIndex = 1;
                    var parameters = inputs.Length > 1 ? inputs[parametersIndex] : string.Empty;
                    commands[index].Item2(parameters);
                }
                else
                {
                    PrintMissedCommandInfo(command);
                }
            }
            while (isRunning);
        }

        private static void PrintMissedCommandInfo(string command)
        {
            Console.WriteLine($"There is no '{command}' command.");
            Console.WriteLine();
        }

        private static void PrintHelp(string parameters)
        {
            if (!string.IsNullOrEmpty(parameters))
            {
                var index = Array.FindIndex(helpMessages, 0, helpMessages.Length, i => string.Equals(i[Program.CommandHelpIndex], parameters, StringComparison.OrdinalIgnoreCase));
                if (index >= 0)
                {
                    Console.WriteLine(helpMessages[index][Program.ExplanationHelpIndex]);
                }
                else
                {
                    Console.WriteLine($"There is no explanation for '{parameters}' command.");
                }
            }
            else
            {
                Console.WriteLine("Available commands:");

                foreach (var helpMessage in helpMessages)
                {
                    Console.WriteLine("\t{0}\t- {1}", helpMessage[Program.CommandHelpIndex], helpMessage[Program.DescriptionHelpIndex]);
                }
            }

            Console.WriteLine();
        }

        private static void Stat(string parameters)
        {
            var recordsCount = Program.fileCabinetService.GetStat();
            Console.WriteLine($"{recordsCount} record(s).");
        }

        private static void Create(string parameters)
        {
            Console.Write("First name: ");
            string firstName = Console.ReadLine();
            Console.Write("Last name: ");
            string lastName = Console.ReadLine();
            Console.Write("Date of birth: ");
            var date = Console.ReadLine().Split('/');
            Console.Write("Height: ");
            var height = Console.ReadLine();
            Console.Write("Salary: ");
            string salary = Console.ReadLine();
            Console.Write("Grade: ");
            var grade = Console.ReadLine();
            int day, month, year;
            short parsedHeight;
            decimal parsedSalary;
            if (!decimal.TryParse(salary, out parsedSalary))
            {
                Console.Write("input valid salary");
            }

            if (!short.TryParse(height, out parsedHeight))
            {
                Console.Write("input valid height");
            }

            if (grade.Length > 1)
            {
                Console.Write("Grade should contain one letter");
            }

            if (date.Length >= 3 && int.TryParse(date[0], out month) && int.TryParse(date[1], out day) && int.TryParse(date[2], out year))
            {
                fileCabinetService.CreateRecord(firstName, lastName, new DateTime(year, month, day), parsedHeight, parsedSalary, grade[0]);
                Console.WriteLine("Record #{0} is created.", Program.fileCabinetService.GetStat());
            }
            else
            {
                Console.Write("Date format should be mm/dd/yyyy");
            }
        }

        private static void List(string parameters)
        {
            var list = fileCabinetService.GetRecords();
            if (list.Length == 0)
            {
                Console.WriteLine("No records yet");
            }
            else
            {
                int i = 0;
                foreach (var record in list)
                {
                    i++;
                    Console.Write("#{0}, {1}, {2}, {3},", i, record.FirstName, record.LastName, record.DateOfBirth.ToString(OutputDateFormat, DateTimeFormatInfo.InvariantInfo));
                    Console.WriteLine(" Salary: {0:F3}, Height: {1}, Grade: {2}", record.Salary, record.Height, record.Grade);
                }
            }

            Console.WriteLine();
        }

        private static void Exit(string parameters)
        {
            Console.WriteLine("Exiting an application...");
            isRunning = false;
        }
    }
}