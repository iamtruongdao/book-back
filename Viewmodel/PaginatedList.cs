using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Api;

namespace BackEnd.Viewmodel
{
    public class PaginatedList<T>
    {
        public int PageIndex { get; private set; }
        public int TotalPages { get; private set; }
        public List<T> Items { get; set; }
        public PaginatedList(List<T> items,int count ,int pageIndex,int pageSize) {
            PageIndex = pageIndex   ;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            Items = items;
        }
       
    }
}