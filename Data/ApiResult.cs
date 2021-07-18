using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;

namespace WorldCities.Data
{
    public class ApiResult<T>
    {
        ///<summary>
        ///Private contructor called  by the creater async method
        /// </summary>
        private ApiResult(List<T> data, int count, int pageIndex, int pageSize)
        {
            Data = data;
            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalCount = count;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);

        }
        /// <summary>
        /// Pages a queriable source 
        /// </summary>
        /// <param name="source"> a queriable source of generic type </param>
        /// <param name="pageIndex">zero base indicates current base index</param>
        /// <param name="pageSize">actual size of each page</param>
        /// <returns>an object cointaining the page results  and all the revelant paging nav info </returns>
        public static async Task<ApiResult<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize) {
            var count = await source.CountAsync();
            source = source.Skip(pageIndex * pageSize).Take(pageSize);

            var data = await source.ToListAsync();
            return new ApiResult<T>(data, count, pageIndex, pageSize);

        }
        #region Properties
        ///<summary>
        ///the data results
        /// </summary>
        public List<T> Data { get; private set; }
        /// <summary>
        /// zero based index of current page 
        /// </summary>
        public int PageIndex { get; private set; }
        /// <summary>
        /// number of items contained in each page 
        /// </summary>
        public int PageSize { get; private set; }
        /// <summary>
        /// total item counts 
        /// </summary>
        public int TotalCount {get; private set;}
        /// <summary>
        /// total pages count 
        /// </summary>
        public int TotalPages { get; private set; }
        /// <summary>
        /// returns true if the current page has previous page otherwise False
        /// </summary>
        public bool HasPreviousPage { get { return (PageIndex > 0); } }
        /// <summary>
        /// True if current page has a next pafe otherwise False 
        /// </summary>
        public bool HasNextPage { get { return ((PageIndex + 1) < TotalPages); } }

        #endregion
    }
}
