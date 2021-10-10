using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;

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
        private static IFileCabinetService fileCabinetService;
        private static IRecordValidator validator;

        private static bool isRunning = true;

        private static Tuple<string, Func<string, ReadOnlyCollection<FileCabinetRecord>>>[] findOptions;

        private static Tuple<string, Func<string, StreamWriter>>[] fileFormats
            = new Tuple<string, Func<string, StreamWriter>>[]
        {
            new Tuple<string, Func<string, StreamWriter>>("csv", File.CreateText),
            new Tuple<string, Func<string, StreamWriter>>("xml", File.CreateText),
        };

        private static Tuple<string, Func<IRecordValidator>>[] validatorsNames = new Tuple<string, Func<IRecordValidator>>[]
        {
            new Tuple<string, Func<IRecordValidator>>("default", () => new FileCabinetDefaultService()),
            new Tuple<string, Func<IRecordValidator>>("custom", () => new FileCabinetDefaultService()),
        };

        private static Tuple<string, bool>[] yesNoStatements = new Tuple<string, bool>[]
        {
            new Tuple<string, bool>("y", true),
            new Tuple<string, bool>("n", false),
            new Tuple<string, bool>("yes", true),
            new Tuple<string, bool>("no", false),
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
            new Tuple<string, Action<string>>("export", Export),
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
            new string[] { "export", "export records to file", "Type export, then csv or xml, then path you want to save your data to" },
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

            fileCabinetService = new FileCabinetMemoryService(validator);

            findOptions = new Tuple<string, Func<string, ReadOnlyCollection<FileCabinetRecord>>>[]
            {
                new Tuple<string, Func<string, ReadOnlyCollection<FileCabinetRecord>>>("firstname", fileCabinetService.FindByFirstName),
                new Tuple<string, Func<string, ReadOnlyCollection<FileCabinetRecord>>>("lastname", fileCabinetService.FindByLastName),
                new Tuple<string, Func<string, ReadOnlyCollection<FileCabinetRecord>>>("dateofbirth", fileCabinetService.FindByDate),
            };
        }

        private static void SetValidationRules(string validationRules)
        {
            var index = Array.FindIndex(validatorsNames, i => i.Item1.Equals(validationRules, StringComparison.OrdinalIgnoreCase));
            if (index < 0)
            {
                System.Console.WriteLine($"{validationRules} is not proper validation rule");
                return;
            }

            validator = validatorsNames[index].Item2();
            System.Console.WriteLine($"Using {validatorsNames[index].Item1} validation rules.");
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

        private static Tuple<bool, string, string> StringConverter(string input)
        {
            return new (true, string.Empty, input);
        }

        private static Tuple<bool, string, DateTime> DateConverter(string input)
        {
            var date = input.Split('/');
            int day, month, year;
            DateTime dateOfBirth;
            if (date.Length >= 3 && int.TryParse(date[0], out month) && int.TryParse(date[1], out day) && int.TryParse(date[2], out year))
            {
                try
                {
                    dateOfBirth = new DateTime(year, month, day);
                }
                catch (ArgumentOutOfRangeException)
                {
                    return new (false, "invalid date", DateTime.Now);
                }

                return new (true, string.Empty, dateOfBirth);
            }

            return new (false, "invalid format, try mm/dd/yyyy", DateTime.Now);
        }

        private static Tuple<bool, string, short> HeightConverter(string input)
        {
            short height;
            if (!short.TryParse(input, out height))
            {
                return new (false, "failed to parse", 0);
            }

            return new (true, string.Empty, height);
        }

        private static Tuple<bool, string, decimal> SalaryConverter(string input)
        {
            decimal salary;
            if (!decimal.TryParse(input, out salary))
            {
                return new Tuple<bool, string, decimal>(false, "failed to parse", 0);
            }

            return new (true, string.Empty, salary);
        }

        private static Tuple<bool, string, char> GradeConverter(string input)
        {
            char grade;

            if (string.IsNullOrWhiteSpace(input) || input.Length > 1)
            {
                grade = ' ';
                return new (false, "should contain one symbol", grade);
            }

            grade = input[0];

            return new (true, string.Empty, grade);
        }

        private static T ReadInput<T>(Func<string, Tuple<bool, string, T>> converter, Func<T, Tuple<bool, string>> validator)
        {
            do
            {
                T value;

                var input = Console.ReadLine();
                var conversionResult = converter(input);

                if (!conversionResult.Item1)
                {
                    Console.WriteLine($"Conversion failed: {conversionResult.Item2}. Please, correct your input.");
                    continue;
                }

                value = conversionResult.Item3;

                var validationResult = validator(value);
                if (!validationResult.Item1)
                {
                    Console.WriteLine($"Validation failed: {validationResult.Item2}. Please, correct your input.");
                    continue;
                }

                return value;
            }
            while (true);
        }

        private static void Create(string parameters)
        {
            Console.Write("First name: ");
            string firstName = ReadInput<string>(StringConverter, validator.ValidateFirstName);

            Console.Write("Last name: ");
            string lastName = ReadInput<string>(StringConverter, validator.ValidateFirstName);

            Console.Write("Date of birth: ");
            DateTime date = ReadInput<DateTime>(DateConverter, validator.ValidateDateOfBirth);

            Console.Write("Height: ");
            short height = ReadInput<short>(HeightConverter, validator.ValidateHeight);

            Console.Write("Salary: ");
            decimal salary = ReadInput<decimal>(SalaryConverter, validator.ValidateSalary);

            Console.Write("Grade: ");
            char grade = ReadInput<char>(GradeConverter, validator.ValidateGrade);

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
            if (list.Count == 0)
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

            Console.WriteLine("Enter new valid arguments to change them");

            Console.Write("First name: ");
            string firstName = ReadInput<string>(StringConverter, validator.ValidateFirstName);

            Console.Write("Last name: ");
            string lastName = ReadInput<string>(StringConverter, validator.ValidateFirstName);

            Console.Write("Date of birth: ");
            DateTime date = ReadInput<DateTime>(DateConverter, validator.ValidateDateOfBirth);

            Console.Write("Height: ");
            short height = ReadInput<short>(HeightConverter, validator.ValidateHeight);

            Console.Write("Salary: ");
            decimal salary = ReadInput<decimal>(SalaryConverter, validator.ValidateSalary);

            Console.Write("Grade: ");
            char grade = ReadInput<char>(GradeConverter, validator.ValidateGrade);

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

            ReadOnlyCollection<FileCabinetRecord> findRecords = findOptions[index].Item2(arguments[ParametersIndex]);
            if (findRecords.Count == 0)
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

        private static void Export(string parameters)
        {
            var arguments = parameters.Split(' ', 2);

            var fileTypeIndex = Array.FindIndex(fileFormats, i => i.Item1.Equals(arguments[0], StringComparison.OrdinalIgnoreCase));

            if (fileTypeIndex < 0)
            {
                Console.WriteLine($"There is no such parameter as '{arguments[0]}'");
                return;
            }

            if (arguments.Length < 2)
            {
                Console.WriteLine($"Your argument should follow '{arguments[0]}' param");
                return;
            }

            if (File.Exists(arguments[1]))
            {
                int index;
                do
                {
                    System.Console.WriteLine($"File is exist - rewrite {arguments[1]} [Y/n]");
                    var input = Console.ReadLine();
                    index = Array.FindIndex(yesNoStatements, i => i.Item1.Equals(input, StringComparison.OrdinalIgnoreCase));
                }
                while (index < 0);

                if (!yesNoStatements[index].Item2)
                {
                    return;
                }
            }

            try
            {
                using (StreamWriter writer = fileFormats[fileTypeIndex].Item2(arguments[1]))
                {
                    FileCabinetServiceSnapshot snapshot = fileCabinetService.GetSnapshot();
                    switch (fileTypeIndex)
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

        private static void Exit(string parameters)
        {
            Console.WriteLine("Exiting an application...");
            isRunning = false;
        }
    }
}