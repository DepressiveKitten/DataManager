using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Interface for all validators.
    /// </summary>
    public interface IRecordValidator
    {
        /// <summary>
        /// validate all fields of record parameter.
        /// </summary>
        /// <param name="recordParameter">Object to validate.</param>
        public void ValidateParameters(RecordParameterObject recordParameter)
        {
            if (recordParameter is null)
            {
                throw new ArgumentNullException(nameof(recordParameter));
            }

            Tuple<bool, string> result;

            result = this.ValidateFirstName(recordParameter.FirstName);
            if (!result.Item1)
            {
                throw new ArgumentException(result.Item2, nameof(recordParameter));
            }

            result = this.ValidateLastName(recordParameter.LastName);
            if (!result.Item1)
            {
                throw new ArgumentException(result.Item2, nameof(recordParameter));
            }

            result = this.ValidateDateOfBirth(recordParameter.DateOfBirth);
            if (!result.Item1)
            {
                throw new ArgumentException(result.Item2, nameof(recordParameter));
            }

            result = this.ValidateHeight(recordParameter.Height);
            if (!result.Item1)
            {
                throw new ArgumentException(result.Item2, nameof(recordParameter));
            }

            result = this.ValidateSalary(recordParameter.Salary);
            if (!result.Item1)
            {
                throw new ArgumentException(result.Item2, nameof(recordParameter));
            }

            result = this.ValidateGrade(recordParameter.Grade);
            if (!result.Item1)
            {
                throw new ArgumentException(result.Item2, nameof(recordParameter));
            }
        }

        /// <summary>
        /// validate first name.
        /// </summary>
        /// <param name="firstName">string that should be validating.</param>
        /// <returns>result of validation and error message if validation fails.</returns>
        public Tuple<bool, string> ValidateFirstName(string firstName);

        /// <summary>
        /// validate last name.
        /// </summary>
        /// <param name="lastName">string that should be validating.</param>
        /// <returns>result of validation and error message if validation fails.</returns>
        public Tuple<bool, string> ValidateLastName(string lastName);

        /// <summary>
        /// validate date of birth.
        /// </summary>
        /// <param name="dateOfBirth">date that should be validating.</param>
        /// <returns>result of validation and error message if validation fails.</returns>
        public Tuple<bool, string> ValidateDateOfBirth(DateTime dateOfBirth);

        /// <summary>
        /// validate salary.
        /// </summary>
        /// <param name="salary">decimal that should be validating.</param>
        /// <returns>result of validation and error message if validation fails.</returns>
        public Tuple<bool, string> ValidateSalary(decimal salary);

        /// <summary>
        /// validate height.
        /// </summary>
        /// <param name="height">short integer that should be validating.</param>
        /// <returns>result of validation and error message if validation fails.</returns>
        public Tuple<bool, string> ValidateHeight(short height);

        /// <summary>
        /// validate grade.
        /// </summary>
        /// <param name="grade">char that should be validating.</param>
        /// <returns>result of validation and error message if validation fails.</returns>
        public Tuple<bool, string> ValidateGrade(char grade);
    }
}
