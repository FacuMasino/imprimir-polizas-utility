using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImprimirPolizas
{
    public class PolicyDocs
    {
        public string SummaryId { get; set; }
        public string PolicyId { get; set; }
        public string CardId { get; set; }
        public string PaymentCupon { get; set; }
        public string MercosurPolicy { get; set; }

        public string GetIdByOption(ScTools.DownloadOpt opt)
        {
            switch (opt)
            {
                case ScTools.DownloadOpt.policy:
                    return PolicyId;
                case ScTools.DownloadOpt.invoice:
                    return SummaryId;
                case ScTools.DownloadOpt.coupons:
                    return PaymentCupon;
                case ScTools.DownloadOpt.policyCard:
                    return CardId;
                case ScTools.DownloadOpt.mercosur:
                    return MercosurPolicy;
                default:
                    return "0";
            }
        }
    }
}
