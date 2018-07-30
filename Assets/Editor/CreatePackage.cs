using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Mlib {

    public class CreatePackage : EditorWindow {

        [Serializable]
        public class PackageJson {
            public string        name;
            public string        displayName;
            public string        version;
            public string        unity;
            public string        description;
            public string        category;
            public string[]      keywords;

            [NonSerialized]
            public string        selectedFolderDisplay;
            [NonSerialized]
            public string        selectedFolderFull;
        }

        [Serializable]
        public class AsmDef {
            public string        name;
            public List<string>  references;
            public List<string>  includePlatforms;
            public List<string>  excludePlatforms;
        }

        private PackageJson      package;
        private string           keywords;
        private bool             showHelp;
        private bool             isValid;

        [MenuItem("Tools/Mlib/Create Package ...")]
        public static void Open() {
            CreatePackage win = GetWindow<CreatePackage>();
            win.titleContent = new GUIContent("Create Package");
            win.minSize = new Vector2(300, 360);
            win.Show();
        }

        private void OnEnable() {
            package = new PackageJson();
            package.selectedFolderDisplay = EditorUtils.GetSelectedProjectFolder();
            if (!string.IsNullOrEmpty(package.selectedFolderDisplay)) {
                package.selectedFolderFull = Path.GetFullPath(package.selectedFolderDisplay);
            }
            keywords = string.Empty;
        }

        private void OnGUI() {
            EditorGUILayout.BeginHorizontal("Toolbar");
            EditorGUILayout.Space();
            if (GUILayout.Button(!showHelp ? "Show Help" : "Hide Help",
                                 new GUIStyle("toolbarbutton"),
                                 GUILayout.Width(60))) {
                showHelp = !showHelp;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            package.name = EditorGUILayout.TextField("Package name", package.name);
            if (showHelp) {
                EditorGUILayout.HelpBox("Use format com.something.something", MessageType.None);
            }

            package.displayName = EditorGUILayout.TextField("Display name", package.displayName);
            if (showHelp) {
                EditorGUILayout.HelpBox("A readable name", MessageType.None);
            }

            package.version = EditorGUILayout.TextField("Version", package.version);
            if (showHelp) {
                EditorGUILayout.HelpBox("Use semantic versioning", MessageType.None);
            }


            using (var h = new EditorGUILayout.HorizontalScope()) {
                EditorGUILayout.LabelField("Create in folder:", package.selectedFolderDisplay);
                string folderFullPath;
                if (SelectFolder("...", 30, out folderFullPath)
                    && !string.IsNullOrEmpty(folderFullPath)) {

                    package.selectedFolderFull = folderFullPath;
                    package.selectedFolderDisplay = "Assets/"
                                                    + EditorUtils.GetRelativePath(Application.dataPath,
                                                                                  package.selectedFolderFull);
                }
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Optionals", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            package.unity = EditorGUILayout.TextField("Unity version", package.unity);
            package.description = EditorGUILayout.TextField("Description", package.description);
            package.category = EditorGUILayout.TextField("Category", package.category);

            keywords = EditorGUILayout.TextField("Keywords", keywords);
            if (keywords.Length > 0) {
                string[] kws = keywords.Split(',');
                List<string> trimmedKeywords = new List<string>();
                for (int i = 0; i < kws.Length; ++i) {
                    string k = kws[i].Trim();
                    if (string.IsNullOrEmpty(k)) {
                        continue;
                    }
                    trimmedKeywords.Add(k);
                }
                package.keywords = trimmedKeywords.ToArray();
            }

            EditorGUILayout.Space();

            isValid = Validate(package);
            GUI.enabled = isValid;

            if (GUILayout.Button("Create Package", GUILayout.ExpandWidth(true))) {
                CreatePackageFiles(package);
            }

            GUI.enabled = true;

            if (MissingFolder(package)) {
                EditorGUILayout.HelpBox($"A directory for writing the package files is required.",
                                        MessageType.Warning);
            }

            if (PackageExists(package)) {
                EditorGUILayout.HelpBox($"A package.json already exists at {package.selectedFolderDisplay}.",
                                        MessageType.Warning);
            }
        }

        private static bool Validate(PackageJson package) {
            bool valid = true;

            valid &= !string.IsNullOrEmpty(package.displayName);

            if (!string.IsNullOrEmpty(package.version)) {
                Match match = Regex.Match(package.version,
                                          @"^(0|[1-9]\d*)\.(0|[1-9]\d*)\.(0|[1-9]\d*)(?:-((?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*)(?:\.(?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*))*))?(?:\+([0-9a-zA-Z-]+(?:\.[0-9a-zA-Z-]+)*))?$",
								          RegexOptions.IgnoreCase);
                valid &= match.Success;
            } else {
                valid = false;
            }

            if (!string.IsNullOrEmpty(package.name)) {
                Match match = Regex.Match(package.name,
                                          @"^[a-z0-9]+.[a-z0-9]+.[a-z0-9]+$",
								          RegexOptions.IgnoreCase);
                valid &= match.Success;
            } else {
                valid = false;
            }

            valid &= !MissingFolder(package);
            valid &= !PackageExists(package);

            return valid;
        }

        private static bool PackageExists(PackageJson package) {
            return File.Exists(package.selectedFolderFull + "/package.json");
        }

        private static bool MissingFolder(PackageJson package) {
            return string.IsNullOrEmpty(package.selectedFolderFull);
        }

        private static void CreatePackageFiles(PackageJson package) {
            File.WriteAllText(Path.Combine(package.selectedFolderFull, "package.json"),
                              JsonUtility.ToJson(package, true));

            AsmDef asmdef = new AsmDef {
                name = package.name
            };
            File.WriteAllText(Path.Combine(package.selectedFolderFull, asmdef.name + ".asmdef"),
                              JsonUtility.ToJson(asmdef));

            AssetDatabase.Refresh();
        }

        private static bool SelectFolder(string label, int width, out string folder) {
            if (!GUILayout.Button(label, GUILayout.Width(width), GUILayout.Height(13))) {
                folder = string.Empty;
                return false;
            }
            folder = EditorUtility.OpenFolderPanel("Select folder", "Assets", "");
            return true;
        }
    }
}
