using System;

namespace StillDesign.PhysX.Samples
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		static void Main(string[] args)
		{
			try
			{
				new FluidBlob();
			}
			catch(Exception ex)
			{
				System.Windows.Forms.MessageBox.Show(ex.Message, "PhysX.Net Initalization Error");
			}
		}
	}
}