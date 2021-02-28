using Routine.Api.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Routine.Api.Models
{
    public class EmployeeAddDto
    {
        public string EmployeeNo { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public Gender Gender { get; set; }

        public DateTimeOffset DateOfBirth { get; set; }
    }
}
