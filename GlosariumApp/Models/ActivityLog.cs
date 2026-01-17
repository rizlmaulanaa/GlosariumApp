using System;

namespace GlosariumApp.Models
{
    // CLASS BARU: Log aktivitas untuk tracking
    // PERSYARATAN: Date, Time, String
    public class ActivityLog
    {
        public DateTime Timestamp { get; set; }
        public string Action { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
        public string User { get; set; } = string.Empty;

        public ActivityLog(string action, string details)
        {
            // PERSYARATAN: Date, Time - Pencatatan waktu otomatis
            Timestamp = DateTime.Now;
            Action = action;
            Details = details;
            User = Environment.UserName;
        }

        // PERSYARATAN: Formatted text untuk log entry
        public override string ToString()
        {
            return $"[{Timestamp:yyyy-MM-dd HH:mm:ss}] {Action}: {Details} (User: {User})";
        }
    }
}