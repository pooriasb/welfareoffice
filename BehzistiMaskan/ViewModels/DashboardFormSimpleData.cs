
namespace BehzistiMaskan.ViewModels
{
    public class DashboardFormSimpleData
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Status { get; set; }

        public int Progress { get; set; }

        // اعتبار مورد نیاز
        public double NeededMoney { get; set; }

        // اعتبار پرداخت شده
        public double PayedMoney { get; set; }

    }
}