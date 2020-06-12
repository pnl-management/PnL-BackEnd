using PnLReporter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace PnLReporter.Repository
{
    public interface IStoreRepository
    {
        IEnumerable<Store> QueryByBrand(string query, int brandId, int offset, int limit);
    }
    public class StoreRepository : IStoreRepository
    {
        private readonly PLSystemContext _context;

        public StoreRepository(PLSystemContext context)
        {
            _context = context;
        }

        public IEnumerable<Store> QueryByBrand(string query, int brandId, int offset, int limit)
        {
            IQueryable<Store> result =
                _context.Store
                .Include(record => record.Brand)
                .Where(record => record.BrandId == brandId)
                .Skip(offset).Take(limit);

            List<string> queryComponent = new List<string>();
            queryComponent = query.Split(",").ToList();
            string field;
            string opt;
            string value;

            foreach (string queryContent in queryComponent)
            {
                string queryContentVal = queryContent.Trim();
                if (queryContentVal.Length > 0)
                {
                    field = queryContentVal.Substring(0, queryContentVal.IndexOf("["));
                    if (String.IsNullOrEmpty(field))
                    {
                        break;
                    }

                    if (queryContentVal.IndexOf("[") == -1 || queryContentVal.IndexOf("]") == -1 ||
                        queryContentVal.IndexOf("[") > queryContentVal.IndexOf("]")) break;

                    opt = queryContentVal.Substring(queryContentVal.IndexOf("[") + 1,
                        queryContentVal.IndexOf("]") - queryContentVal.IndexOf("[") - 1);

                    value = queryContentVal
                        .Substring(queryContentVal.IndexOf("]") + 1,
                        queryContentVal.Length - (queryContentVal.IndexOf("]") + 1));

                    switch (field)
                    {
                        case "name":
                            switch (opt)
                            {
                                case "eq":
                                    result = result.Where(record => record.Name == value);
                                    break;
                                case "like":
                                    result = result.Where(record => record.Name.Contains(value));
                                    break;
                            }
                            break;
                        case "phone":
                            switch (opt)
                            {
                                case "eq":
                                    result = result.Where(record => record.Phone == value);
                                    break;
                                case "like":
                                    result = result.Where(record => record.Phone.Contains(value));
                                    break;
                            }
                            break;
                        case "status":
                            bool status = value == "true" ? true : false;

                            switch (opt)
                            {
                                case "eq":
                                    result = result.Where(record => record.Status == status);
                                    break;
                            }
                            break;
                        case "address":
                            switch (opt)
                            {
                                case "eq":
                                    result = result.Where(record => record.Address == value);
                                    break;
                                case "like":
                                    result = result.Where(record => record.Address.Contains(value));
                                    break;
                            }
                            break;
                    }
                }
            }

            return result.ToList();
        }
    }
}
