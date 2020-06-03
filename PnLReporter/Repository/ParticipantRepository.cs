using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PnLReporter.Models;

namespace PnLReporter.Repository
{
    public interface IParticipantRepository
    {
        Participant FindByUsername(string username);
        StoreParticipantsDetail FindStoreParticipantById(int? participantId);
        BrandParticipantsDetail FindBrandParticipantsById(int? participantId);
    }
    public class ParticipantRepository : IParticipantRepository
    {
        private PLSystemContext _context;

        public ParticipantRepository(PLSystemContext context)
        {
            _context = context;
        }

        public BrandParticipantsDetail FindBrandParticipantsById(int? participantId)
        {
            var result = _context.BrandParticipantsDetail
                .Where(record => record.ParticipantsId == participantId)
                .FirstOrDefault<BrandParticipantsDetail>();
            if (result != null) {
                var tmpBrand = _context.Brand
                    .Where(record => record.Id == result.BrandId)
                    .Select(record => new { record.Id, record.Name, record.Status, record.CreatedTime})
                    .FirstOrDefault();

                result.Brand = new Brand()
                {
                    Id = tmpBrand.Id,
                    Name = tmpBrand.Name,
                    Status = tmpBrand.Status,
                    CreatedTime = tmpBrand.CreatedTime
                };
            }
            return result;
        }

        public Participant FindByUsername(string username)
        {
            return _context.Participant
                .Where(record => record.Username == username).FirstOrDefault<Participant>();
        }

        public StoreParticipantsDetail FindStoreParticipantById(int? participantsId)
        {
            var result = _context.StoreParticipantsDetail
                .Where(record => record.ParticipantId == participantsId)
                .FirstOrDefault<StoreParticipantsDetail>();

            if (result != null)
            {
                var tmpStore = _context.Store
                    .Where(record => record.Id == result.StoreId)
                    .Select(record => new { record.Id, record.Name, record.Phone, record.Address, record.Status })
                    .FirstOrDefault();

                result.Store = new Store()
                {
                    Id = tmpStore.Id,
                    Name = tmpStore.Name,
                    Phone = tmpStore.Phone,
                    Address = tmpStore.Address,
                    Status = tmpStore.Status
                };
            }
            return result;
        }
    }
}
