using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Mlib {

    /// <summary>
    /// Generates a boilerplate custom inspector script for Monobehaviours and populates it with its serialized fields.
    /// Created for Unity Hackweek 2018 by Adrienne Lombardo.
    /// STANDALONE VERSION (no usage of internal UnityEditor classes/methods)
    /// </summary>
    public static class CreateCustomInspector {
        [MenuItem("Assets/Create/Custom Inspector", priority = 81)]
        static void GenerateInspectorEditorClass() {
            foreach (var script in Selection.objects) {
                // We validate the object before running the following method so we don't bother checking again here.
                BuildEditorFile(script as MonoScript);
            }

            AssetDatabase.Refresh();
        }

        // Validate if an inspector can be generated for the selected objects.
        [MenuItem("Assets/Create/Custom Inspector", priority = 81, validate = true)]
        static bool ValidateGenerateInspectorEditorClass() {
            foreach (var script in Selection.objects) {
                string path = AssetDatabase.GetAssetPath(script);
                MonoScript ms = script as MonoScript;

                if (script.GetType() != typeof(MonoScript))
                    return false;
                if (ms == null)
                    return false;
                if (!ms.GetClass().IsSubclassOf(typeof(MonoBehaviour)))
                    return false;
                if (!path.EndsWith(".cs"))
                    return false;
                if (path.Contains("Editor"))
                    return false;
            }

            return true;
        }

        static void BuildEditorFile(MonoScript monoScript) {
            string assetPath = AssetDatabase.GetAssetPath(monoScript);
            string scriptName = Path.GetFileNameWithoutExtension(assetPath);

            // Check that editor folder exists and we aren't creating a potential duplicate file.
            var editorFolder = Path.GetDirectoryName(assetPath) + "/Editor";

            if (!Directory.Exists(editorFolder)) {
                Directory.CreateDirectory(editorFolder);
            }

            if (File.Exists(editorFolder + "/" + scriptName + "Inspector.cs")) {
                Debug.LogError(scriptName + "Inspector.cs already exists.");
                return;
            }

            string scriptNamespace = monoScript.GetClass().Namespace;
            bool usesNamespace = string.IsNullOrEmpty(scriptNamespace);
            string script = usesNamespace ? template : namespaceTemplate;
            string[] scriptFields = GetSerializedFields(monoScript.GetClass());

            // Populate template
            script = script.Replace("#NAMESPACE#", scriptNamespace);
            script = script.Replace("#SCRIPTNAME#", scriptName);
            script = script.Replace("#SERIALIZEDPROPERTY#", GenerateFields(scriptFields, usesNamespace));
            script = script.Replace("#FINDPROPERTY#", GenerateFieldAssignments(scriptFields, usesNamespace));
            script = script.Replace("#PROPERTYFIELD#", GenerateEditorGUI(scriptFields, usesNamespace));

            script = SetLineEndings(script, EditorSettings.lineEndingsForNewScripts);

            // Write script to file.
            File.WriteAllText(editorFolder + "/" + scriptName + "Inspector.cs", script);
        }

        #region Reflection methods

        // Return an array containing the names of all serialized fields in a class.
        static string[] GetSerializedFields(Type t) {
            List<string> fields = new List<string>();

            var bindingFlags = BindingFlags.Instance |
                               BindingFlags.NonPublic |
                               BindingFlags.Public;

            FieldInfo[] fi = t.GetFields(bindingFlags);
            for (int i = 0; i < fi.Length; i++) {
                if (IsSerialized(fi[i]))
                    fields.Add(fi[i].Name);
            }

            return fields.ToArray();
        }

        // Return if the given field has the SerializeField attribute applied to it.
        static bool IsSerialized(FieldInfo fi) {
            if (fi.IsPublic) return true;

            SerializeField[] attributes = fi.GetCustomAttributes(typeof(SerializeField), false) as SerializeField[];
            return attributes != null && attributes.Length > 0;
        }

        #endregion

        #region Generation methods

        //todo: make the code used here for indenting
        static string GenerateFields(string[] s, bool increaseIndent) {
            string output = "";
            for (int i = 0; i < s.Length; i++) {
                output += string.Format("private SerializedProperty {0}Prop;", s[i]);

                if (i < s.Length - 1)
                    output += "\n" + (increaseIndent ? "\t" : "\t\t");
            }

            return output;
        }

        static string GenerateFieldAssignments(string[] s, bool increaseIndent) {
            string output = "";
            for (int i = 0; i < s.Length; i++) {
                output += string.Format("{0}Prop = serializedObject.FindProperty(\"{0}\");", s[i]);

                if (i < s.Length - 1)
                    output += "\n" + (increaseIndent ? "\t\t" : "\t\t\t");
            }

            return output;
        }

        static string GenerateEditorGUI(string[] s, bool increaseIndent) {
            string output = "";
            for (int i = 0; i < s.Length; i++) {
                output += string.Format("EditorGUILayout.PropertyField({0}Prop);", s[i]);

                if (i < s.Length - 1)
                    output += "\n" + (increaseIndent ? "\t\t" : "\t\t\t");
            }

            return output;
        }

        // Fixes line ending warnings thrown by Unity after creating the script. (Code from internal method in ProjectWindowUtil.cs)
        static string SetLineEndings(string content, LineEndingsMode lineEndingsMode) {
            const string windowsLineEndings = "\r\n";
            const string unixLineEndings = "\n";

            string preferredLineEndings;

            switch (lineEndingsMode) {
            case LineEndingsMode.OSNative:
                if (Application.platform == RuntimePlatform.WindowsEditor)
                    preferredLineEndings = windowsLineEndings;
                else
                    preferredLineEndings = unixLineEndings;
                break;
            case LineEndingsMode.Unix:
                preferredLineEndings = unixLineEndings;
                break;
            case LineEndingsMode.Windows:
                preferredLineEndings = windowsLineEndings;
                break;
            default:
                preferredLineEndings = unixLineEndings;
                break;
            }

            content = Regex.Replace(content, @"\r\n?|\n", preferredLineEndings);

            return content;
        }

        #endregion

        #region Template strings

        static readonly string template =
            @"using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(#SCRIPTNAME#))]
public class #SCRIPTNAME#Inspector : Editor {

    #SERIALIZEDPROPERTY#

    void OnEnable() {
        #FINDPROPERTY#
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();

        //DrawDefaultInspector();

        #PROPERTYFIELD#

        serializedObject.ApplyModifiedProperties();
    }
}";

        static readonly string namespaceTemplate =
            @"using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace #NAMESPACE# {
    [CustomEditor(typeof(#SCRIPTNAME#))]
    public class #SCRIPTNAME#Inspector : Editor {

        #SERIALIZEDPROPERTY#

        void OnEnable() {
            #FINDPROPERTY#
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();

            //DrawDefaultInspector();

            #PROPERTYFIELD#

            serializedObject.ApplyModifiedProperties();
        }
    }
}";

        #endregion
    }
}
