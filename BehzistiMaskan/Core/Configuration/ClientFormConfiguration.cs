using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;
using BehzistiMaskan.Core.Models.FormBuilder;

namespace BehzistiMaskan.Core.Configuration
{
    public class ClientFormConfiguration :EntityTypeConfiguration<ClientForm>
    {
        public ClientFormConfiguration()
        {
            //Table
            ToTable("ClientForm");

            //Key
            HasKey(cf => cf.Id);

            //Property


            //Relation

            HasRequired(cf => cf.Client)
                .WithMany(c => c.ClientForms)
                .HasForeignKey(cf => cf.ClientId)
                .WillCascadeOnDelete(false);

            HasRequired(cf => cf.Form)
                .WithMany(f => f.ClientForms)
                .HasForeignKey(cf => cf.FormId)
                .WillCascadeOnDelete(false);


            //relation between this table and ClientFormFields is declared in ClientFormFieldConfiguration.cs
        }
    }
}