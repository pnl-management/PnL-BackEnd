using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PnLReporter.Models;
using PnLReporter.Repository;
using PnLReporter.ViewModels;

namespace PnLReporter.Service
{
    public interface IParticipantService
    {
        UserModel FindByUserId(long id);
        UserModel FindByUsername(string username);
    }
    public class ParticipantService : IParticipantService
    {
        private IParticipantRepository _repository;

        public ParticipantService(PLSystemContext context)
        {
            _repository = new ParticipantRepository(context);
        }

        public UserModel FindByUserId(long id)
        {
            var participant = _repository.FindByUserId(id);
            if (participant != null)
            {
                var storeParticipant = _repository.FindStoreParticipantById(participant.Id);
                var brandParicipant = _repository.FindBrandParticipantsById(participant.Id);

                return new UserModel()
                {
                    Id = participant.Id,
                    Username = participant.Username,
                    Fullname = participant.Fullname,
                    Role = brandParicipant?.Role,
                    Description = brandParicipant?.Description,
                    Store = (storeParticipant != null && storeParticipant.Store != null) ? new StoreVModel()
                    {
                        Id = storeParticipant.Store.Id,
                        Name = storeParticipant.Store.Name,
                        Address = storeParticipant.Store.Address,
                        Phone = storeParticipant.Store.Phone,
                        Status = storeParticipant.Store.Status

                    } : null,
                    Brand = (brandParicipant != null && brandParicipant.Brand != null) ? new BrandVModel()
                    {
                        Id = brandParicipant.Brand.Id,
                        Name = brandParicipant.Brand.Name,
                        Status = brandParicipant.Brand.Status
                    } : null
                };
            }
            return null;
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
                    Role = brandParicipant?.Role,
                    Description = brandParicipant?.Description,
                    Store = (storeParticipant != null && storeParticipant.Store != null) ? new StoreVModel()
                    {
                        Id = storeParticipant.Store.Id,
                        Name = storeParticipant.Store.Name,
                        Address = storeParticipant.Store.Address,
                        Phone = storeParticipant.Store.Phone,
                        Status = storeParticipant.Store.Status

                    } : null,
                    Brand = (brandParicipant != null && brandParicipant.Brand != null) ? new BrandVModel()
                    {
                        Id = brandParicipant.Brand.Id,
                        Name = brandParicipant.Brand.Name,
                        Status = brandParicipant.Brand.Status
                    } : null
                };
            }
            return null;
        }
    }
}
