using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Routine.Api.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Routine.Api.Controllers
{
    [ApiController]
    [Route("api/companies")]
    public class CompaniesController : ControllerBase
    {
        private readonly ICompanyRepository _companyRepository;

        public CompaniesController(ICompanyRepository companyRepository)
        {
            _companyRepository = companyRepository ?? throw new ArgumentNullException(nameof(companyRepository));
        }

        [HttpGet]
        public async Task<IActionResult> GetCompanies()
        {
            var companies = await _companyRepository.GetCompaniesAsync();
            return Ok(companies);
        }

        [HttpGet("{companyId}")]
        public async Task<IActionResult> GetCompanies(Guid companyId)
        {
            /*
            //当多线程请求时，判断存在后，但未查询时，资源被删除了，会出错
            var exist = await _companyRepository.CompanyExistsAsync(companyId);
            if (!exist)
            {
                return NotFound();
            }
            */

            var company = await _companyRepository.GetCompanyAsync(companyId);

            if (company == null)
            {
                return NotFound();
            }
            return Ok(company);
        }
    }
}
