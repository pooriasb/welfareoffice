using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;
using BehzistiMaskan.Core.Models;

namespace BehzistiMaskan.Core.Configuration
{
    public class UserInfoConfiguration : EntityTypeConfiguration<UserInfo>
    {
        public UserInfoConfiguration()
        {

            //Table Name
            ToTable("UserInfo");

            //Key
            HasKey(u => u.Id);

            //Property
            Property(u => u.Id)
                .HasMaxLength(128);

            Property(u => u.Name)
                .HasMaxLength(255)
                .IsRequired();

            Property(u => u.Family)
                .HasMaxLength(255)
                .IsRequired();


            Property(u => u.Mobile)
                .HasMaxLength(11)
                .IsFixedLength();

            //Relation

            //Zero/One to Many relationship between UserInfo and County
            HasOptional(u => u.County)
                .WithMany(c => c.UserInfos)
                .HasForeignKey(u => u.CountyId)
                .WillCascadeOnDelete(false);

            //Zero/One to Many relationship between UserInfo and Province
            HasOptional(u => u.Province)
                .WithMany(p => p.UserInfos)
                .HasForeignKey(u => u.ProvinceId)
                .WillCascadeOnDelete(false);

            // Zero/One to Zero/One relationship between UserInfo and CoOrganizationType
            HasOptional(u => u.CoOrganizationType)
                .WithMany(c => c.UserInfos)
                .HasForeignKey(u => u.CoOrganizationTypeId)
                .WillCascadeOnDelete(false);

            ////Zero/One to Zero/One relationship between UserInfo and Person (While Person is Principal)
            //HasOptional(u => u.Person)
            //    .WithOptionalDependent(p => p.UserInfo)
            //    .Map(m=>m.MapKey("PersonId"));

            HasOptional(u => u.AssistanceType)
                .WithMany(a => a.UserInfos)
                .HasForeignKey(u => u.AssistanceTypeId)
                .WillCascadeOnDelete(false);

            //One to One relationship between User (ApplicationUser) and UserInfo
            HasRequired(u => u.User)
                .WithRequiredDependent(u => u.UserInfo);
        }
    }
}