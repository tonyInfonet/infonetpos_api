// VBConversions Note: VB project level imports
//using AxccrpMonthcal6;
//using AxCCRPDTP6;
using System.Windows.Forms;
// End of VB project level imports

using System.Runtime.InteropServices;
//TODO: Smriti_33 all errors of width height setting commented as ///
namespace Infonet.CStoreCommander.BusinessLayer.Entities
{
    sealed class modColrForm
	{
		public const int GRADIENT_FILL_RECT_H = 0x0;
		public const int GRADIENT_FILL_RECT_V = 0x1;
		public const int GRADIENT_FILL_TRIANGLE = 0x2;
		public static int GRADIENT_FILL_RECT_DIRECTION;
		
		public const int GRADIENT_COLOR1 = 0xFF0000;
		public const int GRADIENT_COLOR2 = 0x0;
		
		public struct TRIVERTEX
		{
			public int x;
			public int Y;
			public short Red;
			public short Green;
			public short Blue;
			public short Alpha;
		}
		
		public struct GRADIENT_TRIANGLE
		{
			public int Vertex1;
			public int Vertex2;
			public int Vertex3;
		}
		
		public struct GRADIENT_RECT
		{
			public int UpperLeft;
			public int LowerRight;
		}
		
		[DllImport("msimg32", ExactSpelling=true, CharSet=CharSet.Ansi, SetLastError=true)]
		public static extern int GradientFill(int hdc, ref object pVertex, int dwNumVertex, ref object pMesh, int dwNumMesh, int dwMode);
		
		public static void DrawGradientFillTriangle(ref int dwColour1, ref int dwColour2, ref int dwColour3, ref int dwColour4, ref Form myFrm)
		{
			
			GRADIENT_TRIANGLE[] grTri = new GRADIENT_TRIANGLE[2];
			TRIVERTEX[] vert = new TRIVERTEX[4];
			
			//Colour at upper-left corner
			TRIVERTEX with_1 = vert[0];
			with_1.x = 0;
			with_1.Y = 0;
			with_1.Red = LongToSignedShort(((short) (dwColour1 & 0xFF)) * 256);
			with_1.Green = LongToSignedShort(System.Convert.ToInt32(((dwColour1 & 0xFF00) / 0x100) * 256));
			with_1.Blue = LongToSignedShort(System.Convert.ToInt32(((dwColour1 & 0xFF0000) / 0x10000) * 256));
			with_1.Alpha = (short) 0;
			
			//Colour at upper-right corner
			TRIVERTEX with_2 = vert[1];
			///with_2.x = System.Convert.ToInt32(Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsX(myFrm.ClientRectangle.Width) / Microsoft.VisualBasic.Compatibility.VB6.Support.TwipsPerPixelX());
			with_2.Y = 0;
			with_2.Red = LongToSignedShort(((short) (dwColour2 & 0xFF)) * 256);
			with_2.Green = LongToSignedShort(System.Convert.ToInt32(((dwColour2 & 0xFF00) / 0x100) * 256));
			with_2.Blue = LongToSignedShort(System.Convert.ToInt32(((dwColour2 & 0xFF0000) / 0x10000) * 256));
			with_2.Alpha = (short) 0;
			
			//Colour at lower-right corner
			TRIVERTEX with_3 = vert[2];
			///with_3.x = System.Convert.ToInt32(Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsX(myFrm.ClientRectangle.Width) / Microsoft.VisualBasic.Compatibility.VB6.Support.TwipsPerPixelX());
			///with_3.Y = System.Convert.ToInt32(Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsY(myFrm.ClientRectangle.Height) / Microsoft.VisualBasic.Compatibility.VB6.Support.TwipsPerPixelY());
			with_3.Red = LongToSignedShort(((short) (dwColour3 & 0xFF)) * 256);
			with_3.Green = LongToSignedShort(System.Convert.ToInt32(((dwColour3 & 0xFF00) / 0x100) * 256));
			with_3.Blue = LongToSignedShort(System.Convert.ToInt32(((dwColour3 & 0xFF0000) / 0x10000) * 256));
			with_3.Alpha = (short) 0;
			
			//Colour at lower-left corner
			TRIVERTEX with_4 = vert[3];
			with_4.x = 0;
			///with_4.Y = System.Convert.ToInt32(Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsY(myFrm.ClientRectangle.Height) / Microsoft.VisualBasic.Compatibility.VB6.Support.TwipsPerPixelX());
			with_4.Red = LongToSignedShort(((short) (dwColour4 & 0xFF)) * 256);
			with_4.Green = LongToSignedShort(System.Convert.ToInt32(((dwColour4 & 0xFF00) / 0x100) * 256));
			with_4.Blue = LongToSignedShort(System.Convert.ToInt32(((dwColour4 & 0xFF0000) / 0x10000) * 256));
			with_4.Alpha = (short) 0;
			
			GRADIENT_TRIANGLE with_5 = grTri[0];
			with_5.Vertex1 = 0;
			with_5.Vertex2 = 1;
			with_5.Vertex3 = 2;
			
			GRADIENT_TRIANGLE with_6 = grTri[1];
			with_6.Vertex1 = 0;
			with_6.Vertex2 = 2;
			with_6.Vertex3 = 3;
			
			myFrm = new Form();
			myFrm.Show();
			
			//parameters:
			//hdc - display context handle of the target window
			//vert(0) - first member of interest in the vert() array
			//4 - number of vert() array members (not ubound(vert))
			//grTri(0) - first member of interest in the grTri() array
			//2 - number of grTri() array members (not ubound(grTri)
			//GRADIENT_FILL_TRIANGLE - fill operation
			///GradientFill(myFrm.CreateGraphics().GetHdc().ToInt32(), ref vert[0], 4, ref grTri[0], 2, GRADIENT_FILL_TRIANGLE);
			
		}
		
		
		public static void DrawGradientFill(int dwColour1, int dwColour2, Form myFrm)
		{
			
			TRIVERTEX[] vert = new TRIVERTEX[2];
			GRADIENT_RECT grRc = new GRADIENT_RECT();
			
			//Colour at upper-left corner
			TRIVERTEX with_1 = vert[0];
			with_1.x = 0;
			with_1.Y = 0;
			with_1.Red = LongToSignedShort(((short) (dwColour1 & 0xFF)) * 256);
			with_1.Green = LongToSignedShort(System.Convert.ToInt32(((dwColour1 & 0xFF00) / 0x100) * 256));
			with_1.Blue = LongToSignedShort(System.Convert.ToInt32(((dwColour1 & 0xFF0000) / 0x10000) * 256));
			with_1.Alpha = (short) 0;
			
			
			//Colour at bottom-right corner
			TRIVERTEX with_2 = vert[1];
			///with_2.x = System.Convert.ToInt32(Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsX(myFrm.ClientRectangle.Width) / Microsoft.VisualBasic.Compatibility.VB6.Support.TwipsPerPixelX());
			///with_2.Y = System.Convert.ToInt32(Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsY(myFrm.ClientRectangle.Height) / Microsoft.VisualBasic.Compatibility.VB6.Support.TwipsPerPixelY());
			with_2.Red = LongToSignedShort(((short) (dwColour2 & 0xFF)) * 256);
			with_2.Green = LongToSignedShort(System.Convert.ToInt32(((dwColour2 & 0xFF00) / 0x100) * 256));
			with_2.Blue = LongToSignedShort(System.Convert.ToInt32(((dwColour2 & 0xFF0000) / 0x10000) * 256));
			with_2.Alpha = (short) 0;
			
			grRc.LowerRight = 0;
			grRc.UpperLeft = 1;
			
			myFrm = new Form();
			myFrm.Show();
			
			//parameters:
			//hdc - display context handle of the target window
			//vert(0) - first member of interest in the vert() array
			//2 - number of vert() array members (not ubound(vert))
			//grRc - GRADIENT_RECT info
			//1 - number of grRc structures passed
			//GRADIENT_FILL_RECT_DIRECTION - fill operation -
			//will toggle between 0 and 1, the values of
			//GRADIENT_FILL_RECT_H and GRADIENT_FILL_RECT_V.
	    	///	GradientFill(myFrm.CreateGraphics().GetHdc().ToInt32(), ref vert[0], 2, ref grRc, 1, System.Math.Abs(GRADIENT_FILL_RECT_DIRECTION));
			
		}
		
		private static short LongToSignedShort(int dwUnsigned)
		{
			short returnValue = 0;
			
			//convert from long to signed short
			if (dwUnsigned < 32768)
			{
				returnValue = (short) dwUnsigned;
			}
			else
			{
				returnValue = (short) (dwUnsigned - 0x10000);
			}
			
			return returnValue;
		}
		
		public static void PictureDrawGradientFillTriangle(ref int dwColour1, ref int dwColour2, ref int dwColour3, ref int dwColour4, PictureBox myFrm)
		{
			
			GRADIENT_TRIANGLE[] grTri = new GRADIENT_TRIANGLE[2];
			TRIVERTEX[] vert = new TRIVERTEX[4];
			
			//Colour at upper-left corner
			TRIVERTEX with_1 = vert[0];
			with_1.x = 0;
			with_1.Y = 0;
			with_1.Red = LongToSignedShort(((short) (dwColour1 & 0xFF)) * 256);
			with_1.Green = LongToSignedShort(System.Convert.ToInt32(((dwColour1 & 0xFF00) / 0x100) * 256));
			with_1.Blue = LongToSignedShort(System.Convert.ToInt32(((dwColour1 & 0xFF0000) / 0x10000) * 256));
			with_1.Alpha = (short) 0;
			
			//Colour at upper-right corner
			TRIVERTEX with_2 = vert[1];
			///with_2.x = System.Convert.ToInt32(Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsX(myFrm.ClientRectangle.Width) / Microsoft.VisualBasic.Compatibility.VB6.Support.TwipsPerPixelX());
			with_2.Y = 0;
			with_2.Red = LongToSignedShort(((short) (dwColour2 & 0xFF)) * 256);
			with_2.Green = LongToSignedShort(System.Convert.ToInt32(((dwColour2 & 0xFF00) / 0x100) * 256));
			with_2.Blue = LongToSignedShort(System.Convert.ToInt32(((dwColour2 & 0xFF0000) / 0x10000) * 256));
			with_2.Alpha = (short) 0;
			
			//Colour at lower-right corner
			TRIVERTEX with_3 = vert[2];
			///with_3.x = System.Convert.ToInt32(Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsX(myFrm.ClientRectangle.Width) / Microsoft.VisualBasic.Compatibility.VB6.Support.TwipsPerPixelX());
			///with_3.Y = System.Convert.ToInt32(Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsY(myFrm.ClientRectangle.Height) / Microsoft.VisualBasic.Compatibility.VB6.Support.TwipsPerPixelY());
			with_3.Red = LongToSignedShort(((short) (dwColour3 & 0xFF)) * 256);
			with_3.Green = LongToSignedShort(System.Convert.ToInt32(((dwColour3 & 0xFF00) / 0x100) * 256));
			with_3.Blue = LongToSignedShort(System.Convert.ToInt32(((dwColour3 & 0xFF0000) / 0x10000) * 256));
			with_3.Alpha = (short) 0;
			
			//Colour at lower-left corner
			TRIVERTEX with_4 = vert[3];
			with_4.x = 0;
			///with_4.Y = System.Convert.ToInt32(Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsY(myFrm.ClientRectangle.Height) / Microsoft.VisualBasic.Compatibility.VB6.Support.TwipsPerPixelX());
			with_4.Red = LongToSignedShort(((short) (dwColour4 & 0xFF)) * 256);
			with_4.Green = LongToSignedShort(System.Convert.ToInt32(((dwColour4 & 0xFF00) / 0x100) * 256));
			with_4.Blue = LongToSignedShort(System.Convert.ToInt32(((dwColour4 & 0xFF0000) / 0x10000) * 256));
			with_4.Alpha = (short) 0;
			
			GRADIENT_TRIANGLE with_5 = grTri[0];
			with_5.Vertex1 = 0;
			with_5.Vertex2 = 1;
			with_5.Vertex3 = 2;
			
			GRADIENT_TRIANGLE with_6 = grTri[1];
			with_6.Vertex1 = 0;
			with_6.Vertex2 = 2;
			with_6.Vertex3 = 3;
			
			myFrm.Image = null;			
		}
	}
}
