using System.Collections.Generic;

namespace BehzistiMaskan.Core.Dtos
{
    public class ClientWaitingApplicantSimpleDto
    {
        public int Id { get; set; }
        public string NationalCode { get; set; }
        public string Name { get; set; }
        public string Family { get; set; }
        public string FatherName { get; set; }

        public string CityName { get; set; }
        public string DistrictName { get; set; }
        public string CountyName { get; set; }

        public bool HasClientUser { get; set; }

        public List<string> Requests { get; set; }


    }
}