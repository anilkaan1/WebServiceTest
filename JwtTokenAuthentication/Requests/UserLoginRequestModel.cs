using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JwtTokenAuthentication.Requests
{
    public class UserLoginRequestModel:IValidatableObject
    {


        [Required]
        [MinLength(4)]
        public string? USERNAME{ get; set; }

        [Required]
        [MinLength(4)]
        public string? PASSWORDX { get; set; }

        [Required]
        [MinLength(0)]
        public string? COMMAND { get; set; }

        public string SESSIONID { get; set; }

        [Required]
        [MinLength(0)]
        public string? PROJECTNAME { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validation)
        {
            if(USERNAME.Length > 300 || PASSWORDX.Length > 300)
            {
                yield return new ValidationResult("UID and PASSWORD Can have max 30 length.");
            }
        }
    }
}
