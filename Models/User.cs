using System;
using System.Collections.Generic;

namespace PrayerTrackerWebAPI.Models;

public partial class User
{
    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public bool? IsActive { get; set; }

    public string Role { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<AdminLog> AdminLogs { get; set; } = new List<AdminLog>();

    public virtual ICollection<PrayerRecord> PrayerRecords { get; set; } = new List<PrayerRecord>();
}
