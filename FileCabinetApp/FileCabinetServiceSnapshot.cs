using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace FileCabinetApp
{
    /// <summary>
    /// Instance of state of service class.
    /// </summary>
    public class FileCabinetServiceSnapshot
    {
        private readonly FileCabinetRecord[] records;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetServiceSnapshot"/> class.
        /// </summary>
        /// <param name="records">Records that should be saved in snapshot.</param>
        public FileCabinetServiceSnapshot(FileCabinetRecord[] records)
        {
            this.records = records;
        }

        /// <summary>
        /// Saves snapshot as csv document.
        /// </summary>
        /// <param name="writer">stream to save file to.</param>
        public void SaveToCSV(StreamWriter writer)
        {
            FileCabinetRecordCsvWriter csvWriter = new FileCabinetRecordCsvWriter(writer);
            foreach (FileCabinetRecord record in this.records)
            {
                csvWriter.Write(record);
            }
        }

        /// <summary>
        /// Saves snapshot as xml document.
        /// </summary>
        /// <param name="writer">stream to save file to.</param>
        public void SaveToXML(StreamWriter writer)
        {
            FileCabinetRecordXmlWriter csvWriter = new FileCabinetRecordXmlWriter(writer);
            foreach (FileCabinetRecord record in this.records)
            {
                csvWriter.Write(record);
            }

            csvWriter.Save();
        }
    }
}
