using PnLReporter.Models;
using PnLReporter.Repository;
using PnLReporter.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using PnLReporter.Helper;

namespace PnLReporter.Service
{
    public interface ITransactionCategoryService
    {
        IEnumerable<TransactionCategoryVModel> QueryByBrand(string query, string sort, int brandId, int offset, int limit);
        IEnumerable<Object> FilterColumns(string filter, IEnumerable<TransactionCategoryVModel> list);
        int GetQueryListLength(string query, int? brandId);
    }
    public class TransactionCategoryService : ITransactionCategoryService
    {
        private readonly ITransactionCategoryRepository _repository;

        public TransactionCategoryService(PLSystemContext context)
        {
            _repository = new TransactionCategoryRepository(context);
        }

        public IEnumerable<Object> FilterColumns(string filter, IEnumerable<TransactionCategoryVModel> list)
        {
            IEnumerable<object> result = list;
            if (filter == null)
            {
                return list;
            }
            filter = filter.ToLower();
            if (filter != "any" && filter != null && filter.Trim() != "")
            {
                List<string> fields = filter.Split(",").ToList();
                string filterStr = "";

                foreach (string field in fields)
                {
                    switch (field)
                    {
                        case "id":
                            filterStr += "Id,";
                            break;
                        case "name":
                            filterStr += "Name,";
                            break;
                        case "type":
                            filterStr += "Type,";
                            break;
                        case "required":
                            filterStr += "Required,";
                            break;
                        case "status":
                            filterStr += "Status,";
                            break;
                        case "brand":
                            filterStr += "Brand,";
                            break;
                        case "created-time":
                            filterStr += "CreatedTime,";
                            break;
                        case "last-modified":
                            filterStr += "LastModified,";
                            break;
                    }
                }

                if (filterStr.Length > 0) filterStr = filterStr.Substring(0, filterStr.Length - 1);
                if (fields.Count > 0)
                {
                    result = list.Select(Helper.Helper.DynamicSelectGenerator<TransactionCategoryVModel>(filterStr));
                }
            }

            return result;
        }

        public int GetQueryListLength(string query, int? brandId)
        {
            if (query == null) query = "";
            return _repository.GetQueryListLength(query, brandId);
        }

        public IEnumerable<TransactionCategoryVModel> QueryByBrand(string query, string sort, int brandId, int offset, int limit)
        {
            if (query == null) query = "";

            return this.ParseToVModel(_repository.QueryByBrand(query, sort, brandId, offset, limit));
        }

        private IEnumerable<TransactionCategoryVModel> ParseToVModel(IEnumerable<TransactionCategory> list)
        {
            List<TransactionCategoryVModel> result = new List<TransactionCategoryVModel>();

            foreach (TransactionCategory category in list)
            {
                var newCate = new TransactionCategoryVModel()
                {
                    Id = category.Id,
                    Name = category.Name,
                    Brand = category.Brand != null ? new BrandVModel()
                    {
                        Id = category.Brand.Id,
                        Name = category.Brand.Name,
                        Status = category.Brand.Status,
                        CreatedTime = category.Brand.CreatedTime
                    } : null,
                    Required = category.Required,
                    Type = category.Type,
                    Status = category.Status,
                    CreatedTime = category.CreatedTime,
                    LastModified = category.LastModified
                };

                result.Add(newCate);
            }

            return result;
        }
    }
}
