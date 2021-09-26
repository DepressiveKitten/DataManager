using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;

namespace FileCabinetApp
{
    /// <summary>
    /// Contains all records and process data.
    /// </summary>
    public class FileCabinetService : IFileCabinetService
    {
        private static readonly StringComparer DictionaryComparer = StringComparer.OrdinalIgnoreCase;
        private readonly IRecordValidator validator;
        private readonly List<FileCabinetRecord> recordsList = new List<FileCabinetRecord>();
        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new Dictionary<string, List<FileCabinetRecord>>(DictionaryComparer);
        private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new Dictionary<string, List<FileCabinetRecord>>(DictionaryComparer);
        private readonly Dictionary<DateTime, List<FileCabinetRecord>> dateOfBirthDictionary = new Dictionary<DateTime, List<FileCabinetRecord>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetService"/> class.
        /// </summary>
        /// <param name="validator">validator that will be used in current service.</param>
        public FileCabinetService(IRecordValidator validator)
        {
            this.validator = validator;
        }

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

            this.validator.ValidateParameters(recordParameter);

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

            this.validator.ValidateParameters(recordParameter);

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
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            return this.recordsList.AsReadOnly();
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
        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
        {
            if (this.firstNameDictionary.ContainsKey(firstName))
            {
                return new ReadOnlyCollection<FileCabinetRecord>(this.firstNameDictionary[firstName]);
            }

            return new List<FileCabinetRecord>().AsReadOnly();
        }

        /// <summary>
        /// Finds all records with given last name.
        /// </summary>
        /// <param name="lastName">Last name of wanted record.</param>
        /// <returns>List of all record with given Last name.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName)
        {
            if (this.lastNameDictionary.ContainsKey(lastName))
            {
                return this.lastNameDictionary[lastName].AsReadOnly();
            }

            return new List<FileCabinetRecord>().AsReadOnly();
        }

        /// <summary>
        /// Finds all records with date of birth.
        /// </summary>
        /// <param name="dateOfBirth">date of birth of wanted record.</param>
        /// <returns>List of all record with given date of birth.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByDate(string dateOfBirth)
        {
            DateTime date;
            if (!DateTime.TryParse(dateOfBirth, out date))
            {
                return new List<FileCabinetRecord>().AsReadOnly();
            }

            if (this.dateOfBirthDictionary.ContainsKey(date))
            {
                return this.dateOfBirthDictionary[date].AsReadOnly();
            }

            return new List<FileCabinetRecord>().AsReadOnly();
        }

        /// <summary>
        /// Return total amount of all records.
        /// </summary>
        /// <returns>Total amount of all records.</returns>
        public int GetStat()
        {
            return this.recordsList.Count;
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
