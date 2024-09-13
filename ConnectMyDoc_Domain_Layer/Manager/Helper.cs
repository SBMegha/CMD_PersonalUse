using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ConnectMyDoc_Domain_Layer.DTOs;
using ConnectMyDoc_Domain_Layer.Entity;
using ConnectMyDoc_Domain_Layer.Exceptions;
using ConnectMyDoc_Domain_Layer.Services;

namespace ConnectMyDoc_Domain_Layer.Manager
{
    public class Helper
    {
        private readonly IMessageService _exceptionMessageService = null;
        private readonly HttpClient _httpClient = null;

        public Helper(IMessageService _exceptionMessageService, HttpClient httpClient)
        {
            this._exceptionMessageService = _exceptionMessageService;
            this._httpClient = httpClient;
        }

        public PatientDTO MapPatientToPatientDTO(Patient patient)
        {
            PatientDTO patientDTO = new PatientDTO
            {
                PatientId = patient.PatientId,
                PatientName = patient.PatientName,
                Email = patient.Email,
                Phone = patient.Phone,
                Age = patient.Age,
                Dob = patient.Dob,
                Gender = patient.Gender,
                PreferredStartTime = patient.PreferredStartTime,
                PreferredEndTime = patient.PreferredEndTime,
                CreatedDate = patient.CreatedDate,
                CreatedBy = patient.CreatedBy,
                LastModifiedDate = patient.LastModifiedDate,
                LastModifiedBy = patient.LastModifiedBy,
                PreferredClinicId = patient.PreferredClinicId, // Assuming PreferredClinic can be directly assigned
                Image = patient.Image != null ? Convert.ToBase64String(patient.Image) : null,// Convert byte array to Base64 string
                PatientAddressId = patient.PatientAddressId,
                StreetAddress = patient.PatientAddress?.StreetAddress ?? string.Empty, // Default to empty string if address is null
                City = patient.PatientAddress?.City ?? string.Empty,
                State = patient.PatientAddress?.State ?? string.Empty,
                Country = patient.PatientAddress?.Country ?? string.Empty,
                ZipCode = patient.PatientAddress?.ZipCode ?? string.Empty,
                PreferredDoctorId = patient.PreferredDoctorId
            };
            if (patient.Age < 18 || patient.PatientGuardianId!=null)
            {
                patientDTO.PatientGuardianId = patient.PatientGuardianId;
                patientDTO.PatientGuardianName = patient.PatientGuardian.PatientGuardianName;
                patientDTO.PatientGuardianPhoneNumber = patient.PatientGuardian.PatientGuardianPhoneNumber;
                patientDTO.PatientGuardianRelationship = patient.PatientGuardian.PatientGuardianRelationship;

            }
            return patientDTO;
        }

        public Patient MapPatientDtoToPatient(PatientDTO patientDTO, PatientAddress patientAddress, PatientGuardian patientGuardian)
        {
            Patient patient = new Patient
            {
                PatientName = patientDTO.PatientName,
                Email = patientDTO.Email,
                Phone = patientDTO.Phone,
                Age = patientDTO.Age.Value,
                Dob = patientDTO.Dob,
                Gender = patientDTO.Gender,
                PreferredStartTime = patientDTO.PreferredStartTime,
                PreferredEndTime = patientDTO.PreferredEndTime,
                CreatedDate = patientDTO.CreatedDate,
                CreatedBy = patientDTO.CreatedBy,
                LastModifiedDate = patientDTO.LastModifiedDate,
                LastModifiedBy = patientDTO.LastModifiedBy,
                PreferredClinicId = patientDTO.PreferredClinicId,
                Image = !string.IsNullOrEmpty(patientDTO.Image) ? Convert.FromBase64String(patientDTO.Image) : null,
                PatientAddressId = patientAddress.PatientAddressId,
                PatientAddress = patientAddress,
                PreferredDoctorId = patientDTO.PreferredDoctorId
            };

            // If patient is below 18, assign the guardian details
            if (patient.Age < 18)
            {
                patient.PatientGuardianId = patientDTO.PatientGuardianId;
                patient.PatientGuardian = patientGuardian;
            }
            return patient;
        }

        public int CalculateAge(DateTime dob)
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

        public void ValidatePatientGuardian(PatientDTO patientDTO)
        {
            if (string.IsNullOrWhiteSpace(patientDTO.PatientGuardianName))
            {
                throw new BusinessException(_exceptionMessageService.GetMessage("MissingGuardianName"));
            }
            if (string.IsNullOrWhiteSpace(patientDTO.PatientGuardianPhoneNumber))
            {
                throw new BusinessException(_exceptionMessageService.GetMessage("MissingGuardianPhone"));
            }
            if (string.IsNullOrWhiteSpace(patientDTO.PatientGuardianRelationship))
            {
                throw new BusinessException(_exceptionMessageService.GetMessage("MissingGuardianRelationship"));
            }

        }

        public void ValidateRequiredFieldsForPatientAsync(PatientDTO patientDTO)
        {
            if (patientDTO == null)
            {
                throw new BusinessException(_exceptionMessageService.GetMessage("NullPatientDTO"));
            }
            if (string.IsNullOrWhiteSpace(patientDTO.PatientName))
            {
                throw new BusinessException(_exceptionMessageService.GetMessage("InvalidPatientName"));
            }
            if (string.IsNullOrWhiteSpace(patientDTO.Email))
            {
                throw new BusinessException(_exceptionMessageService.GetMessage("InvalidEmail"));
            }
            if (!IsValidEmail(patientDTO.Email))
            {
                throw new BusinessException(_exceptionMessageService.GetMessage("InvalidEmailFormat"));
            }
            if (string.IsNullOrWhiteSpace(patientDTO.Phone))
            {
                throw new BusinessException(_exceptionMessageService.GetMessage("InvalidPhone"));
            }
            if (patientDTO.Dob == default(DateTime))
            {
                throw new BusinessException(_exceptionMessageService.GetMessage("MissingDobOrAge"));
            }
            if (string.IsNullOrWhiteSpace(patientDTO.Gender))
            {
                throw new BusinessException(_exceptionMessageService.GetMessage("InvalidGender"));
            }
            if (patientDTO.PreferredStartTime == default(DateTime))
            {
                throw new BusinessException(_exceptionMessageService.GetMessage("InvalidPreferredStartTime"));
            }
            if (patientDTO.PreferredEndTime == default(DateTime))
            {
                throw new BusinessException(_exceptionMessageService.GetMessage("InvalidPreferredEndTime"));
            }
            if (patientDTO.PreferredStartTime >= patientDTO.PreferredEndTime)
            {
                throw new BusinessException(_exceptionMessageService.GetMessage("InvalidPreferredTimeRange"));
            }
            if (patientDTO.CreatedBy <= 0)
            {
                throw new BusinessException(_exceptionMessageService.GetMessage("InvalidCreatedBy"));
            }
            if (patientDTO.LastModifiedBy <= 0)
            {
                throw new BusinessException(_exceptionMessageService.GetMessage("InvalidLastModifiedBy"));
            }
            if (string.IsNullOrWhiteSpace(patientDTO.StreetAddress))
            {
                throw new BusinessException(_exceptionMessageService.GetMessage("InvalidStreetAddress"));
            }
            if (patientDTO.PreferredClinicId <= 0)
            {
                throw new BusinessException(_exceptionMessageService.GetMessage("InvalidClinicId"));
            }
            if (patientDTO.PreferredDoctorId <= 0)
            {
                throw new BusinessException(_exceptionMessageService.GetMessage("InvalidDoctorId"));
            }
            if (string.IsNullOrWhiteSpace(patientDTO.State))
            {
                throw new BusinessException(_exceptionMessageService.GetMessage("InvalidState"));
            }
            if (string.IsNullOrWhiteSpace(patientDTO.City))
            {
                throw new BusinessException(_exceptionMessageService.GetMessage("InvalidCity"));
            }
            if (string.IsNullOrWhiteSpace(patientDTO.Country))
            {
                throw new BusinessException(_exceptionMessageService.GetMessage("InvalidCountry"));
            }
            if (string.IsNullOrWhiteSpace(patientDTO.ZipCode))
            {
                throw new BusinessException(_exceptionMessageService.GetMessage("InvalidZipCode"));
            }
        }

        private void ValidateImageSize(string base64Image, int maxSizeInBytes)
        {
            try
            {
                var imageBytes = Convert.FromBase64String(base64Image);
                if (imageBytes.Length > maxSizeInBytes)
                {
                    throw new ArgumentException("Image size exceeds the allowed limit.");
                }
            }
            catch (FormatException)
            {
                throw new ArgumentException("Invalid image format.");
            }
        }

        public PatientAddress CreatePatientAddress(string streetAddress, string city, string state, string country, string zipcode, DateTime createdDate, int createdBy, DateTime lastModifiedDate, int lastModifiedBy)
        {
            PatientAddress patientAddress = new PatientAddress
            {
                StreetAddress = streetAddress,
                City = city,
                State = state,
                Country = country,
                ZipCode = zipcode,
                CreatedDate = createdDate,
                CreatedBy = createdBy,
                LastModifiedDate = lastModifiedDate,
                LastModifiedBy = lastModifiedBy
            };
            return patientAddress;
        }

        public PatientGuardian CreatePatientGuardian(string guradianName, string guardianPhone, string relationship)
        {
            PatientGuardian patientGuardian = new PatientGuardian
            {
                PatientGuardianName = guradianName,
                PatientGuardianPhoneNumber = guardianPhone,
                PatientGuardianRelationship = relationship
            };
            return patientGuardian;
        }

        public bool IsValidEmail(string email)
        {
            // A more robust email regex pattern
            var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

            return emailRegex.IsMatch(email);
        }

        public async Task<bool> ValidateDoctorIdAsync(int doctorId)
        {
            var response = await _httpClient.GetAsync($"https://cmd-doctor-api.azurewebsites.net/api/Doctor/{doctorId}");

            if (response.IsSuccessStatusCode)
            {
                return true; 
            }

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return false; // Doctor ID is invalid
            }

            // If another status code is received, throw an exception or handle accordingly
            throw new BusinessException(_exceptionMessageService.GetMessage("UnexpectedApiResponse")+": "+ response.StatusCode);
        }

    }
}
