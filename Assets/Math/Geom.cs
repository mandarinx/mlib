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
    }
}
