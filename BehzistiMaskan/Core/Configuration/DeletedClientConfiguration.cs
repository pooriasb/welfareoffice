using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;
using BehzistiMaskan.Core.Models;

namespace BehzistiMaskan.Core.Configuration
{
    public class DeletedClientConfiguration :EntityTypeConfiguration<DeletedClient>
    {
        public DeletedClientConfiguration()
        {
            //Table
            ToTable("DeletedClient");

            //Key

            HasKey(d => d.Id);


            //Property


            //Relation
            HasRequired(d => d.Client)
                .WithOptional(c => c.DeletedClient);
        }
    }
}