using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace WcfServiceHosting
{
    public class HostingContext : DbContext
    {
        public HostingContext() : base("HostingContext")
        {

        }

        public DbSet<UserFile> UserFiles { get; set; }
        public DbSet<CurrentUser> CurrentUsers { get; set; }
    }

   
    
}