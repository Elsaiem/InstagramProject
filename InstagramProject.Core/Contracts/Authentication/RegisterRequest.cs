﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstagramProject.Core.Contracts.Authentication
{
    public record RegisterRequest
     (
         string Email,
         string UserName,
         string Password,
         string FullName,
         DateOnly BirthDay
     );
}
