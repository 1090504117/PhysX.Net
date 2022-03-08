using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using PhysX.Samples.Engine;

namespace PhysX.CustomizedSamples.CustomizedEngine
{
    public class CustomizedEngine : Engine
    {
		public event UpdateEventHandler PreUpdate;

		public static SimulationFilterShader? SimulationFilterShader { set; get; }

		public Action? ResetPhysicsAction;

		private Action<SceneDesc>? _sceneDescCallback;

		public CustomizedEngine(Action<SceneDesc>? sceneDescCallback = null, SimulationFilterShader? simulationFilterShader = null) : base(sceneDescCallback)
        {
			OnKeyDown = _onKeyDown;
			_sceneDescCallback = sceneDescCallback;
		}

		protected override void Update(TimeSpan elapsed, bool isNeedMoveCamera)
		{
			isNeedMoveCamera = false;
			this.FrameTime = elapsed;

			if (PreUpdate != null)
				PreUpdate(elapsed);

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

		protected override SceneDesc CreateSceneDesc(Foundation foundation)
		{
#if GPU
			var cudaContext = new CudaContextManager(foundation);
#endif

			var sceneDesc = new SceneDesc
			{
				Gravity = new Vector3(0, -9.81f, 0),
#if GPU
				GpuDispatcher = cudaContext.GpuDispatcher,
#endif
				FilterShader = SimulationFilterShader == null? new SampleFilterShader(): SimulationFilterShader
			};

#if GPU
			sceneDesc.Flags |= SceneFlag.EnableGpuDynamics;
			sceneDesc.BroadPhaseType |= BroadPhaseType.Gpu;
#endif

			_sceneDescCallback?.Invoke(sceneDesc);

			return sceneDesc;
		}
	}
}
