﻿using UnityEditor;

namespace Mlib {
    public static class SaveProject {

        [MenuItem("File/Save Project! &%s")]
        public static void SaveTheDarnProject() {
            EditorApplication.ExecuteMenuItem("File/Save Project");
        }
    }
}
