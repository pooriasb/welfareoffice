using System.Collections.Generic;

namespace BehzistiMaskan.Core.Models.FormBuilder
{
    public class PhysicalProgress
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int Order { get; set; }

        public int Percentage { get; set; }

        public ICollection<FormPhysicalProgress> FormPhysicalProgresses { get; set; }
        public ICollection<ClientPhysicalProgress> ClientPhysicalProgresses { get; set; }
    }
}