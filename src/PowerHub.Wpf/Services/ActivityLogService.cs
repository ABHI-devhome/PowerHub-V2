using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Threading;

namespace PowerHub.UI.Services
{
    public sealed class ActivityLogService : IActivityLogService
    {
        public ObservableCollection<ActivityEntry> Entries { get; } = new ObservableCollection<ActivityEntry>();

        public void Add(ActivityKind kind, string message)
        {
            if (string.IsNullOrWhiteSpace(message)) return;
            var app = Application.Current;
            if (app == null) return;
            var msg = message.Trim();
            var k = kind;
            var now = DateTimeOffset.Now;
            app.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                Entries.Insert(0, new ActivityEntry
                {
                    Timestamp = now,
                    Kind = k,
                    Message = msg
                });
                while (Entries.Count > 200)
                    Entries.RemoveAt(Entries.Count - 1);
            }));
        }
    }
}
