using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace FileCabinetApp
{
    public class FileCabinetRecordCsvWriter
    {
        private const string FileFormat = "Id,First Name,Last Name,Date of Birth,Height,Salary,Grade";
        private const string OutputDateFormat = "yyyy-MMM-d";
        private TextWriter textWriter;

        public FileCabinetRecordCsvWriter(TextWriter writer)
        {
            if (writer is null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            this.textWriter = writer;

            writer.WriteLine(FileFormat);
        }

        public void Write(FileCabinetRecord fileCabinetRecord)
        {
            if (fileCabinetRecord is null)
            {
                throw new ArgumentNullException(nameof(fileCabinetRecord));
            }

            this.textWriter.WriteLine($"{fileCabinetRecord.Id}," +
                $"{fileCabinetRecord.FirstName}," +
                $"{fileCabinetRecord.LastName}," +
                $"{fileCabinetRecord.DateOfBirth.ToString(OutputDateFormat, DateTimeFormatInfo.InvariantInfo)}," +
                $"{fileCabinetRecord.Height}," +
                $"{fileCabinetRecord.Salary}," +
                $"{fileCabinetRecord.Grade}");
        }
    }
}
