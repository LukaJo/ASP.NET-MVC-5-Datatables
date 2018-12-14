using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ASP.NET_MVC_5_Datatables.Models
{
    public class DatabaseContext:DbContext
    {
        public DatabaseContext():base("DBConnection")
        {

        }

        public DbSet<Customers> Customers { get; set; }
    }
}