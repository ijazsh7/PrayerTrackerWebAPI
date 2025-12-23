using System;
using System.Collections.Generic;

namespace PrayerTrackerWebAPI.Models;

public partial class AdminLog
{
    public int LogId { get; set; }

    public int AdminId { get; set; }

    public string Action { get; set; } = null!;

    public DateTime? Timestamp { get; set; }

    public virtual User Admin { get; set; } = null!;
}
