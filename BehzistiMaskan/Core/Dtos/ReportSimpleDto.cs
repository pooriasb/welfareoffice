using MD.PersianDateTime;

namespace BehzistiMaskan.Core.Dtos
{
    public class ReportSimpleDto
    {
        public int Id { get; set; }
        public string ReportName { get; set; }

        public string CreatorName { get; set; }

        public PersianDateTime? CreatedAt { get; set; }
        public PersianDateTime? UpdateAt { get; set; }
    }
}