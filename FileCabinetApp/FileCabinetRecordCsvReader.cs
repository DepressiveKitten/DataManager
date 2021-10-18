using System;
using System.Collections.Generic;
using System.IO;

namespace FileCabinetApp
{
    /// <summary>
    /// Reads records from csv documents.
    /// </summary>
    public class FileCabinetRecordCsvReader
    {
        private const char Separator = '|';
        private readonly TextReader textReader;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordCsvReader"/> class.
        /// </summary>
        /// <param name="reader">stream to read csv file from.</param>
        public FileCabinetRecordCsvReader(TextReader reader)
        {
            if (reader is null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            this.textReader = reader;

            reader.ReadLine();
        }

        /// <summary>
        /// Read all records from document.
        /// </summary>
        /// <returns>Return all record.</returns>
        public IList<FileCabinetRecord> ReadAll()
        {
            IList<FileCabinetRecord> records = new List<FileCabinetRecord>();
            FileCabinetRecord record;
            while ((record = this.Read()) != null)
            {
                records.Add(record);
            }

            return records;
        }

        /// <summary>
        /// Read next record from document.
        /// </summary>
        /// <returns>Return next record or null if no records left.</returns>
        public FileCabinetRecord Read()
        {
            if (this.textReader.Peek() < 0)
            {
                return null;
            }

            var writenRecord = this.textReader.ReadLine().Split(Separator, 7);

            if (writenRecord.Length < 7)
            {
                return null;
            }

            int id;
            if (!int.TryParse(writenRecord[0], out id))
            {
                return null;
            }

            DateTime dateOfBirth;
            if (!DateTime.TryParse(writenRecord[3], out dateOfBirth))
            {
                return null;
            }

            short height;
            if (!short.TryParse(writenRecord[4], out height))
            {
                return null;
            }

            decimal salary;
            if (!decimal.TryParse(writenRecord[5], out salary))
            {
                return null;
            }

            return new FileCabinetRecord()
            {
                Id = id,
                FirstName = writenRecord[1].Trim(),
                LastName = writenRecord[2].Trim(),
                DateOfBirth = dateOfBirth,
                Height = height,
                Salary = salary,
                Grade = writenRecord[6][0],
            };
        }
    }
}
