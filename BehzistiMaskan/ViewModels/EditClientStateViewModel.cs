using System.Collections.Generic;
using BehzistiMaskan.Core.Dtos;
using BehzistiMaskan.Core.Utility;

namespace BehzistiMaskan.ViewModels
{
    public class EditClientStateViewModel
    {
        public ClientDto Client { get; set; }

        public ModelEnums.ClientStateTypeE NewClientStateTypeE { get; set; }

        public bool ClientHasCompleteData { get; set; }

        public bool NeedCoOrganizationApprove { get; set; }

        public string Description { get; set; }

        public IEnumerable<ClientStateLogDto> ClientStateLogs { get; set; }

        public bool IsShahrestanUser { get; set; }

        public bool IsNeedSazmanHamkarApprove { get; set; }
    }
}