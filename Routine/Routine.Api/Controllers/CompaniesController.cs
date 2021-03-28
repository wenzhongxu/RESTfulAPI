using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Routine.Api.DtoParameters;
using Routine.Api.Entities;
using Routine.Api.Helpers;
using Routine.Api.Models;
using Routine.Api.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace Routine.Api.Controllers
{
    [ApiController]
    [Route("api/companies")]
    public class CompaniesController : ControllerBase
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IMapper _mapper;
        private readonly IPropertyMappingService _propertyMappingService;
        private readonly IPropertyCheckerService _propertyCheckerService;

        public CompaniesController(ICompanyRepository companyRepository, IMapper mapper, IPropertyMappingService propertyMappingService, IPropertyCheckerService propertyCheckerService)
        {
            _companyRepository = companyRepository ?? throw new ArgumentNullException(nameof(companyRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _propertyMappingService = propertyMappingService ?? throw new ArgumentNullException(nameof(propertyMappingService));
            _propertyCheckerService = propertyCheckerService ?? throw new ArgumentNullException(nameof(propertyCheckerService));
        }

        [HttpGet(Name = nameof(GetCompanies))]
        public async Task<IActionResult> GetCompanies([FromQuery] CompanyDtoParameters parameters)
        {
            if (!_propertyMappingService.ValidMappingExistsFor<CompanyDto, Company>(parameters.OrderBy))
            {
                return BadRequest();
            }

            if (!_propertyCheckerService.TypeHasProperties<CompanyDto>(parameters.Fields))
            {
                return BadRequest();
            }

            var companies = await _companyRepository.GetCompaniesAsync(parameters);

            var paginationMetadata = new
            {
                totalCount = companies.TotalCount,
                pageSize = companies.PageSize,
                currentPage = companies.CurrentPage,
                totalPages = companies.TotalPages
            };

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata, new JsonSerializerOptions 
            { 
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping //避免link的特殊字符被转义
            }));

            var companyDtos = _mapper.Map<IEnumerable<CompanyDto>>(companies);
            var shapedData = companyDtos.ShapeData(parameters.Fields);

            var links = CreateLinksForCompany(parameters, companies.HasPrevious, companies.HasNext);

            var shapedCompaniesWithLinks = shapedData.Select(c =>
            {
                var companyDict = c as IDictionary<string, object>;
                var companyLinks = CreateLinksForCompany((Guid)companyDict["Id"], null);
                companyDict.Add("links", companyLinks);
                return companyDict;
            });

            var linkedCollectionResource = new
            {
                value = shapedCompaniesWithLinks,
                links
            };

            return Ok(linkedCollectionResource);
        }

        [HttpGet("{companyId}", Name = nameof(GetCompany))]
        public async Task<IActionResult> GetCompany(Guid companyId, string fields)
        {
            /*
            //当多线程请求时，判断存在后，但未查询时，资源被删除了，会出错
            var exist = await _companyRepository.CompanyExistsAsync(companyId);
            if (!exist)
            {
                return NotFound();
            }
            */

            if (!_propertyCheckerService.TypeHasProperties<CompanyDto>(fields))
            {
                return BadRequest();
            }

            var company = await _companyRepository.GetCompanyAsync(companyId);

            if (company == null)
            {
                return NotFound();
            }

            var links = CreateLinksForCompany(companyId, fields);

            var linkedDict = _mapper.Map<CompanyDto>(company).ShapeData(fields)
                as IDictionary<string, object>;

            linkedDict.Add("links", links);

            return Ok(linkedDict);
        }

        [HttpPost(Name = nameof(CreateCompany))]
        public async Task<ActionResult<CompanyDto>> CreateCompany(CompanyAddDto company)
        {
            var entity = _mapper.Map<Company>(company);

            _companyRepository.AddCompany(entity);

            await _companyRepository.SaveAsync();

            var returnDto = _mapper.Map<CompanyDto>(entity);

            var links = CreateLinksForCompany(returnDto.Id, null);
            var linkedDict = returnDto.ShapeData(null) as IDictionary<string, object>;

            linkedDict.Add("links", links);


            return CreatedAtRoute(nameof(GetCompany), new { companyId = linkedDict["Id"] }, linkedDict);
        }

        [HttpOptions]
        public IActionResult GetCompaniesOptions()
        {
            Response.Headers.Add("Allow", "GET,POST,OPTIONS");
            return Ok();
        }

        [HttpDelete("{companyId}", Name = nameof(DeleteCompany))]
        public async Task<IActionResult> DeleteCompany(Guid companyId)
        {
            var companyEntity = await _companyRepository.GetCompanyAsync(companyId);

            if (companyEntity == null)
            {
                return NotFound();
            }

            await _companyRepository.GetEmployeesAsync(companyId, null);//级联删除为什么要先查出来？不查可不可以？

            _companyRepository.DeletCompany(companyEntity);
            await _companyRepository.SaveAsync();

            return NoContent();
        }

        private string CreateCompaniesResourceUri(CompanyDtoParameters parameters, ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return Url.Link(nameof(GetCompanies), new 
                    {
                        fields = parameters.Fields,
                        orderBy = parameters.OrderBy,
                        pageNumber = parameters.PageNumber - 1,
                        pageSize = parameters.PageSize,
                        companyName = parameters.CompanyName,
                        searchTerm = parameters.SearchTerm
                    });
                case ResourceUriType.NextPage:
                    return Url.Link(nameof(GetCompanies), new
                    {
                        fields = parameters.Fields,
                        orderBy = parameters.OrderBy,
                        pageNumber = parameters.PageNumber + 1,
                        pageSize = parameters.PageSize,
                        companyName = parameters.CompanyName,
                        searchTerm = parameters.SearchTerm
                    });
                case ResourceUriType.CurrentPage:
                default:
                    return Url.Link(nameof(GetCompanies), new
                    {
                        fields = parameters.Fields,
                        orderBy = parameters.OrderBy,
                        pageNumber = parameters.PageNumber,
                        pageSize = parameters.PageSize,
                        companyName = parameters.CompanyName,
                        searchTerm = parameters.SearchTerm
                    });
            }
        }

        private IEnumerable<LinkDto> CreateLinksForCompany(Guid companyId, string fields)
        {
            var links = new List<LinkDto>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                links.Add(new LinkDto(Url.Link(nameof(GetCompany), new { companyId }), "self", "GET"));
            }
            else
            {
                links.Add(new LinkDto(Url.Link(nameof(GetCompany), new { companyId, fields }), "self", "GET"));
            }

            links.Add(new LinkDto(Url.Link(nameof(DeleteCompany), new { companyId }), "delete_company", "DELETE"));

            links.Add(new LinkDto(Url.Link(nameof(EmployeesController.CreateEmployeeForCompany), new { companyId }), "create_employee_for_company", "POST"));

            links.Add(new LinkDto(Url.Link(nameof(EmployeesController.GetEmployeesForComapny), new { companyId }), "employees", "GET"));

            return links;
        }

        private IEnumerable<LinkDto> CreateLinksForCompany(CompanyDtoParameters parameters, bool hasPrevious, bool hasNext)
        {
            var links = new List<LinkDto>();

            links.Add(new LinkDto(CreateCompaniesResourceUri(parameters, ResourceUriType.CurrentPage), "self", "GET"));

            if (hasPrevious)
            {
                links.Add(new LinkDto(CreateCompaniesResourceUri(parameters, ResourceUriType.PreviousPage), "previous_page", "GET"));
            }

            if (hasNext)
            {
                links.Add(new LinkDto(CreateCompaniesResourceUri(parameters, ResourceUriType.NextPage), "next_page", "GET"));
            }

            return links;
        }
    }
}
