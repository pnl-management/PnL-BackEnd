using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PnLReporter.Models;
using PnLReporter.Repository;

namespace PnLReporter.Service
{
    public interface IParticipantService
    {
        UserModel FindByUsername(string username);
    }
    public class ParticipantService : IParticipantService
    {
        private PLSystemContext _context;
        private IParticipantRepository _repository;

        public ParticipantService(IParticipantRepository repository)
        {
            _repository = repository;
        }

        public UserModel FindByUsername(string username)
        {
            var participant = _repository.FindByUsername(username);
            if (participant != null)
            {
                var storeParticipant = _repository.FindStoreParticipantById(participant.Id);
                var brandParicipant = _repository.FindBrandParticipantsById(participant.Id);

                return new UserModel()
                {
                    Id = participant.Id,
                    Username = participant.Username,
                    Fullname = participant.Fullname,
                    Role = (brandParicipant != null) ? brandParicipant.Role : null,
                    Description = (brandParicipant != null) ? brandParicipant.Description : null,
                    Store = (storeParticipant != null) ? storeParticipant.Store : null,
                    Brand = (brandParicipant != null) ? brandParicipant.Brand : null
                };
            }
            return null;
        }
    }
}
