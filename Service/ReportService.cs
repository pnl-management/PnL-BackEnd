using PnLReporter.Repository;
using PnLReporter.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PnLReporter.Models;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace PnLReporter.Service
{
    public interface IReportService
    {
        IEnumerable<ReportVModel> GetReport(int brandId);
        IList<IList<object>> ListDataToGgSheet(int brandId, out int storeSize);
        ReportVModel GetReportOfStore(int storeId, int periodId);
        IEnumerable<ReportVModel> GetReportOfBrand(int brandId, int periodId);
    }
    public class ReportService : IReportService
    {
        private readonly IReportRepository _repository;
        private readonly IDistributedCache _cache;

        public ReportService(PLSystemContext context, IDistributedCache cache)
        {
            _repository = new ReportRepository(context);
            _cache = cache;
        }

        public IEnumerable<ReportVModel> GetReport(int brandId)
        {
            var listStore = _repository.GetListStoreOfBrand(brandId);

            var listPeriod = _repository.GetListClosedPeriod(brandId);

            var listReport = new List<ReportVModel>();

            foreach (var store in listStore)
            {
                foreach (var period in listPeriod)
                {
                    var id = period.Id + " - " + store.Id;

                    var lisTransaction = _repository.GetListTransactionOfStoreAndPeriod(store.Id, period.Id);
                    var report = new ReportVModel()
                    {
                        Period = new AccountingPeriodVModel()
                        {
                            Id = period.Id,
                            Brand = new BrandVModel()
                            {
                                Id = period.BrandId ?? default
                            },
                            StartDate = period.StartDate,
                            EndDate = period.EndDate,
                            Deadline = period.Deadline,
                            Title = period.Title,
                            Status = period.Status
                        },
                        Store = new StoreVModel()
                        {
                            Id = store.Id,
                            Brand = new BrandVModel()
                            {
                                Id = store.BrandId ?? default
                            },
                            Name = store.Name
                        },
                        ListTransactions = this.ParseToTransactionVModel(lisTransaction)
                    };

                    //_cache.SetString(report.Id, JsonConvert.SerializeObject(report));
                    listReport.Add(report);

                    /*
                    var cacheItem = _cache.GetString(id);
                    if (false)
                    {
                        var cacheParsed = JsonConvert.DeserializeObject<ReportVModel>(cacheItem);
                        var report = new ReportVModel()
                        {
                            Period = new AccountingPeriodVModel()
                            {
                                Id = period.Id,
                                Brand = new BrandVModel()
                                {
                                    Id = period.BrandId ?? default
                                },
                                StartDate = period.StartDate,
                                EndDate = period.EndDate,
                                Deadline = period.Deadline,
                                Title = period.Title,
                                Status = period.Status
                            },
                            Store = new StoreVModel()
                            {
                                Id = store.Id,
                                Brand = new BrandVModel()
                                {
                                    Id = store.BrandId ?? default
                                },
                                Name = store.Name
                            },
                            ListTransactions = cacheParsed.ListTransactions
                        };
                        listReport.Add(report);
                    }
                    else
                    {
                        
                    }*/
                }
            }
            return listReport;
        }

        public IEnumerable<ReportVModel> GetReportOfBrand(int brandId, int periodId)
        {
            var listStore = _repository.GetListStoreOfBrand(brandId);
            var result = new List<ReportVModel>();

            foreach (var store in listStore)
            {
                var listTran = _repository.GetListTransactionOfStoreAndPeriod(store.Id, periodId);

                var report = new ReportVModel()
                {
                    ListTransactions = listTran != null ? this.ParseToTransactionVModel(listTran) : null,
                    Period = new AccountingPeriodVModel() { Id = periodId },
                    Store = new StoreVModel() { Id = store.Id }
                };
                result.Add(report);
            }

            return result;
        }

        public ReportVModel GetReportOfStore(int storeId, int periodId)
        {
            var listTran = _repository.GetListTransactionOfStoreAndPeriod(storeId, periodId);
            var report = new ReportVModel()
            {
                ListTransactions = listTran != null ? this.ParseToTransactionVModel(listTran) : null,
                Period = new AccountingPeriodVModel() { Id = periodId },
                Store = new StoreVModel() { Id = storeId }
            };

            return report;
        }

        public IList<IList<object>> ListDataToGgSheet(int brandId, out int storeSize)
        {
            var listReport = this.GetReport(brandId);

            var listStore = _repository.GetListStoreOfBrand(brandId);
            storeSize = listStore.Count();

            var listPeriod = _repository.GetListClosedPeriod(brandId);

            var result = new List<IList<object>>();

            var firstRow = new List<object>();
            firstRow.Add("Quý");
            foreach (var store in listStore)
            {
                firstRow.Add(store.Name);
            }

            result.Add(firstRow);

            foreach (var period in listPeriod)
            {
                var row = new List<object>();
                row.Add(period.Title);

                foreach (var store in listStore)
                {
                    var reportInStore = listReport
                        .Where(record => record.Store.Id == store.Id && record.Period.Id == period.Id)
                        .FirstOrDefault();

                    if (reportInStore != null)
                    {
                        long profit = 0;

                        foreach (var trans in reportInStore.ListTransactions)
                        {
                            if (trans.Category.Type == 1)
                            {
                                long parse;
                                long.TryParse(trans.Value, out parse);
                                profit += parse;
                            }
                            else
                            {
                                long parse;
                                long.TryParse(trans.Value, out parse);
                                profit -= parse;
                            }
                        }
                        row.Add(profit + "");
                    }
                }
                result.Add(row);
            }
            return result;
        }

        private IEnumerable<TransactionVModel> ParseToTransactionVModel(IEnumerable<Transaction> transList)
        {
            var transVModelLst = new List<TransactionVModel>();

            if (transList != null)
            {
                transList.ToList().ForEach(trans =>
                {
                    transVModelLst.Add(new TransactionVModel()
                    {
                        Id = trans.Id,
                        Name = trans.Name,
                        Value = trans.Value,
                        Description = trans.Description,
                        Category = trans.Category != null ? new TransactionCategoryVModel()
                        {
                            Id = trans.Category.Id,
                            Name = trans.Category.Name,
                            Type = trans.Category.Type
                        } : null,
                        Period = trans.Period != null ? new AccountingPeriodVModel()
                        {
                            Id = trans.Period.Id,
                            Title = trans.Period.Title
                        } : null,
                        Brand = trans.Brand != null ? new BrandVModel()
                        {
                            Id = trans.Brand.Id,
                            Name = trans.Brand.Name
                        } : null,
                        Store = trans.Store != null ? new StoreVModel()
                        {
                            Id = trans.Store.Id,
                            Name = trans.Store.Name
                        } : null,
                        CreatedTime = trans.CreatedTime,
                        CreateByParticipant = trans.CreatedByNavigation != null ? new ParticipantVModel()
                        {
                            Id = trans.CreatedBy,
                            Username = trans.CreatedByNavigation.Username
                        } : null
                    });
                });
            }

            return transVModelLst;
        }
    }
}
