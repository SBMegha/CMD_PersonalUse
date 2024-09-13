using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConnectMyDoc_Data_Layer.Context;
using ConnectMyDoc_Domain_Layer.Entity;
using ConnectMyDoc_Domain_Layer.Repository;

namespace ConnectMyDoc_Data_Layer.Repositories
{
    public class PatientAddressRepository : IPatientAddressRepository
    {
        private readonly PatientCMDDbContext _dbContext = null;
        public PatientAddressRepository(PatientCMDDbContext db)
        {
            this._dbContext = db;
        }

        public async Task<PatientAddress> AddPatientAddressAsync(PatientAddress patientAddress)
        {
            await _dbContext.PatientAddresses.AddAsync(patientAddress);
            await _dbContext.SaveChangesAsync();
            return patientAddress;
        }

        public Task<bool> DeletePatientAddressAsync(int patientId)
        {
            throw new NotImplementedException();
        }

        public async Task<PatientAddress> GetPatientAddressByIdAsync(int patientAddressId)
        {
            return await _dbContext.PatientAddresses.FindAsync(patientAddressId);
        }

        public async Task<PatientAddress> UpdatePatientAddressAsync(PatientAddress patientAddress)
        {
            var existingPatientAddress=await _dbContext.PatientAddresses.FindAsync(patientAddress.PatientAddressId);
            if (existingPatientAddress != null)
            {
                _dbContext.Entry(existingPatientAddress).CurrentValues.SetValues(patientAddress);
                await _dbContext.SaveChangesAsync();
            }
            return existingPatientAddress;
        }
    }
}
