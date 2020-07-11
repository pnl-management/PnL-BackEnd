using PnLReporter.Models;
using PnLReporter.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using PnLReporter.Repository;
using Microsoft.Extensions.Caching.Distributed;

namespace PnLReporter.Service
{
    public interface IStoreService
    {
        IEnumerable<StoreVModel> QueryByBrand(string query, string sort, int brandId, int offset, int limit);
        IEnumerable<Object> FilterColumns(string filter, IEnumerable<StoreVModel> list);
        int CountQueryList(string query, int brandId);
        StoreVModel GetById(int id);
    }
    public class StoreService : IStoreService
    {
        private readonly IStoreRepository _repository;

        public StoreService (PLSystemContext context, IDistributedCache cache)
        {
            _repository = new StoreRepository(context, cache);
        }

        public int CountQueryList(string query, int brandId)
        {
            if (query == null) query = "";
            return _repository.CountListQuery(query, brandId);
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

        public StoreVModel GetById(int id)
        {
            var result = _repository.GetById(id);

            if (result == null) return null;

            return this.ParseToVModel(new List<Store>() { result }).FirstOrDefault();
        }

        public IEnumerable<StoreVModel> QueryByBrand(string query, string sort, int brandId, int offset, int limit)
        {
            if (query == null) query = "";

            return this.ParseToVModel(_repository.QueryByBrand(query, sort, brandId, offset, limit));
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
                    Brand = new BrandVModel()
                    {
                        Id = store.Brand != null ? store.Brand.Id : (store.BrandId ?? default),
                        Name = store.Brand != null ? store.Brand.Name : null,
                        Status = store.Brand != null ? store.Brand.Status : null,
                        CreatedTime = store.Brand != null ? store.Brand.CreatedTime : null
                    },
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
