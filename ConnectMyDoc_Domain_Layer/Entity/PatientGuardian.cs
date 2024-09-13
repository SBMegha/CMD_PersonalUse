
﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectMyDoc_Domain_Layer.Entity
{
    public class PatientGuardian
    {
        [Key]
        public int PatientGuardianId { get; set; }
        [Required(ErrorMessage = "Guardian's name is required.")]
        [MaxLength(100, ErrorMessage = "Guardian's name cannot exceed 100 characters.")]
        public string PatientGuardianName { get; set; }
        [Required(ErrorMessage = "Guardian's phone number is required.")]
        [RegularExpression(@"^\+?[1-9]\d{1,14}$", ErrorMessage = "Invalid phone number format.")]
        public string PatientGuardianPhoneNumber { get; set; }

        [Required(ErrorMessage = "Guardian's relationship is required.")]
        [MaxLength(50, ErrorMessage = "Relationship cannot exceed 50 characters.")]
        public string PatientGuardianRelationship { get; set; }
    }
}
