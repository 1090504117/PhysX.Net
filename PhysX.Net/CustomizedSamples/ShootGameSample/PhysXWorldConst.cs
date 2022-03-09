using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace PhysX.CustomizedSamples.ShootGameSample
{
    public enum BodyType
    {
        None,
        Bump
    }

    public enum SceneStage
    {
        PreUpdate,
        Update,
        LateUpdate
    }

    public class UserData
    {
        public BodyType BodyType;
        public int Id;
    }

    public class Cube
    {
        public Vector3 Pos;
        public Quaternion Quat;
        public Vector3 HalfShap;
        public float Weight;
    }

    public class Sphere
    {
        public Vector3 Pos;
        public Quaternion Quat;
        public float Radius;
        public float Weight;
    }

    public class Plane
    {
        public Vector3 Pos;
        public Quaternion Quat;
        public Vector3 HalfShap;
    }


    static class PhysXWorldConst
    {
        public static Cube[] CubeArray = new Cube[]
        {
            new Cube(){ Pos=new Vector3(1,1,0), Quat=Quaternion.Identity, HalfShap=new Vector3(1,1,1), Weight=10},
            new Cube(){ Pos=new Vector3(-13,1,0), Quat=Quaternion.Identity, HalfShap=new Vector3(2,1,1), Weight=20},
            new Cube(){ Pos=new Vector3(5,1,0), Quat=Quaternion.Identity, HalfShap=new Vector3(4,1,1), Weight=100},
            new Cube(){ Pos=new Vector3(27,1,0), Quat=Quaternion.Identity, HalfShap=new Vector3(1,1,1), Weight=50},
        };

        public static Sphere[] SphereArray = new Sphere[]
        {
            new Sphere(){ Pos=new Vector3(1,10,0), Quat=Quaternion.Identity, Radius=1, Weight=10},
            new Sphere(){ Pos=new Vector3(3,10,0), Quat=Quaternion.Identity, Radius=3, Weight=20},
            new Sphere(){ Pos=new Vector3(5,10,0), Quat=Quaternion.Identity, Radius=2, Weight=100},
            new Sphere(){ Pos=new Vector3(7,10,0), Quat=Quaternion.Identity, Radius=4, Weight=50},
        };

        private static float WallPlaneHalfLength = 60;
        private static float WallPlaneHalfHeight = 1;

        public static Plane[] WallArray = new Plane[]
        {
            new Plane(){ Pos=new Vector3(0,-WallPlaneHalfHeight,0), Quat=Quaternion.Identity, HalfShap=new Vector3(WallPlaneHalfLength,WallPlaneHalfHeight,WallPlaneHalfLength)},
            new Plane(){ Pos=new Vector3(0,2*WallPlaneHalfLength+WallPlaneHalfHeight,0), Quat=Quaternion.Identity, HalfShap=new Vector3(WallPlaneHalfLength,WallPlaneHalfHeight,WallPlaneHalfLength)},
            new Plane(){ Pos=new Vector3(-(WallPlaneHalfLength+WallPlaneHalfHeight),WallPlaneHalfLength,0), Quat=Quaternion.Identity, HalfShap=new Vector3(WallPlaneHalfHeight,WallPlaneHalfLength,WallPlaneHalfLength)},
            new Plane(){ Pos=new Vector3((WallPlaneHalfLength+WallPlaneHalfHeight),WallPlaneHalfLength,0), Quat=Quaternion.Identity, HalfShap=new Vector3(WallPlaneHalfHeight,WallPlaneHalfLength,WallPlaneHalfLength)},
            new Plane(){ Pos=new Vector3(0,WallPlaneHalfLength,-(WallPlaneHalfLength+WallPlaneHalfHeight)), Quat=Quaternion.Identity, HalfShap=new Vector3(WallPlaneHalfLength,WallPlaneHalfLength,WallPlaneHalfHeight)},
            new Plane(){ Pos=new Vector3(0,WallPlaneHalfLength,(WallPlaneHalfLength+WallPlaneHalfHeight)), Quat=Quaternion.Identity, HalfShap=new Vector3(WallPlaneHalfLength,WallPlaneHalfLength,WallPlaneHalfHeight)},
        };
    }
}
