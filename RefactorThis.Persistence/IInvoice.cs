using System.Collections.Generic;

namespace RefactorThis.Persistence
{
    public interface IInvoice
    {
        decimal Amount { get; set; }
        decimal AmountPaid { get; set; }
        List<Payment> Payments { get; set; }

        void Save();

        void AddPayments(Payment payment);
    }
}
