using System;
using System.Collections.Generic;

using BehzistiMaskan.Core.Models.Utility;

namespace BehzistiMaskan.Core.Models.FormBuilder
{
    // گروه های کاربری سازمان های همکار که در این فرم مشارکت دارند
    // این گروه های کاربری اجازه دسترسی به اطلاعات طرح و اطلاعات مددجویان ثبت شده در 
    // آن طرح را دارند
    public class FormCoOrganizationRole
    {
        public int Id { get; set; }

        public int FormId { get; set; }
        public Form Form { get; set; }

        // نام گروه کاربری که به این طرح دسترسی دارد
        // به طور مثال بنیاد مسکن یا مجمع خیرین و سایر سازمان های همکار
        public int CoOrganizationTypeId { get; set; }
        public CoOrganizationType CoOrganizationType { get; set; }
    }
}