using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Custom validator.
    /// </summary>
    public class FileCabinetCustomService : IRecordValidator
    {
        private static readonly DateTime MinimalValidDate = new DateTime(1900, 1, 1);
        private static readonly DateTime MaximumValidDate = DateTime.Now;

        /// <summary>
        /// validate first name.
        /// </summary>
        /// <param name="firstName">string that should be validating.</param>
        /// <returns>result of validation and error message if validation fails.</returns>
        public Tuple<bool, string> ValidateFirstName(string firstName)
        {
            if (string.IsNullOrWhiteSpace(firstName) || firstName.Length < 2 || firstName.Length > 10)
            {
                return new (false, "first name should contain from 2 to 10 symbols");
            }

            return new (true, string.Empty);
        }

        /// <summary>
        /// validate last name.
        /// </summary>
        /// <param name="lastName">string that should be validating.</param>
        /// <returns>result of validation and error message if validation fails.</returns>
        public Tuple<bool, string> ValidateLastName(string lastName)
        {
            if (string.IsNullOrWhiteSpace(lastName) || lastName.Length < 2 || lastName.Length > 20)
            {
                return new (false, "last name should contain from 2 to 20 symbols");
            }

            return new (true, string.Empty);
        }

        /// <summary>
        /// validate date of birth.
        /// </summary>
        /// <param name="dateOfBirth">date that should be validating.</param>
        /// <returns>result of validation and error message if validation fails.</returns>
        public Tuple<bool, string> ValidateDateOfBirth(DateTime dateOfBirth)
        {
            if (dateOfBirth < MinimalValidDate || dateOfBirth > MaximumValidDate)
            {
                return new (false, "Enter a valid date");
            }

            return new (true, string.Empty);
        }

        /// <summary>
        /// validate salary.
        /// </summary>
        /// <param name="salary">decimal that should be validating.</param>
        /// <returns>result of validation and error message if validation fails.</returns>
        public Tuple<bool, string> ValidateSalary(decimal salary)
        {
            if (salary < 0 || salary > 10000)
            {
                return new (false, "salary should be positive and lesser than 10000");
            }

            return new (true, string.Empty);
        }

        /// <summary>
        /// validate height.
        /// </summary>
        /// <param name="height">short integer that should be validating.</param>
        /// <returns>result of validation and error message if validation fails.</returns>
        public Tuple<bool, string> ValidateHeight(short height)
        {
            if (height < 50 || height > 250)
            {
                return new (false, "Enter a valid height");
            }

            return new (true, string.Empty);
        }

        /// <summary>
        /// validate grade.
        /// </summary>
        /// <param name="grade">char that should be validating.</param>
        /// <returns>result of validation and error message if validation fails.</returns>
        public Tuple<bool, string> ValidateGrade(char grade)
        {
            if (!char.IsDigit(grade))
            {
                return new (false, "grade should conrain one digit");
            }

            return new (true, string.Empty);
        }
    }
}
