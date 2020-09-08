using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace login.Models
{
    public class usercontext: DbContext
    {
        public usercontext() : base("name=Dbconfig")
        {

        
        }
        public DbSet<user> Users { get; set; }
    }
}
