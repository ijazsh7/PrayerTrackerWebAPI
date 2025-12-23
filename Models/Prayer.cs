using System;
using System.Collections.Generic;

namespace PrayerTrackerWebAPI.Models;

public partial class Prayer
{
    public int PrayerId { get; set; }

    public string Name { get; set; } = null!;

    public TimeOnly Time { get; set; }

    public virtual ICollection<PrayerRecord> PrayerRecords { get; set; } = new List<PrayerRecord>();
}
