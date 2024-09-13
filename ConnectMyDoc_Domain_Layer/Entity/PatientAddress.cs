
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;



namespace ConnectMyDoc_Domain_Layer.Entity
{
    public class PatientAddress
    {
        [Required]
        [Key]
        public int PatientAddressId { get; set; } 

        [Required(ErrorMessage = "Street address is required.")]
        [MaxLength(200, ErrorMessage = "Street address cannot exceed 200 characters.")]
        public string StreetAddress { get; set; }

        [Required(ErrorMessage = "City is required.")]
        [MaxLength(100, ErrorMessage = "City name cannot exceed 100 characters.")] 
        public string City { get; set; } 

        [Required(ErrorMessage = "State is required.")]
        [MaxLength(100, ErrorMessage = "State name cannot exceed 100 characters.")]
        public string State { get; set; } 

        [Required(ErrorMessage = "Country is required.")]
        [MaxLength(100, ErrorMessage = "Country name cannot exceed 100 characters.")]
        public string Country { get; set; }

        [Required]
        public string ZipCode { get; set; } 

        [DataType(DataType.DateTime, ErrorMessage = "Invalid date format.")]
        public DateTime? CreatedDate { get; set; } 

        public int? CreatedBy { get; set; } 
        [DataType(DataType.DateTime, ErrorMessage = "Invalid date format.")]
        public DateTime? LastModifiedDate { get; set; } 

        public int? LastModifiedBy { get; set; }
    }
}
