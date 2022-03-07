using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Windows.Input;
using PhysX.Samples;

namespace PhysX.CustomizedSamples.ShootArrowSample
{
    public class ShootArrowSample : Sample
    {
        private CapsuleController? _controller;
        //private float _rotation;
        private const float _controllerSpeed = 0.1f;

        private RigidBody? _ballActor;

        private Vector3 _originPostion = new Vector3(0, 0, 0);

        public ShootArrowSample():base(null, new CustomizedEngine.CustomizedEngine())
        {
            if (Engine != null)
            {
                CustomizedEngine.CustomizedEngine? engine = Engine as CustomizedEngine.CustomizedEngine;
                //if (engine != null) engine.ResetPhysicsAction = ResetPhysics;
            }

            Run();
        }

        protected override void LoadContent()
        {

        }

        protected override void LoadPhysics(Scene scene)
        {
            var material = scene.Physics.CreateMaterial(0.1f, 0.1f, 0.1f);

            var controllerManager = scene.CreateControllerManager();

            //User controllable character
            {
                var desc = new CapsuleControllerDesc()
                {
                    Height = 4,
                    Radius = 1,
                    Material = material,
                    UpDirection = new Vector3(0, 1, 0),
                    Position = _originPostion,
                    //ReportCallback = new ControllerHitReport()
                };

                _controller = controllerManager.CreateController<CapsuleController>(desc);
                RigidDynamic? controllerActor = _controller?.Actor;
                controllerActor?.SetMassAndUpdateInertia(100);
                if (controllerActor != null) controllerActor.RigidBodyFlags = RigidBodyFlag.EnableCCD;
            }

            /*
            // 添加胶囊体，但是站不起来
            {
                _ballActor = this.Scene.Physics.CreateRigidDynamic();

                CapsuleGeometry geom = new CapsuleGeometry(radius: 2, halfHeight: 2);
                Shape boxShape = RigidActorExt.CreateExclusiveShape(_ballActor, geom, material, null);

                _ballActor.GlobalPose = Matrix4x4.CreateTranslation(new Vector3(0, 0, 10));
                _ballActor.SetMassAndUpdateInertia(100);

                this.Scene.AddActor(_ballActor);
            }
            */
        }

        private void ResetPhysics()
        {
            if (_controller != null)
            {
                RigidDynamic? controllerActor = _controller?.Actor;
                controllerActor.AngularVelocity = Vector3.Zero;
                controllerActor.LinearVelocity = Vector3.Zero;
                if (controllerActor != null) controllerActor.RigidBodyFlags = RigidBodyFlag.Kinematic;

                _controller.UpDirection = new Vector3(0, 1, 0);
                _controller.Position = _originPostion;

                //if (controllerActor != null) controllerActor.RigidBodyFlags = RigidBodyFlag.EnableCCD;
            }
        }

        protected override void Update(TimeSpan elapsed)
        {
            if (_controller != null)
            {
                //_controller.UpDirection = new Vector3(0, 1, 0);

                //_controller.Actor?.AddForceAtPosition(new Vector3(0, 981, 0), _controller.Position, ForceMode.Force, true);
            }
        }


        protected override void ProcessKeyboard(Key[] pressedKeys)
        {
            RigidDynamic? controllerActor = _controller?.Actor;
            if (controllerActor != null) controllerActor.RigidBodyFlags = RigidBodyFlag.EnableCCD;

            if (pressedKeys.Length <= 0) return;

            Vector3 forceVector;
            float powerX = 1000;
            //float powerY = 0;
            float powerY = 300;
            float powerZ = 1000;

            Vector3 forceLocalPostion = new Vector3(0, 0, 0);
            if (pressedKeys.Contains(Key.A))
            {
                forceVector = new Vector3(-powerX, powerY, 0);
                //forceLocalPostion = new Vector3(1, 0, 0);
                forceLocalPostion = new Vector3(0, 0, 0);
            }
            else if (pressedKeys.Contains(Key.D))
            {
                forceVector = new Vector3(powerX, powerY, 0);
                //forceLocalPostion = new Vector3(1, 0, 0);
                forceLocalPostion = new Vector3(0, 0, 0);
            }
            else if (pressedKeys.Contains(Key.W))
            {
                forceVector = new Vector3(0, powerY, 0);
            }
            else if (pressedKeys.Contains(Key.S))
            {
                forceVector = new Vector3(0, -powerY, 0);
            }
            else
            {
                forceVector = new Vector3(0, 0, 0);
            }

            _ballActor?.AddForceAtLocalPosition(forceVector, forceLocalPostion, ForceMode.Impulse, true);
            _controller?.Actor.AddForceAtLocalPosition(forceVector, forceLocalPostion, ForceMode.Impulse, true);

            if (forceVector.LengthSquared() != 0f )
            {

            }

            if (pressedKeys.Contains(Key.Space))
            {
                Vector3 jumpForceLocalPostion = new Vector3(0, 0, 0);
                _controller?.Actor.AddForceAtLocalPosition(new Vector3(0, 30, 0), jumpForceLocalPostion, ForceMode.Impulse, true);
            }


            if (pressedKeys.Contains(Key.R))
            {
                ResetPhysics();
            }
        }


        protected override void Draw()
        {

        }
    }
}
