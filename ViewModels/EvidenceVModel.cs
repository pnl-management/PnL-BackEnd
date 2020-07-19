using PnLReporter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PnLReporter.ViewModels
{
    public class EvidenceVModel
    {
        public long? Id { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public ReceiptVModel Receipt { get; set; }
        public string Description { get; set; }
        public static EvidenceVModel ToVModel(Evidence model)
        {
            if (model == null) return null;

            var vmodel = new EvidenceVModel()
            {
                Id = model.Id,
                Url = model.Url,
                Title = model.Title,
                Receipt = null,
                Description = model.Description
            };

            return vmodel;
        }
    }
}
