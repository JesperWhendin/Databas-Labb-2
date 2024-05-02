namespace Labb2_DbFirst_Template.Entities;

public partial class Author
{
    public int Id { get; set; }

    public string? FirstName { get; set; }

    public string LastName { get; set; } = null!;

    public DateOnly DateOfBirth { get; set; }

    public DateOnly? DateOfDeath { get; set; }

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}
