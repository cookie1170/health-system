using System;
using UnityEditor;
using UnityEngine;

namespace Cookie.HealthSystem.Editor
{
    internal static class HealthSystemInitializer
    {
        [InitializeOnLoadMethod]
        private static void Initialize() {
            if (LayerMask.NameToLayer("Hitboxes") == -1)
                CreateLayer("Hitboxes");
        }

        /*
         * Can't seem to find a way to change layer names through script, so have to do some hacky stuff
         * Credit: https://discussions.unity.com/t/adding-layer-by-script/407882/16
         */
        private static void CreateLayer(string name) {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name), "New layer name string is either null or empty.");

            SerializedObject tagManager = new(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty layerProps = tagManager.FindProperty("layers");
            int propCount = layerProps.arraySize;

            SerializedProperty firstEmptyProp = null;

            for (int i = 0; i < propCount; i++) {
                SerializedProperty layerProp = layerProps.GetArrayElementAtIndex(i);

                string stringValue = layerProp.stringValue;

                if (stringValue == name) return;

                if (stringValue != string.Empty) continue;

                firstEmptyProp ??= layerProp;
            }

            if (firstEmptyProp == null) {
                Debug.LogError($"Maximum limit of {propCount} layers exceeded. Layer \"{name}\" not created.");

                return;
            }

            firstEmptyProp.stringValue = name;
            tagManager.ApplyModifiedProperties();
        }
    }
}