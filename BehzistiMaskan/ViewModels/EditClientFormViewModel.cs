using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BehzistiMaskan.Core.Dtos;
using BehzistiMaskan.Core.Models.FormBuilder;

namespace BehzistiMaskan.ViewModels
{
    public class EditClientFormViewModel
    {
        public ClientDto Client { get; set; }

        [Display(Name = "طرح هایی که مددجو می تواند در آنها ثبت شود")]
        public IEnumerable<Form> AvailableForms { get; set; }

        [Required(ErrorMessage = "انتخاب طرح ضروری می باشد")]
        public int SelectedFormId { get; set; }

        [Display(Name = "طرح هایی که مددجو در آن ثبت شده است")]
        public List<ClientForm> ClientRegisteredForms { get; set; }

        [Display(Name = "سایر طرح ها")]
        public IEnumerable<Form> NotAvailableForms { get; set; }

    }
}