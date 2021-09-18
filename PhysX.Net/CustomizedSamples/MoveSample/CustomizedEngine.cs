using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhysX.Samples.Engine;

namespace PhysX.CustomizedSamples.MoveSample
{
    class CustomizedEngine : Engine
    {
		protected override void Update(TimeSpan elapsed, bool isMoveCamera)
		{
			isMoveCamera = false;
			base.Update(elapsed, isMoveCamera);
		}
	}
}
