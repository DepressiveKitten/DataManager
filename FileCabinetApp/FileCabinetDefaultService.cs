using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    public class FileCabinetDefaultService : IRecordValidator
    {
        private static readonly DateTime MinimalValidDate = new DateTime(1950, 1, 1);
        private static readonly DateTime MaximumValidDate = DateTime.Now;

        public void ValidateFirstName(string firstName)
        {
            if (string.IsNullOrWhiteSpace(firstName) || firstName.Length < 2 || firstName.Length > 60)
            {
                throw new ArgumentException("first name should contain from 2 to 60 symbols", nameof(firstName));
            }
        }

        public void ValidateLastName(string lastName)
        {
            if (string.IsNullOrWhiteSpace(lastName) || lastName.Length < 2 || lastName.Length > 60)
            {
                throw new ArgumentException("last name should contain from 2 to 60 symbols", nameof(lastName));
            }
        }

        public void ValidateDateOfBirth(DateTime dateOfBirth)
        {
            if (dateOfBirth < MinimalValidDate || dateOfBirth > MaximumValidDate)
            {
                throw new ArgumentException("Enter a valid date", nameof(dateOfBirth));
            }
        }

        public void ValidateSalary(decimal salary)
        {
            if (salary < 0)
            {
                throw new ArgumentException("salary should be positive", nameof(salary));
            }
        }

        public void ValidateHeight(short height)
        {
            if (height < 100 || height > 220)
            {
                throw new ArgumentException("Enter a valid height", nameof(height));
            }
        }

        public void ValidateGrade(char grade)
        {
            if (!char.IsLetter(grade))
            {
                throw new ArgumentException("grade should conrain one letter", nameof(grade));
            }
        }
    }
}
