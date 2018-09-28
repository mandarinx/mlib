using UnityEngine;
using System;
using System.Collections.Generic;

namespace Mlib {

    // Taken from https://gist.github.com/bugshake/9adbb023494b7b886c78d1f82e67df65

    public static class SortedGizmos {

        public static Color color { get; set; }

        private static readonly List<ICommand> commands = new List<ICommand>(1000);

        public static void BatchCommit() {
#if UNITY_EDITOR
            Camera cam;
            var sv = UnityEditor.SceneView.currentDrawingSceneView;
            if (sv != null && sv.camera != null) {
                cam = sv.camera;
            } else {
                cam = Camera.main;
            }
            if (cam != null) {
                var mat = cam.worldToCameraMatrix;
                for (int i = 0; i < commands.Count; ++i) {
                    commands[i].Transform(mat);
                }
                // sort by z
                var a = commands.ToArray();
                Array.Sort<ICommand>(a, CompareCommands);
                // draw
                for (int i = 0; i < a.Length; ++i) {
                    a[i].Draw();
                }
            }
    #endif
            commands.Clear();
        }

        public static void DrawSphere(Vector3 center, float radius) {
            commands.Add(new DrawSolidSphereCommand(color, center, radius));
        }

        public static void DrawWireSphere(Vector3 center, float radius) {
            commands.Add(new DrawWireSphereCommand(color, center, radius));
        }

        public static void DrawSolidCube(Vector3 center, Vector3 size) {
            commands.Add(new DrawSolidCubeCommand(color, center, size));
        }

        public static void DrawWireCube(Vector3 center, Vector3 size) {
            commands.Add(new DrawWireCubeCommand(color, center, size));
        }

        public static void DrawSolidMesh(Vector3 center, Vector3 size, Mesh mesh, Quaternion rotation, Vector3 scale) {
            commands.Add(new DrawSolidMeshCommand(color, center, mesh, scale, rotation));
        }

        public static void DrawWireMesh(Vector3 center, Vector3 size, Mesh mesh, Quaternion rotation, Vector3 scale) {
            commands.Add(new DrawWireMeshCommand(color, center, mesh, scale, rotation));
        }

        private static int CompareCommands(ICommand a, ICommand b) {
            float diff = a.SortValue - b.SortValue;
            if (diff < 0f) return -1;
            return diff > 0f ? 1 : 0;
        }

        private interface ICommand {
            void Transform(Matrix4x4 worldToCamera);
            void Draw();
            float SortValue { get; }
        }

        private struct DrawSolidSphereCommand : ICommand {
            private readonly Color color;
            private readonly Vector3 position;
            private readonly float radius;
            private Vector3 transformedPosition;

            public DrawSolidSphereCommand(Color color, Vector3 position, float radius) {
                this.color = color;
                this.position = position;
                this.radius = radius;
                transformedPosition = Vector3.zero;
            }

            public void Transform(Matrix4x4 mat) {
                transformedPosition = mat.MultiplyPoint(position);
            }

            public void Draw() {
                Gizmos.color = color;
                Gizmos.DrawSphere(position, radius);
            }

            public float SortValue => transformedPosition.z;
        }

        private struct DrawWireSphereCommand : ICommand {
            private readonly Color color;
            private readonly Vector3 position;
            private readonly float radius;
            private Vector3 transformedPosition;

            public DrawWireSphereCommand(Color color, Vector3 position, float radius) {
                this.color = color;
                this.position = position;
                this.radius = radius;
                transformedPosition = Vector3.zero;
            }

            public void Transform(Matrix4x4 mat) {
                transformedPosition = mat.MultiplyPoint(position);
            }

            public void Draw() {
                Gizmos.color = color;
                Gizmos.DrawWireSphere(position, radius);
            }

            public float SortValue => transformedPosition.z;
        }

        private struct DrawSolidCubeCommand : ICommand {
            private readonly Color color;
            private readonly Vector3 position;
            private readonly Vector3 size;
            private Vector3 transformedPosition;

            public DrawSolidCubeCommand(Color color, Vector3 position, Vector3 size) {
                this.color = color;
                this.position = position;
                this.size = size;
                transformedPosition = Vector3.zero;
            }

            public void Transform(Matrix4x4 mat) {
                transformedPosition = mat.MultiplyPoint(position);
            }

            public void Draw() {
                Gizmos.color = color;
                Gizmos.DrawCube(position, size);
            }

            public float SortValue => transformedPosition.z;
        }

        private struct DrawWireCubeCommand : ICommand {
            private readonly Color color;
            private readonly Vector3 position;
            private readonly Vector3 size;
            private Vector3 transformedPosition;

            public DrawWireCubeCommand(Color color, Vector3 position, Vector3 size) {
                this.color = color;
                this.position = position;
                this.size = size;
                transformedPosition = Vector3.zero;
            }

            public void Transform(Matrix4x4 mat) {
                transformedPosition = mat.MultiplyPoint(position);
            }

            public void Draw() {
                Gizmos.color = color;
                Gizmos.DrawWireCube(position, size);
            }

            public float SortValue => transformedPosition.z;
        }

        private struct DrawSolidMeshCommand : ICommand {
            private readonly Color color;
            private readonly Vector3 position;
            private readonly Vector3 scale;
            private readonly Quaternion rotation;
            private readonly Mesh mesh;
            private Vector3 transformedPosition;

            public DrawSolidMeshCommand(Color color, Vector3 position, Mesh mesh, Vector3 scale, Quaternion rotation) {
                this.color = color;
                this.position = position;
                this.mesh = mesh;
                this.scale = scale;
                this.rotation = rotation;
                transformedPosition = Vector3.zero;
            }

            public void Transform(Matrix4x4 mat) {
                transformedPosition = mat.MultiplyPoint(position);
            }

            public void Draw() {
                Gizmos.color = color;
                Gizmos.DrawMesh(mesh, position, rotation, scale);
            }

            public float SortValue => transformedPosition.z;
        }

        private struct DrawWireMeshCommand : ICommand {
            private readonly Color color;
            private readonly Vector3 position;
            private readonly Vector3 scale;
            private readonly Quaternion rotation;
            private readonly Mesh mesh;
            private Vector3 transformedPosition;

            public DrawWireMeshCommand(Color color, Vector3 position, Mesh mesh, Vector3 scale, Quaternion rotation) {
                this.color = color;
                this.position = position;
                this.mesh = mesh;
                this.scale = scale;
                this.rotation = rotation;
                transformedPosition = Vector3.zero;
            }

            public void Transform(Matrix4x4 mat) {
                transformedPosition = mat.MultiplyPoint(position);
            }

            public void Draw() {
                Gizmos.color = color;
                Gizmos.DrawWireMesh(mesh, position, rotation, scale);
            }

            public float SortValue => transformedPosition.z;
        }
    }
}
