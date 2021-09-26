using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    public class FileCabinetCustomService : FileCabinetService
    {
        private static readonly DateTime MinimalValidDate = new DateTime(1900, 1, 1);
        private static readonly DateTime MaximumValidDate = DateTime.Now;

        public override void ValidateFirstName(string firstName)
        {
            if (string.IsNullOrWhiteSpace(firstName) || firstName.Length < 2 || firstName.Length > 10)
            {
                throw new ArgumentException("first name should contain from 2 to 10 symbols", nameof(firstName));
            }
        }

        public override void ValidateLastName(string lastName)
        {
            if (string.IsNullOrWhiteSpace(lastName) || lastName.Length < 2 || lastName.Length > 20)
            {
                throw new ArgumentException("last name should contain from 2 to 20 symbols", nameof(lastName));
            }
        }

        public override void ValidateDateOfBirth(DateTime dateOfBirth)
        {
            if (dateOfBirth < MinimalValidDate || dateOfBirth > MaximumValidDate)
            {
                throw new ArgumentException("Enter a valid date", nameof(dateOfBirth));
            }
        }

        public override void ValidateSalary(decimal salary)
        {
            if (salary < 0 || salary > 10000)
            {
                throw new ArgumentException("salary should be positive and lesser than 10000", nameof(salary));
            }
        }

        public override void ValidateHeight(short height)
        {
            if (height < 50 || height > 250)
            {
                throw new ArgumentException("Enter a valid height", nameof(height));
            }
        }

        public override void ValidateGrade(char grade)
        {
            if (!char.IsDigit(grade))
            {
                throw new ArgumentException("grade should conrain one digit", nameof(grade));
            }
        }
    }
}
