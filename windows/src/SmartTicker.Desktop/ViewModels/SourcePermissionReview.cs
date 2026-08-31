using System;

namespace SmartTicker.Desktop.ViewModels;

public sealed record SourcePermissionReview(
    Uri SourceUri,
    string Host,
    string SourceNames,
    string Symbols,
    string PolicySummary,
    string Guidance);

public enum SourcePermissionDecision
{
    Cancel,
    Skip,
    Approve,
}