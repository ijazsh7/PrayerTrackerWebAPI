using System;
using System.Collections.Generic;

namespace PrayerTrackerWebAPI.Models;

public partial class PrayerRecord
{
    public int PrayerRecordId { get; set; }

    public int UserId { get; set; }

    public int PrayerId { get; set; } 

    public string Status { get; set; } = null!;

    public DateTime? RecordedAt { get; set; }

    public virtual Prayer Prayer { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
