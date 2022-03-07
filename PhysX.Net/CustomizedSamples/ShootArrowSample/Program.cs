using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhysX.CustomizedSamples.ShootArrowSample
{
	class Program
	{
		[STAThread]
		static void Main(string[] args)
		{
			CustomizedEngine.CustomizedEngine.SimulationFilterShader = new ShootArrowSampleFilterShader();
			new ShootArrowSample();
		}
	}
}