using System;
using System.Collections.ObjectModel;

namespace PowerHub.UI.Services
{
    public enum ActivityKind
    {
        Info,
        Success,
        Warning,
        Error
    }

    public sealed class ActivityEntry
    {
        public DateTimeOffset Timestamp { get; init; }
        public ActivityKind Kind { get; init; }
        public string Message { get; init; } = string.Empty;
    }

    public interface IActivityLogService
    {
        ObservableCollection<ActivityEntry> Entries { get; }
        void Add(ActivityKind kind, string message);
    }
}
