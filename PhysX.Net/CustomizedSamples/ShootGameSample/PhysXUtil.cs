using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace PhysX.CustomizedSamples.ShootGameSample
{
    class PhysXUtil
    {
        public static Quaternion GetShortestRotation(Vector3 v0, Vector3 v1)
        {
            float d = Vector3.Dot(v0, v1);
            Vector3 cross = Vector3.Cross(v0, v1);
            Quaternion q = d > -1 ? new Quaternion(cross.X, cross.Y, cross.Z, 1 + d) : MathF.Abs(v0.X) < 1.0f ? new Quaternion(0f, v0.Z, -v0.Y, 0f) : new Quaternion(v0.Y, -v0.X, 0f, 0f);
            return Quaternion.Normalize(q);
        }

        private static Vector3 _normalizedXDirection = new Vector3(1, 0, 0);
        public static Quaternion GetQuatByDirection(Vector3 direction)
        {
            Vector3 d = Vector3.Normalize(direction);
            return GetShortestRotation(_normalizedXDirection, d);
        }

        private static int _innerId = 0;

        public static int GenId()
        {
            ++_innerId;
            return _innerId;
        }

        public static void OnContact(ContactPairHeader pairHeader, ContactPair[] pairs)
        {
            foreach (var pair in pairs)
            {
            }
        }
    }
}
