using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Windows.Input;
using PhysX.Samples;

namespace PhysX.CustomizedSamples.ShootGameSample
{
    public class ShootGameSample : Sample
    {
        private int _bumpIndex = 0;

        private PhysXStage _physXStage;

        public ShootGameSample() : base(null, new CustomizedEngine.CustomizedEngine(null, new ShootGameSampleFilterShader()))
        {
            if (Engine != null)
            {
                CustomizedEngine.CustomizedEngine? engine = Engine as CustomizedEngine.CustomizedEngine;
                if (engine!=null)
                {
                    engine.PreUpdate += PreUpdate;
                }
            }

            Run();
        }

        protected override void LoadContent()
        {

        }

        protected override void LoadPhysics(Scene scene)
        {
            EventCallback callback = new EventCallback(this);
            scene.SetSimulationEventCallback(callback);

            CreateWorld();
        }

        private void CreateWorld()
        {
            foreach (Plane wall in PhysXWorldConst.WallArray)
            {
                var material = Scene.Physics.CreateMaterial(0.1f, 0.1f, 0.1f);
                var body = Scene.Physics.CreateRigidStatic();
                body.Name = "Wall";
                body.GlobalPosePosition = wall.Pos;
                body.GlobalPoseQuat = wall.Quat;
                var geom = new BoxGeometry(wall.HalfShap);
                RigidActorExt.CreateExclusiveShape(body, geom, material, null);
                Scene.AddActor(body);
            }

            foreach (Cube cube in PhysXWorldConst.CubeArray)
            {
                var material = Scene.Physics.CreateMaterial(0.1f, 0.1f, 0.1f);
                var body = Scene.Physics.CreateRigidDynamic();
                body.Name = "Box";
                body.GlobalPosePosition = cube.Pos;
                body.GlobalPoseQuat = cube.Quat;
                var geom = new BoxGeometry(cube.HalfShap);
                RigidActorExt.CreateExclusiveShape(body, geom, material, null);
                Scene.AddActor(body);
            }

            foreach (Sphere sphere in PhysXWorldConst.SphereArray)
            {
                var material = Scene.Physics.CreateMaterial(0.1f, 0.1f, 0.1f);
                var body = Scene.Physics.CreateRigidDynamic();
                body.Name = "Sphere";
                body.GlobalPosePosition = sphere.Pos;
                body.GlobalPoseQuat = sphere.Quat;
                body.UserData = (object)new UserData() { BodyType=BodyType.Bump, Id= };
                var geom = new SphereGeometry(sphere.Radius);
                RigidActorExt.CreateExclusiveShape(body, geom, material, null);
                Scene.AddActor(body);
            }
        }

        public void Shoot(Vector3 pos, Vector3 direction)
        {
            var material = Scene.Physics.CreateMaterial(0.1f, 0.1f, 0.1f);
            var body = Scene.Physics.CreateRigidDynamic();
            body.Name = "Sphere";
            body.UserData = (object)new UserData() { BodyType = BodyType.Bump, Id = PhysXUtil.GenId()};
            var geom = new SphereGeometry(1f);
            RigidActorExt.CreateExclusiveShape(body, geom, material, null);
            Scene.AddActor(body);
            body?.AddForceAtLocalPosition(pos, Vector3.Zero, ForceMode.Impulse, true);
        }

        protected void PreUpdate(TimeSpan elapsed)
        {
            _physXStage = PhysXStage.PreUpdate;



            _physXStage = PhysXStage.Update;
        }


        protected override void Update(TimeSpan elapsed)
        {
            _physXStage = PhysXStage.LateUpdate;
        }


        protected override void ProcessKeyboard(Key[] pressedKeys)
        {
            if (pressedKeys.Length <= 0) return;

            if (pressedKeys.Contains(Key.D))
            {
            }
            else if (pressedKeys.Contains(Key.R))
            {
            }
        }

        protected override void Draw()
        {

        }
    }


    public class EventCallback : SimulationEventCallback
    {
        private ShootGameSample _sample;

        public EventCallback(ShootGameSample sample)
        {
            _sample = sample;
        }

        public override void OnContact(ContactPairHeader pairHeader, ContactPair[] pairs)
        {
            base.OnContact(pairHeader, pairs);

            PhysXUtil.OnContact(pairHeader, pairs);
        }
    }
}
