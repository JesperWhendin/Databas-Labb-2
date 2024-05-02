using System.Windows;
using System.Windows.Controls;
using Labb2_DbFirst_Template.Entities;
using Microsoft.EntityFrameworkCore;

namespace Labb2_WPFApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            using var dB = new BookDbContext();
            DataContext = this;
            InitializeComponent();
            StoreComboBox.ItemsSource = dB.Stores.ToList();
            BooksListBox.ItemsSource = dB.Books.ToList();
            BookInfoBox.Text = BookInfoBox_OnStartUp();
        }

        private string BookInfoBox_OnStartUp()
        {
            var infoTemplate = $"ISBN:\n" +
                                     $"Title:\n" +
                                     $"Price:\n" +
                                     $"Date:\n" +
                                     $"Pages:\n" +
                                     $"Language:\n" +
                                     $"Author:\n" +
                                     $"Publisher:\n" +
                                     $"Genre:";
            return infoTemplate;
        }

        private void LoadStoreBalance()
        {
            using var dB = new BookDbContext();
            StoreBalanceListBox.Items.Clear();
            var storeInventory = dB.Inventories
                .Include(i => i.Store)
                .Include(b => b.Isbn13Navigation)
                .Where(s => s.Store == StoreComboBox.SelectedItem);
            foreach (var b in storeInventory)
            {
                StoreBalanceListBox.Items.Add(b);
            }
        }

        private void StoreComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadStoreBalance();
        }

        private void StoreBalanceListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            using var dB = new BookDbContext();
            if (StoreBalanceListBox.SelectedItem is null)
            {
                BookInfoBox.Clear();
                BookInfoBox.Text = $"ISBN:\n" +
                                   $"Title:\n" +
                                   $"Price:\n" +
                                   $"Date:\n" +
                                   $"Pages:\n" +
                                   $"Language:\n" +
                                   $"Author:\n" +
                                   $"Publisher:\n" +
                                   $"Genre:";
            }

            if (StoreBalanceListBox.SelectedItem is Inventory selectedBook)
            {
                var title = dB.Books
                    .Where(b => b.Isbn13 == selectedBook.Isbn13)
                    .Select(b => b.Title)
                    .FirstOrDefault();

                var price = dB.Books
                    .Where(p => p.Isbn13 == selectedBook.Isbn13)
                    .Select(p => p.Price)
                    .FirstOrDefault();

                var date = dB.Books
                    .Where(d => d.Isbn13 == selectedBook.Isbn13)
                    .Select(d => d.DateOfIssue)
                    .FirstOrDefault();

                var pages = dB.Books
                    .Where(p => p.Isbn13 == selectedBook.Isbn13)
                    .Select(p => p.Pages)
                    .FirstOrDefault();

                var language = dB.Books
                    .Where(l => l.Isbn13 == selectedBook.Isbn13)
                    .Select(l => l.Language)
                    .FirstOrDefault();

                var author = dB.Books
                    .Where(a => a.Isbn13 == selectedBook.Isbn13)
                    .Select(a => a.Author)
                    .FirstOrDefault();

                var publisher = dB.Books
                    .Where(p => p.Isbn13 == selectedBook.Isbn13)
                    .Select(p => p.Publisher)
                    .FirstOrDefault();

                var genre = dB.Books
                    .Where(g => g.Isbn13 == selectedBook.Isbn13)
                    .Select(g => g.Genre)
                    .FirstOrDefault();

                BookInfoBox.Text = $"ISBN: {selectedBook.Isbn13}\n" +
                                   $"Title: {title}\n" +
                                   $"Price: {Convert.ToString(price)} kr\n" +
                                   $"Date: {date.ToShortDateString()}\n" +
                                   $"Pages: {pages}\n" +
                                   $"Language: {language}\n" +
                                   $"Author: {author.FirstName} {author.LastName}\n" +
                                   $"Publisher: {publisher.Name}\n" +
                                   $"Genre: {genre.Genre1}";


                UpdateBalanceTextBox.Text = Convert.ToString(selectedBook.Balance);
            }
        }

        private void UpdateBalanceBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (StoreBalanceListBox.SelectedItem is null)
            {
                return;
            }
            if (StoreBalanceListBox.SelectedItem is Inventory selectedBook && StoreComboBox.SelectedItem is Store selectedStore)
            {
                using var dB = new BookDbContext();
                int updatedBalance;
                if (!int.TryParse(UpdateBalanceTextBox.Text, out updatedBalance))
                {
                    updatedBalance = 0;
                    MessageBox.Show("Explicitly type an integer and nothing else.", "Error.");
                    return;
                }
                if (updatedBalance < 0)
                {
                    updatedBalance = 0;
                    MessageBox.Show("The inventory balance cannot be less than zero.", "Error.");
                    return;
                }
                var bookBalance = dB.Inventories.FirstOrDefault(i => i.Isbn13 == selectedBook.Isbn13 && i.StoreId == selectedStore.Id);
                bookBalance.Balance = updatedBalance;
                dB.SaveChanges();
                LoadStoreBalance();
            }
        }

        private void AddBookToStoreBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (StoreComboBox.SelectedItem is null || BooksListBox.SelectedItem is null)
            {
                MessageBox.Show(
                    "Make sure there's a store selected in the drop down menu to the left and that you have selected a book in the list of available books to the right.", "Error.");
                return;
            }
            if (StoreComboBox.SelectedItem is Store selectedStore && BooksListBox.SelectedItem is Book selectedBook)
            {
                using var dB = new BookDbContext();
                if (dB.Inventories.Any(i => i.Isbn13 == selectedBook.Isbn13 && i.StoreId == selectedStore.Id))
                {
                    MessageBox.Show("This book has already been added to the selected store.", "Error");
                    return;
                }
                var newInventory = new Inventory()
                {
                    Balance = 0,
                    Isbn13 = selectedBook.Isbn13,
                    StoreId = selectedStore.Id
                };
                dB.Inventories.Add(newInventory);
                dB.SaveChanges();
                LoadStoreBalance();
            }
        }

        private void RemoveBookFromStoreBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (StoreComboBox.SelectedItem is null || StoreBalanceListBox.SelectedItem is null)
            {
                MessageBox.Show("Make sure there's a store selected in the drop down menu to the left and that you have selected a book in the inventory balance box below.", "Error.");
                return;
            }
            if (StoreComboBox.SelectedItem is Store selectedStore && StoreBalanceListBox.SelectedItem is Inventory selectedInventory)
            {
                using var dB = new BookDbContext();
                var bokrem = dB.Inventories.FirstOrDefault(i => i.Isbn13 == selectedInventory.Isbn13 && i.StoreId == selectedStore.Id);
                dB.Inventories.Remove(bokrem);
                dB.SaveChanges();
                LoadStoreBalance();
            }
        }
    }
}