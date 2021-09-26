using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Contains fields that associate with each record.
    /// </summary>
    public class RecordParameterObject
    {
        /// <summary>
        /// Gets the first name for the record.
        /// </summary>
        /// <value>First name of the record.</value>
        public string FirstName { get; init; }

        /// <summary>
        /// Gets the last name for the record.
        /// </summary>
        /// <value>Last name of the record.</value>
        public string LastName { get; init; }

        /// <summary>
        /// Gets the height of the person.
        /// </summary>
        /// <value>Height of the person.</value>
        public short Height { get; init; }

        /// <summary>
        /// Gets the salary of the person.
        /// </summary>
        /// <value>Salary of the person.</value>
        public decimal Salary { get; init; }

        /// <summary>
        /// Gets the grade of the person.
        /// </summary>
        /// <value>Grade of the person.</value>
        public char Grade { get; init; }

        /// <summary>
        /// Gets the date of birth of the person.
        /// </summary>
        /// <value>Date of birth of the person.</value>
        public DateTime DateOfBirth { get; init; }
    }
}
