using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class UserNotification
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int NotificationId { get; set; }

    public bool IsRead { get; set; }

    public DateTime? ReadAt { get; set; }

    public virtual Notification Notification { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
