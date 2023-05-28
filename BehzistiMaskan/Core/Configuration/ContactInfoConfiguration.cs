using System.Data.Entity.ModelConfiguration;
using BehzistiMaskan.Core.Models;

namespace BehzistiMaskan.Core.Configuration
{
    public class ContactInfoConfiguration : EntityTypeConfiguration<ContactInfo>
    {
        public ContactInfoConfiguration()
        {
            //Table Name
            ToTable("ContactInfo");

            //Key
            HasKey(c => c.Id);

            //Property
            Property(c => c.EmergencyTel).HasMaxLength(30)
                .IsRequired();

            Property(c => c.HomeTel).HasMaxLength(30);

            Property(c => c.WorkAddress).HasMaxLength(2000);

            Property(c => c.Mobile)
                .HasMaxLength(11)
                .IsFixedLength()
                .IsRequired();

            Property(c => c.WorkTel).HasMaxLength(30);

            //Relations
            HasRequired(c => c.Client)
                .WithOptional(p => p.ContactInfo);


        }
    }
}