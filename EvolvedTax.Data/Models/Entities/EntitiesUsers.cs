﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvolvedTax.Data.Models.Entities
{
    public class EntitiesUsers
    {
        public int Id { get; set; }
        public int EntityId { get; set; }
        public Guid UserId { get; set; }
    }
}
