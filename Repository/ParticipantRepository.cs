using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using PnLReporter.Models;

namespace PnLReporter.Repository
{
    public interface IParticipantRepository
    {
        Participant FindByUserId(long id);
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
                .Include(record => record.Brand)
                .Where(record => record.ParticipantsId == participantId)
                .FirstOrDefault<BrandParticipantsDetail>();
            
            return result;
        }

        public Participant FindByUserId(long id)
        {
            return _context.Participant
                .Where(record => record.Id == id).FirstOrDefault();
        }

        public Participant FindByUsername(string username)
        {
            return _context.Participant
                .Where(record => record.Username == username).FirstOrDefault<Participant>();
        }

        public StoreParticipantsDetail FindStoreParticipantById(int? participantsId)
        {
            var result = _context.StoreParticipantsDetail
                .Include(record => record.Store)
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
