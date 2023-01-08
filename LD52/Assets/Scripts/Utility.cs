using UnityEngine;

namespace LD52
{
    public static class Utility
    {
        public static float DistanceSqr(Vector3 a, Vector3 b)
        {
            float x = a.x - b.x;
            float y = a.y - b.y;
            
            x *= x;
            y *= y;
            
            return x + y;
        }
    }
}