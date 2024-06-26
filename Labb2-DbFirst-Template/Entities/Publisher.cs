﻿namespace Labb2_DbFirst_Template.Entities;

public partial class Publisher
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}
