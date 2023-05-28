using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BehzistiMaskan.Core.Models
{
    public class ClientUser
    {
        public string NationalCode { get; set; }

        public string ActivationCode { get; set; }

        //در صورتی که مددجوی تایید شده باشد یعنی در لیست جدول مددجویان می باشد
        // اما اگر تایید شده نبود در لیست مددجویان در لیست انتظار می باشند
        public bool IsApproved { get; set; }
    }
}