using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConnectMyDoc_Data_Layer.Context;
using ConnectMyDoc_Domain_Layer.Entity;
using ConnectMyDoc_Domain_Layer.Repository;
using Microsoft.EntityFrameworkCore;

namespace ConnectMyDoc_Data_Layer.Repositories
{

    public class PatientGuardianRepository : IPatientGuardianRepository
	{
		private readonly PatientCMDDbContext _dbContext = null;
		public PatientGuardianRepository(PatientCMDDbContext db)
		{
			this._dbContext = db;
		}
		public async Task<PatientGuardian> AddPatientGuardian(PatientGuardian patientGuardian)
		{
			await _dbContext.PatientGuardians.AddAsync(patientGuardian);
			await _dbContext.SaveChangesAsync();
			return patientGuardian;

		}

        public async Task<bool> DeletePatientGuardianAsync(int patientGuardianId)
        {
            var patientGuardian = await GetPatientGuardianByIdAsync(patientGuardianId);
            if(patientGuardian != null)
            {
                _dbContext.PatientGuardians.Remove(patientGuardian);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            else { return false; }
        }

        public async Task<PatientGuardian> GetPatientGuardianByIdAsync(int patientGuardianId)
        {
            return await _dbContext.PatientGuardians.FindAsync(patientGuardianId);
        }
        public async Task<PatientGuardian> UpdatePatientGuardianAsync(PatientGuardian patientGuardian)
        {
            var existingPatientGuardian = await _dbContext.PatientGuardians.FindAsync(patientGuardian.PatientGuardianId);
            if (existingPatientGuardian != null)
            {
                existingPatientGuardian.PatientGuardianName = patientGuardian.PatientGuardianName;
                existingPatientGuardian.PatientGuardianPhoneNumber = patientGuardian.PatientGuardianPhoneNumber;
                existingPatientGuardian.PatientGuardianRelationship = patientGuardian.PatientGuardianRelationship;
                await _dbContext.SaveChangesAsync();
            }
            return existingPatientGuardian;
        }
    }

}
