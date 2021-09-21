using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Windows.Input;
using PhysX.Samples;
using PhysX.Samples.Engine;


namespace PhysX.CustomizedSamples.MoveSample
{
    public class MoveSample : Sample
    {
        private CapsuleController? _controller;
        //private float _rotation;
        private const float _controllerSpeed = 0.1f;

        public MoveSample():base(null, new CustomizedEngine.CustomizedEngine())
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
                    Position = new Vector3(0, 10, 0),
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
            //var rotation = Matrix4x4.CreateFromAxisAngle(Vector3.UnitY, _rotation);

            //Vector3 forward = Vector3.TransformNormal(Vector3.UnitZ, rotation);
            //Vector3 right = Vector3.TransformNormal(Vector3.UnitX, rotation);

            Vector3 forward = new Vector3(0, 0, 1);
            Vector3 right = new Vector3(1, 0, 0);
            Vector3 up = new Vector3(0, 1, 0);

            Vector3 moveDelta = Vector3.Zero;

            if (pressedKeys.Contains(Key.W))
                //moveDelta += forward;
                moveDelta += up;
            if (pressedKeys.Contains(Key.S))
                //moveDelta += -forward;
                moveDelta += -up;
            if (pressedKeys.Contains(Key.A))
                moveDelta += -right;
            if (pressedKeys.Contains(Key.D))
                moveDelta += right;

            // Normalize the distance vector (as we may of added two or more components together)
            if (moveDelta.LengthSquared() == 0)
                return;

            Vector3 d = Vector3.Normalize(moveDelta);
            _controller.Move(d * _controllerSpeed, this.Engine.FrameTime);
        }


        protected override void Draw()
        {

        }
    }
}
