using System;
using System.Collections.Generic;

namespace PrayerTrackerWebAPI.Models;

public partial class PrayerGuidance
{
    public int GuidanceId { get; set; }

    public string Title { get; set; } = null!;

    public string? VideoUrl { get; set; }

    public string Content { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }
}
