using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JwtTokenAuthentication
{
    public class RefleshToken
    {
        public string jwtid { get; set; }
        public string UID { get; set; }
        public string Token { get; set; }
        public DateTime Created { get; set; }
        public DateTime Expires { get; set; } 
        public bool Used { get; set; }
        public bool Invalidated { get; set; }
    }
}
