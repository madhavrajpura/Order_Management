using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class WishList
{
    public int Id { get; set; }

    public int ItemId { get; set; }

    public DateTime? LikedAt { get; set; }

    public int? LikedBy { get; set; }

    public virtual Item Item { get; set; } = null!;

    public virtual User? LikedByNavigation { get; set; }
}
