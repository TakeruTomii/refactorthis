namespace RefactorThis.Domain
{
    public class ResponseMessages
    {
        public const string RES_NO_PAYMENT_NEEDED = "no payment needed";
        public const string RES_INVOICE_ALREADY_FULLY_PAID = "invoice was already fully paid";
        public const string RES_PAYMENT_GREATER_THAN_PARTIAL_AMOUNT_REMAINING = "the payment is greater than the partial amount remaining";
        public const string RES_FINAL_INVOICE_FULLY_PAID = "final partial payment received, invoice is now fully paid";
        public const string RES_ANOTHER_PARTIAL_NOT_FULLY_PAID = "another partial payment received, still not fully paid";
        public const string RES_PAYMENT_GREATER_THAN_INVOICE_AMOUNT = "the payment is greater than the invoice amount";
        public const string RES_INVOICE_NOW_FULLY_PAID = "invoice is now fully paid";
        public const string RES_INVOICE_NOW_PARTIALLY_PAID = "invoice is now partially paid";
    }
}
