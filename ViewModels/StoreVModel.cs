﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PnLReporter.Models;

namespace PnLReporter.ViewModels
{
    public class StoreVModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public bool? Status { get; set; }
        public BrandVModel Brand { get; set; }
    }
}
