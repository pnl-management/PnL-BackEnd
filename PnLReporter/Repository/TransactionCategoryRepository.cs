﻿using PnLReporter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace PnLReporter.Repository
{
    public interface ITransactionCategoryRepository
    {
        IEnumerable<TransactionCategory> QueryByBrand(string query, int brandId, int offset, int limit);
    }
    public class TransactionCategoryRepository : ITransactionCategoryRepository
    {
        private readonly PLSystemContext _context;

        public TransactionCategoryRepository(PLSystemContext context)
        {
            _context = context;
        }

        public IEnumerable<TransactionCategory> QueryByBrand(string query, int brandId, int offset, int limit)
        {
            IQueryable<TransactionCategory> result =
                _context.TransactionCategory
                .Include(record => record.Brand)
                .Where(trans => trans.BrandId == brandId)
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
                        case "type":
                            int type;
                            if (int.TryParse(value, out type))
                            {
                                switch (opt)
                                {
                                    case "eq":
                                        result = result.Where(record => record.Type == type);
                                        break;
                                }
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
                        case "required":
                            bool required = value == "true" ? true : false;

                            switch (opt)
                            {
                                case "eq":
                                    result = result.Where(record => record.Required == required);
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
