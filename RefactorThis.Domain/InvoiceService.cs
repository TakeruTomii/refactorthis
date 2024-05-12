using System;
using System.Linq;
using RefactorThis.Persistence;

namespace RefactorThis.Domain
{
	public class InvoiceService
	{
		private readonly InvoiceRepository _invoiceRepository;

        public InvoiceService( InvoiceRepository invoiceRepository )
		{
			_invoiceRepository = invoiceRepository;
        }

		public string ProcessPayment( Payment payment )
        {
            var invoice = _invoiceRepository.GetInvoice(payment.Reference);

            var responseMessage = string.Empty;

            if (invoice == null)
            {
                throw new InvalidOperationException(ErrorMessages.ERROR_NO_INVOICE_MATCHING_THIS_PAYMENT);
            }

            if (IsPaymentNeeded(invoice))
            {
                if (HasPaymentInInvoice(invoice))
                {
                    throw new InvalidOperationException(ErrorMessages.ERROR_INVOICE_INVALID_STATE);
                }
                responseMessage = ResponseMessages.RES_NO_PAYMENT_NEEDED;
            }
            else
            {
                if (HasPaymentInInvoice(invoice))
                {
                    if (IsInvoiceAlreadyPaid(invoice))
                    {
                        responseMessage = ResponseMessages.RES_INVOICE_ALREADY_FULLY_PAID;
                    }
                    else if (IsOverPaidForRemainingInvoice(payment, invoice))
                    {
                        responseMessage = ResponseMessages.RES_PAYMENT_GREATER_THAN_PARTIAL_AMOUNT_REMAINING;
                    }
                    else
                    {
                        if (IsFullyPaidForRemainingInvoice(payment, invoice))
                        {
                            responseMessage = ResponseMessages.RES_FINAL_INVOICE_FULLY_PAID;
                        }
                        else
                        {
                            responseMessage = ResponseMessages.RES_ANOTHER_PARTIAL_NOT_FULLY_PAID;
                        }
                        invoice.AddPayments(payment);
                    }
                }
                else
                {
                    if (IsOverPaid(payment, invoice))
                    {
                        responseMessage = ResponseMessages.RES_PAYMENT_GREATER_THAN_INVOICE_AMOUNT;
                    }
                    else
                    {
                        if(IsFullyPaid(payment, invoice))
                        {
                            responseMessage = ResponseMessages.RES_INVOICE_NOW_FULLY_PAID;
                        }
                        else
                        {
                            responseMessage = ResponseMessages.RES_INVOICE_NOW_PARTIALLY_PAID;
                        }
                        invoice.AddPayments(payment);
                        
                    }
                }
            }

            invoice.Save();

            return responseMessage;
        }

        private static bool IsFullyPaid(Payment payment, IInvoice invoice)
        {
            return invoice.Amount == payment.Amount;
        }

        private static bool IsOverPaid(Payment payment, IInvoice invoice)
        {
            return payment.Amount > invoice.Amount;
        }

        private static bool IsFullyPaidForRemainingInvoice(Payment payment, IInvoice invoice)
        {
            return (invoice.Amount - invoice.AmountPaid) == payment.Amount;
        }

        private static bool IsOverPaidForRemainingInvoice(Payment payment, IInvoice invoice)
        {
            return invoice.Payments.Sum(x => x.Amount) != 0 && payment.Amount > (invoice.Amount - invoice.AmountPaid);
        }

        private static bool IsInvoiceAlreadyPaid(IInvoice invoice)
        {
            return invoice.Payments.Sum(x => x.Amount) != 0 && invoice.Amount == invoice.Payments.Sum(x => x.Amount);
        }

        private static bool HasPaymentInInvoice(IInvoice invoice)
        {
            return invoice.Payments != null && invoice.Payments.Any();
        }

        private static bool IsPaymentNeeded(IInvoice invoice)
        {
            return invoice.Amount == 0;
        }
    }
}