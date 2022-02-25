﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Windows.Input;
using PhysX.Samples;

namespace PhysX.CustomizedSamples.ExplosionSample
{
    public class ExplosionSample : Sample
    {
        private RigidBody? _capsuleBody;

        public static RigidBody? _ballBody;

        public static RigidBody? _ballBody2;

        public static RigidBody? _largeBallBody;

        private Shape? _capsuleShape;

        private static float _halfHeight = 5;
        private Vector3 _startPostion = new Vector3(-15, 1, 0);


        public ExplosionSample() : base(null, new CustomizedEngine.CustomizedEngine(null, new ExplosionSampleFilterShader()))
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
            if (_ballBody != null)
            {
                _ballBody.GlobalPosePosition = new Vector3(2, 0, 0); ;
                _ballBody.LinearVelocity = Vector3.Zero;
                _ballBody.AngularVelocity = Vector3.Zero;
                
                _ballBody2.GlobalPosePosition = new Vector3(-2, 0, 0); ;
                _ballBody2.LinearVelocity = Vector3.Zero;
                _ballBody2.AngularVelocity = Vector3.Zero;

                if (_largeBallBody != null)
                {
                    Scene.RemoveActor(_largeBallBody);
                    _largeBallBody.Dispose();
                    Debug.WriteLine($" _largeBallBody.Disposed = {_largeBallBody.Disposed}");
                    _largeBallBody = null;
                }
            }
        }

        protected override void LoadPhysics(Scene scene)
        {
            Material material = scene.Physics.CreateMaterial(1f, 1f, 1f);

            _ballBody = this.Scene.Physics.CreateRigidDynamic();
            SphereGeometry geom2 = new SphereGeometry(radius: 1f);
            RigidActorExt.CreateExclusiveShape(_ballBody, geom2, material);
            _ballBody.SetMassAndUpdateInertia(10);
            _ballBody.GlobalPosePosition = new Vector3(2, 0, 0);
            this.Scene.AddActor(_ballBody);


            _ballBody2 = this.Scene.Physics.CreateRigidDynamic();
            SphereGeometry geom4 = new SphereGeometry(radius: 1f);
            RigidActorExt.CreateExclusiveShape(_ballBody2, geom4, material);
            _ballBody2.SetMassAndUpdateInertia(10);
            _ballBody2.GlobalPosePosition = new Vector3(-2, 0, 0);
            this.Scene.AddActor(_ballBody2);


            EventCallback callback = new EventCallback(this);
            scene.SetSimulationEventCallback(callback);
            //ResetActorPosition();
        }

        private void ResetPhysics()
        {
            ResetActorPosition();
        }

        protected override void Update(TimeSpan elapsed)
        {
        }


        protected override void ProcessKeyboard(Key[] pressedKeys)
        {
            if (pressedKeys.Length <= 0) return;

            if (pressedKeys.Contains(Key.D))
            {
                Material material = Scene.Physics.CreateMaterial(1f, 1f, 1f);
                _largeBallBody = Scene.Physics.CreateRigidDynamic();
                SphereGeometry geom3 = new SphereGeometry(radius: 2f);
                RigidActorExt.CreateExclusiveShape(_largeBallBody, geom3, material, ShapeFlag.TriggerShape | ShapeFlag.Visualization);
                _largeBallBody.Flags = ActorFlag.DisableGravity | ActorFlag.Visualization;
                _largeBallBody.SetMassAndUpdateInertia(10);
                _largeBallBody.GlobalPosePosition = new Vector3(0, 0, 0);
                Scene.AddActor(_largeBallBody);

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
            Debug.WriteLine($"ExplosionSample::OnTrigger");
        }
    }


    public class EventCallback : SimulationEventCallback
    {
        private ExplosionSample _sample;

        public EventCallback(ExplosionSample sample)
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
                    ExplosionSample.OnTrigger(pairs);
                }
            }
        }
    }
}