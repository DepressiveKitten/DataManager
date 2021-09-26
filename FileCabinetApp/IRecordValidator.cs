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

            this.ValidateFirstName(recordParameter.FirstName);

            this.ValidateLastName(recordParameter.LastName);

            this.ValidateDateOfBirth(recordParameter.DateOfBirth);

            this.ValidateHeight(recordParameter.Height);

            this.ValidateSalary(recordParameter.Salary);

            this.ValidateGrade(recordParameter.Grade);
        }

        public void ValidateFirstName(string firstName);

        public void ValidateLastName(string lastName);

        public void ValidateDateOfBirth(DateTime dateOfBirth);

        public void ValidateSalary(decimal salary);

        public void ValidateHeight(short height);

        public void ValidateGrade(char grade);
    }
}
