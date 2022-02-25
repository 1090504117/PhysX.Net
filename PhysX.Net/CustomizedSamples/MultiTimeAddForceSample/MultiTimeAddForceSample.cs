using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Windows.Input;
using PhysX.Samples;

namespace PhysX.CustomizedSamples.AddForceSample
{
    public class MultiTimeAddForceSample : Sample
    {
        private CapsuleController? _controller;
        //private float _rotation;
        private const float _controllerSpeed = 0.1f;

        private RigidBody? _ballActor;

        private Vector3 _originPostion = new Vector3(0, 0, 0);

        private Double _addForceMillSeconds = 0;
        private bool _isNeedTimeCountDown = false;

        private double _oneImpulseForceTime = 1000;

        private int _impulseForceIndex = 0;

        private static float _power = 3000;

        private Vector3[] _forceVectorArray = new Vector3[] 
        { 
            new Vector3(_power, _power, 0), 
            new Vector3(-2*_power, 0, 0),
            new Vector3(2*_power, _power, 0),
            new Vector3(0, -2*_power, 0)
        };

        public MultiTimeAddForceSample():base(null, new CustomizedEngine.CustomizedEngine())
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
                };

                _controller = controllerManager.CreateController<CapsuleController>(desc);
                RigidDynamic? controllerActor = _controller?.Actor;
                controllerActor?.SetMassAndUpdateInertia(100);
                if (controllerActor != null) controllerActor.RigidBodyFlags = RigidBodyFlag.EnableCCD;
            }
        }

        private void ResetPhysics()
        {
            if (_controller != null)
            {
                RigidDynamic? controllerActor = _controller.Actor;

                if (controllerActor != null)
                {
                    controllerActor.AngularVelocity = Vector3.Zero;
                    controllerActor.LinearVelocity = Vector3.Zero;
                    controllerActor.RigidBodyFlags = RigidBodyFlag.Kinematic;
                }

                _controller.UpDirection = new Vector3(0, 1, 0);
                _controller.Position = _originPostion;
            }

            _addForceMillSeconds = 0;
            _isNeedTimeCountDown = false;
        }

        private void AddForce()
        {
            if (_controller == null) return;

            _addForceMillSeconds = 0;

            Vector3 forceVector = _forceVectorArray[_impulseForceIndex];

            System.Diagnostics.Debug.WriteLine($"_impulseForceIndex = {_impulseForceIndex}");


            _controller.Actor.LinearVelocity = Vector3.Zero;
            _controller.Actor.AngularVelocity = Vector3.Zero;

            if (_impulseForceIndex == _forceVectorArray.Length - 1)
            {
                _controller.Actor.GlobalPoseQuat = GetQuatByDirection(new Vector3(1,0,0));
            }

            _controller.Actor.AddForceAtLocalPosition(forceVector, new Vector3(0,0,0), ForceMode.Impulse, true);



            if (_impulseForceIndex < _forceVectorArray.Length - 1)
            {
                _impulseForceIndex = _impulseForceIndex + 1;
            }
            else
            {
                _isNeedTimeCountDown = false;
            }
        }

        protected override void Update(TimeSpan elapsed)
        {
            if (_controller != null)
            {
                if (_isNeedTimeCountDown)
                {
                    _addForceMillSeconds = _addForceMillSeconds + elapsed.TotalMilliseconds;
                    if (_addForceMillSeconds >= _oneImpulseForceTime)
                    {
                        AddForce();
                    }
                }
            }
        }


        protected override void ProcessKeyboard(Key[] pressedKeys)
        {
            RigidDynamic? controllerActor = _controller?.Actor;
            if (controllerActor != null) controllerActor.RigidBodyFlags = RigidBodyFlag.EnableCCD;

            if (pressedKeys.Length <= 0) return;

            Vector3 forceVector;

            if (pressedKeys.Contains(Key.A))
            {
                if (!_isNeedTimeCountDown)
                { 
                    _isNeedTimeCountDown = true;
                    _impulseForceIndex = 0;
                    _power = 6000;
                    _oneImpulseForceTime = 500;
                    _forceVectorArray = new Vector3[]
                    {
                        new Vector3(_power, _power, 0),
                        new Vector3(-_power, 100, 0),
                        new Vector3(_power, _power, 0),
                        new Vector3(0, -_power, 0)
                    };
                    AddForce();
                }
            }

            if (pressedKeys.Contains(Key.R))
            {
                ResetPhysics();
            }
        }


        protected override void Draw()
        {

        }

        private Quaternion ShortestRotation(Vector3 v0, Vector3 v1)
        {
            float d = Vector3.Dot(v0, v1);
            Vector3 cross = Vector3.Cross(v0, v1);
            Quaternion q = d > -1 ? new Quaternion(cross.X, cross.Y, cross.Z, 1 + d) : MathF.Abs(v0.X) < 1.0f ? new Quaternion(0f, v0.Z, -v0.Y, 0f) : new Quaternion(v0.Y, -v0.X, 0f, 0f);
            return Quaternion.Normalize(q);
        }

        private Vector3 _normalizedXDirection = new Vector3(1, 0, 0);
        private Quaternion GetQuatByDirection(Vector3 direction)
        {
            Vector3 d = Vector3.Normalize(direction);
            return ShortestRotation(_normalizedXDirection, d);
        }
    }
}
