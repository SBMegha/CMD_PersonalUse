using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConnectMyDoc_Data_Layer.Context;
using ConnectMyDoc_Domain_Layer.Entity;
using ConnectMyDoc_Domain_Layer.Exceptions;
using ConnectMyDoc_Domain_Layer.Repository;
using ConnectMyDoc_Domain_Layer.Services;
using Microsoft.EntityFrameworkCore;

namespace ConnectMyDoc_Data_Layer.Repositories
{
    public class PatientRepository : IPatientRepository
    {
        private readonly PatientCMDDbContext _dbContext= null;
        private readonly IPatientAddressRepository _patientAddressRepository = null;
        private readonly IPatientGuardianRepository _patientGuardianRepository = null;
        private readonly IMessageService _exceptionMessageService = null;
        public PatientRepository(PatientCMDDbContext db,IPatientAddressRepository patientAddressRepository,IPatientGuardianRepository patientGuardianRepository, IMessageService messageService) 
        {
            _dbContext = db;
            _patientAddressRepository = patientAddressRepository;
            _patientGuardianRepository=patientGuardianRepository;
            _exceptionMessageService =messageService;

        }
        public async Task<Patient> AddPatientAsync(Patient patient)
        {
            try
            {
                int patientAge = CalculateAge(patient.Dob);
                if (patientAge < 18)
                {
                    if (patient.PatientGuardianId == null || patient.PatientGuardian == null)
                    {
                        throw new Exception("Patient is under 18, guardian details must be provided.");
                    }
                }
                
                await _dbContext.Patients.AddAsync(patient);
                await _dbContext.SaveChangesAsync();
                return patient;
            }
            catch (Exception ex)
            {
                //log
                return null;
            }
        }

        public async Task<bool> DeletePatientAsync(int patientId)
        {
            try
            {
                Patient patient = await _dbContext.Patients.FindAsync(patientId);
                if (patient == null)
                    return false;
                _dbContext.Patients.Remove(patient);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                //log
                throw;
            }
        }

		private int CalculateAge(DateTime dob)
		{
            // Get today's date
            DateTime today = DateTime.Today;

            // Calculate the initial age based on year difference
            int age = today.Year - dob.Year;

            // Adjust age if the birthday has not occurred yet this year
            if (today < dob.AddYears(age))
            {
                age--;
            }

            return age;
        }

        public async Task<List<Patient>> GetAllPatientsAsync(int pageNumber, int pageSize)
        {
            return await _dbContext.Patients
                .Include(p=>p.PatientGuardian)
                .Include(p=>p.PatientAddress)
                .Skip(pageNumber).Take(pageSize).ToListAsync();
        }

        public async Task<Patient> GetPatientByIdAsync(int patientId)
        {
            try
            {
                var patient = await _dbContext.Patients
                    .Include(p => p.PatientAddress)
                    .Include(p => p.PatientGuardian)
                    .SingleOrDefaultAsync(p => p.PatientId == patientId);
                return patient;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving the patient.", ex);
            }
        }

        public async Task<int> GetTotalPatientsCountAsync()
        {
            return await _dbContext.Patients.CountAsync();
        }

        public async Task<bool> SetPrimaryClinicAsync(Patient patient, int clinic)
        {
            try
            {
                patient.PreferredClinicId = clinic;
                return true;
            }
            catch
            {
                //log
                return false;
            }
        }


        public async Task<bool> SetPrimaryDoctorAsync(Patient patient, int doctorId)
        {
            try
            {
                patient.PreferredDoctorId = doctorId;
                return true;
            }
            catch 
            { 
                //log
                return false; 
            }
        }

        public async Task<Patient> UpdatePatientAsync(Patient patient)
        {
            try
            {
                var existingPatient = await GetPatientByIdAsync(patient.PatientId);
                if (existingPatient != null)
                {
                    _dbContext.Entry(existingPatient).CurrentValues.SetValues(patient);
                    await _dbContext.SaveChangesAsync();
                    return existingPatient;

                }
                else
                {
                    throw new BusinessException(_exceptionMessageService.GetMessage("PatientNotFound"));
                }
            }

            catch (Exception ex)
            {
                throw ;
            }
        }


        /* public async Task<Patient> UpdatePatientAsync(Patient patient, int patientId, PatientAddress patientAddress, PatientGuardian patientGuardian)
         {
             try
             {
                 var existingPatient = await _dbContext.Patients
                     .Include(p => p.PatientAddress)
                     .Include(p => p.PatientGuardian)
                     .SingleOrDefaultAsync(p => p.PatientId == patientId);

                 if (existingPatient != null)
                 {
                     _dbContext.Entry(existingPatient).CurrentValues.SetValues(patient);

                     if (patientAddress != null)
                     {
                         var existingAddress = await _dbContext.PatientAddresses.FindAsync(patientAddress.PatientAddressId);
                         if (existingAddress != null)
                         {
                             await _patientAddressRepo.UpdatePatientAddressAsync(patientAddress);
                         }
                         else
                         {
                             // Handle missing address if necessary
                         }
                     }

                     if (patientGuardian != null && patient.Age < 18)
                     {
                         var existingGuardian = await _dbContext.PatientGuardians.FindAsync(patientGuardian.PatientGuardianId);
                         if (existingGuardian != null)
                         {
                             await _guardianRepo.UpdatePatientGuardianAsync(patientGuardian);
                         }
                         else
                         {
                             // Handle missing guardian if necessary
                         }
                     }

                     existingPatient.LastModifiedDate = DateTime.UtcNow;

                     await _dbContext.SaveChangesAsync();
                 }
                 return existingPatient;
             }
             catch (Exception ex)
             {
                 // Consider logging the exception
                 throw new KeyNotFoundException("Patient not found.", ex);
             }
         }
     */

        /*        public async Task<Patient> UpdatePatientAsync(Patient patient, int patientId,PatientAddress patientAddress,PatientGuardian patientGuardian)
            {
                try
                {
                    var existingPatient = await _dbContext.Patients.FindAsync(patientId);
                    if (existingPatient != null)
                    {
                        existingPatient.PatientName = patient.PatientName;
                        existingPatient.Email = patient.Email;
                        existingPatient.Phone = patient.Phone;
                        existingPatient.Age = patient.Age;
                        existingPatient.Dob = patient.Dob;
                        existingPatient.Gender = patient.Gender;
                        existingPatient.PreferredStartTime = patient.PreferredStartTime;
                        existingPatient.PreferredEndTime = patient.PreferredEndTime;
                        existingPatient.CreatedDate = patient.CreatedDate; // Consider if this should be updated
                        existingPatient.CreatedBy = patient.CreatedBy; // Consider if this should be updated
                        existingPatient.LastModifiedDate = DateTime.UtcNow; // Always set to current time
                        existingPatient.LastModifiedBy = patient.LastModifiedBy;
                        existingPatient.PatientAddressId = patient.PatientAddressId;
                        existingPatient.PatientAddress = await _patientAddressRepo.UpdatePatientAddressAsync(patient.PatientAddress);
                        if (existingPatient.Age < 18)
                        {
                            existingPatient.PatientGuardian = await _guardianRepo.UpdatePatientGuardianAsync(patient.PatientGuardian);
                            existingPatient.PatientGuardianId = existingPatient.PatientGuardian.PatientGuardianId;
                        }
                        existingPatient.PreferredDoctorId = patient.PreferredDoctorId;
                        existingPatient.Image = patient.Image;

                        await _dbContext.SaveChangesAsync();
                    }
                    return existingPatient;
                }
                catch(Exception ex) 
                {
                    throw new KeyNotFoundException("Patient not found.");
                    //log here
                }
            }
    */
    }
}
