using OpenTelemetry.Trace;

namespace Micro.Core.Telemetry;

public static class TracerExtensions
{
    public static TelemetrySpan StartActive(this Tracer tracer, string name, SpanKind kind = SpanKind.Internal, TelemetrySpan? parentSpan = null)
    {
        return tracer.StartActiveSpan(name, kind, parentSpan ?? Tracer.CurrentSpan);
    }
}

