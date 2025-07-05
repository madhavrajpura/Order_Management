using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class Notification
{
    public int Id { get; set; }

    public string Message { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public int CreatedBy { get; set; }

    public bool? IsActive { get; set; }

    public int? ItemId { get; set; }

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual Item? Item { get; set; }

    public virtual ICollection<UserNotification> UserNotifications { get; } = new List<UserNotification>();
}
