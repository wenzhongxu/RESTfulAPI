﻿using Microsoft.EntityFrameworkCore;
using Routine.Api.DbContexts;
using Routine.Api.DtoParameters;
using Routine.Api.Entities;
using Routine.Api.Helpers;
using Routine.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Routine.Api.Services
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly RoutineDbContext _context;

        private readonly IPropertyMappingService _propertyMappingService;

        public CompanyRepository(RoutineDbContext context, IPropertyMappingService propertyMappingService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _propertyMappingService = propertyMappingService ?? throw new ArgumentNullException(nameof(propertyMappingService));
        }

        public async Task<IEnumerable<Employee>> GetEmployeesAsync(Guid companyId, EmployeeDtoParameters parameters)
        {
            if (companyId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(companyId));
            }


            var items = _context.Employees.Where(x => x.CompanyId == companyId);

            if (!string.IsNullOrWhiteSpace(parameters.Gender))
            {
                parameters.Gender = parameters.Gender.Trim();
                var gender = Enum.Parse<Gender>(parameters.Gender);

                items = items.Where(x => x.Gender == gender);
            }

            if (!string.IsNullOrWhiteSpace(parameters.Q))
            {
                parameters.Q = parameters.Q.Trim();
                items = items.Where(x => x.EmployeeNo.Contains(parameters.Q) || x.FirstName.Contains(parameters.Q) || x.LastName.Contains(parameters.Q));
            }

            //排序
            //if (!string.IsNullOrWhiteSpace(parameters.OrderBy))
            //{
            //    if (parameters.OrderBy.ToLowerInvariant() == "name")
            //    {
            //        items = items.OrderBy(x => x.FirstName).ThenBy(x => x.LastName);
            //    }
            //}
            // 上面的排序纯人肉，需要一堆的switch，下面采用映射

            var mappingDictionary = _propertyMappingService.GetPropertyMapping<EmployeeDto, Employee>();
            items = items.ApplySort(parameters.OrderBy, mappingDictionary);

            return await items.ToListAsync();


        }

        public async Task<Employee> GetEmployeeAsync(Guid companyId, Guid employeeId)
        {
            if (companyId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(companyId));
            }

            if (employeeId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(employeeId));
            }

            return await _context.Employees.Where(x => x.CompanyId == companyId && x.Id == employeeId).FirstOrDefaultAsync();
        }


        public void AddEmployee(Guid companyId, Employee employee)
        {
            if (companyId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(companyId));
            }

            if (employee == null)
            {
                throw new ArgumentNullException(nameof(employee));
            }

            employee.CompanyId = companyId;
            _context.Employees.Add(employee);
        }


        public void UpdateEmployee(Employee employee)
        {

        }


        public void DeleteEmployee(Employee employee)
        {
            _context.Employees.Remove(employee);
        }


        public async Task<PagedList<Company>> GetCompaniesAsync(CompanyDtoParameters parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            var queryExpression = _context.Companies as IQueryable<Company>;

            //过滤
            if (!string.IsNullOrWhiteSpace(parameters.CompanyName))
            {
                parameters.CompanyName = parameters.CompanyName.Trim();

                queryExpression = queryExpression.Where(x => x.Name == parameters.CompanyName);
            }

            //筛选
            if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
            {
                parameters.SearchTerm = parameters.SearchTerm.Trim();

                queryExpression = queryExpression.Where(x => x.Name.Contains(parameters.SearchTerm) || x.Introduction.Contains(parameters.CompanyName));
            }

            //排序
            var mappingDictionary = _propertyMappingService.GetPropertyMapping<CompanyDto, Company>();

            queryExpression = queryExpression.ApplySort(parameters.OrderBy, mappingDictionary);

            return await PagedList<Company>.CreateAsync(queryExpression, parameters.PageNumber, parameters.PageSize);
        }

        public async Task<Company> GetCompanyAsync(Guid companyId)
        {
            if (companyId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(companyId));
            }

            return await _context.Companies.FirstOrDefaultAsync(x => x.Id == companyId);
        }

        public async Task<IEnumerable<Company>> GetCompaniesAsync(IEnumerable<Guid> companyIds)
        {
            if (companyIds == null)
            {
                throw new ArgumentNullException(nameof(companyIds));
            }

            return await _context.Companies.Where(x => companyIds.Contains(x.Id)).OrderBy(a => a.Name).ToListAsync();
        }

        public void AddCompany(Company company)
        {
            if (company == null)
            {
                throw new ArgumentNullException(nameof(company));
            }

            company.Id = Guid.NewGuid();

            if (company.Employees != null)
            {

                foreach (var emplyee in company.Employees)
                {
                    emplyee.Id = Guid.NewGuid();
                }
            }

            _context.Companies.Add(company);
        }

        public void UpdateCompany(Company company)
        {

        }

        public void DeletCompany(Company company)
        {
            if (company == null)
            {
                throw new ArgumentNullException(nameof(company));
            }

            _context.Companies.Remove(company);
        }

        public async Task<bool> CompanyExistsAsync(Guid companyId)
        {
            if (companyId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(companyId));
            }

            return await _context.Companies.AnyAsync(x => x.Id == companyId);
        }

        public async Task<bool> SaveAsync()
        {
            return await _context.SaveChangesAsync() >= 0;
        }
    }
}
