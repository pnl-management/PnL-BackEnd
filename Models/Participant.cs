using System;
using System.Collections.Generic;

namespace PnLReporter.Models
{
    public partial class Participant
    {
        public Participant()
        {
            BrandParticipantsDetail = new HashSet<BrandParticipantsDetail>();
            ReceiptCreatedByNavigation = new HashSet<Receipt>();
            ReceiptLastModifiedByNavigation = new HashSet<Receipt>();
            RecepitTransactionDetailCreatedBy = new HashSet<RecepitTransactionDetail>();
            RecepitTransactionDetailLastModifiedBy = new HashSet<RecepitTransactionDetail>();
            StoreParticipantsDetail = new HashSet<StoreParticipantsDetail>();
            Transaction = new HashSet<Transaction>();
            TransactionJourney = new HashSet<TransactionJourney>();
        }

        public int Id { get; set; }
        public string Username { get; set; }
        public string Fullname { get; set; }
        public DateTime? CreatedTime { get; set; }
        public DateTime? LastModified { get; set; }

        public virtual ICollection<BrandParticipantsDetail> BrandParticipantsDetail { get; set; }
        public virtual ICollection<Receipt> ReceiptCreatedByNavigation { get; set; }
        public virtual ICollection<Receipt> ReceiptLastModifiedByNavigation { get; set; }
        public virtual ICollection<RecepitTransactionDetail> RecepitTransactionDetailCreatedBy { get; set; }
        public virtual ICollection<RecepitTransactionDetail> RecepitTransactionDetailLastModifiedBy { get; set; }
        public virtual ICollection<StoreParticipantsDetail> StoreParticipantsDetail { get; set; }
        public virtual ICollection<Transaction> Transaction { get; set; }
        public virtual ICollection<TransactionJourney> TransactionJourney { get; set; }
    }
}
