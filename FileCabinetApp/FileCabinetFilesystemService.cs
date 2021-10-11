using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace FileCabinetApp
{
    public class FileCabinetFilesystemService : IFileCabinetService
    {
        //TODO review all documentation
        private const char StringBufferDefaultValue = '!';
        private const int LenghtOfStringInFile = 60;
        private const int LengthOfRecordInFile = (LenghtOfStringInFile * 2) + (sizeof(short) * 2) + (sizeof(int) * 4) + sizeof(decimal) + sizeof(char) - 1;
        private const int IdOffsetInWrittenRecord = sizeof(short);
        private readonly IRecordValidator validator;
        private readonly BinaryWriter writer;
        private readonly BinaryReader reader;
        private readonly FileStream fileStream;
        private int nextId;

        public FileCabinetFilesystemService(FileStream fileStream, IRecordValidator validator)
        {
            this.validator = validator;
            this.fileStream = fileStream;
            this.writer = new BinaryWriter(fileStream, System.Text.Encoding.UTF8);
            this.reader = new BinaryReader(this.fileStream, System.Text.Encoding.UTF8);
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

            this.WriteRecord(SeekOrigin.End, 0, this.nextId, recordParameter, 0);

            return this.nextId++;
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

            int offset = this.GetRecordOffset(id);
            if (offset < 0)
            {
                throw new ArgumentException(id + " record is not existing", nameof(id));
            }

            this.validator.ValidateParameters(recordParameter);

            WriteRecord(SeekOrigin.Begin, offset, id, recordParameter, 0);
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
    }
}
