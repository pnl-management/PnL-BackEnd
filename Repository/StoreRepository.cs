using PnLReporter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace PnLReporter.Repository
{
    public interface IStoreRepository
    {
        IEnumerable<Store> QueryByBrand(string query, string sort, int brandId, int offset, int limit);
        int CountListQuery(string query, int brandId);
        Store GetById(int id);
        Brand GetBrandOfStore(int storeId);
    }
    public class StoreRepository : IStoreRepository
    {
        private readonly PLSystemContext _context;
        private readonly IDistributedCache _distributedCache;

        public StoreRepository(PLSystemContext context, IDistributedCache cache)
        {
            _context = context;
            _distributedCache = cache;
        }

        public int CountListQuery(string query, int brandId)
        {
            return this.GetQueryStatement(query, "", brandId).Count();
        }

        public Brand GetBrandOfStore(int storeId)
        {
            var store = this.GetById(storeId);
            if (store == null) return null;

            var detail = _context.StoreParticipantsDetail
                .Where(record => record.StoreId == store.Id)
                .FirstOrDefault();

            if (detail == null) return null;

            var participantId = detail.ParticipantId;

            var brandParticipant = _context.BrandParticipantsDetail
                .Where(record => record.ParticipantsId == participantId)
                .FirstOrDefault();

            if (brandParticipant == null) return null;

            var brand = _context.Brand.Find(brandParticipant.BrandId);

            if (brand == null) return null;
            return brand;
        }

        public Store GetById(int id)
        {
            Store result;
            if (_distributedCache != null)
            {
                string json = _distributedCache.GetString(id + "");
                if (json == null)
                {
                    result = _context.Store
                        .Include(record => record.Brand)
                        .Where(record => record.Id == id)
                        .FirstOrDefault();

                    if (result != null)
                    {
                        var options = new DistributedCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromDays(1));
                        _distributedCache.SetString(id + "", JsonConvert.SerializeObject(result), options);
                    }
                }
                else
                {
                    result = JsonConvert.DeserializeObject<Store>(json);
                }
            }
            else
            {
                result = _context.Store
                        .Include(record => record.Brand)
                        .Where(record => record.Id == id)
                        .FirstOrDefault();
            }

            return result;
        }

        public IEnumerable<Store> QueryByBrand(string query, string sort, int brandId, int offset, int limit)
        {
            return this.GetQueryStatement(query, sort, brandId).Skip(offset).Take(limit).ToList();
        }

        private IQueryable<Store> GetQueryStatement(string query, string sort, int brandId)
        {
            IQueryable<Store> result =
                _context.Store
                .Include(record => record.Brand)
                .Where(record => record.BrandId == brandId);

            if (sort != null && sort.Length > 0)
            {
                switch (sort.ToLower().Trim())
                {
                    case "name-asc":
                        result = result.OrderBy(record => record.Name);
                        break;
                    case "name-des":
                        result = result.OrderByDescending(record => record.Name);
                        break;
                }
            }

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

            return result;
        }
    }
}
