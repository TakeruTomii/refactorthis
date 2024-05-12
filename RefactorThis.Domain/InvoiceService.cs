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
			var invoice = _invoiceRepository.GetInvoice( payment.Reference );

			var responseMessage = string.Empty;

			if ( invoice == null )
			{
				throw new InvalidOperationException(ErrorMessages.ERROR_NO_INVOICE_MATCHING_THIS_PAYMENT);
			}

            if (invoice.Amount == 0)
            {
                if (!(invoice.Payments != null && invoice.Payments.Any()))
                {
                    responseMessage = ResponseMessages.RES_NO_PAYMENT_NEEDED;
                }
                else
                {
                    throw new InvalidOperationException(ErrorMessages.ERROR_INVOICE_INVALID_STATE);
                }
            }
            else
            {
                if (invoice.Payments != null && invoice.Payments.Any())
                {
                    if (invoice.Payments.Sum(x => x.Amount) != 0 && invoice.Amount == invoice.Payments.Sum(x => x.Amount))
                    {
                        responseMessage = ResponseMessages.RES_INVOICE_ALREADY_FULLY_PAID;
                    }
                    else if (invoice.Payments.Sum(x => x.Amount) != 0 && payment.Amount > (invoice.Amount - invoice.AmountPaid))
                    {
                        responseMessage = ResponseMessages.RES_PAYMENT_GREATER_THAN_PARTIAL_AMOUNT_REMAINING;
                    }
                    else
                    {
                        if ((invoice.Amount - invoice.AmountPaid) == payment.Amount)
                        {
                            switch (invoice.Type)
                            {
                                case InvoiceType.Standard:
                                    invoice.AmountPaid += payment.Amount;
                                    invoice.Payments.Add(payment);
                                    responseMessage = ResponseMessages.RES_FINAL_INVOICE_FULLY_PAID;
                                    break;
                                case InvoiceType.Commercial:
                                    invoice.AmountPaid += payment.Amount;
                                    invoice.TaxAmount += payment.Amount * InvoiceConstants.TAX_RATE;
                                    invoice.Payments.Add(payment);
                                    responseMessage = ResponseMessages.RES_FINAL_INVOICE_FULLY_PAID;
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }

                        }
                        else
                        {
                            switch (invoice.Type)
                            {
                                case InvoiceType.Standard:
                                    invoice.AmountPaid += payment.Amount;
                                    invoice.Payments.Add(payment);
                                    responseMessage = ResponseMessages.RES_ANOTHER_PARTIAL_NOT_FULLY_PAID;
                                    break;
                                case InvoiceType.Commercial:
                                    invoice.AmountPaid += payment.Amount;
                                    invoice.TaxAmount += payment.Amount * InvoiceConstants.TAX_RATE;
                                    invoice.Payments.Add(payment);
                                    responseMessage = ResponseMessages.RES_ANOTHER_PARTIAL_NOT_FULLY_PAID;
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                        }
                    }
                }
                else
                {
                    if (payment.Amount > invoice.Amount)
                    {
                        responseMessage = ResponseMessages.RES_PAYMENT_GREATER_THAN_INVOICE_AMOUNT;
                    }
                    else if (invoice.Amount == payment.Amount)
                    {
                        switch (invoice.Type)
                        {
                            case InvoiceType.Standard:
                                invoice.AmountPaid = payment.Amount;
                                invoice.TaxAmount = payment.Amount * InvoiceConstants.TAX_RATE;
                                invoice.Payments.Add(payment);
                                responseMessage = ResponseMessages.RES_INVOICE_NOW_FULLY_PAID;
                                break;
                            case InvoiceType.Commercial:
                                invoice.AmountPaid = payment.Amount;
                                invoice.TaxAmount = payment.Amount * InvoiceConstants.TAX_RATE;
                                invoice.Payments.Add(payment);
                                responseMessage = ResponseMessages.RES_INVOICE_NOW_FULLY_PAID;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                    else
                    {
                        switch (invoice.Type)
                        {
                            case InvoiceType.Standard:
                                invoice.AmountPaid = payment.Amount;
                                invoice.TaxAmount = payment.Amount * InvoiceConstants.TAX_RATE;
                                invoice.Payments.Add(payment);
                                responseMessage = ResponseMessages.RES_INVOICE_NOW_PARTIALLY_PAID;
                                break;
                            case InvoiceType.Commercial:
                                invoice.AmountPaid = payment.Amount;
                                invoice.TaxAmount = payment.Amount * InvoiceConstants.TAX_RATE;
                                invoice.Payments.Add(payment);
                                responseMessage = ResponseMessages.RES_INVOICE_NOW_PARTIALLY_PAID;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                }
            }

            invoice.Save();

			return responseMessage;
		}
	}
}