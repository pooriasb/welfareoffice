
using System.Data.Entity.ModelConfiguration;
using BehzistiMaskan.Core.Models;

namespace BehzistiMaskan.Core.Configuration
{
    public class DownloadRequestConfiguration : EntityTypeConfiguration<DownloadRequest>
    {
        public DownloadRequestConfiguration()
        {
            ToTable("DownloadRequests");

            HasKey(d => d.Id);

            Property(d => d.RequestExpireTime)
                .IsRequired();

            Property(d => d.RequestTime)
                .IsRequired();



            HasRequired(d=>d.Client)
                .WithMany(c=>c.DownloadRequests)
                .HasForeignKey(d=>d.ClientId)
                .WillCascadeOnDelete(false);


            HasRequired(d=>d.UserInfo)
                .WithMany(u=>u.DownloadRequests)
                .HasForeignKey(d=>d.UserId)
                .WillCascadeOnDelete(false);

        }
    }
}