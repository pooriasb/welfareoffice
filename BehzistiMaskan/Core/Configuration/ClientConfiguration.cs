using System.Data.Entity.ModelConfiguration;
using BehzistiMaskan.Core.Models;

namespace BehzistiMaskan.Core.Configuration
{
    public class ClientConfiguration : EntityTypeConfiguration<Client>
    {
        public ClientConfiguration()
        {
            //Table Name
            ToTable("Client");

            //Key
            HasKey(c => c.Id);

            //Property
            Property(c => c.GlobalBehzistiUiCode)
                .HasMaxLength(100);

            //Relations
            HasRequired(c => c.Person)
                .WithOptional(p => p.Client);

            HasRequired(c => c.CurrentHousing)
                .WithRequiredPrincipal(b => b.Client);


            HasMany(c => c.ClientDocuments)
                .WithRequired(cd => cd.Client)
                .HasForeignKey(cd => cd.ClientId)
                .WillCascadeOnDelete(false);

            HasRequired(c=>c.City)
                .WithMany(c=>c.Clients)
                .HasForeignKey(c=>c.CityId)
                .WillCascadeOnDelete(false);

            /*
             * relation between --Client-- and --BankInfo-- is declared in BankInfoConfiguration
             */

        }
    }
}