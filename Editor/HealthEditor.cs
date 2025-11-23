using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Cookie.HealthSystem.Editor
{
    [CustomEditor(typeof(Health))]
    public class HealthEditor : UnityEditor.Editor
    {
        [SerializeField] private VisualTreeAsset inspector;

        public override VisualElement CreateInspectorGUI() {
            VisualElement root = new();

            var health = (Health)target;

            inspector.CloneTree(root);

            var dataObject = root.Q<PropertyField>("DataObject");
            var createDataObject = root.Q<Button>("GenerateDataObject");
            var dataObjectInspectorPanel = root.Q<VisualElement>("DataObjectInspectorPanel");
            var dataTitle = root.Q<Foldout>("DataTitle");

            createDataObject.RegisterCallback<ClickEvent>(CreateDataObject);
            dataObject.RegisterValueChangeCallback(_ => CheckDataObject());

            return root;

            void CheckDataObject() {
                var dataInspectorCurrent = dataObjectInspectorPanel.Q<VisualElement>("DataInspector");
                if (dataInspectorCurrent != null) dataTitle.Remove(dataInspectorCurrent);

                if (health.data) {
                    createDataObject.style.display = DisplayStyle.None;
                    InspectorElement dataInspector = new(health.data) {
                        name = "DataInspector",
                    };

                    dataObjectInspectorPanel.style.display = DisplayStyle.Flex;
                    dataTitle.Add(dataInspector);
                    dataTitle.text = health.data.name;
                } else {
                    createDataObject.style.display = DisplayStyle.Flex;
                    dataObjectInspectorPanel.style.display = DisplayStyle.None;
                }
            }

            void CreateDataObject(ClickEvent evt) {
                string path = EditorUtility.SaveFilePanelInProject(
                    "Create health data", $"{health.name}_HealthData", "asset",
                    "Choose a path for the data object"
                );

                var data = CreateInstance<HealthData>();

                AssetDatabase.CreateAsset(data, path);
                Undo.RegisterCreatedObjectUndo(data, "Created health data");
                Undo.RecordObject(health, "Assigned health data");
                health.data = data;

                CheckDataObject();
            }
        }
    }
}