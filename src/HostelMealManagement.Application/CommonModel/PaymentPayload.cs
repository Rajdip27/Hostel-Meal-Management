namespace HostelMealManagement.Application.CommonModel;

public class PaymentPayload
{
    public long BillId { get; set; }
    public long CycleId { get; set; }
    public long MemberId { get; set; }
    public DateTime ExpireAt { get; set; }
}
