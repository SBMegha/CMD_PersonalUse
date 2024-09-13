using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectMyDoc_Domain_Layer.DTOs
{
    public class PatientDTO
    {
        public int? PatientId { get; set; }

        [Required]
        [MinLength(3)]
        public string PatientName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Length(10,15)]
        public string Phone { get; set; }

        [Range(0, 150)]
        public int? Age { get; set; }

        [Required]
        public DateTime Dob { get; set; }
        [Required]
        public string Gender { get; set; }
        [Required]
        public DateTime PreferredStartTime { get; set; }
        [Required]
        public DateTime PreferredEndTime { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public int LastModifiedBy { get; set; }
        [Required]
        public int PreferredClinicId { get; set; }
        //For image
        public string? Image { get; set; }

        public int? PatientAddressId { get; set; } 

        [Required]
        public string StreetAddress { get; set; } 
        [Required]
        public string City { get; set; }
        [Required]
        public string State { get; set; }
        [Required]
        public string Country { get; set; } 

        [Required]
        public string ZipCode { get; set; }
        [Required]
        public int PreferredDoctorId { get; set; }

        public int? PatientGuardianId { get; set; }
        public string? PatientGuardianName { get; set; }
        public string? PatientGuardianPhoneNumber { get; set; }
        public string? PatientGuardianRelationship { get; set; }

    }

}
