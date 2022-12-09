using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SweetPOS
{
    public class EnumClass
    {
        public enum SystemOperatingMode
        {
            ChitaleRFID,
            TouchScreenWithRFID,
            TouchScreenWithNetworking,
            TouchScreenAtCounterWithBarCode,
            ManualBilling,
            TouchScreenWithoutRFID
        }

        public enum FormViewMode
        {
            Addmode,
            EditMode
        }

        public enum ReportFormMode
        {
            SaleSummaryVat,
            SaleSummaryPeriodical,
            CashierwiseSale,
            ShowBill,
            StockReport,
            ChallanReport,
            PurchaseReport,
            CashBankTransaction,
            SaleSummaryPeriodicalItemwise,
            TimewiseSaleSummary,
            iSaleSummary,
            SaleSummaryByBill,
            MaterialRegister,
            SalePayment,
            SaleTypeWise,
            CustomerMessages,
            MilkSummary,
            ProductSummary,
            CustomerBalance,
            CustomerBalanceWithMilkDetails,
            LineWiseOutStanding,
            CustomerOutStanding,
            CowBuffloMilkQuantity
            
        }

        public enum SaleProcessingMode
        {
            ProcessingMode,
            PrintMode,
            PaymentMode
        }
        public enum UserRoles
        {
            Administrator,
            Cashier,
            SalesCounter
        }
    }
}