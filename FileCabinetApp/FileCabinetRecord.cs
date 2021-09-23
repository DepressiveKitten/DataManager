using System;

namespace FileCabinetApp
{
    /// <summary>
    /// Contains fields that associate with each record.
    /// </summary>
    public class FileCabinetRecord
    {
        /// <summary>
        /// Gets or sets the Id of the record.
        /// </summary>
        /// <value>Id of the record.</value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the first name for the record.
        /// </summary>
        /// <value>First name of the record.</value>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name for the record.
        /// </summary>
        /// <value>Last name of the record.</value>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the height of the person.
        /// </summary>
        /// <value>Height of the person.</value>
        public short Height { get; set; }

        /// <summary>
        /// Gets or sets the salary of the person.
        /// </summary>
        /// <value>Salary of the person.</value>
        public decimal Salary { get; set; }

        /// <summary>
        /// Gets or sets the grade of the person.
        /// </summary>
        /// <value>Grade of the person.</value>
        public char Grade { get; set; }

        /// <summary>
        /// Gets or sets the date of birth of the person.
        /// </summary>
        /// <value>Date of birth of the person.</value>
        public DateTime DateOfBirth { get; set; }
    }
}
