using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using System.IO.Compression;

namespace Find_fb2
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine(@"Вставьте путь к папкам с книгами (к *.fb2 или *.zip). Например: C:\Users\q\Desktop\book\");
			string address;
			address = Console.ReadLine();
			if (!address.EndsWith("\\"))
				address += "\\";
			FileInfo[] array_zip_file = FindFile(address, "*.zip");
			string temp_dir = "__temp_fb2_rename\\";     // используется для вытаскивания файлов из zip. в конце удаляется.
			DirectoryInfo dir = new DirectoryInfo(address);
			if (!dir.Exists)
			{
				Console.WriteLine("Не существует такого пути!");
				return;
			}
			if (!Directory.Exists(address + temp_dir))
				Directory.CreateDirectory(address + temp_dir);  // создание папки __temp_fb2_rename
			foreach (FileInfo zip in array_zip_file)
			{
				if (zip.FullName.EndsWith(".zip"))  // вторая проверка. но пусть будет
				{
					using (ZipArchive archive = ZipFile.OpenRead(zip.FullName))
					{
						try
						{
							archive.ExtractToDirectory(address + temp_dir); // извлечение всего архива
						}
						catch
						{
							continue;
						}
					}
				}
			}
			FileInfo[] array_fb2_file = FindFile(address, "*.fb2");
			string new_directory = @"ru_name\";
			if (!Directory.Exists(address + new_directory))
				Directory.CreateDirectory(address + new_directory);  // создание папки ru_name
			foreach (FileInfo fb2 in array_fb2_file)
			{
				if (fb2.FullName.EndsWith(".fb2"))
				{
					using (XmlReader xmlReader = new XmlTextReader(fb2.FullName))
					{
						try
						{
							string last_name = "", book_title = "";
							while (xmlReader.Name != "last-name" && xmlReader.Read()) ;  //Пока не найдем узел book-title или конец файла
							if (xmlReader.Name == "last-name")
							{
								xmlReader.Read();                   //Читаем узел last-name
								last_name = xmlReader.Value;
								last_name = last_name.Trim();
								var matchs = Regex.Matches(last_name, @"[\w.,!()&#№ \-]*");
								last_name = "";
								foreach (Match mat in matchs)
									last_name += mat.Value;
							}
							else
								Console.WriteLine("Не нашел фамилию автора в файле: " + fb2.FullName);
							while (xmlReader.Name != "book-title" && xmlReader.Read()) ;  //Пока не найдем узел book-title или конец файла
							if (xmlReader.Name == "book-title")
							{
								xmlReader.Read();                   //Читаем узел book-title
								book_title = xmlReader.Value;
								book_title = book_title.Trim();
								var matchs = Regex.Matches(book_title, @"[\w.,!()&#№ \-]*");
								book_title = "";
								foreach (Match mat in matchs)
									book_title += mat.Value;
								Console.WriteLine(fb2.Name + "\t->\t" + last_name.ToUpper() + "\\" + book_title);
								if (!System.IO.Directory.Exists(address + new_directory + last_name.ToUpper() + @"\"))
								{
									System.IO.Directory.CreateDirectory(address + new_directory + last_name.ToUpper() + @"\");
								}
								File.Copy(fb2.FullName, address + new_directory + last_name.ToUpper() + @"\" + book_title + ".fb2", true);
							}
							else
								Console.WriteLine("Не нашел название книги в файле: " + fb2.FullName);

						}
						catch
						{
							Console.WriteLine("Ошибка в файле: " + fb2.FullName);
							continue;
						}
					}
				}
			}
			if (Directory.Exists(address + temp_dir))
				Directory.Delete(address + temp_dir, true);
		}
		static FileInfo[] FindFile(string address, string filename)
		{
			DirectoryInfo dir = new DirectoryInfo(address);
			FileInfo[] file_inf = null;
			if (dir.Exists)
			{
				file_inf = dir.GetFiles(filename);

				foreach (DirectoryInfo subdir in dir.GetDirectories())
					__FindFile(subdir, filename, ref file_inf);
			}
			return file_inf;
		}
		static void __FindFile(DirectoryInfo dir, string filename, ref FileInfo[] file_inf)
		{
			if (dir.Exists)
			{
				FileInfo[] file_inf2 = dir.GetFiles(filename);
				file_inf = file_inf.Concat(file_inf2).ToArray();

				foreach (DirectoryInfo subdir in dir.GetDirectories())
					__FindFile(subdir, filename, ref file_inf);
			}
		}

	}
}
