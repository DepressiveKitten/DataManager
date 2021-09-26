using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Default validator.
    /// </summary>
    public class FileCabinetDefaultService : IRecordValidator
    {
        private static readonly DateTime MinimalValidDate = new DateTime(1950, 1, 1);
        private static readonly DateTime MaximumValidDate = DateTime.Now;

        /// <summary>
        /// validate first name.
        /// </summary>
        /// <param name="firstName">string that should be validating.</param>
        /// <returns>result of validation and error message if validation fails.</returns>
        public Tuple<bool, string> ValidateFirstName(string firstName)
        {
            if (string.IsNullOrWhiteSpace(firstName) || firstName.Length < 2 || firstName.Length > 60)
            {
                return new (false, "first name should contain from 2 to 60 symbols");
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
            if (string.IsNullOrWhiteSpace(lastName) || lastName.Length < 2 || lastName.Length > 60)
            {
                return new (false, "last name should contain from 2 to 60 symbols");
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
            if (salary < 0)
            {
                return new (false, "salary should be positive");
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
            if (height < 100 || height > 220)
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
            if (!char.IsLetter(grade))
            {
                return new (false, "grade should conrain one letter");
            }

            return new (true, string.Empty);
        }
    }
}
