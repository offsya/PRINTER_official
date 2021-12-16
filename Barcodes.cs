using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Windows.Media.Imaging;

using ZXing;
using ZXing.Datamatrix;
using ZXing.OneD;

namespace TPExpCon
{
	public static class Barcodes
	{

		public static Bitmap GetDataMatrixBitmap(string code, Point size)
		{
			var writer = new BarcodeWriter
			{
				Format = BarcodeFormat.DATA_MATRIX,
				Options = new DatamatrixEncodingOptions { GS1Format = true, Width = size.Y, Height = size.X}
			};
			return writer.Write(code);
		}


	}
}
