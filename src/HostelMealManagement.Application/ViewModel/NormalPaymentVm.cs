using System;

namespace HostelMealManagement.Application.ViewModel;

public class NormalPaymentVm
{
    public long Id { get; set; }

    public DateTimeOffset PaymentDate { get; set; }

    public long MemberId { get; set; }

    public long CycleId { get; set; }

    public string MemberName { get; set; } = string.Empty;

    public decimal PaymentAmount { get; set; }

    public string Remarks { get; set; } = string.Empty;
}
