using System;

namespace Routine.Api.DtoParameters
{
    public class CompanyDtoParameters
    {
        private const int maxPageSize = 20;

        public string CompanyName { get; set; }

        public string SearchTerm { get; set; }

        public int PageNumber { get; set; } = 1;

        private int _pageSize = 5;

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > maxPageSize) ? maxPageSize : value;
        }

        public string OrderBy { get; set; } = "CompanyName";

    }
}
