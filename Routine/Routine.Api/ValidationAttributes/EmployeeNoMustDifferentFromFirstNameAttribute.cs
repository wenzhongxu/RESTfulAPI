using Routine.Api.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Routine.Api.ValidationAttributes
{
    public class EmployeeNoMustDifferentFromFirstNameAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            //var addDto = (EmployeeAddDto)value;
            var addDto = (EmployeeAddOrUpdateDto)validationContext.ObjectInstance;

            if (addDto.EmployeeNo == addDto.FirstName)
            {
                return new ValidationResult("员工编号不可以等于名", new[] { nameof(EmployeeAddOrUpdateDto) });
            }

            return ValidationResult.Success;
        }
    }
}
