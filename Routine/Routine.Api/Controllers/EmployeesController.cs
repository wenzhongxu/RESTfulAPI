using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Routine.Api.Models;
using Routine.Api.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Routine.Api.Controllers
{
    [ApiController]
    [Route("api/companies/{companyId}/employees")]
    public class EmployeesController: ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ICompanyRepository _companyRepository;

        public EmployeesController(IMapper mapper, ICompanyRepository companyRepository)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

            _companyRepository = companyRepository ?? throw new ArgumentNullException(nameof(companyRepository));
        }

        public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetEmployees(Guid companyId)
        {
            var employees = await _companyRepository.GetEmployeesAsync(companyId);

            var employeeDtos = _mapper.Map<IEnumerable<EmployeeDto>>(employees);

            return Ok(employeeDtos);
        }
    }
}
