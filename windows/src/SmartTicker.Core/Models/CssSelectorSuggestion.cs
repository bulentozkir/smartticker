namespace SmartTicker.Core.Models;

public sealed record CssSelectorSuggestion(
    string Selector,
    string SampleValue,
    int Confidence,
    string Reason);