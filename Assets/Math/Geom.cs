using UnityEngine;

namespace Mlib.Math {

    public static class Geom {
        // based on the math here:
        // http://math.stackexchange.com/a/1367732
        // based on the code from:
        // https://gist.github.com/jupdike/bfe5eb23d1c395d8a0a1a4ddd94882ac

        // circleA is the center of the first circle, with radius radiusA
        // circleB is the center of the second circle, with radius radiusB
        public static int CircleCircleIntersect(Vector2       circleA,
                                                float         radiusA,
                                                Vector2       circleB,
                                                float         radiusB,
                                                out Vector2[] intersections) {

            float centerDx = circleA.x - circleB.x;
            float centerDy = circleB.y - circleB.y;
            float r = Mathf.Sqrt(centerDx * centerDx + centerDy * centerDy);

            // no intersection
            if (!(Mathf.Abs(radiusA - radiusB) <= r && r <= radiusA + radiusB)) {
                intersections = new Vector2[0];
                return 0;
            }

            float r2d = r * r;
            float r4d = r2d * r2d;
            float rASquared = radiusA * radiusA;
            float rBSquared = radiusB * radiusB;
            float a = (rASquared - rBSquared) / (2 * r2d);
            float r2r2 = (rASquared - rBSquared);
            float c = Mathf.Sqrt(2 * (rASquared + rBSquared) / r2d - (r2r2 * r2r2) / r4d - 1);

            float fx = (circleA.x + circleB.x) / 2 + a * (circleB.x - circleA.x);
            float gx = c * (circleB.y - circleA.y) / 2;
            float ix1 = fx + gx;
            float ix2 = fx - gx;

            float fy = (circleA.y + circleB.y) / 2 + a * (circleB.y - circleA.y);
            float gy = c * (circleA.x - circleB.x) / 2;
            float iy1 = fy + gy;
            float iy2 = fy - gy;

            // if gy == 0 and gx == 0 then the circles are tangent and there is only one solution
            if (Mathf.Abs(gx) < float.Epsilon && Mathf.Abs(gy) < float.Epsilon) {
                intersections = new [] {
                    new Vector2(ix1, iy1)
                };
                return 1;
            }

            intersections = new [] {
                new Vector2(ix1, iy1),
                new Vector2(ix2, iy2),
            };
            return 2;
        }

        // Taken from https://stackoverflow.com/questions/1073336/circle-line-segment-collision-detection-algorithm/1088058#1088058
        public static int LineCircleIntersection(Vector2     lineFrom,
                                                 Vector2     lineTo,
                                                 Vector2     circleCenter,
                                                 float       radius,
                                                 out Vector2 intersectionA,
                                                 out Vector2 intersectionB) {
            // compute the euclidean distance between lineFrom and lineTo
            float lab = Mathf.Sqrt(Mathf.Pow(lineTo.x - lineFrom.x, 2) + Mathf.Pow(lineTo.y - lineFrom.y, 2));
            // compute the direction vector D from lineFrom to lineTo
            Vector3 d = (lineTo - lineFrom) / lab;

            // Now the line equation is x = Dx*t + Ax, y = Dy*t + Ay with 0 <= t <= 1.

            // compute the value t of the closest point to the circle center
            float t = d.x * (circleCenter.x - lineFrom.x) + d.y * (circleCenter.y - lineFrom.y);

            // This is the projection of circleCenter on the line from lineFrom to lineTo.

            // compute the coordinates of the point E on line and closest to C
            Vector2 e = new Vector2(t * d.x + lineFrom.x,
                                    t * d.y + lineFrom.y);
            // compute the euclidean distance from E to C
            float lec = Mathf.Sqrt(Mathf.Pow(e.x - circleCenter.x, 2) + Mathf.Pow(e.y - circleCenter.y, 2));

            // test if the line intersects the circle
            if (lec < radius) {
                // compute distance from t to circle intersection point
                float dt = Mathf.Sqrt(Mathf.Pow(radius, 2) - Mathf.Pow(lec, 2));

                // compute first intersection point
                intersectionA = new Vector2((t - dt) * d.x + lineFrom.x,
                                            (t - dt) * d.y + lineFrom.y);
                // compute second intersection point
                intersectionB = new Vector2((t + dt) * d.x + lineFrom.x,
                                            (t + dt) * d.y + lineFrom.y);
                return 2;
            }

            // else test if the line is tangent to circle
            if (Mathf.Abs(lec - radius) < float.Epsilon) {
                intersectionA = e;
                intersectionB = new Vector2(float.NaN, float.NaN);
                return 1;
            }

            // line doesn't touch circle
            intersectionA = new Vector2(float.NaN, float.NaN);
            intersectionB = new Vector2(float.NaN, float.NaN);
            return 0;
        }

    }
}
