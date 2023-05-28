using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;
using BehzistiMaskan.Core.Models;

namespace BehzistiMaskan.Core.Configuration
{
    public class RequiredMessageConfiguration:EntityTypeConfiguration<RequiredMessage>
    {
        public RequiredMessageConfiguration()
        {
            //table
            ToTable("RequiredMessage");
                //key
                HasKey(x => x.Id);
           
        }
    }
}