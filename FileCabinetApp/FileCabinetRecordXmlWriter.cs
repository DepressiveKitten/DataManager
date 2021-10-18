using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace FileCabinetApp
{
    /// <summary>
    /// Saves given records as xml documents.
    /// </summary>
    public class FileCabinetRecordXmlWriter
    {
        private readonly StreamWriter writer;
        private readonly List<FileCabinetRecord> records = new List<FileCabinetRecord>();

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordXmlWriter"/> class.
        /// </summary>
        /// <param name="writer">stream to save xml file to.</param>
        public FileCabinetRecordXmlWriter(StreamWriter writer)
        {
            if (writer is null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            this.writer = writer;
        }

        /// <summary>
        /// Add a new record to document.
        /// </summary>
        /// <param name="fileCabinetRecord">Record that should be added to document.</param>
        public void Write(FileCabinetRecord fileCabinetRecord)
        {
            this.records.Add(fileCabinetRecord);
        }

        /// <summary>
        /// Create a new document with all given records.
        /// </summary>
        public void Save()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(FileCabinetRecord[]));
            serializer.Serialize(this.writer, this.records.ToArray());
        }
    }
}
