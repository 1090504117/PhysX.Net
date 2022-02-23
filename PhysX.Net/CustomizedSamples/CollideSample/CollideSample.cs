using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Windows.Input;
using PhysX.Samples;

namespace PhysX.CustomizedSamples.CollideSample
{
    public class CollideSample : Sample
    {
        private RigidBody? _capsuleBody;
        public RigidBody CapsuleBody
        {
            get
            {
                return _capsuleBody;
            }
        }

        private static float _halfHeight = 5;
        private Vector3 _startPostion = new Vector3(-10, _halfHeight, 0);

        private RigidBody? _ballBody;

        public CollideSample() : base(null, new CustomizedEngine.CustomizedEngine(null, new CollideSampleFilterShader()))
        {


            if (Engine != null)
            {
                CustomizedEngine.CustomizedEngine? engine = Engine as CustomizedEngine.CustomizedEngine;
            }

            Run();
        }

        protected override void LoadContent()
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

        private void ResetActorPosition()
        {
            if (_capsuleBody != null)
            { 
                _capsuleBody.GlobalPosePosition = _startPostion;
                _capsuleBody.LinearVelocity = Vector3.Zero;
                _capsuleBody.AngularVelocity = Vector3.Zero;
            }
        }

        protected override void LoadPhysics(Scene scene)
        {

            _capsuleBody = this.Scene.Physics.CreateRigidDynamic();
            CapsuleGeometry geom = new CapsuleGeometry(radius: 0.5f, halfHeight: _halfHeight);
            Material material = scene.Physics.CreateMaterial(1f, 1f, 1f);
            Shape capsuleShape = RigidActorExt.CreateExclusiveShape(_capsuleBody, geom, material, null);

            _capsuleBody.SetMassAndUpdateInertia(10, new Vector3(4,0,0));
            this.Scene.AddActor(_capsuleBody);

            _ballBody = this.Scene.Physics.CreateRigidDynamic();
            SphereGeometry geom2 = new SphereGeometry(radius: 1f);
            RigidActorExt.CreateExclusiveShape(_ballBody, geom2, material, null);
            _ballBody.SetMassAndUpdateInertia(10);
            _ballBody.GlobalPosePosition = new Vector3(0, 0, 0);
            this.Scene.AddActor(_ballBody);


            EventCallback callback = new EventCallback(this);
            scene.SetSimulationEventCallback(callback);
            //ResetActorPosition();
        }

        private void ResetPhysics()
        {
            if (_capsuleBody != null)
            {

                ResetActorPosition();

                //if (controllerActor != null) controllerActor.RigidBodyFlags = RigidBodyFlag.EnableCCD;
            }
        }

        protected override void Update(TimeSpan elapsed)
        {
        }


        protected override void ProcessKeyboard(Key[] pressedKeys)
        {
            if (pressedKeys.Length <= 0) return;

            if (pressedKeys.Contains(Key.D))
            {
                _ballBody.GlobalPosePosition = new Vector3(0, 0, 0);

                float powerX = 20;
                float powerY = 0;

                Vector3 forceVector = new Vector3(powerX, powerY, 0);
                Vector3 forceLocalPosition = new Vector3(0, 0, 0);
                Vector3 targetPosition = _startPostion + forceVector;
                _capsuleBody.GlobalPosePosition = _startPostion;
                _capsuleBody.GlobalPoseQuat = GetQuatByDirection(forceVector);
                _capsuleBody?.AddForceAtLocalPosition(forceVector, forceLocalPosition, ForceMode.Impulse, true);
                _capsuleBody?.AddForceAtLocalPosition(new Vector3(0, -1, 0), forceLocalPosition, ForceMode.Impulse, true);
            }
            else if (pressedKeys.Contains(Key.R))
            {
                ResetPhysics();
            }
        }
         

        protected override void Draw()
        {

        }
    }


    public class EventCallback : SimulationEventCallback
    {
        private CollideSample _sample;

        public EventCallback(CollideSample sample)
        {
            _sample = sample;
        }

        public override void OnContact(ContactPairHeader pairHeader, ContactPair[] pairs)
        {
            base.OnContact(pairHeader, pairs);

            foreach (var pair in pairs)
            {
                if (pair.Shape0 != null && pair.Shape1 != null)
                {
                    if (pair.Shape0.Actor == _sample.CapsuleBody)
                    {
                        Debug.WriteLine("pair.Shape0 is capsuleActor");
                        Debug.WriteLine($"pair.ContactPoint = {pair.ContactCount.ToString()}");
                    }
                    else if(pair.Shape1.Actor == _sample.CapsuleBody)
                    {
                        Debug.WriteLine("pair.Shape1 is capsuleActor");
                        Debug.WriteLine($"pair.ContactPoint = {pair.ContactCount.ToString()}");
                    }
                }
            }
        }
    }
}
