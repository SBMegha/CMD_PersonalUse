using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConnectMyDoc_Domain_Layer.Entity;

namespace ConnectMyDoc_Domain_Layer.Repository
{
	public interface IPatientGuardianRepository
	{
		Task<PatientGuardian> AddPatientGuardian(PatientGuardian patientGuardian);
        Task<PatientGuardian> GetPatientGuardianByIdAsync(int patientId);
        Task<PatientGuardian> UpdatePatientGuardianAsync(PatientGuardian patientGuardian);
        public Task<bool> DeletePatientGuardianAsync(int patientGuardianId);
    }
}
