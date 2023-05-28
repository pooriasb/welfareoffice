using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;
using BehzistiMaskan.Core.Models;

namespace BehzistiMaskan.Core.Configuration
{
    public class BankTypeConfiguration : EntityTypeConfiguration<BankType>
    {
        public BankTypeConfiguration()
        {
            //Table Name

            //Key
            HasKey(c => c.Id);

            //Property
            Property(c => c.Description)
                .HasMaxLength(3000);

            Property(c => c.Name)
                .HasMaxLength(300)
                .IsRequired();

            Property(b => b.Category)
                .HasMaxLength(50);

            //Relations

            // -- relation between --BankType-- And --BankInfo-- is declared in BankInfoConfiguration.cs
        }
    }
}