using System;
using System.Globalization;
using System.IO;
using System.Xml.Linq;

namespace FileCabinetApp
{
    /// <summary>
    /// Saves given records as xml documents.
    /// </summary>
    public class FileCabinetRecordXmlWriter
    {
        private const string OutputDateFormat = "yyyy-MMM-d";
        private readonly StreamWriter writer;
        private readonly XElement xmlRecords = new XElement("records");

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
            if (fileCabinetRecord is null)
            {
                throw new ArgumentNullException(nameof(fileCabinetRecord));
            }

            XElement xmlRecord = new XElement("record");
            XAttribute xmlId = new XAttribute("id", fileCabinetRecord.Id);
            xmlRecord.Add(xmlId);

            XElement xmlName = new XElement("name");
            XAttribute xmlFirst = new XAttribute("first", fileCabinetRecord.FirstName);
            XAttribute xmlLast = new XAttribute("last", fileCabinetRecord.LastName);
            xmlName.Add(xmlFirst);
            xmlName.Add(xmlLast);
            xmlRecord.Add(xmlName);

            XElement xmlDate = new XElement("dateofbirth", fileCabinetRecord.DateOfBirth.ToString(OutputDateFormat, DateTimeFormatInfo.InvariantInfo));
            XElement xmlHeight = new XElement("height", fileCabinetRecord.Height);
            XElement xmlSalary = new XElement("salary", fileCabinetRecord.Salary);
            XElement xmlGrade = new XElement("grade", fileCabinetRecord.Grade);
            xmlRecord.Add(xmlDate);
            xmlRecord.Add(xmlHeight);
            xmlRecord.Add(xmlSalary);
            xmlRecord.Add(xmlGrade);

            this.xmlRecords.Add(xmlRecord);
        }

        /// <summary>
        /// Create a new document with all given records.
        /// </summary>
        public void Save()
        {
            XDocument xmlDocument = new XDocument(this.xmlRecords);
            xmlDocument.Save(this.writer);
        }
    }
}
