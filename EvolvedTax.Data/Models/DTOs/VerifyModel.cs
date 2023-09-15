using EvolvedTax.Data.Models.Entities._1099;

namespace EvolvedTax.Data.Models.Entities
{
    public class VerifyModel
    {
        public IList<AuditTrail1099> AuditTrails { get; set; } = new List<AuditTrail1099>();
        public List<CheckboxItem> Items { get; set; } = new List<CheckboxItem>();

        public class CheckboxItem
        {
            public string FormName { get; set; }
            public bool IsSelected { get; set; }
        }
    }
}
