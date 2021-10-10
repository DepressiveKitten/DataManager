using System;
using System.Collections.ObjectModel;
using System.IO;

namespace FileCabinetApp
{
    public class FileCabinetFilesystemService
    {
        private readonly IRecordValidator validator;
        private FileStream fileStream;

        public FileCabinetFilesystemService(FileStream fileStream, IRecordValidator validator)
        {
            this.validator = validator;
            this.fileStream = fileStream;
        }

        /// <summary>
        /// Create a new record and adds it to the list.
        /// </summary>
        /// <param name="recordParameter">Data that should be stored in new record.</param>
        /// <returns>Id of added record.</returns>
        public int CreateRecord(RecordParameterObject recordParameter)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Create a new record and adds it to the list.
        /// </summary>
        /// <param name="id">Id of record you want to edit.</param>
        /// <param name="recordParameter">Data that should be edited in record.</param>
        public void EditRecord(int id, RecordParameterObject recordParameter)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Return all records.
        /// </summary>
        /// <returns>List with all records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Return record with given id.
        /// </summary>
        /// <param name="id">id of wanted record.</param>
        /// <returns>Record with given id.</returns>
        public FileCabinetRecord GetRecord(int id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Finds all records with given first name.
        /// </summary>
        /// <param name="firstName">First name of wanted record.</param>
        /// <returns>List of all record with given first name.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Finds all records with given last name.
        /// </summary>
        /// <param name="lastName">Last name of wanted record.</param>
        /// <returns>List of all record with given Last name.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Finds all records with date of birth.
        /// </summary>
        /// <param name="dateOfBirth">date of birth of wanted record.</param>
        /// <returns>List of all record with given date of birth.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByDate(string dateOfBirth)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Return total amount of all records.
        /// </summary>
        /// <returns>Total amount of all records.</returns>
        public int GetStat()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get the snapshot of current state of service.
        /// </summary>
        /// <returns>snapshot of current state of service.</returns>
        public FileCabinetServiceSnapshot GetSnapshot()
        {
            throw new NotImplementedException();
        }
    }
}
