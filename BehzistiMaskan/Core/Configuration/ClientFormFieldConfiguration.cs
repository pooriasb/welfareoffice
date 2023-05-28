using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;
using BehzistiMaskan.Core.Models.FormBuilder;

namespace BehzistiMaskan.Core.Configuration
{
    public class ClientFormFieldConfiguration : EntityTypeConfiguration<ClientFormField>
    {
        public ClientFormFieldConfiguration()
        {
            //Table
            ToTable("FormField");

            //Key
            HasKey(cf => cf.Id);

            //Property
            Property(ff => ff.Value)
                .IsRequired();

            //Relation

            HasRequired(ff=>ff.ClientForm)
                .WithMany(cf => cf.ClientFormFields)
                .HasForeignKey(ff=>ff.ClientFormId)
                .WillCascadeOnDelete(false);

            HasRequired(ff=>ff.Field)
                .WithMany(f=>f.ClientFormFields)
                .HasForeignKey(ff=>ff.FieldId)
                .WillCascadeOnDelete(false);
        }
    }
}