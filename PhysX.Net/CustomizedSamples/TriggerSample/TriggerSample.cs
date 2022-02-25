using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Windows.Input;
using PhysX.Samples;

namespace PhysX.CustomizedSamples.TriggerSample
{
    public class TriggerSample : Sample
    {
        private RigidBody? _capsuleBody;

        public static RigidBody? _ballBody;

        public static RigidBody? _largeBallBody;

        private Shape? _capsuleShape;

        private static float _halfHeight = 5;
        private Vector3 _startPostion = new Vector3(-15, 1, 0);


        public TriggerSample() : base(null, new CustomizedEngine.CustomizedEngine(null, new TriggerSampleFilterShader()))
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

            Vector3 height = new Vector3(0, 10, 0);
            if (_largeBallBody != null)
            {
                _largeBallBody.GlobalPosePosition = height;
                _largeBallBody.LinearVelocity = Vector3.Zero;
                _largeBallBody.AngularVelocity = Vector3.Zero;
            }

            if (_ballBody != null)
            {
                _ballBody.GlobalPosePosition = new Vector3(0, 2, 0); ;
                _ballBody.LinearVelocity = Vector3.Zero;
                _ballBody.AngularVelocity = Vector3.Zero;
            }
        }

        protected override void LoadPhysics(Scene scene)
        {

            _capsuleBody = this.Scene.Physics.CreateRigidDynamic();
            Debug.WriteLine(_capsuleBody);
            CapsuleGeometry geom = new CapsuleGeometry(radius: 0.5f, halfHeight: _halfHeight);
            Material material = scene.Physics.CreateMaterial(1f, 1f, 1f);
            _capsuleShape = RigidActorExt.CreateExclusiveShape(_capsuleBody, geom, material, null);
            _capsuleShape.Flags = ShapeFlag.TriggerShape | ShapeFlag.Visualization;
            _capsuleBody.Flags = ActorFlag.DisableGravity | ActorFlag.Visualization;

            _capsuleBody.SetMassAndUpdateInertia(10, new Vector3(4,0,0));



            _largeBallBody = this.Scene.Physics.CreateRigidDynamic();
            SphereGeometry geom3 = new SphereGeometry(radius: 2f);
            RigidActorExt.CreateExclusiveShape(_largeBallBody, geom3, material, ShapeFlag.SimulationShape | ShapeFlag.Visualization);
            _largeBallBody.Flags = ActorFlag.DisableGravity | ActorFlag.Visualization;
            _largeBallBody.SetMassAndUpdateInertia(10);
            _largeBallBody.GlobalPosePosition = new Vector3(0, 0, 0);
            this.Scene.AddActor(_largeBallBody);


            _ballBody = this.Scene.Physics.CreateRigidDynamic();
            SphereGeometry geom2 = new SphereGeometry(radius: 1f);
            RigidActorExt.CreateExclusiveShape(_ballBody, geom2, material, ShapeFlag.TriggerShape | ShapeFlag.Visualization);
            _ballBody.SetMassAndUpdateInertia(10);
            _ballBody.GlobalPosePosition = new Vector3(0, 0, 0);

            _ballBody.Flags = ActorFlag.DisableGravity | ActorFlag.Visualization;
            this.Scene.AddActor(_ballBody);
            Debug.WriteLine(_ballBody);


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
                ////_ballBody.GlobalPosePosition = new Vector3(0, 0, 0);

                //float powerX = 20;
                //float powerY = 0;

                //Vector3 forceVector = new Vector3(powerX, powerY, 0);
                //Vector3 forceLocalPosition = new Vector3(0, 0, 0);
                //Vector3 targetPosition = _startPostion + forceVector;
                //_capsuleBody.GlobalPosePosition = _startPostion;
                //_capsuleBody.GlobalPoseQuat = GetQuatByDirection(forceVector);
                //_capsuleBody?.AddForceAtLocalPosition(forceVector, forceLocalPosition, ForceMode.Impulse, true);

                _ballBody.GlobalPosePosition = _ballBody.GlobalPosePosition + new Vector3(0,1,0);

                /* 
                //这段废弃，对于非controller类的物体无法动态变长
                if (_capsuleShape != null)
                {
                    float halfHeight = _capsuleShape.GetCapsuleGeometry().HalfHeight;
                    _capsuleShape.GetCapsuleGeometry().HalfHeight = halfHeight + 1;
                    _capsuleShape.SetGeometry(_capsuleShape.GetCapsuleGeometry());
                }
                 */
            }
            else if (pressedKeys.Contains(Key.R))
            {
                ResetPhysics();
            }
        }
         

        protected override void Draw()
        {

        }

        public static void OnTrigger(TriggerPair[] pairs)
        {

        }
    }


    public class EventCallback : SimulationEventCallback
    {
        private TriggerSample _sample;

        public EventCallback(TriggerSample sample)
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
                    Debug.WriteLine($"pair.ContactPoint = {pair.ContactCount.ToString()}");
                }
            }
        }

        public override void OnTrigger(TriggerPair[] pairs)
        {
            Debug.WriteLine($"OnTrigger = OnTrigger");

            base.OnTrigger(pairs);

            foreach (var pair in pairs)
            {
                if (pair.TriggerActor != null && pair.OtherActor != null)
                {
                    Debug.WriteLine($"pair.Status = {pair.Status.ToString()}");
                    Debug.WriteLine($"pair.TriggerActor == TriggerSample._ballBody = {pair.TriggerActor == TriggerSample._ballBody }");
                    Debug.WriteLine($"pair.OtherActor == TriggerSample._largeBallBody = {pair.OtherActor == TriggerSample._largeBallBody}");

                    Debug.WriteLine($"pair.TriggerActor == TriggerSample._largeBallBody = {pair.TriggerActor == TriggerSample._largeBallBody }");
                    Debug.WriteLine($"pair.OtherActor == TriggerSample._ballBody = {pair.OtherActor == TriggerSample._ballBody}");
                }
            }
        }
    }
}
