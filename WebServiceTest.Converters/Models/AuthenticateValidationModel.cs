using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServiceTest.Models
{
    public class AuthenticateValidationModel:IValidatableObject
    {
        public AuthenticateValidationModel(string uSERID, string sESSIONID, string gROUPID)
        {
            USERID = uSERID;
            SESSIONID = sESSIONID;
            GROUPID = gROUPID;
        }
        [Required]
        public string USERID { get; set; }
        [Required]
        public string SESSIONID { get; set; }
        [Required]
        public string GROUPID { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();
            Validator.TryValidateProperty(this.USERID,new ValidationContext(this, null, null) { MemberName = "USERID" },results);
            Validator.TryValidateProperty(this.GROUPID, new ValidationContext(this, null, null) { MemberName = "USERID" }, results);
            Validator.TryValidateProperty(this.SESSIONID, new ValidationContext(this, null, null) { MemberName = "USERID" }, results);


            return results;
        }
    }
}
