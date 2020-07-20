﻿using System;
using System.Collections.Generic;

namespace PnLReporter.Models
{
    public partial class Brand
    {
        public Brand()
        {
            AccountingPeriod = new HashSet<AccountingPeriod>();
            BrandParticipantsDetail = new HashSet<BrandParticipantsDetail>();
            Receipt = new HashSet<Receipt>();
            Store = new HashSet<Store>();
            Transaction = new HashSet<Transaction>();
            TransactionCategory = new HashSet<TransactionCategory>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public bool? Status { get; set; }
        public DateTime? CreatedTime { get; set; }

        public virtual ICollection<AccountingPeriod> AccountingPeriod { get; set; }
        public virtual ICollection<BrandParticipantsDetail> BrandParticipantsDetail { get; set; }
        public virtual ICollection<Receipt> Receipt { get; set; }
        public virtual ICollection<Store> Store { get; set; }
        public virtual ICollection<Transaction> Transaction { get; set; }
        public virtual ICollection<TransactionCategory> TransactionCategory { get; set; }
    }
}
