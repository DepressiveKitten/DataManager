using System;
using System.Collections.ObjectModel;
using System.IO;

namespace FileCabinetApp
{
    public class FileCabinetFilesystemService : IFileCabinetService
    {
        private const int LenghtOfStringInFile = 60;
        private const int LengthOfRecordInFile = (LenghtOfStringInFile * 2) + (sizeof(short) * 2) + (sizeof(int) * 4) + sizeof(decimal) + sizeof(char);
        private readonly IRecordValidator validator;
        private readonly BinaryWriter writer;
        private readonly FileStream fileStream;
        private int nextId;

        public FileCabinetFilesystemService(FileStream fileStream, IRecordValidator validator)
        {
            this.validator = validator;
            this.fileStream = fileStream;
            this.writer = new BinaryWriter(fileStream);
            //TODO read all id from file and set to last
            this.nextId = 1;
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

            this.writer.Seek(0, SeekOrigin.End);

            this.writer.Write((short)0);
            this.writer.Write(this.nextId);
            string bufferString = new string('\0', LenghtOfStringInFile);
            this.writer.Write(bufferString.Insert(0, recordParameter.FirstName));
            this.writer.Write(bufferString.Insert(0, recordParameter.LastName));
            this.writer.Write(recordParameter.DateOfBirth.Year);
            this.writer.Write(recordParameter.DateOfBirth.Month);
            this.writer.Write(recordParameter.DateOfBirth.Day);
            this.writer.Write(recordParameter.Height);
            this.writer.Write(recordParameter.Salary);
            this.writer.Write(recordParameter.Grade);
            this.writer.Flush();

            return this.nextId++;
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
