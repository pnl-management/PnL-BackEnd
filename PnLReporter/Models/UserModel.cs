﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PnLReporter.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Fullname { get; set; }
        public int? Role { get; set; }
        public string Description { get; set; }
        public Store Store { get; set; }
        public Brand Brand { get; set; }
    }
}
