using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sqldiff
{
    [Serializable]
    public class SystemEvent
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string SortKey { get; set; }
        public string Description { get; set; }
        public string Category1 { get; set; }
        public string Category2 { get; set; }
        public string CustomerSPERA { get; set; }
        public string DeviceSPERA { get; set; }
        public string ProductPERA { get; set; }
        public string InvoiceSPERA { get; set; }
        public string AccountSPERA { get; set; }
        public string EnableRequireReason { get; set; }
        public string EnableEditReason { get; set; }
        public string AgreementSPERA { get; set; }
        public string ShippingOrderSPERA { get; set; }
        public string WorkOrderSPERA { get; set; }
        public string EntityKey { get; set; }
        public string IsObsolete { get; set; }
        public string AllowForLedgerAccount { get; set; }
        public string SkipOnHistoryScreen { get; set; }
        public string ContactSPEAR { get; set; }
        public string IsSettlement { get; set; }
    }
}
