using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;
using BehzistiMaskan.Core.Models;

namespace BehzistiMaskan.Core.Configuration
{
    public class ClientStateConfiguration:EntityTypeConfiguration<ClientState>
    {
        public ClientStateConfiguration()
        {
            HasRequired(cs => cs.Client)
                .WithRequiredDependent(c => c.ClientState);
        }
    }
}