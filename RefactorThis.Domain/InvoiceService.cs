using System;
using System.Deployment.Internal;
using System.Linq;
using RefactorThis.Persistence;

namespace RefactorThis.Domain
{
	public class InvoiceService
	{
		private readonly InvoiceRepository _invoiceRepository;

		public const string ERROR_NO_INVOICE_MATCHING_THIS_PAYMENT = "There is no invoice matching this payment";
		public const string RES_NO_PAYMENT_NEEDED = "no payment needed";
        public const string ERROR_INVOICE_INVALID_STATE = "The invoice is in an invalid state, it has an amount of 0 and it has payments.";
        public const string RES_INVOICE_ALREADY_FULLY_PAID = "invoice was already fully paid";
        public const string RES_PAYMENT_GREATER_THAN_PARTIAL_AMOUNT_REMAINING = "the payment is greater than the partial amount remaining";
		public const string RES_FINAL_INVOICE_FULLY_PAID = "final partial payment received, invoice is now fully paid";
		public const string RES_ANOTHER_PARTIAL_NOT_FULLY_PAID = "another partial payment received, still not fully paid";
        public const string RES_PAYMENT_GREATER_THAN_INVOICE_AMOUNT = "the payment is greater than the invoice amount";
        public const string RES_INVOICE_NOW_FULLY_PAID = "invoice is now fully paid";
        public const string RES_INVOICE_NOW_PARTIALLY_PAID = "invoice is now partially paid";
		public const decimal TAX_RATE = 0.14m;

        public InvoiceService( InvoiceRepository invoiceRepository )
		{
			_invoiceRepository = invoiceRepository;
        }

		public string ProcessPayment( Payment payment )
		{
			var inv = _invoiceRepository.GetInvoice( payment.Reference );

			var responseMessage = string.Empty;

			if ( inv == null )
			{
				throw new InvalidOperationException(ERROR_NO_INVOICE_MATCHING_THIS_PAYMENT);
			}
			else
			{
				if ( inv.Amount == 0 )
				{
					if ( inv.Payments == null || !inv.Payments.Any( ) )
					{
						responseMessage = RES_NO_PAYMENT_NEEDED;
					}
					else
					{
						throw new InvalidOperationException(ERROR_INVOICE_INVALID_STATE);
					}
				}
				else
				{
					if ( inv.Payments != null && inv.Payments.Any( ) )
					{
						if ( inv.Payments.Sum( x => x.Amount ) != 0 && inv.Amount == inv.Payments.Sum( x => x.Amount ) )
						{
							responseMessage = RES_INVOICE_ALREADY_FULLY_PAID;
						}
						else if ( inv.Payments.Sum( x => x.Amount ) != 0 && payment.Amount > ( inv.Amount - inv.AmountPaid ) )
						{
							responseMessage = RES_PAYMENT_GREATER_THAN_PARTIAL_AMOUNT_REMAINING;
						}
						else
						{
							if ( ( inv.Amount - inv.AmountPaid ) == payment.Amount )
							{
								switch ( inv.Type )
								{
									case InvoiceType.Standard:
										inv.AmountPaid += payment.Amount;
										inv.Payments.Add( payment );
										responseMessage = RES_FINAL_INVOICE_FULLY_PAID;
										break;
									case InvoiceType.Commercial:
										inv.AmountPaid += payment.Amount;
										inv.TaxAmount += payment.Amount * TAX_RATE;
										inv.Payments.Add( payment );
										responseMessage = RES_FINAL_INVOICE_FULLY_PAID;
										break;
									default:
										throw new ArgumentOutOfRangeException( );
								}
								
							}
							else
							{
								switch ( inv.Type )
								{
									case InvoiceType.Standard:
										inv.AmountPaid += payment.Amount;
										inv.Payments.Add( payment );
										responseMessage = RES_ANOTHER_PARTIAL_NOT_FULLY_PAID;
										break;
									case InvoiceType.Commercial:
										inv.AmountPaid += payment.Amount;
										inv.TaxAmount += payment.Amount * TAX_RATE;
										inv.Payments.Add( payment );
										responseMessage = RES_ANOTHER_PARTIAL_NOT_FULLY_PAID;
										break;
									default:
										throw new ArgumentOutOfRangeException( );
								}
							}
						}
					}
					else
					{
						if ( payment.Amount > inv.Amount )
						{
							responseMessage = RES_PAYMENT_GREATER_THAN_INVOICE_AMOUNT;
						}
						else if ( inv.Amount == payment.Amount )
						{
							switch ( inv.Type )
							{
								case InvoiceType.Standard:
									inv.AmountPaid = payment.Amount;
									inv.TaxAmount = payment.Amount * TAX_RATE;
									inv.Payments.Add( payment );
									responseMessage = RES_INVOICE_NOW_FULLY_PAID;
									break;
								case InvoiceType.Commercial:
									inv.AmountPaid = payment.Amount;
									inv.TaxAmount = payment.Amount * TAX_RATE;
									inv.Payments.Add( payment );
									responseMessage = RES_INVOICE_NOW_FULLY_PAID;
									break;
								default:
									throw new ArgumentOutOfRangeException( );
							}
						}
						else
						{
							switch ( inv.Type )
							{
								case InvoiceType.Standard:
									inv.AmountPaid = payment.Amount;
									inv.TaxAmount = payment.Amount * TAX_RATE;
									inv.Payments.Add( payment );
									responseMessage = RES_INVOICE_NOW_PARTIALLY_PAID;
									break;
								case InvoiceType.Commercial:
									inv.AmountPaid = payment.Amount;
									inv.TaxAmount = payment.Amount * TAX_RATE;
									inv.Payments.Add( payment );
									responseMessage = RES_INVOICE_NOW_PARTIALLY_PAID;
									break;
								default:
									throw new ArgumentOutOfRangeException( );
							}
						}
					}
				}
			}
			
			inv.Save();

			return responseMessage;
		}
	}
}