using System;
using System.Globalization;

namespace FileCabinetApp
{
    /// <summary>
    /// Contains main programm functions.
    /// </summary>
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
        private static FileCabinetService fileCabinetService;
        private static IRecordValidator validator;

        private static bool isRunning = true;

        private static Tuple<string, Func<string, FileCabinetRecord[]>>[] findOptions;

        private static string[] validorsNames = new string[]
        {
            "default",
            "custom",
        };

        private static Tuple<string, string, Action<string>>[] commandLineArguments = new Tuple<string, string, Action<string>>[]
        {
            new Tuple<string, string, Action<string>>("--validation-rules", "-v", SetValidationRules),
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

        /// <summary>
        /// Starting point of programm.
        /// </summary>
        /// <param name="args">Arguments from command line.</param>
        public static void Main(string[] args)
        {
            Console.WriteLine($"File Cabinet Application, developed by {Program.DeveloperName}");

            if (args == null)
            {
                args = Array.Empty<string>();
            }

            Initialize(args);
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

                Console.WriteLine();
            }
            while (isRunning);
        }

        private static void Initialize(string[] args)
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

            if (validator is null)
            {
                SetValidationRules("default");
            }

            fileCabinetService = new FileCabinetService(validator);

            findOptions = new Tuple<string, Func<string, FileCabinetRecord[]>>[]
            {
                new Tuple<string, Func<string, FileCabinetRecord[]>>("firstname", fileCabinetService.FindByFirstName),
                new Tuple<string, Func<string, FileCabinetRecord[]>>("lastname", fileCabinetService.FindByLastName),
                new Tuple<string, Func<string, FileCabinetRecord[]>>("dateofbirth", fileCabinetService.FindByDate),
            };
        }

        private static void SetValidationRules(string validationRules)
        {
            if (validationRules.Equals(validorsNames[0], StringComparison.OrdinalIgnoreCase))
            {
                validator = new FileCabinetDefaultService();
                System.Console.WriteLine($"Using {validorsNames[0]} validation rules.");
            }

            if (validationRules.Equals(validorsNames[1], StringComparison.OrdinalIgnoreCase))
            {
                validator = new FileCabinetCustomService();
                System.Console.WriteLine($"Using {validorsNames[1]} validation rules.");
            }

            if (validator is null)
            {
                System.Console.WriteLine($"{validationRules} is not proper validation rule");
            }
        }

        private static void PrintMissedCommandInfo(string command)
        {
            Console.WriteLine($"There is no '{command}' command.");
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
        }

        private static void Stat(string parameters)
        {
            var recordsCount = Program.fileCabinetService.GetStat();
            Console.WriteLine($"{recordsCount} record(s).");
        }

        private static bool InputFirstName(out string firstName)
        {
            firstName = Console.ReadLine();
            try
            {
                validator.ValidateFirstName(firstName);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

            return true;
        }

        private static bool InputLastName(out string lastName)
        {
            lastName = Console.ReadLine();
            try
            {
                validator.ValidateLastName(lastName);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
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

                try
                {
                    validator.ValidateDateOfBirth(dateOfBirth);
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine(ex.Message);
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

            try
            {
                validator.ValidateHeight(height);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
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

            try
            {
                validator.ValidateSalary(salary);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
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

            try
            {
                validator.ValidateGrade(grade);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

            return true;
        }

        private static void Create(string parameters)
        {
            Console.Write("First name: ");
            string firstName;
            while (!InputFirstName(out firstName)) ;

            Console.Write("Last name: ");
            string lastName;
            while (!InputLastName(out lastName)) ;

            Console.Write("Date of birth: ");
            DateTime date;
            while (!InputDate(out date)) ;

            Console.Write("Height: ");
            short height;
            while (!InputHeight(out height)) ;

            Console.Write("Salary: ");
            decimal salary;
            while (!InputSalary(out salary)) ;

            Console.Write("Grade: ");
            char grade;
            while (!InputGrade(out grade)) ;

            RecordParameterObject recordParameterObject = new RecordParameterObject()
            {
                FirstName = firstName,
                LastName = lastName,
                DateOfBirth = date,
                Height = height,
                Salary = salary,
                Grade = grade,
            };
            fileCabinetService.CreateRecord(recordParameterObject);
            Console.WriteLine("Record #{0} is created.", Program.fileCabinetService.GetStat());
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
            if (!InputFirstName(out firstName)) ;

            Console.Write("Last name: ");
            string lastName;
            if (!InputLastName(out lastName)) ;

            Console.Write("Date of birth: ");
            DateTime date;
            if (!InputDate(out date)) ;

            Console.Write("Height: ");
            short height;
            if (!InputHeight(out height)) ;

            Console.Write("Salary: ");
            decimal salary;
            if (!InputSalary(out salary)) ;

            Console.Write("Grade: ");
            char grade;
            if (!InputGrade(out grade)) ;

            RecordParameterObject recordParameterObject = new RecordParameterObject()
            {
                FirstName = firstName,
                LastName = lastName,
                DateOfBirth = date,
                Height = height,
                Salary = salary,
                Grade = grade,
            };
            fileCabinetService.EditRecord(id, recordParameterObject);
            Console.WriteLine("Record #{0} is updated.", id);
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
        }

        private static void Exit(string parameters)
        {
            Console.WriteLine("Exiting an application...");
            isRunning = false;
        }
    }
}