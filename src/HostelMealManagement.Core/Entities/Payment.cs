using HostelMealManagement.Core.Entities.BaseEntities;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace HostelMealManagement.Core.Entities;

public class Payment : AuditableEntity
{

 public string Month { get; set; }
 public int AmountPay { get; set; }
 public DateTimeOffset PaymentDate { get; set; }
 public string PaymentMethode { get; set; }
 public string InvoiceFile { get; set; }
 public bool Status { get; set; }
}
