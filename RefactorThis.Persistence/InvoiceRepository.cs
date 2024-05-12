namespace RefactorThis.Persistence {
	public class InvoiceRepository
	{
		private IInvoice _invoice;

		public IInvoice GetInvoice( string reference )
		{
			return _invoice;
		}

		public void SaveInvoice(IInvoice invoice )
		{
			//saves the invoice to the database
		}

		public void Add(IInvoice invoice )
		{
			_invoice = invoice;
		}
	}
}