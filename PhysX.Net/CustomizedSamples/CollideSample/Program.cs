using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhysX.CustomizedSamples.CollideSample
{
	class Program
	{
		[STAThread]
		static void Main(string[] args)
		{
			CustomizedEngine.CustomizedEngine.SimulationFilterShader = new CollideSampleFilterShader();
			new CollideSample();
		}
	}
}