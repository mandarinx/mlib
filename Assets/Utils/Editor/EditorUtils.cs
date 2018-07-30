using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Mlib {

    public static class EditorUtils {

        public static Texture2D CreateTexture2D(int width, int height, Color col) {
            Color[] pix = new Color[width * height];

            for (int i = 0; i < pix.Length; ++i) {
                pix[i] = col;
            }

            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();

            return result;
        }

        public static string GetSelectedProjectFolder() {
            Object[] assets = Selection.GetFiltered(typeof(Object), SelectionMode.Assets);
            for (int i = 0; i < assets.Length; ++i) {
                string path = AssetDatabase.GetAssetPath(assets[i]);
                if (Directory.Exists(path)) {
                    return path;
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// Creates a relative path from one file or folder to another.
        /// </summary>
        /// <param name="fromPath">Contains the directory that defines the start of the relative path.</param>
        /// <param name="toPath">Contains the path that defines the endpoint of the relative path.</param>
        /// <returns>The relative path from the start directory to the end path.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="fromPath"/> or <paramref name="toPath"/> is <c>null</c>.</exception>
        /// <exception cref="System.UriFormatException"></exception>
        /// <exception cref="System.InvalidOperationException"></exception>
        public static string GetRelativePath(string fromPath, string toPath) {

            if (string.IsNullOrEmpty(fromPath)) {
                throw new ArgumentNullException(nameof(fromPath));
            }

            if (string.IsNullOrEmpty(toPath)) {
                throw new ArgumentNullException(nameof(toPath));
            }

            Uri fromUri = new Uri(AppendDirectorySeparatorChar(fromPath));
            Uri toUri = new Uri(AppendDirectorySeparatorChar(toPath));

            if (fromUri.Scheme != toUri.Scheme) {
                return toPath;
            }

            Uri relativeUri = fromUri.MakeRelativeUri(toUri);
            string relativePath = Uri.UnescapeDataString(relativeUri.ToString());

            if (string.Equals(toUri.Scheme,
                              Uri.UriSchemeFile,
                              StringComparison.OrdinalIgnoreCase)) {
                relativePath = relativePath.Replace(Path.AltDirectorySeparatorChar,
                                                    Path.DirectorySeparatorChar);
            }

            return relativePath;
        }

        private static string AppendDirectorySeparatorChar(string path) {
            // Append a slash only if the path is a directory and does not have a slash.
            if (!Path.HasExtension(path) &&
                !path.EndsWith(Path.DirectorySeparatorChar.ToString())) {
                return path + Path.DirectorySeparatorChar;
            }

            return path;
        }
    }
}
