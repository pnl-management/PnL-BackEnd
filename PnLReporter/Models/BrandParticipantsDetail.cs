using System;
using System.Collections.Generic;

namespace PnLReporter.Models
{
    public partial class BrandParticipantsDetail
    {
        public int BrandId { get; set; }
        public int ParticipantsId { get; set; }
        public bool? Status { get; set; }
        public int? Role { get; set; }
        public string Description { get; set; }
        public DateTime? CreatedTime { get; set; }
        public DateTime? LastModified { get; set; }

        public virtual Brand Brand { get; set; }
        public virtual Participant Participants { get; set; }
    }
}
