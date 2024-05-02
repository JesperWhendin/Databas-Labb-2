namespace Labb2_DbFirst_Template.Entities;

public partial class Book
{
    public string Isbn13 { get; set; } = null!;

    public string Title { get; set; } = null!;

    public double Price { get; set; }

    public DateOnly DateOfIssue { get; set; }

    public int Pages { get; set; }

    public string Language { get; set; } = null!;

    public int AuthorId { get; set; }

    public int PublisherId { get; set; }

    public int? GenreId { get; set; }

    public virtual Author Author { get; set; } = null!;

    public virtual Genre? Genre { get; set; }

    public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();

    public virtual Publisher Publisher { get; set; } = null!;

    public override string ToString()
    {
        var book = string.Empty;
        book += $"{Title}";
        return book;
    }
}
