
namespace BehzistiMaskan.Core.Dtos
{
    public class FamilyRelationDto
    {
        public int Id { get; set; }

        public int PersonId { get; set; }

        public string NationalCode { get; set; }

        public string Name { get; set; }

        public string Family { get; set; }

        public string RelationTypeName { get; set; }

        public int RelationTypeId { get; set; }

        public string Description { get; set; }

        public bool IsDisabled { get; set; }

    }
}