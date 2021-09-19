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
        private const int ParametersIndex = 1;
        private const int CommandIndex = 0;
        private static readonly DateTime MinDate = new DateTime(1950, 1, 1);
        private static FileCabinetService fileCabinetService = new FileCabinetService();

        private static bool isRunning = true;

        private static Tuple<string, Func<string, FileCabinetRecord[]>>[] findOptions = new Tuple<string, Func<string, FileCabinetRecord[]>>[]
        {
            new Tuple<string, Func<string, FileCabinetRecord[]>>("firstname", fileCabinetService.FindByFirstName),
            new Tuple<string, Func<string, FileCabinetRecord[]>>("lastname", fileCabinetService.FindByLastName),
        };

        private static Tuple<string, Action<string>>[] commands = new Tuple<string, Action<string>>[]
        {
            new Tuple<string, Action<string>>("help", PrintHelp),
            new Tuple<string, Action<string>>("exit", Exit),
            new Tuple<string, Action<string>>("stat", Stat),
            new Tuple<string, Action<string>>("create", Create),
            new Tuple<string, Action<string>>("edit", Edit),
            new Tuple<string, Action<string>>("list", List),
            new Tuple<string, Action<string>>("find", Find),
        };

        private static string[][] helpMessages = new string[][]
        {
            new string[] { "help", "prints the help screen", "The 'help' command prints the help screen." },
            new string[] { "exit", "exits the application", "The 'exit' command exits the application." },
            new string[] { "stat", "shows statistics about service", "The 'stat' command shows statistics about service." },
            new string[] { "create", "create a new record", "The 'create' command create a new record." },
            new string[] { "edit", "edit existing record", "The 'edit' edit existing record, should contain wanted Id." },
            new string[] { "list", "show all records", "The 'list' command show all records." },
            new string[] { "find", "find record by parameters", "Type parametr you want to search for after 'find' command." },
        };

        public static void Main(string[] args)
        {
            Console.WriteLine($"File Cabinet Application, developed by {Program.DeveloperName}");
            Console.WriteLine(Program.HintMessage);
            Console.WriteLine();

            do
            {
                Console.Write("> ");
                var inputs = Console.ReadLine().Split(' ', 2);
                var command = inputs[CommandIndex];

                if (string.IsNullOrEmpty(command))
                {
                    Console.WriteLine(Program.HintMessage);
                    continue;
                }

                var index = Array.FindIndex(commands, 0, commands.Length, i => i.Item1.Equals(command, StringComparison.OrdinalIgnoreCase));
                if (index >= 0)
                {
                    var parameters = inputs.Length > 1 ? inputs[ParametersIndex] : string.Empty;
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
            Console.WriteLine();
        }

        private static bool InputName(out string name)
        {
            name = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(name) || name.Length < 2 || name.Length > 60)
            {
                return false;
            }

            return true;
        }

        private static bool InputDate(out DateTime dateOfBirth)
        {
            var date = Console.ReadLine().Split('/');
            int day, month, year;
            if (date.Length >= 3 && int.TryParse(date[0], out month) && int.TryParse(date[1], out day) && int.TryParse(date[2], out year))
            {
                try
                {
                    dateOfBirth = new DateTime(year, month, day);
                }
                catch (ArgumentOutOfRangeException)
                {
                    dateOfBirth = DateTime.MinValue;
                    return false;
                }

                if (dateOfBirth < MinDate || dateOfBirth > DateTime.Now)
                {
                    return false;
                }

                return true;
            }

            dateOfBirth = DateTime.MinValue;
            return false;
        }

        private static bool InputHeight(out short height)
        {
            var str = Console.ReadLine();
            if (!short.TryParse(str, out height))
            {
                return false;
            }

            if (height < 100 || height > 220)
            {
                return false;
            }

            return true;
        }

        private static bool InputSalary(out decimal salary)
        {
            var str = Console.ReadLine();
            if (!decimal.TryParse(str, out salary))
            {
                return false;
            }

            if (salary < 0)
            {
                return false;
            }

            return true;
        }

        private static bool InputGrade(out char grade)
        {
            var str = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(str) || str.Length > 1)
            {
                grade = ' ';
                return false;
            }

            grade = str[0];

            if (!char.IsLetter(grade))
            {
                return false;
            }

            return true;
        }

        private static void Create(string parameters)
        {
            Console.Write("First name: ");
            string firstName;
            while (!InputName(out firstName))
            {
                Console.WriteLine("First name should contain from 2 to 60 symbols");
            }

            Console.Write("Last name: ");
            string lastName;
            while (!InputName(out lastName))
            {
                Console.WriteLine("Last name should contain from 2 to 60 symbols");
            }

            Console.Write("Date of birth: ");
            DateTime date;
            while (!InputDate(out date))
            {
                Console.WriteLine("Date format should be mm/dd/yyyy");
            }

            Console.Write("Height: ");
            short height;
            while (!InputHeight(out height))
            {
                Console.WriteLine("Enter a valid height");
            }

            Console.Write("Salary: ");
            decimal salary;
            while (!InputSalary(out salary))
            {
                Console.WriteLine("Enter a valid salary");
            }

            Console.Write("Grade: ");
            char grade;
            while (!InputGrade(out grade))
            {
                Console.WriteLine("Grade should contain one letter");
            }

            fileCabinetService.CreateRecord(firstName, lastName, date, height, salary, grade);
            Console.WriteLine("Record #{0} is created.", Program.fileCabinetService.GetStat());
            Console.WriteLine();
        }

        private static void PrintRecordData(FileCabinetRecord record)
        {
            Console.Write("#{0}) {1}, {2}, {3},", record.Id, record.FirstName, record.LastName, record.DateOfBirth.ToString(OutputDateFormat, DateTimeFormatInfo.InvariantInfo));
            Console.WriteLine(" Salary: {0:F3}, Height: {1}, Grade: {2}", record.Salary, record.Height, record.Grade);
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
                foreach (var record in list)
                {
                    PrintRecordData(record);
                }
            }

            Console.WriteLine();
        }

        private static void Edit(string parameters)
        {
            int id;
            if (!int.TryParse(parameters, out id))
            {
                Console.WriteLine("Wrong argument");
                return;
            }

            FileCabinetRecord record = fileCabinetService.GetRecord(id);
            if (record is null)
            {
                Console.WriteLine("#{0} record is not found.", id);
                return;
            }

            PrintRecordData(record);

            Console.WriteLine("Enter new valid arguments to change them or anything else to leave them");

            Console.Write("First name: ");
            string firstName;
            if (!InputName(out firstName))
            {
                firstName = record.FirstName;
            }

            Console.Write("Last name: ");
            string lastName;
            if (!InputName(out lastName))
            {
                lastName = record.LastName;
            }

            Console.Write("Date of birth: ");
            DateTime date;
            if (!InputDate(out date))
            {
                date = record.DateOfBirth;
            }

            Console.Write("Height: ");
            short height;
            if (!InputHeight(out height))
            {
                height = record.Height;
            }

            Console.Write("Salary: ");
            decimal salary;
            if (!InputSalary(out salary))
            {
                salary = record.Salary;
            }

            Console.Write("Grade: ");
            char grade;
            if (!InputGrade(out grade))
            {
                grade = record.Grade;
            }

            fileCabinetService.EditRecord(id, firstName, lastName, date, height, salary, grade);
            Console.WriteLine("Record #{0} is updated.", id);
            Console.WriteLine();
        }

        private static void Find(string parameters)
        {
            var arguments = parameters.Split(' ', 2);
            var index = Array.FindIndex(findOptions, i => i.Item1.Equals(arguments[CommandIndex], StringComparison.OrdinalIgnoreCase));
            if (index < 0)
            {
                Console.WriteLine($"There is no such parameter as '{arguments[CommandIndex]}'");
                return;
            }

            if (arguments.Length < 2)
            {
                Console.WriteLine($"Your argument should follow '{arguments[CommandIndex]}' param");
                return;
            }

            FileCabinetRecord[] findRecords = findOptions[index].Item2(arguments[ParametersIndex]);
            if (findRecords.Length == 0)
            {
                Console.WriteLine($"No records was found");
            }
            else
            {
                foreach (var record in findRecords)
                {
                    PrintRecordData(record);
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