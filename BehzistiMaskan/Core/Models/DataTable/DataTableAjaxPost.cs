using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BehzistiMaskan.Core.Models.DataTable
{
    public class DataTableAjaxPost
    {
        public int Id { get; set; }
        public int Draw { get; set; }
        public int Start { get; set; }
        public int Length { get; set; }
        public List<DataTableColumn> Columns { get; set; }
        public DataTableSearch Search { get; set; }
        public List<DataTableOrder> Order { get; set; }
    }
}