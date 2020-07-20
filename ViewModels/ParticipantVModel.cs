using PnLReporter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PnLReporter.ViewModels
{
    public class ParticipantVModel
    {
        public int? Id { get; set; }
        public string Username { get; set; }
        public string Fullname { get; set; }
        public DateTime? CreatedTime { get; set; }
        public DateTime? LastModified { get; set; }

        public static ParticipantVModel ToVModel(Participant model)
        {
            if (model == null) return null;

            var vmodel = new ParticipantVModel()
            {
                Id = model.Id,
                Username = model.Username,
                Fullname = model.Fullname,
                CreatedTime = model.CreatedTime,
                LastModified = model.LastModified
            };

            return vmodel;
        }
    }
}
