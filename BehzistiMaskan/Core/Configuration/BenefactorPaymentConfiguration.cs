using System.Data.Entity.ModelConfiguration;
using BehzistiMaskan.Core.Models;

namespace BehzistiMaskan.Core.Configuration
{
    public class BenefactorPaymentConfiguration : EntityTypeConfiguration<BenefactorPayment>
    {
        public BenefactorPaymentConfiguration()
        {
            //Table Name
            ToTable("BenefactorPayment");

            //Key
            HasKey(c => c.Id);

            //Property


            //Relations
            HasRequired(b => b.Benefactor)
                .WithMany(b => b.BenefactorPayments)
                .HasForeignKey(b => b.BenefactorId);

        }
    }
}