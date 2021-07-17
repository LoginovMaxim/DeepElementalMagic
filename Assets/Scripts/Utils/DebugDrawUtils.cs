using UnityEngine;

namespace Utils
{
    public static class DebugDrawUtils
    {
        public static void DrawLine(Vector3 startPosition, Vector3 endPosition, Color color)
        {
            Debug.DrawLine(startPosition, endPosition, color, float.MaxValue);
        }
    
        public static void DrawSquare(Vector3 APosition, Vector3 BPosition, Vector3 CPosition, Vector3 DPosition, Color color)
        {
            Debug.DrawLine(APosition, BPosition, color, float.MaxValue);
            Debug.DrawLine(BPosition, CPosition, color, float.MaxValue);
            Debug.DrawLine(CPosition, DPosition, color, float.MaxValue);
            Debug.DrawLine(DPosition, APosition, color, float.MaxValue);
        }
    }
}
