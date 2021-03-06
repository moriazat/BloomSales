﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BloomSales.Data.Entities;

namespace BloomSales.Data.Repositories
{
    public class ShipperRepository : IShipperRepository
    {
        private ShippingDb db;

        public ShipperRepository()
        {
            this.db = new ShippingDb();
        }

        internal ShipperRepository(ShippingDb context)
        {
            this.db = context;
        }

        public void AddShipper(Shipper shipper)
        {
            db.Shippers.Add(shipper);
            db.SaveChanges();
        }

        public IEnumerable<Shipper> GetAllShippers()
        {
            var result = db.Shippers.ToArray();

            return result;
        }

        public Shipper GetShipper(string name)
        {
            var result = (from s in db.Shippers
                          where name.Equals(s.Name)
                          select s).SingleOrDefault();

            return result;
        }

        public void Dispose()
        {
            if (this.db != null)
                db.Dispose();
        }
    }
}
