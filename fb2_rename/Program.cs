using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace fb2_rename
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(@"Вставьте путь к папкам с книгами. Например: C:\Users\q\Desktop\book\");
            string address;
            address = Console.ReadLine();
            if (!address.EndsWith("\\"))
                address += "\\";
            string new_directory = @"ru_name\";
            
            DirectoryInfo dir = new DirectoryInfo(address);
            foreach (var item in dir.GetDirectories())  // обход по каталогам
            {
                foreach (var it in item.GetFiles())     // обход по файлам
                {
                    //Console.Write(it.Name + " = ");
                    XmlReader xmlReader = new XmlTextReader(it.FullName);
                    do xmlReader.Read(); //Читаем документ
                    while (xmlReader.Name != "book-title"); //Пока не найдем узел book-title

                    xmlReader.Read();                   //Читаем узел book-title
                    string book_title = xmlReader.Value;
                    book_title =  book_title.Trim();
                    var matchs = Regex.Matches(book_title, @"[\w.,!()&#№ \-]*");
                    book_title = "";
                    foreach(Match mat in matchs)
                        book_title += mat.Value;
                    Console.WriteLine(item.Name + "\t->\t" + book_title);
                    if (!System.IO.Directory.Exists(address + new_directory))
                    {
                        System.IO.Directory.CreateDirectory(address + new_directory);
                    }
                    if (!System.IO.Directory.Exists(address + new_directory + item.Name + @"\"))
                    {
                        System.IO.Directory.CreateDirectory(address + new_directory + item.Name + @"\");
                    }
                    File.Copy(it.FullName, address + new_directory + item.Name + @"\" + book_title + ".fb2", true);
                }
            }
            foreach (var it in dir.GetFiles())     // обход по файлам
            {
                //Console.Write(it.Name + " = ");
                XmlReader xmlReader = new XmlTextReader(it.FullName);
                do xmlReader.Read(); //Читаем документ
                while (xmlReader.Name != "book-title"); //Пока не найдем узел book-title

                xmlReader.Read();                   //Читаем узел book-title
                string book_title = xmlReader.Value;
                book_title = book_title.Trim();
                var matchs = Regex.Matches(book_title, @"[\w.,!()&#№ \-]*");
                book_title = "";
                foreach (Match mat in matchs)
                    book_title += mat.Value;
                Console.WriteLine(book_title);
                if (!System.IO.Directory.Exists(address + new_directory))
                {
                    System.IO.Directory.CreateDirectory(address + new_directory);
                }
                if (!System.IO.Directory.Exists(address + new_directory))
                {
                    System.IO.Directory.CreateDirectory(address + new_directory);
                }
                File.Copy(it.FullName, address + new_directory + book_title + ".fb2", true);
            }
            Console.WriteLine();
            Console.WriteLine(@"Результат работы в папке: " + address + new_directory);
            Console.ReadLine();
        }
    }
}
