using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Session11
{
    public class Book
    {
        public string ISBN { get; set; }
        public string Title { get; set; }
        public string[] Authors { get; set; }
        public DateTime PublicationDate { get; set; }
        public decimal Price { get; set; }

        public Book(string _ISBN, string _Title, string[] _Authors, DateTime _PublicationDate, decimal _Price)
        {
            ISBN = _ISBN;
            Title = _Title;
            Authors = _Authors;
            PublicationDate = _PublicationDate;
            Price = _Price;
        }

        public override string ToString() =>
        $"ISBN: {ISBN}, Title: {Title}, Authors: {string.Join(", ", Authors)}, " +
        $"Published: {PublicationDate:yyyy-MM-dd}, Price: {Price:C}";
    }

    public class BookFunctions
    {
        public static string GetTitle(Book book) => book.Title;
        public static string GetAuthors(Book book) => string.Join(", ", book.Authors);
        public static string GetPrice(Book book) => book.Price.ToString("C", CultureInfo.CurrentCulture);
        public static string GetISBN(Book book) => book.ISBN;
        public static string GetPublicationDate(Book book) => book.PublicationDate.ToString("yyyy-MM-dd");
    }

    public delegate string BookFormatter(Book book);

    public static class LibraryEngine
    {
        public static void ProcessBooks(List<Book> books, BookFormatter formatter)
        {
            foreach (Book book in books)
            {
                Console.WriteLine(formatter(book));
            }
        }

        public static void ProcessBooks(List<Book> books, Func<Book, string> formatter)
        {
            foreach (Book book in books)
            {
                Console.WriteLine(formatter(book));
            }
        }
    }
}
