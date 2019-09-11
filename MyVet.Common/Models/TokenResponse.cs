using System;
using System.Collections.Generic;
using System.Text;

namespace MyVet.Common.Models
{
    public class TokenResponse
    {
        public string MyProperty { get; set; }
        public string Token { get; set; }
        public DateTime Experition { get; set; }   
        public DateTime ExperationLocal => Experition.ToLocalTime();
    }
}
