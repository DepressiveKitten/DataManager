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
        /// <param name="recordParameter">Data that should be stored in new record.</param>
        /// <returns>Id of added record.</returns>
        public int CreateRecord(RecordParameterObject recordParameter)
        {
            if (recordParameter is null)
            {
                throw new ArgumentNullException(nameof(recordParameter));
            }

            ValidateData(recordParameter);

            var record = new FileCabinetRecord
            {
                Id = this.recordsList.Count + 1,
                FirstName = recordParameter.FirstName,
                LastName = recordParameter.LastName,
                DateOfBirth = recordParameter.DateOfBirth,
                Height = recordParameter.Height,
                Salary = recordParameter.Salary,
                Grade = recordParameter.Grade,
            };

            this.recordsList.Add(record);
            this.AddByFirstNameToDictionary(recordParameter.FirstName, record);
            this.AddBySecondNameToDictionary(recordParameter.LastName, record);
            this.AddByDateOfBirthToDictionary(recordParameter.DateOfBirth, record);

            return record.Id;
        }

        /// <summary>
        /// Create a new record and adds it to the list.
        /// </summary>
        /// <param name="id">Id of record you want to edit.</param>
        /// <param name="recordParameter">Data that should be edited in record.</param>
        public void EditRecord(int id, RecordParameterObject recordParameter)
        {
            if (recordParameter is null)
            {
                throw new ArgumentNullException(nameof(recordParameter));
            }

            FileCabinetRecord record = this.recordsList.Find((FileCabinetRecord record) => record.Id == id);
            if (record is null)
            {
                throw new ArgumentException(id + " record is not existing", nameof(id));
            }

            ValidateData(recordParameter);

            if (!string.Equals(record.FirstName, recordParameter.FirstName, StringComparison.OrdinalIgnoreCase))
            {
                this.firstNameDictionary[record.FirstName].Remove(record);
                this.AddByFirstNameToDictionary(recordParameter.FirstName, record);
            }

            if (!string.Equals(record.LastName, recordParameter.LastName, StringComparison.OrdinalIgnoreCase))
            {
                this.lastNameDictionary[record.LastName].Remove(record);
                this.AddBySecondNameToDictionary(recordParameter.LastName, record);
            }

            if (!DateTime.Equals(record.DateOfBirth, recordParameter.DateOfBirth))
            {
                this.dateOfBirthDictionary[record.DateOfBirth].Remove(record);
                this.AddByDateOfBirthToDictionary(recordParameter.DateOfBirth, record);
            }

            record.FirstName = recordParameter.FirstName;
            record.LastName = recordParameter.LastName;
            record.DateOfBirth = recordParameter.DateOfBirth;
            record.Height = recordParameter.Height;
            record.Salary = recordParameter.Salary;
            record.Grade = recordParameter.Grade;
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

        private static void ValidateData(RecordParameterObject recordParameter)
        {
            if (string.IsNullOrWhiteSpace(recordParameter.FirstName) || recordParameter.FirstName.Length < 2 || recordParameter.FirstName.Length > 60)
            {
                throw new ArgumentException("first name should contain from 2 to 60 symbols", nameof(recordParameter));
            }

            if (string.IsNullOrWhiteSpace(recordParameter.LastName) || recordParameter.LastName.Length < 2 || recordParameter.LastName.Length > 60)
            {
                throw new ArgumentException("last name should contain from 2 to 60 symbols", nameof(recordParameter));
            }

            if (recordParameter.DateOfBirth < MinimalValidDate || recordParameter.DateOfBirth > DateTime.Now)
            {
                throw new ArgumentException("Enter a valid date", nameof(recordParameter));
            }

            if (recordParameter.Height < 100 || recordParameter.Height > 220)
            {
                throw new ArgumentException("Enter a valid height", nameof(recordParameter));
            }

            if (recordParameter.Salary < 0)
            {
                throw new ArgumentException("salary should be positive", nameof(recordParameter));
            }

            if (!char.IsLetter(recordParameter.Grade))
            {
                throw new ArgumentException("grade should conrain one letter", nameof(recordParameter));
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
