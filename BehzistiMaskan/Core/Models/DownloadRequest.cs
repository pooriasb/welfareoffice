
using System;

namespace BehzistiMaskan.Core.Models
{
    public class DownloadRequest
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        public UserInfo UserInfo { get; set; }

        public int ClientId { get; set; }
        public Client Client { get; set; }

        public DateTime RequestTime { get; set; }

        public DateTime RequestExpireTime { get; set; }
    }
}