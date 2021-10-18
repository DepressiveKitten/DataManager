using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace FileCabinetApp
{
    /// <summary>
    /// Stores all records in file and process data.
    /// </summary>
    public class FileCabinetFilesystemService : IFileCabinetService, IDisposable
    {
        private const char StringBufferDefaultValue = '!';
        private const int LenghtOfStringInFile = 60;
        private const int LengthOfRecordInFile = (LenghtOfStringInFile * 2) + (sizeof(short) * 2) + (sizeof(int) * 4) + sizeof(decimal) + sizeof(char) - 1;
        private const int IdOffsetInWrittenRecord = sizeof(short);
        private static readonly StringComparer DictionaryComparer = StringComparer.OrdinalIgnoreCase;
        private readonly IRecordValidator validator;
        private readonly BinaryWriter writer;
        private readonly BinaryReader reader;
        private readonly FileStream fileStream;
        private readonly Dictionary<string, List<int>> firstNameDictionary = new Dictionary<string, List<int>>(DictionaryComparer);
        private readonly Dictionary<string, List<int>> lastNameDictionary = new Dictionary<string, List<int>>(DictionaryComparer);
        private readonly Dictionary<DateTime, List<int>> dateOfBirthDictionary = new Dictionary<DateTime, List<int>>();
        private int nextId;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetFilesystemService"/> class.
        /// </summary>
        /// <param name="fileStream">file to store records in.</param>
        /// <param name="validator">validator that will be used in current service.</param>
        public FileCabinetFilesystemService(FileStream fileStream, IRecordValidator validator)
        {
            this.validator = validator;
            this.fileStream = fileStream;
            this.writer = new BinaryWriter(fileStream, System.Text.Encoding.UTF8);
            this.reader = new BinaryReader(this.fileStream, System.Text.Encoding.UTF8);

            this.nextId = 0;
            for (int i = 0; i < this.fileStream.Length / LengthOfRecordInFile; i++)
            {
                this.fileStream.Seek(IdOffsetInWrittenRecord + (i * LengthOfRecordInFile), SeekOrigin.Begin);
                var id = this.reader.ReadInt32();
                if (id > this.nextId)
                {
                    this.nextId = id;
                }

                this.AddToDictionary(null, null, null, i * LengthOfRecordInFile);
            }

            this.nextId += 1;
        }

        /// <summary>
        /// Adds new record to the file.
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

            this.WriteRecord(SeekOrigin.End, 0, this.nextId, recordParameter, 0);
            this.AddToDictionary(null, null, null, (int)this.fileStream.Length - LengthOfRecordInFile);
            return this.nextId++;
        }

        /// <summary>
        /// Edit existing record.
        /// </summary>
        /// <param name="id">Id of record you want to edit.</param>
        /// <param name="recordParameter">Data that should be edited in record.</param>
        public void EditRecord(int id, RecordParameterObject recordParameter)
        {
            if (recordParameter is null)
            {
                throw new ArgumentNullException(nameof(recordParameter));
            }

            int offset = this.GetRecordOffset(id);
            if (offset < 0)
            {
                throw new ArgumentException(id + " record is not existing", nameof(id));
            }

            this.validator.ValidateParameters(recordParameter);

            FileCabinetRecord lastRecord = this.ReadRecord(SeekOrigin.Begin, offset);
            this.WriteRecord(SeekOrigin.Begin, offset, id, recordParameter, 0);
            this.AddToDictionary(lastRecord.FirstName, lastRecord.LastName, lastRecord.DateOfBirth, offset);
        }

        /// <summary>
        /// Return all records.
        /// </summary>
        /// <returns>List with all records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            this.fileStream.Seek(0, SeekOrigin.Begin);
            List<FileCabinetRecord> recordsList = new List<FileCabinetRecord>();
            for (int i = 0; i < this.fileStream.Length / LengthOfRecordInFile; i++)
            {
                recordsList.Add(this.ReadRecord(SeekOrigin.Begin, i * LengthOfRecordInFile));
            }

            return recordsList.AsReadOnly();
        }

        /// <summary>
        /// Return record with given id.
        /// </summary>
        /// <param name="id">id of wanted record.</param>
        /// <returns>Record with given id.</returns>
        public FileCabinetRecord GetRecord(int id)
        {
            int offset = this.GetRecordOffset(id);
            if (offset < 0)
            {
                return null;
            }

            return this.ReadRecord(SeekOrigin.Begin, offset);
        }

        /// <summary>
        /// Finds all records with given first name.
        /// </summary>
        /// <param name="firstName">First name of wanted record.</param>
        /// <returns>List of all record with given first name.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
        {
            if (!this.firstNameDictionary.ContainsKey(firstName))
            {
                return new List<FileCabinetRecord>().AsReadOnly();
            }

            List<FileCabinetRecord> records = new List<FileCabinetRecord>();
            foreach (int offset in this.firstNameDictionary[firstName])
            {
                records.Add(this.ReadRecord(SeekOrigin.Begin, offset));
            }

            return records.AsReadOnly();
        }

        /// <summary>
        /// Finds all records with given last name.
        /// </summary>
        /// <param name="lastName">Last name of wanted record.</param>
        /// <returns>List of all record with given Last name.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName)
        {
            if (!this.lastNameDictionary.ContainsKey(lastName))
            {
                return new List<FileCabinetRecord>().AsReadOnly();
            }

            List<FileCabinetRecord> records = new List<FileCabinetRecord>();
            foreach (int offset in this.lastNameDictionary[lastName])
            {
                records.Add(this.ReadRecord(SeekOrigin.Begin, offset));
            }

            return records.AsReadOnly();
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

            if (!this.dateOfBirthDictionary.ContainsKey(date))
            {
                return new List<FileCabinetRecord>().AsReadOnly();
            }

            List<FileCabinetRecord> records = new List<FileCabinetRecord>();
            foreach (int offset in this.dateOfBirthDictionary[date])
            {
                records.Add(this.ReadRecord(SeekOrigin.Begin, offset));
            }

            return records.AsReadOnly();
        }

        /// <summary>
        /// Return total amount of all records.
        /// </summary>
        /// <returns>Total amount of all records.</returns>
        public int GetStat()
        {
            return (int)this.fileStream.Length / LengthOfRecordInFile;
        }

        /// <summary>
        /// Get the snapshot of current state of service.
        /// </summary>
        /// <returns>snapshot of current state of service.</returns>
        public FileCabinetServiceSnapshot GetSnapshot()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Add all records from snapshot to service.
        /// </summary>
        /// <param name="snapshot">Snapshot to get records from.</param>
        public void Restore(FileCabinetServiceSnapshot snapshot)
        {
            List<int> idlist = new List<int>();
            this.fileStream.Seek(IdOffsetInWrittenRecord, SeekOrigin.Begin);
            for (int i = 0; i < this.fileStream.Length / LengthOfRecordInFile; i++, this.reader.ReadBytes(LengthOfRecordInFile - sizeof(int)))
            {
                idlist.Add(this.reader.ReadInt32());
            }

            if (snapshot is null)
            {
                return;
            }

            ReadOnlyCollection<FileCabinetRecord> shapshotRecords = snapshot.Records;
            foreach (FileCabinetRecord record in shapshotRecords)
            {
                RecordParameterObject parameterObject = new RecordParameterObject()
                {
                    DateOfBirth = record.DateOfBirth,
                    LastName = record.LastName,
                    FirstName = record.FirstName,
                    Grade = record.Grade,
                    Height = record.Height,
                    Salary = record.Salary,
                };
                try
                {
                    this.validator.ValidateParameters(parameterObject);
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine($"Validation in record {record.Id} failed: {ex.Message}");
                    continue;
                }

                if (idlist.Contains(record.Id))
                {
                    this.EditRecord(record.Id, parameterObject);
                }
                else
                {
                    this.CreateRecord(parameterObject);
                }
            }
        }

        /// <summary>
        /// Dispose object resourses.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose opened reader and writer streams.
        /// </summary>
        /// <param name="disposing">if streams should be disposed.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.reader.Dispose();
                this.writer.Dispose();
            }
        }

        private void WriteRecord(SeekOrigin origin, int offset, int id, RecordParameterObject recordParameter, short status)
        {
            this.writer.Seek(offset, origin);

            this.writer.Write(status);
            this.writer.Write(id);
            char[] bufferCharArray = Enumerable.Repeat(StringBufferDefaultValue, LenghtOfStringInFile).ToArray();
            recordParameter.FirstName.CopyTo(0, bufferCharArray, 0, recordParameter.FirstName.Length);
            this.writer.Write(bufferCharArray);
            bufferCharArray = Enumerable.Repeat(StringBufferDefaultValue, LenghtOfStringInFile).ToArray();
            recordParameter.LastName.CopyTo(0, bufferCharArray, 0, recordParameter.LastName.Length);
            this.writer.Write(bufferCharArray);
            this.writer.Write(recordParameter.DateOfBirth.Year);
            this.writer.Write(recordParameter.DateOfBirth.Month);
            this.writer.Write(recordParameter.DateOfBirth.Day);
            this.writer.Write(recordParameter.Height);
            this.writer.Write(recordParameter.Salary);
            this.writer.Write(recordParameter.Grade);
            this.writer.Flush();
        }

        private FileCabinetRecord ReadRecord(SeekOrigin origin, int offset)
        {
            this.fileStream.Seek(offset, origin);
            this.reader.ReadInt16();
            var id = this.reader.ReadInt32();
            char[] charBuffer = this.reader.ReadChars(LenghtOfStringInFile);
            string firstName = new string(charBuffer[0..new string(charBuffer).IndexOf(StringBufferDefaultValue, StringComparison.Ordinal)]);
            charBuffer = this.reader.ReadChars(LenghtOfStringInFile);
            string lastName = new string(charBuffer[0..new string(charBuffer).IndexOf(StringBufferDefaultValue, StringComparison.Ordinal)]);
            DateTime dateOfBirth = new DateTime(this.reader.ReadInt32(), this.reader.ReadInt32(), this.reader.ReadInt32());
            var height = this.reader.ReadInt16();
            decimal salary = this.reader.ReadDecimal();
            var grade = this.reader.ReadChar();
            return new FileCabinetRecord
            {
                Id = id,
                FirstName = firstName,
                LastName = lastName,
                DateOfBirth = dateOfBirth,
                Height = height,
                Salary = salary,
                Grade = grade,
            };
        }

        private int GetRecordOffset(int id)
        {
            this.fileStream.Seek(IdOffsetInWrittenRecord, SeekOrigin.Begin);
            for (int i = 0; i < this.fileStream.Length / LengthOfRecordInFile; i++, this.reader.ReadBytes(LengthOfRecordInFile - sizeof(int)))
            {
                if (this.reader.ReadInt32() == id)
                {
                    return i * LengthOfRecordInFile;
                }
            }

            return -1;
        }

        private void AddToDictionary(string previousFirstName, string previousLastName, DateTime? previousDateOfBirth, int offset)
        {
            FileCabinetRecord record = this.ReadRecord(SeekOrigin.Begin, offset);

            if (previousFirstName != null)
            {
                this.firstNameDictionary[previousFirstName].Remove(offset);
            }

            if (this.firstNameDictionary.ContainsKey(record.FirstName))
            {
                this.firstNameDictionary[record.FirstName].Add(offset);
            }
            else
            {
                this.firstNameDictionary[record.FirstName] = new List<int>() { offset };
            }

            if (previousLastName != null)
            {
                this.lastNameDictionary[previousLastName].Remove(offset);
            }

            if (this.lastNameDictionary.ContainsKey(record.LastName))
            {
                this.lastNameDictionary[record.LastName].Add(offset);
            }
            else
            {
                this.lastNameDictionary[record.LastName] = new List<int>() { offset };
            }

            if (previousDateOfBirth != null)
            {
                this.dateOfBirthDictionary[(DateTime)previousDateOfBirth].Remove(offset);
            }

            if (this.dateOfBirthDictionary.ContainsKey(record.DateOfBirth))
            {
                this.dateOfBirthDictionary[record.DateOfBirth].Add(offset);
            }
            else
            {
                this.dateOfBirthDictionary[record.DateOfBirth] = new List<int>() { offset };
            }
        }
    }
}
