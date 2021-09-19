using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Windows.Input;
using PhysX.Samples;
using PhysX.Samples.Engine;


namespace PhysX.CustomizedSamples.AddForceSample
{
    public class AddForceSample : Sample
    {
        private CapsuleController? _controller;
        //private float _rotation;
        private const float _controllerSpeed = 0.1f;

        public AddForceSample():base(null, new CustomizedEngine.CustomizedEngine())
        {
            Run();
        }

        protected override void LoadContent()
        {

        }

        protected override void LoadPhysics(Scene scene)
        {
            var material = scene.Physics.CreateMaterial(0.1f, 0.1f, 0.1f);

            var controllerManager = scene.CreateControllerManager();

            // User controllable character
            {
                var desc = new CapsuleControllerDesc()
                {
                    Height = 4,
                    Radius = 1,
                    Material = material,
                    UpDirection = new Vector3(0, 1, 0),
                    Position = new Vector3(0, 3, 0),
                    //ReportCallback = new ControllerHitReport()
                };

                _controller = controllerManager.CreateController<CapsuleController>(desc);
            }
        }

        protected override void Update(TimeSpan elapsed)
        {

        }


        protected override void ProcessKeyboard(Key[] pressedKeys)
        {
            if (pressedKeys.Contains(Key.S))
                _controller.Actor.AddForceAtLocalPosition(new System.Numerics.Vector3(100, 0, 0), new System.Numerics.Vector3(0, 0, 0), ForceMode.Impulse, true);
        }


        protected override void Draw()
        {

        }
    }
}
