using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Windows.Input;
using PhysX.Samples;

namespace PhysX.CustomizedSamples.RemoveActorSample
{
    public class RemoveActorSample : Sample
    {
        public static RigidBody? _largeBallBody;

        public RemoveActorSample() : base(null, new CustomizedEngine.CustomizedEngine(null, new RemoveActorSampleFilterShader()))
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

        private void RemoveOneActor()
        {
            if (_largeBallBody != null)
            {
                Scene.RemoveActor(_largeBallBody);
                _largeBallBody.Dispose();
                _largeBallBody = null;
            }
        }

        protected override void LoadPhysics(Scene scene)
        {
            EventCallback callback = new EventCallback(this);
            scene.SetSimulationEventCallback(callback);
            //RemoveOneActor();
        }

        protected override void Update(TimeSpan elapsed)
        {
        }


        protected override void ProcessKeyboard(Key[] pressedKeys)
        {
            if (pressedKeys.Length <= 0) return;

            if (pressedKeys.Contains(Key.D))
            {
                if (_largeBallBody == null)
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
            }
            else if (pressedKeys.Contains(Key.A))
            {
                if (_largeBallBody != null && !_largeBallBody.Disposed && _largeBallBody.Scene == null)
                {
                    Scene.AddActor(_largeBallBody);
                }
            }
            else if (pressedKeys.Contains(Key.R))
            {
                RemoveOneActor();
            }
        }
         

        protected override void Draw()
        {

        }

        public static void OnTrigger(TriggerPair[] pairs)
        {
            Debug.WriteLine($"RemoveActorSample::OnTrigger");
        }
    }


    public class EventCallback : SimulationEventCallback
    {
        private RemoveActorSample _sample;

        public EventCallback(RemoveActorSample sample)
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
                    RemoveActorSample.OnTrigger(pairs);
                }
            }
        }
    }
}
