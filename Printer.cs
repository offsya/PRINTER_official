using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Printing;

using Image = System.Drawing.Image;
using Point = System.Drawing.Point;

namespace TPExpCon
{
	public static class Printer
	{
		private static PrintDocument PrintDocument { get; set; }
	
		private static List<Image> Images { get; set; }
		private static Point Position { get; set; }

		static Printer()
		{
			var printerName = GetPrinters().FirstOrDefault(printer => printer.Contains("Val"));
			PrintDocument = new PrintDocument
			{
				PrinterSettings = { PrinterName = printerName },
				PrintController = new StandardPrintController()
			};
			PrintDocument.PrintPage += Pd_PrintPage;
		}
		/// <summary>
		/// Get list of installed printers
		/// </summary>
		/// <returns>List of installed printers</returns>
		public static IEnumerable<string> GetPrinters() => new LocalPrintServer()
				.GetPrintQueues()
				.Select(p => p.Name);

		/// <summary>
		/// Print codes one-by-one (1 code - 1 job for printer)
		/// </summary>
		/// <param name="codes">List of codes</param>
		/// <param name="position">Position on page</param>
		public static void PrintOneByOne(List<string> codes, Point position, Point size)
		{	
			if (codes.Count == 1)
			{
				var bitmap = Barcodes.GetDataMatrixBitmap(codes[0], size);
				bitmap.Save("New.bmp");
			}
			Position = position;
			Images = codes.Select(code => BitmapToImage(Barcodes.GetDataMatrixBitmap(code, size))).ToList();
			var count = codes.Count();

			
			IEnumerable<string> printers = Printer.GetPrinters();
			int i = 1;
			int lenght;
			bool k = true;
			Console.Write("\n\t\tДоступные принтеры:\t\t\n");
			foreach (string printer in printers)
			{
				Console.Write("\t{0} - {1}\n", i, printer);
				i++;
				
			}
			;
			lenght = i;
			string n;
			do
			{
				Console.Write("Введите номер принтера для печати(1 - {0})>> \n", lenght-1);
			} while (!Int32.TryParse(Console.ReadLine(), out i) || i < 1 || i >= lenght);
			int j = 1;
			foreach (string printer in printers)
			{
				if (j == i)
				{
					Printer.PrintDocument.PrinterSettings.PrinterName = printer;
					break;
				}
				j++;
			}
			Console.Write("Выберите режим печати >> \n");
			Console.Write("1.Одно нажатие - одна печать >> \n");
			Console.Write("2.Печатать N-ое количество >> \n");
			Console.Write("3.Печатать всё(Беспрерывная печать) >> \n");
			k = true;
			while (k)
			{
				n = Console.ReadLine();
				Console.Write("{0}\n", n);
				if (n == "1")
				{
					k = false;
					for (i = 0; i < count; i++)
					{
						Console.Write("Нажимайте <Space> для печати: {0}\n", codes[i]);

						while (Console.ReadKey().Key != ConsoleKey.Spacebar) { }
						PrintDocument.Print(); 
					}
					
				}
				else if (n == "2")
				{
					while (k)
					{
						k = false;
						Console.Write("Введите количество печатаемых кодов >> \n");
						var buf = Console.ReadLine();
						int N;
						try
						{
							N = Convert.ToInt32(buf);
							
						}
						catch
						{
							k = true;
							Console.Write("Введите количество печатаемых кодов(Только числа) >> \n");
							continue;
						}
						try
						{
							for (i = 0; i < N; i++)
							{
								Console.Write("Будет распечатанно N кодов: {0}\n", N);
								PrintDocument.Print();
							}
						}
						catch
						{
							k = true;
							break;
						}
					}
				}
				else if (n == "3")
				{
					k = false;
					PrintDocument.Print();
				}
				else
				{
					Console.Write("Только числа от 1 до 3!\n");
					
				}
			}
		}

		private static Image BitmapToImage(Bitmap codeBitmap)
		{
			using (var imageStream = new MemoryStream())
			{
				codeBitmap.Save(imageStream, ImageFormat.Png);
				return Image.FromStream(imageStream);
			}
		}
		/// <summary>
		/// Add image to printer job
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private static void Pd_PrintPage(object sender, PrintPageEventArgs e)
		{
			var image = Images.ElementAt(0);
			e.Graphics.DrawImage(image, Position);
			Images.RemoveAt(0);
		}
	
	}
}
