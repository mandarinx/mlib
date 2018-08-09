using UnityEngine;

namespace Mlib {

    public static class GizmoUtils {

        public static float res = Mathf.PI * 2f / 32f;

        public static void DrawCircleXY(Vector3 position, float radius) {
            DrawCircle(position, radius, Vector3.forward);
        }

        public static void DrawCircleXZ(Vector3 position, float radius) {
            DrawCircle(position, radius, Vector3.up);
        }

        public static void DrawCircleYZ(Vector3 position, float radius) {
            DrawCircle(position, radius, Vector3.right);
        }

        public static void DrawArcXY(Vector3 position,
                                     float   radius,
                                     float   startRadians,
                                     float   endRadians) {
            DrawArc(position, radius, startRadians, endRadians, Vector3.forward);
        }

        public static void DrawArcXZ(Vector3 position,
                                     float   radius,
                                     float   startRadians,
                                     float   endRadians) {
            DrawArc(position, radius, startRadians, endRadians, Vector3.down);
        }

        public static void DrawArcYZ(Vector3 position,
                                     float   radius,
                                     float   startRadians,
                                     float   endRadians) {
            DrawArc(position, radius, startRadians, endRadians, Vector3.right);
        }

        public static void DrawCircle(Vector3 position, float radius, Vector3 normal) {
            Vector3[] circleCoords;
            int numCoords = GetCircleCoords(radius, out circleCoords);
            if (numCoords == 0) {
                return;
            }

            Vector3 lastPos = RotateVector(circleCoords[0], normal);
            for (int i = 1; i < numCoords; ++i) {
                Gizmos.DrawLine(position + lastPos,
                                position + RotateVector(circleCoords[i], normal));
                lastPos = RotateVector(circleCoords[i], normal);
            }

            Gizmos.DrawLine(position + RotateVector(circleCoords[circleCoords.Length - 1], normal),
                            position + RotateVector(circleCoords[0], normal));
        }

        // res = angle resolution in radians
        public static int GetCircleCoords(float radius, out Vector3[] coords) {
            float theta = 0f;
            float x = radius * Mathf.Cos(theta);
            float y = radius * Mathf.Sin(theta);
            const float TWOPI = Mathf.PI * 2;
            int segments = Mathf.FloorToInt(TWOPI / res);
            coords = new Vector3[segments];
            coords[0] = new Vector3(x, y, 0f);
            int ci = 1;
            for (theta = res; theta < TWOPI; theta += res, ++ci) {
                x = radius * Mathf.Cos(theta);
                y = radius * Mathf.Sin(theta);
                if (ci < segments) {
                    coords[ci] = new Vector3(x, y, 0f);
                }
            }

            return segments;
        }

        public static Vector3 RotateVector(Vector3 vector, Vector3 normal) {
            return Quaternion.LookRotation(normal, Vector3.up) * vector;
        }

        public static void DrawArc(Vector3 pos,
                                   float   radius,
                                   float   startRadians,
                                   float   endRadians,
                                   Vector3 normal) {

            if (startRadians > endRadians) {
                float start = startRadians;
                startRadians = endRadians;
                endRadians = start;
            }

            startRadians = Mathf.Clamp(startRadians, 0f, Mathf.PI * 2f);
            endRadians = Mathf.Clamp(endRadians, 0f, Mathf.PI * 2f);
            float diff = endRadians - startRadians;
            int segments = Mathf.CeilToInt(diff / res);
            Vector3 from = new Vector3(Mathf.Cos(startRadians) * radius,
                                       Mathf.Sin(startRadians) * radius);
            from = RotateVector(from, normal);

            for (int i = 1; i <= segments; ++i) {
                float rad = Mathf.Clamp(startRadians + res * i, startRadians, endRadians);
                Vector3 to = new Vector3(Mathf.Cos(rad) * radius,
                                         Mathf.Sin(rad) * radius);
                to = RotateVector(to, normal);
                Gizmos.DrawLine(pos + from, pos + to);
                from = to;
            }
        }
    }
}
