using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhysX.CustomizedSamples.TriggerSample
{
	class Program
	{
		[STAThread]
		static void Main(string[] args)
		{
			CustomizedEngine.CustomizedEngine.SimulationFilterShader = new TriggerSampleFilterShader();
			new TriggerSample();
		}
	}
}