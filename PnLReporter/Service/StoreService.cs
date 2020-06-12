using PnLReporter.Models;
using PnLReporter.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using PnLReporter.Repository;

namespace PnLReporter.Service
{
    public interface IStoreService
    {
        IEnumerable<StoreVModel> QueryByBrand(string query, int brandId, int offset, int limit);
        IEnumerable<StoreVModel> SortList(string sort, IEnumerable<StoreVModel> list);
        IEnumerable<Object> FilterColumns(string filter, IEnumerable<StoreVModel> list);
    }
    public class StoreService : IStoreService
    {
        private readonly IStoreRepository _repository;

        public StoreService (PLSystemContext context)
        {
            _repository = new StoreRepository(context);
        }
        public IEnumerable<object> FilterColumns(string filter, IEnumerable<StoreVModel> list)
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
                        case "phone":
                            filterStr += "Phone,";
                            break;
                        case "address":
                            filterStr += "Address,";
                            break;
                        case "status":
                            filterStr += "Status,";
                            break;
                        case "brand":
                            filterStr += "Brand,";
                            break;
                    }
                }

                if (filterStr.Length > 0) filterStr = filterStr.Substring(0, filterStr.Length - 1);
                if (fields.Count > 0)
                {
                    result = list.Select(Helper.Helper.DynamicSelectGenerator<StoreVModel>(filterStr));
                }
            }

            return result;
        }

        public IEnumerable<StoreVModel> QueryByBrand(string query, int brandId, int offset, int limit)
        {
            if (query == null) query = "";

            return this.ParseToVModel(_repository.QueryByBrand(query, brandId, offset, limit));
        }

        public IEnumerable<StoreVModel> SortList(string sort, IEnumerable<StoreVModel> list)
        {
            switch (sort.ToLower().Trim())
            {
                case "name-asc":
                    return list.OrderBy(record => record.Name);
                case "name-des":
                    return list.OrderByDescending(record => record.Name);
            }
            return list;
        }

        private IEnumerable<StoreVModel> ParseToVModel(IEnumerable<Store> list)
        {
            List<StoreVModel> result = new List<StoreVModel>();

            foreach (Store store in list)
            {
                var newStore = new StoreVModel()
                {
                    Id = store.Id,
                    Name = store.Name,
                    Brand = store.Brand != null ? new BrandVModel()
                    {
                        Id = store.Brand.Id,
                        Name = store.Brand.Name,
                        Status = store.Brand.Status,
                        CreatedTime = store.Brand.CreatedTime
                    } : null,
                    Phone = store.Phone,
                    Address = store.Address,
                    Status = store.Status
                };

                result.Add(newStore);
            }

            return result;
        }
    }
}
