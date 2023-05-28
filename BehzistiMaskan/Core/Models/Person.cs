using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using BehzistiMaskan.Core.Models.Geographic;
using BehzistiMaskan.Core.Models.Utility;

namespace BehzistiMaskan.Core.Models
{
    public class Person
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Family { get; set; }

        public string FatherName { get; set; }

        public string MotherName { get; set; }

        [Index(IsUnique = true)]
        public string NationalCode { get; set; }

        public string BirthCertificateNo { get; set; }

        public string BirthCertificateMosalsal { get; set; }

        public string BirthCertificateDescription { get; set; }

        public DateTime? Birthdate { get; set; }

        public int? GenderTypeId { get; set; }

        public GenderType GenderType { get; set; }

        public int? MarriageTypeId { get; set; }

        public MarriageType MarriageType { get; set; }

        public int? NumberOfChildren { get; set; }

        public City CityOfBirth { get; set; }

        public int? CityOfBirthId { get; set; }

        public bool? IsDisabled { get; set; }

        public bool? IsDeleted { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public Client Client { get; set; }

        public ICollection<FamilyRelation> FamilyRelations { get; set; }
    }
}