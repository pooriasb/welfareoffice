using BehzistiMaskan.Core.Models.Utility;

namespace BehzistiMaskan.Core.Models.FormBuilder
{
    /// <summary>
    /// میزان کمک بلاعوض سازمان های همکار در یک طرح در این جدول ذخیره می گردد
    /// </summary>
    public class FormPhysicalProgressHelp
    {
        public int Id { get; set; }
        public int FormPhysicalProgressId { get; set; }
        public FormPhysicalProgress FormPhysicalProgress { get; set; }

        public int? CoOrganizationTypeId { get; set; }
        public CoOrganizationType CoOrganizationType { get; set; }

        /// <summary>
        /// میزان کمک بلاعوض
        /// </summary>
        public long HelpAmount { get; set; }
    }
}