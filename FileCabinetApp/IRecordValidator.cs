using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    public interface IRecordValidator
    {
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

        public Tuple<bool, string> ValidateFirstName(string firstName);

        public Tuple<bool, string> ValidateLastName(string lastName);

        public Tuple<bool, string> ValidateDateOfBirth(DateTime dateOfBirth);

        public Tuple<bool, string> ValidateSalary(decimal salary);

        public Tuple<bool, string> ValidateHeight(short height);

        public Tuple<bool, string> ValidateGrade(char grade);
    }
}
