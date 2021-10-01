using System;
using System.Collections.Generic;
using System.IO;

namespace FileCabinetApp
{
    public class FileCabinetServiceSnapshot
    {
        private FileCabinetRecord[] records;

        public FileCabinetServiceSnapshot(FileCabinetRecord[] records)
        {
            this.records = records;
        }

        public void SaveToCSV(StreamWriter writer)
        {
            FileCabinetRecordCsvWriter csvWriter = new FileCabinetRecordCsvWriter(writer);
            foreach (FileCabinetRecord record in this.records)
            {
                csvWriter.Write(record);
            }
        }

        public void SaveToXML(StreamWriter writer)
        {

        }
    }
}
