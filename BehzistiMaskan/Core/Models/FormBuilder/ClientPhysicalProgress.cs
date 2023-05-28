using System.Collections.Generic;

namespace BehzistiMaskan.Core.Models.FormBuilder
{
    public class ClientPhysicalProgress
    {
        public int Id { get; set; }

        public int ClientId { get; set; }
        public Client Client { get; set; }

        public int PhysicalProgressId { get; set; }
        public PhysicalProgress PhysicalProgress { get; set; }

        //یعنی این مرحله از پیشرفت فیزیکی برای این مددجو انجام شده است
        // مثلا الان ساختمان در پایان مرحله سفت کاری می باشد
        public bool IsDone { get; set; }

        public ICollection<ClientPhysicalProgressPhoto> ClientPhysicalProgressPhotos { get; set; }


    }
}