using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MeetupApp.API.Models
{
    public class PagedList<T> : List<T>
    {
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }

        public PagedList(int totalCount, int PageSize, int CurrentPage, List<T> items)
        {
            this.PageSize = PageSize;
            this.CurrentPage = CurrentPage;
            this.TotalPages = (int)Math.Ceiling(totalCount / (double)PageSize);
            this.TotalCount = totalCount;
            this.AddRange(items);
        }

        public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize)
        {
            int count = await source.CountAsync();
            var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PagedList<T>(count, pageSize, pageNumber, items);
        }

    }
}