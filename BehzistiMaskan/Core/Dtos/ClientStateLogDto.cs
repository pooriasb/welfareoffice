
using MD.PersianDateTime;

namespace BehzistiMaskan.Core.Dtos
{
    public class ClientStateLogDto
    {
        public string ClientStateTypeStr { get; set; }

        public PersianDateTime LogDate { get; set; }
        public string Description { get; set; }
    }
}