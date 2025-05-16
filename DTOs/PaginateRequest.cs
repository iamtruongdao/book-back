using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.DTOs
{
    public class PaginateRequest
    {
        public string? CurrentFilter {get;set;}
        public string? SortOrder {get;set;}
        public string? SearchString {get;set;}
        public int? PageNumber {get;set;}
        public int PageSize {get;set;}
    }
}