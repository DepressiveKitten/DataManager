using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace FileCabinetApp
{
    /// <summary>
    /// Saves given records as csv documents.
    /// </summary>
    public class FileCabinetRecordCsvWriter
    {
        private const string FileFormat = "Id,First Name,Last Name,Date of Birth,Height,Salary,Grade";
        private const string OutputDateFormat = "yyyy-MMM-d";
        private const char Separator = '|';
        private readonly TextWriter textWriter;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordCsvWriter"/> class.
        /// </summary>
        /// <param name="writer">stream to save csv file to.</param>
        public FileCabinetRecordCsvWriter(TextWriter writer)
        {
            if (writer is null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            this.textWriter = writer;

            writer.WriteLine(FileFormat);
        }

        /// <summary>
        /// Add a new record to document.
        /// </summary>
        /// <param name="fileCabinetRecord">Record that should be added to document.</param>
        public void Write(FileCabinetRecord fileCabinetRecord)
        {
            if (fileCabinetRecord is null)
            {
                throw new ArgumentNullException(nameof(fileCabinetRecord));
            }

            this.textWriter.WriteLine($"{fileCabinetRecord.Id}{Separator}" +
                $"{fileCabinetRecord.FirstName}{Separator}" +
                $"{fileCabinetRecord.LastName}{Separator}" +
                $"{fileCabinetRecord.DateOfBirth.ToString(OutputDateFormat, DateTimeFormatInfo.InvariantInfo)}{Separator}" +
                $"{fileCabinetRecord.Height}{Separator}" +
                $"{fileCabinetRecord.Salary}{Separator}" +
                $"{fileCabinetRecord.Grade}");
        }
    }
}
