using System;
using System.Collections.Generic;
using System.Globalization;

namespace FileCabinetApp
{
    /// <summary>
    /// Contains all records and process data.
    /// </summary>
    public class FileCabinetService
    {
        private static readonly DateTime MinimalValidDate = new DateTime(1950, 1, 1);
        private static readonly StringComparer DictionaryComparer = StringComparer.OrdinalIgnoreCase;
        private readonly List<FileCabinetRecord> recordsList = new List<FileCabinetRecord>();
        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new Dictionary<string, List<FileCabinetRecord>>(DictionaryComparer);
        private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new Dictionary<string, List<FileCabinetRecord>>(DictionaryComparer);
        private readonly Dictionary<DateTime, List<FileCabinetRecord>> dateOfBirthDictionary = new Dictionary<DateTime, List<FileCabinetRecord>>();

        /// <summary>
        /// Create a new record and adds it to the list.
        /// </summary>
        /// <param name="firstName">The first that stored in record.</param>
        /// <param name="lastName">The last that stored in record.</param>
        /// <param name="dateOfBirth">Person's date of birth.</param>
        /// <param name="height">Person's height.</param>
        /// <param name="salary">Person's salary.</param>
        /// <param name="grade">Person's grade.</param>
        /// <returns>Id of added record.</returns>
        public int CreateRecord(string firstName, string lastName, DateTime dateOfBirth, short height, decimal salary, char grade)
        {
            ValidateData(firstName, lastName, dateOfBirth, height, salary, grade);

            var record = new FileCabinetRecord
            {
                Id = this.recordsList.Count + 1,
                FirstName = firstName,
                LastName = lastName,
                DateOfBirth = dateOfBirth,
                Height = height,
                Salary = salary,
                Grade = grade,
            };

            this.recordsList.Add(record);
            this.AddByFirstNameToDictionary(firstName, record);
            this.AddBySecondNameToDictionary(lastName, record);
            this.AddByDateOfBirthToDictionary(dateOfBirth, record);

            return record.Id;
        }

        /// <summary>
        /// Create a new record and adds it to the list.
        /// </summary>
        /// <param name="id">Id of record you want to edit.</param>
        /// <param name="firstName">The first that stored in record.</param>
        /// <param name="lastName">The last that stored in record.</param>
        /// <param name="dateOfBirth">Person's date of birth.</param>
        /// <param name="height">Person's height.</param>
        /// <param name="salary">Person's salary.</param>
        /// <param name="grade">Person's grade.</param>
        public void EditRecord(int id, string firstName, string lastName, DateTime dateOfBirth, short height, decimal salary, char grade)
        {
            FileCabinetRecord record = this.recordsList.Find((FileCabinetRecord record) => record.Id == id);
            if (record is null)
            {
                throw new ArgumentException(id + " record is not existing", nameof(id));
            }

            ValidateData(firstName, lastName, dateOfBirth, height, salary, grade);

            if (!string.Equals(record.FirstName, firstName, StringComparison.OrdinalIgnoreCase))
            {
                this.firstNameDictionary[record.FirstName].Remove(record);
                this.AddByFirstNameToDictionary(firstName, record);
            }

            if (!string.Equals(record.LastName, lastName, StringComparison.OrdinalIgnoreCase))
            {
                this.lastNameDictionary[record.LastName].Remove(record);
                this.AddBySecondNameToDictionary(lastName, record);
            }

            if (!DateTime.Equals(record.DateOfBirth, dateOfBirth))
            {
                this.dateOfBirthDictionary[record.DateOfBirth].Remove(record);
                this.AddByDateOfBirthToDictionary(dateOfBirth, record);
            }

            record.FirstName = firstName;
            record.LastName = lastName;
            record.DateOfBirth = dateOfBirth;
            record.Height = height;
            record.Salary = salary;
            record.Grade = grade;
        }

        /// <summary>
        /// Return all records.
        /// </summary>
        /// <returns>List with all records.</returns>
        public FileCabinetRecord[] GetRecords()
        {
            return this.recordsList.ToArray();
        }

        /// <summary>
        /// Return record with given id.
        /// </summary>
        /// <param name="id">id of wanted record.</param>
        /// <returns>Record with given id.</returns>
        public FileCabinetRecord GetRecord(int id)
        {
            return this.recordsList.Find((FileCabinetRecord record) => record.Id == id);
        }

        /// <summary>
        /// Finds all records with given first name.
        /// </summary>
        /// <param name="firstName">First name of wanted record.</param>
        /// <returns>List of all record with given first name.</returns>
        public FileCabinetRecord[] FindByFirstName(string firstName)
        {
            if (this.firstNameDictionary.ContainsKey(firstName))
            {
                return this.firstNameDictionary[firstName].ToArray();
            }

            return Array.Empty<FileCabinetRecord>();
        }

        /// <summary>
        /// Finds all records with given last name.
        /// </summary>
        /// <param name="lastName">Last name of wanted record.</param>
        /// <returns>List of all record with given Last name.</returns>
        public FileCabinetRecord[] FindByLastName(string lastName)
        {
            if (this.lastNameDictionary.ContainsKey(lastName))
            {
                return this.lastNameDictionary[lastName].ToArray();
            }

            return Array.Empty<FileCabinetRecord>();
        }

        /// <summary>
        /// Finds all records with date of birth.
        /// </summary>
        /// <param name="dateOfBirth">date of birth of wanted record.</param>
        /// <returns>List of all record with given date of birth.</returns>
        public FileCabinetRecord[] FindByDate(string dateOfBirth)
        {
            DateTime date;
            if (!DateTime.TryParse(dateOfBirth, out date))
            {
                return Array.Empty<FileCabinetRecord>();
            }

            if (this.dateOfBirthDictionary.ContainsKey(date))
            {
                return this.dateOfBirthDictionary[date].ToArray();
            }

            return Array.Empty<FileCabinetRecord>();
        }

        /// <summary>
        /// Return total amount of all records.
        /// </summary>
        /// <returns>Total amount of all records.</returns>
        public int GetStat()
        {
            return this.recordsList.Count;
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

            if (dateOfBirth < MinimalValidDate || dateOfBirth > DateTime.Now)
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

        private void AddByDateOfBirthToDictionary(DateTime dateOfBirth, FileCabinetRecord record)
        {
            if (this.dateOfBirthDictionary.ContainsKey(dateOfBirth))
            {
                this.dateOfBirthDictionary[dateOfBirth].Add(record);
            }
            else
            {
                this.dateOfBirthDictionary[dateOfBirth] = new List<FileCabinetRecord>() { record };
            }
        }

        private void AddBySecondNameToDictionary(string lastName, FileCabinetRecord record)
        {
            if (this.lastNameDictionary.ContainsKey(lastName))
            {
                this.lastNameDictionary[lastName].Add(record);
            }
            else
            {
                this.lastNameDictionary[lastName] = new List<FileCabinetRecord>() { record };
            }
        }

        private void AddByFirstNameToDictionary(string firstName, FileCabinetRecord record)
        {
            if (this.firstNameDictionary.ContainsKey(firstName))
            {
                this.firstNameDictionary[firstName].Add(record);
            }
            else
            {
                this.firstNameDictionary[firstName] = new List<FileCabinetRecord>() { record };
            }
        }
    }
}
