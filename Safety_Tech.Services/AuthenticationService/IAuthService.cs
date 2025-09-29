using Safety_Tech.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Safety_Tech.Services.AuthenticationService
{
    public interface IAuthService 
    {
        ServiceResponse<bool> IsEmailUsed(string emailId);
    }
}
