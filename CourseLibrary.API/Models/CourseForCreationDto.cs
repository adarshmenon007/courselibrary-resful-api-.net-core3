using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrary.API.Models
{
    public class CourseForCreationDto : IValidatableObject // implementing this interface provides a way for an object to be invalidated in any way we want.
    {
        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        [MaxLength(1500)]
        public string Description { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Title == Description)
            {
                // yield return will make sure the validation result is immediately returned after which the code execution will continue. 
                // ValidationResult is an object that is used to provide error messages and relay the property's member or model names.
                yield return new ValidationResult("The provided description should be different from the title.", new[] {"CourseForCreationDto"});
            }
        }
    }
}
