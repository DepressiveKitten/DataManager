using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace FileCabinetApp
{
    /// <summary>
    /// Reads records from xml documents.
    /// </summary>
    public class FileCabinetRecordXmlReader
    {
        private readonly TextReader textReader;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordXmlReader"/> class.
        /// </summary>
        /// <param name="reader">stream to read xml file from.</param>
        public FileCabinetRecordXmlReader(TextReader reader)
        {
            if (reader is null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            this.textReader = reader;
        }

        /// <summary>
        /// Read all records from document.
        /// </summary>
        /// <returns>Return all record.</returns>
        public IList<FileCabinetRecord> ReadAll()
        {
            IList<FileCabinetRecord> records;
            XmlSerializer serializer = new XmlSerializer(typeof(FileCabinetRecord[]));
            using (XmlReader xmlReader = XmlReader.Create(this.textReader))
            {
                FileCabinetRecord[] recordsArray = (FileCabinetRecord[])serializer.Deserialize(xmlReader);
                records = new List<FileCabinetRecord>(recordsArray);
            }

            return records;
        }
    }
}
