using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class ItemImage
{
    public int Id { get; set; }

    public int ItemId { get; set; }

    public string ImageUrl { get; set; } = null!;

    public bool IsThumbnail { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }

    public bool IsDelete { get; set; }

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual Item Item { get; set; } = null!;

    public virtual User? UpdatedByNavigation { get; set; }
}
