using System.Data.Entity.ModelConfiguration;
using BehzistiMaskan.Core.Models;

namespace BehzistiMaskan.Core.Configuration
{
    public class BankInfoConfiguration : EntityTypeConfiguration<BankInfo>
    {
        public BankInfoConfiguration()
        {
            //Table Name
            ToTable("BankInfo");

            //Key
            HasKey(c => c.Id);

            //Property


            Property(c => c.AccountNumber)
                .HasMaxLength(300)
                .IsRequired();

            Property(c => c.CardNumber)
                .HasMaxLength(19)
                .IsRequired();

            //Relations
            HasRequired(c => c.Client)
                .WithOptional(p => p.BankInfo);

            HasRequired(f => f.BankType)
                .WithMany(b => b.FinanceInfos)
                .HasForeignKey(f => f.BankTypeId)
                .WillCascadeOnDelete(false);

            HasOptional(b => b.AccountApproveImage)
                .WithMany(cd => cd.BankInfos)
                .HasForeignKey(b => b.AccountApproveImageId)
                .WillCascadeOnDelete(false);

        }
    }
}