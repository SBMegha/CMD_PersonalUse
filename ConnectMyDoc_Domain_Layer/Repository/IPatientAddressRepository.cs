using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConnectMyDoc_Domain_Layer.Entity;

namespace ConnectMyDoc_Domain_Layer.Repository
{
    public interface IPatientAddressRepository
    {
        //Admin, user(patient)
        Task<PatientAddress> AddPatientAddressAsync(PatientAddress patientAddress);
        Task<PatientAddress> UpdatePatientAddressAsync( PatientAddress patientAddress);
        Task<PatientAddress> GetPatientAddressByIdAsync(int patientAddressId);

        //Admin( if admin deletes patient, the address deleted on cascaded), This method is not required ..
        Task<bool> DeletePatientAddressAsync(int patientId);
        
    }
}
