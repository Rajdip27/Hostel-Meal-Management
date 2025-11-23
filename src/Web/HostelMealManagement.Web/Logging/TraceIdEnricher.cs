using Serilog.Core;
using Serilog.Events;
using System.Diagnostics;

namespace HostelMealManagement.Web.Logging;

public class TraceIdEnricher : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var traceId = Activity.Current?.TraceId.ToString() ?? "none";
        logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("TraceId", traceId));
    }
}
