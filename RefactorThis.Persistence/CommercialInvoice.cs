using System.Collections.Generic;

namespace RefactorThis.Persistence
{
    public class CommercialInvoice : IInvoice
    {
        private readonly InvoiceRepository _repository;
        private const decimal TAX_RATE = 0.14m;
        public CommercialInvoice(InvoiceRepository repository)
        {
            _repository = repository;
        }

        public void Save()
        {
            _repository.SaveInvoice(this);
        }

        public void AddPayments(Payment payment)
        {
            AmountPaid = payment.Amount;
            TaxAmount = payment.Amount * TAX_RATE;
            Payments.Add(payment);
        }

        public decimal Amount { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal TaxAmount { get; set; }
        public List<Payment> Payments { get; set; }
    }
}
