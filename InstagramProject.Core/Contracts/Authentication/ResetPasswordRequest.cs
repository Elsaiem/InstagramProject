﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstagramProject.Core.Contracts.Authentication
{
    public record ResetPasswordRequest(
         string Email,
         string Code,
         string NewPassword
     );
}
