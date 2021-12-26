using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOC.WebApi.Dto
{
    public class ResetUserPasswordtInput
    {
        public string Email { get; set; }

        public string NewPassword { get; set; }

        public string PasswordConfirmation { get; set; }

        public string ResetToken { get; set; }
    }
}
