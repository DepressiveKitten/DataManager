using System;
using System.Collections.Generic;
using System.Globalization;

namespace FileCabinetApp
{
    public class FileCabinetService
    {
        private static readonly DateTime MinDate = new DateTime(1950, 1, 1);
        private static StringComparer dictionaryComparer = StringComparer.OrdinalIgnoreCase;
        private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();
        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new Dictionary<string, List<FileCabinetRecord>>(dictionaryComparer);
        private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new Dictionary<string, List<FileCabinetRecord>>(dictionaryComparer);
        private readonly Dictionary<DateTime, List<FileCabinetRecord>> dateOfBirthDictionary = new Dictionary<DateTime, List<FileCabinetRecord>>();

        public int CreateRecord(string firstName, string lastName, DateTime dateOfBirth, short height, decimal salary, char grade)
        {
            ValidateData(firstName, lastName, dateOfBirth, height, salary, grade);

            var record = new FileCabinetRecord
            {
                Id = this.list.Count + 1,
                FirstName = firstName,
                LastName = lastName,
                DateOfBirth = dateOfBirth,
                Height = height,
                Salary = salary,
                Grade = grade,
            };

            this.list.Add(record);

            if (this.firstNameDictionary.ContainsKey(firstName))
            {
                this.firstNameDictionary[firstName].Add(record);
            }
            else
            {
                this.firstNameDictionary[firstName] = new List<FileCabinetRecord>() { record };
            }

            if (this.lastNameDictionary.ContainsKey(lastName))
            {
                this.lastNameDictionary[lastName].Add(record);
            }
            else
            {
                this.lastNameDictionary[lastName] = new List<FileCabinetRecord>() { record };
            }

            if (this.dateOfBirthDictionary.ContainsKey(dateOfBirth))
            {
                this.dateOfBirthDictionary[dateOfBirth].Add(record);
            }
            else
            {
                this.dateOfBirthDictionary[dateOfBirth] = new List<FileCabinetRecord>() { record };
            }

            return record.Id;
        }

        public void EditRecord(int id, string firstName, string lastName, DateTime dateOfBirth, short height, decimal salary, char grade)
        {
            FileCabinetRecord record = this.list.Find((FileCabinetRecord record) => record.Id == id);
            if (record is null)
            {
                throw new ArgumentException(id + " record is not existing", nameof(id));
            }

            ValidateData(firstName, lastName, dateOfBirth, height, salary, grade);

            if (!string.Equals(record.FirstName, firstName, StringComparison.OrdinalIgnoreCase))
            {
                this.firstNameDictionary[record.FirstName].Remove(record);
                if (this.firstNameDictionary.ContainsKey(firstName))
                {
                    this.firstNameDictionary[firstName].Add(record);
                }
                else
                {
                    this.firstNameDictionary[firstName] = new List<FileCabinetRecord>() { record };
                }
            }

            if (!string.Equals(record.LastName, lastName, StringComparison.OrdinalIgnoreCase))
            {
                this.lastNameDictionary[record.LastName].Remove(record);
                if (this.lastNameDictionary.ContainsKey(lastName))
                {
                    this.lastNameDictionary[lastName].Add(record);
                }
                else
                {
                    this.lastNameDictionary[lastName] = new List<FileCabinetRecord>() { record };
                }
            }

            if (!DateTime.Equals(record.DateOfBirth, dateOfBirth))
            {
                this.dateOfBirthDictionary[record.DateOfBirth].Remove(record);
                if (this.dateOfBirthDictionary.ContainsKey(dateOfBirth))
                {
                    this.dateOfBirthDictionary[dateOfBirth].Add(record);
                }
                else
                {
                    this.dateOfBirthDictionary[dateOfBirth] = new List<FileCabinetRecord>() { record };
                }
            }

            record.FirstName = firstName;
            record.LastName = lastName;
            record.DateOfBirth = dateOfBirth;
            record.Height = height;
            record.Salary = salary;
            record.Grade = grade;
        }

        public FileCabinetRecord[] GetRecords()
        {
            return this.list.ToArray();
        }

        public FileCabinetRecord GetRecord(int id)
        {
            return this.list.Find((FileCabinetRecord record) => record.Id == id);
        }

        public FileCabinetRecord[] FindByFirstName(string firstName)
        {
            if (this.firstNameDictionary.ContainsKey(firstName))
            {
                return this.firstNameDictionary[firstName].ToArray();
            }

            return Array.Empty<FileCabinetRecord>();
        }

        public FileCabinetRecord[] FindByLastName(string lastName)
        {
            if (this.lastNameDictionary.ContainsKey(lastName))
            {
                return this.lastNameDictionary[lastName].ToArray();
            }

            return Array.Empty<FileCabinetRecord>();
        }

        public FileCabinetRecord[] FindByDate(string dateOfBirth)
        {
            CultureInfo provider = CultureInfo.InvariantCulture;
            DateTime date;
            try
            {
                date = DateTime.Parse(dateOfBirth, provider);
            }
            catch (FormatException)
            {
                return Array.Empty<FileCabinetRecord>();
            }

            if (this.dateOfBirthDictionary.ContainsKey(date))
            {
                return this.dateOfBirthDictionary[date].ToArray();
            }

            return Array.Empty<FileCabinetRecord>();
        }

        public int GetStat()
        {
            return this.list.Count;
        }

        private static void ValidateData(string firstName, string lastName, DateTime dateOfBirth, short height, decimal salary, char grade)
        {
            if (string.IsNullOrWhiteSpace(firstName) || firstName.Length < 2 || firstName.Length > 60)
            {
                throw new ArgumentException("first name should contain from 2 to 60 symbols", nameof(firstName));
            }

            if (string.IsNullOrWhiteSpace(lastName) || lastName.Length < 2 || lastName.Length > 60)
            {
                throw new ArgumentException("last name should contain from 2 to 60 symbols", nameof(lastName));
            }

            if (dateOfBirth < MinDate || dateOfBirth > DateTime.Now)
            {
                throw new ArgumentException("Enter a valid date", nameof(dateOfBirth));
            }

            if (height < 100 || height > 220)
            {
                throw new ArgumentException("Enter a valid height", nameof(height));
            }

            if (salary < 0)
            {
                throw new ArgumentException("salary should be positive", nameof(salary));
            }

            if (!char.IsLetter(grade))
            {
                throw new ArgumentException("grade should conrain one letter", nameof(grade));
            }
        }
    }
}
