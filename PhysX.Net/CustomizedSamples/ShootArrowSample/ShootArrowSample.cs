using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Windows.Input;
using PhysX.Samples;

namespace PhysX.CustomizedSamples.ShootArrowSample
{
    public class ShootArrowSample : Sample
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

        private bool _isShooting;
        private bool _isNeedRemoveArrow;


        public ShootArrowSample() : base(null, new CustomizedEngine.CustomizedEngine(null, new ShootArrowSampleFilterShader()))
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
            _capsuleBody.Name = "Arrow";
            SphereGeometry geom = new SphereGeometry(radius: 0.5f);
            Material material = scene.Physics.CreateMaterial(1f, 1f, 1f);
            Shape capsuleShape = RigidActorExt.CreateExclusiveShape(_capsuleBody, geom, material, null);
            _capsuleBody.SetMassAndUpdateInertia(10);
            //_capsuleBody.SetMassAndUpdateInertia(10, new Vector3(4,0,0));   //对于球体的质心还是有用的（在碰撞过程中）
            ResetActorPosition();
            this.Scene.AddActor(_capsuleBody);


            for(int i = 0; i < 4; i++)
            {
                var ballBody = this.Scene.Physics.CreateRigidDynamic();
                ballBody.Name = "Ball";
                SphereGeometry ballGeom = new SphereGeometry(radius: 1f);
                Material ballMaterial = scene.Physics.CreateMaterial(1f, 1f, 1f);
                Shape ballShape = RigidActorExt.CreateExclusiveShape(ballBody, ballGeom, ballMaterial, null);
                ballBody.SetMassAndUpdateInertia(i* 10 + 3);
                ballBody.GlobalPosePosition = new Vector3((i-2) * 2, 1, 0);
                this.Scene.AddActor(ballBody);
            }

            var boxBody = this.Scene.Physics.CreateRigidStatic();
            boxBody.Name = "Box";
            BoxGeometry boxGeom = new BoxGeometry(1,2,3);
            Material boxMaterial = scene.Physics.CreateMaterial(1f, 1f, 1f);
            Shape boxShape = RigidActorExt.CreateExclusiveShape(boxBody, boxGeom, boxMaterial, null);
            boxBody.GlobalPosePosition = new Vector3(10, 1, 0);
            this.Scene.AddActor(boxBody);


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
            if (_capsuleBody != null && _capsuleBody.LinearVelocity.LengthSquared() < 0.0001)
            {
                if (_isShooting)
                {
                    _capsuleBody.GlobalPoseQuat = GetQuatByDirection(_capsuleBody.LinearVelocity);
                }
            }

            if (_isNeedRemoveArrow)
            {
                _isNeedRemoveArrow = false;
                RemoveArrowActor();
            }
        }


        protected override void ProcessKeyboard(Key[] pressedKeys)
        {
            if (pressedKeys.Length <= 0) return;

            if (pressedKeys.Contains(Key.D))
            {
                float powerX = 20;
                float powerY = 0;

                Vector3 forceVector = new Vector3(powerX, powerY, 0);
                Vector3 forceLocalPosition = new Vector3(0, 0, 0);
                Vector3 targetPosition = _startPostion + forceVector;
                _capsuleBody.LinearVelocity = Vector3.Zero;
                _capsuleBody.AngularVelocity = Vector3.Zero;
                _capsuleBody.GlobalPosePosition = _startPostion;
                _capsuleBody.GlobalPoseQuat = GetQuatByDirection(forceVector);
                _capsuleBody?.AddForceAtLocalPosition(forceVector, forceLocalPosition, ForceMode.Impulse, true);
                //_capsuleBody?.AddForceAtLocalPosition(new Vector3(0, -1, 0), forceLocalPosition, ForceMode.Impulse, true);

                _isShooting = true;
            }
            else if (pressedKeys.Contains(Key.R))
            {
                ResetPhysics();
            }
        }

        private void RemoveArrowActor()
        {
            Debug.WriteLine("RemoveArrowActor");
            if (_capsuleBody != null)
            {
                Scene.RemoveActor(_capsuleBody);
                _capsuleBody.Dispose();
                _capsuleBody = null;
            }
        }


        protected override void Draw()
        {

        }
        public void OnContact(ContactPairHeader pairHeader, ContactPair[] pairs)
        {
            Debug.WriteLine($"ShootArrowSample::OnContact");

            foreach (var pair in pairs)
            {
                if (pair.Shape0 != null && pair.Shape1 != null)
                {
                    var arrowName = "Arrow";
                    if (pair.Shape0.Actor.Name == arrowName || pair.Shape1.Actor.Name == arrowName)
                    {
                        //可以获取碰撞点和法线
                        /*
                        {
                            var extractContacts = pair.ExtractContacts();
                            if (extractContacts.Length > 0)
                            {
                                Debug.WriteLine(extractContacts[0].Position);
                            }
                        }
                        */
                        if (_isShooting)
                        {
                            if((pair.Flags & (ContactPairFlag.ActorPairHasFirstTouch)) != 0)
                            {
                                _isShooting = false;
                                Debug.WriteLine(arrowName);


                                SphereGeometry geom = new SphereGeometry(radius: 10f);
                                Matrix4x4 pose = Matrix4x4.CreateTranslation(_capsuleBody.GlobalPosePosition);
                           

                                OverlapHit[]? hits = null;
                                bool status = Scene.Overlap
                                (
                                    geometry: geom,
                                    pose: pose,
                                    maximumOverlaps: 100,
                                    hitCall: hit =>
                                    {
                                        hits = hit;

                                        return true;
                                    }
                                );

                                if (hits != null)
                                {
                                    foreach (OverlapHit hit in hits)
                                    {
                                        RigidActor? actor = hit.Actor;
                                        if (actor != null && actor.Type == ActorType.RigidDynamic)
                                        {
                                            RigidDynamic? dynamicActor = actor as RigidDynamic;
                                            if (dynamicActor != null)
                                            {
                                                Vector3 forceVector = actor.GlobalPosePosition - pose.Translation;
                                                forceVector = Vector3.Normalize(forceVector) * 100;
                                                dynamicActor.AddForceAtLocalPosition(forceVector, Vector3.Zero, ForceMode.Impulse, true);
                                            }
                                        }

                                    }
                                }
                                _isNeedRemoveArrow = true;
                            }
                        }
                    }
                }
            }
        }
    }


    public class EventCallback : SimulationEventCallback
    {
        private ShootArrowSample _sample;

        public EventCallback(ShootArrowSample sample)
        {
            _sample = sample;
        }

        public override void OnContact(ContactPairHeader pairHeader, ContactPair[] pairs)
        {
            base.OnContact(pairHeader, pairs);

            _sample.OnContact(pairHeader, pairs);
        }
    }
}
