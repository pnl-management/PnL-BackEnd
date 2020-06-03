﻿using System;
using System.Collections.Generic;

namespace PnLReporter.Models
{
    public partial class Participant
    {
        public Participant()
        {
            BrandParticipantsDetail = new HashSet<BrandParticipantsDetail>();
            StoreParticipantsDetail = new HashSet<StoreParticipantsDetail>();
            TransactionJourney = new HashSet<TransactionJourney>();
        }

        public int Id { get; set; }
        public string Username { get; set; }
        public string Fullname { get; set; }
        public DateTime? CreatedTime { get; set; }
        public DateTime? LastModified { get; set; }

        public virtual ICollection<BrandParticipantsDetail> BrandParticipantsDetail { get; set; }
        public virtual ICollection<StoreParticipantsDetail> StoreParticipantsDetail { get; set; }
        public virtual ICollection<TransactionJourney> TransactionJourney { get; set; }
    }
}
