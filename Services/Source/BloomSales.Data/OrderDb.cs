﻿using BloomSales.Data.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloomSales.Data
{
    internal class OrderDb : DbContext
    {
        public OrderDb() : base("name = OrderDatabase")
        {
            // do nothing!
        }
        public virtual DbSet<Order> Orders { get; set; }

        public virtual DbSet<OrderItem> OrderItems { get; set; }
    }
}
