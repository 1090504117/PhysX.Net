using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using PhysX.Samples.Engine;

namespace PhysX.CustomizedSamples.CustomizedEngine
{
    public class CustomizedEngine : Engine
    {
		public Action? ResetPhysicsAction;

		public CustomizedEngine()
        {
			OnKeyDown = _onKeyDown;
		}

		protected override void Update(TimeSpan elapsed, bool isNeedMoveCamera)
		{
			isNeedMoveCamera = false;
			base.Update(elapsed, isNeedMoveCamera);
		}

		private void _onKeyDown(object sender, KeyEventArgs e)
        {
			if (e.Key == Key.R)
			{
				ResetPhysics();
			}
		}

		public virtual void ResetPhysics()
        {
			ResetPhysicsAction?.Invoke();

		}
	}
}
