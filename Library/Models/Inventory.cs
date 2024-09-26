using Microsoft.CodeAnalysis.Elfie.Serialization;

namespace Library.Models
{
    public class Inventory
    {
        private Dictionary<Book,int> bookStock = new Dictionary<Book,int>();

        public void AddBook(Book book, int count)
        {
            if (bookStock.ContainsKey(book))
            {
                bookStock[book] += count;
            }
            else 
            {
                bookStock[book] = count;
            }
        }

        public void RemoveBook(Book book, int count)
        {
            if (bookStock.ContainsKey(book) && bookStock[book] >= count)
            {
                bookStock[book] -= count;
                if (bookStock[book] == 0)
                {
                    bookStock.Remove(book);
                }
            }
            else
            {
                Console.WriteLine("Не може да се премахне повече от наличната бройка.");
            }
        }
        public void ShowInvetory()
        {
            foreach (var item in bookStock)
            {
                Console.WriteLine($"{item} - {item.Value} налични");
            }
        }
    }
}
