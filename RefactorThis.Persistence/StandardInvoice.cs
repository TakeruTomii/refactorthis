using System.Collections.Generic;

namespace RefactorThis.Persistence
{
    public class StandardInvoice : IInvoice
    {
        private readonly InvoiceRepository _repository;
        public StandardInvoice(InvoiceRepository repository)
        {
            _repository = repository;
        }

        public void Save()
        {
            _repository.SaveInvoice(this);
        }

        public void AddPayments(Payment payment)
        {
            AmountPaid += payment.Amount;
            Payments.Add(payment);
        }

        public decimal Amount { get; set; }
        public decimal AmountPaid { get; set; }
        public List<Payment> Payments { get; set; }
    }
}