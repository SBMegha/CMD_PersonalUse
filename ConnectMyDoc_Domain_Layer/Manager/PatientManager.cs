
using ConnectMyDoc_Domain_Layer.DTOs;
using ConnectMyDoc_Domain_Layer.Entity;
using ConnectMyDoc_Domain_Layer.Exceptions;
using ConnectMyDoc_Domain_Layer.Repository;
using ConnectMyDoc_Domain_Layer.Services;

namespace ConnectMyDoc_Domain_Layer.Manager
{
    public class PatientManager : IPatientManager
    {
        private readonly IPatientRepository _patientRepository ;
        private readonly IPatientAddressRepository _patientAddressRepository;
        private readonly IPatientGuardianRepository _patientGuardianRepository;
        private readonly IMessageService _exceptionMessageService;
        private readonly HttpClient _httpClient;
        private readonly Helper _helper;

        public PatientManager(IPatientRepository patientRepository, IPatientAddressRepository patientAddressRepository, IPatientGuardianRepository patientGuardianRepository ,Helper helper, IMessageService messageService, HttpClient httpClient)
        {
            _patientRepository = patientRepository;
            _patientAddressRepository = patientAddressRepository;
            _patientGuardianRepository = patientGuardianRepository;
            _exceptionMessageService = messageService;
            _httpClient = httpClient;
            _helper = helper;

        }
       /// <summary>
       /// Adding patient 
       /// </summary>
       /// <param name="patientDTO"></param>
       /// <returns></returns>
       /// <exception cref="Exception"></exception>
        public async Task<PatientDTO> AddPatientAsync(PatientDTO patientDTO)
        {
           
                PatientAddress patientAddress = null;
                PatientGuardian patientGuardian = null;
                Patient patient = null;
                _helper.ValidateRequiredFieldsForPatientAsync(patientDTO);
                if(patientDTO.Image != null)
                {

                }

                var isDoctorValid = await _helper.ValidateDoctorIdAsync(patientDTO.PreferredDoctorId);
                if (!isDoctorValid)
                {
                    throw new BusinessException(_exceptionMessageService.GetMessage("DoctorNotFound")+ " : "+ patientDTO.PreferredDoctorId);
                }

                int age = _helper.CalculateAge(patientDTO.Dob);
                if (patientDTO.Age.HasValue)
                { 
                    if (patientDTO.Age != age)
                    {
                        throw new Exception("Age and DOB input field dont match");
                    }
                    else
                    {
                        patientDTO.Age = age;
                        if (patientDTO.Age < 18)
                        {
                            _helper.ValidatePatientGuardian(patientDTO);
                            patientGuardian = _helper.CreatePatientGuardian(patientDTO.PatientGuardianName, patientDTO.PatientGuardianPhoneNumber, patientDTO.PatientGuardianRelationship);
                            patientGuardian = await _patientGuardianRepository.AddPatientGuardian(patientGuardian);
                        }
                    }
                }
                patientAddress = _helper.CreatePatientAddress(
                                                  patientDTO.StreetAddress,
                                                  patientDTO.City,
                                                  patientDTO.State,
                                                  patientDTO.Country,
                                                  patientDTO.ZipCode,
                                                  patientDTO.CreatedDate,
                                                  patientDTO.CreatedBy,
                                                  patientDTO.LastModifiedDate,
                                                  patientDTO.LastModifiedBy);

                patientAddress = await _patientAddressRepository.AddPatientAddressAsync(patientAddress);


                patient = _helper.MapPatientDtoToPatient(patientDTO, patientAddress, patientGuardian);
                
                patient = await _patientRepository.AddPatientAsync(patient);
                patientDTO = _helper.MapPatientToPatientDTO(patient);
                return patientDTO;
           
        }
        /// <summary>
        /// Deleting a patient by id 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> DeletePatientByIdAsync(int id)
        {
            return await _patientRepository.DeletePatientAsync(id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<(IEnumerable<PatientDTO> Patients, int TotalCount)> GetAllPatientsAsync(int pageNumber, int pageSize)
        {
            var skip = (pageNumber - 1) * pageSize;
            var patients = await _patientRepository.GetAllPatientsAsync(skip, pageSize);
            var totalCountOfPatients = await _patientRepository.GetTotalPatientsCountAsync();
            List<PatientDTO> patientDTOs = patients.Select(patient => _helper.MapPatientToPatientDTO(patient)).ToList();

            return (patientDTOs, totalCountOfPatients);

        }

        public async Task<PatientDTO> GetPatientByIdAsync(int id)
        {
            var patient = await _patientRepository.GetPatientByIdAsync(id);
            if (patient == null)
            {
                return null;
            }
            return _helper.MapPatientToPatientDTO(patient);
        }

        public async Task<PatientDTO> UpdatePatient(PatientDTO patientDTO, int patientId)
        {

            if (patientDTO == null)
                throw new ArgumentNullException("Patient details cannot be empty");

            _helper.ValidateRequiredFieldsForPatientAsync(patientDTO);

            Patient existingPatient = await _patientRepository.GetPatientByIdAsync(patientId);

            if (existingPatient == null)
            {
                return null;
            }

            bool wasMinor = false;
            bool nowMinor=false;
            if (existingPatient.Age < 18)
            {
                wasMinor = true;
            }
            int newAge = _helper.CalculateAge(patientDTO.Dob);
            if(newAge<18 )
            {
                nowMinor = true;
            }


            
            var isDoctorValid = await _helper.ValidateDoctorIdAsync(patientDTO.PreferredDoctorId);
            if (!isDoctorValid)
            {
                throw new BusinessException(_exceptionMessageService.GetMessage("DoctorNotFound") + " : " + patientDTO.PreferredDoctorId);
            }
            

            existingPatient.PreferredDoctorId = patientDTO.PreferredDoctorId;
            PatientGuardian existingPatientGuadian = null;
            if (wasMinor && nowMinor)
            {
                existingPatientGuadian = await _patientGuardianRepository.GetPatientGuardianByIdAsync(patientDTO.PatientGuardianId.Value);
                if(existingPatientGuadian == null)
                {
                    throw new BusinessException(_exceptionMessageService.GetMessage("PatientGuardianNotFound"));
                }
                _helper.ValidatePatientGuardian(patientDTO);
                existingPatientGuadian.PatientGuardianName = patientDTO.PatientGuardianName;
                existingPatientGuadian.PatientGuardianPhoneNumber = patientDTO.PatientGuardianPhoneNumber;
                existingPatientGuadian.PatientGuardianRelationship = patientDTO.PatientGuardianRelationship;
                existingPatientGuadian = await _patientGuardianRepository.UpdatePatientGuardianAsync(existingPatientGuadian);
            }
            PatientGuardian newPatientGuardian = null;
            if (!wasMinor && nowMinor)
            {
                _helper.ValidatePatientGuardian(patientDTO);
                newPatientGuardian = _helper.CreatePatientGuardian(patientDTO.PatientGuardianName, patientDTO.PatientGuardianPhoneNumber, patientDTO.PatientGuardianRelationship);
                newPatientGuardian = await _patientGuardianRepository.AddPatientGuardian(newPatientGuardian);
            }

            PatientAddress existingPatientAddress = await _patientAddressRepository.GetPatientAddressByIdAsync(patientDTO.PatientAddressId.Value);
            if (existingPatientAddress == null)
            {
                throw new BusinessException(_exceptionMessageService.GetMessage("PatientAddressNotFound"));
            }
            if (patientDTO.PatientAddressId.Value == existingPatient.PatientAddressId)
            { 
                existingPatientAddress.StreetAddress = patientDTO.StreetAddress;
                existingPatientAddress.City = patientDTO.City;
                existingPatientAddress.State= patientDTO.State;
                existingPatientAddress.Country = patientDTO.Country;
                existingPatientAddress.ZipCode = patientDTO.ZipCode;
                existingPatientAddress = await _patientAddressRepository.UpdatePatientAddressAsync(existingPatientAddress);    
            }
            else
            {
                throw new BusinessException(_exceptionMessageService.GetMessage("AddressIdDoNotMatch"));
            }

            // Update non-key properties
            existingPatient.PatientName = patientDTO.PatientName;
            existingPatient.Email = patientDTO.Email;
            existingPatient.Phone = patientDTO.Phone;
            existingPatient.Age = _helper.CalculateAge(patientDTO.Dob);
            existingPatient.Dob = patientDTO.Dob;
            existingPatient.Gender = patientDTO.Gender;
            existingPatient.PreferredStartTime = patientDTO.PreferredStartTime;
            existingPatient.PreferredEndTime = patientDTO.PreferredEndTime;
            existingPatient.CreatedDate = patientDTO.CreatedDate;
            existingPatient.CreatedBy = patientDTO.CreatedBy;
            existingPatient.LastModifiedDate = patientDTO.LastModifiedDate;
            existingPatient.LastModifiedBy = patientDTO.LastModifiedBy;
            existingPatient.PreferredClinicId = patientDTO.PreferredClinicId;
            existingPatient.Image = !string.IsNullOrEmpty(patientDTO.Image) ? Convert.FromBase64String(patientDTO.Image) : null;

            existingPatient.PatientAddressId = patientDTO.PatientAddressId.Value;
            //existingPatient.PatientAddress = existingPatientAddress;

            if (wasMinor && nowMinor)
            {
                existingPatient.PatientGuardianId = patientDTO.PatientGuardianId;
                //existingPatient.PatientGuardian = existingPatientGuadian;
            }
            else if (!wasMinor && nowMinor)
            {
                existingPatient.PatientGuardianId = newPatientGuardian.PatientGuardianId;
                existingPatient.PatientGuardian = newPatientGuardian;
            }
            if(!wasMinor && !nowMinor && existingPatient.PatientGuardianId!=null)
            {
                if (existingPatient.PatientGuardianId == patientDTO.PatientGuardianId.Value)
                {
                    existingPatientGuadian = await _patientGuardianRepository.GetPatientGuardianByIdAsync(existingPatient.PatientGuardianId.Value);
                    if (existingPatientGuadian == null)
                    {
                        throw new BusinessException(_exceptionMessageService.GetMessage("PatientGuardianNotFound"));
                    }
                    _helper.ValidatePatientGuardian(patientDTO);
                    existingPatientGuadian.PatientGuardianName = patientDTO.PatientGuardianName;
                    existingPatientGuadian.PatientGuardianPhoneNumber = patientDTO.PatientGuardianPhoneNumber;
                    existingPatientGuadian.PatientGuardianRelationship = patientDTO.PatientGuardianRelationship;
                    existingPatientGuadian = await _patientGuardianRepository.UpdatePatientGuardianAsync(existingPatientGuadian);
                    existingPatient.PatientGuardian = existingPatientGuadian;
                }
                
            }

            existingPatient = await _patientRepository.UpdatePatientAsync(existingPatient);
            if (existingPatient != null)
            {
                return _helper.MapPatientToPatientDTO(existingPatient);
            }
            else
                throw new BusinessException(_exceptionMessageService.GetMessage("UpdateMethodFailed"));            
        }     
    }
}
