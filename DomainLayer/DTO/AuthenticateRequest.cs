﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.DTO
{
    public class AuthenticateRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}