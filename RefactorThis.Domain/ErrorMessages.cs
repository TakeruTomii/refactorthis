namespace RefactorThis.Domain
{
    public class ErrorMessages
    {
        public const string ERROR_NO_INVOICE_MATCHING_THIS_PAYMENT = "There is no invoice matching this payment";
        public const string ERROR_INVOICE_INVALID_STATE = "The invoice is in an invalid state, it has an amount of 0 and it has payments.";
    }
}
