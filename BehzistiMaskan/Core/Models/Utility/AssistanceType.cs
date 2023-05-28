using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using BehzistiMaskan.Core.Models;

namespace BehzistiMaskan.Core.Utility
{
    public class AssistanceType
    {
        public AssistanceType()
        {
            Clients = new HashSet<Client>();
            UserInfos = new HashSet<UserInfo>();

        }
        public int Id { get; set; }

        [StringLength(60)]
        public string Name { get; set; }

        public ICollection<Client> Clients { get; set; }
        public ICollection<UserInfo> UserInfos { get; set; }
    }
}