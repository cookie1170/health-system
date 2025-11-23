using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Cookie.HealthSystem.Editor
{
    [CustomEditor(typeof(Hurtbox3D))]
    public class Hurtbox3DEditor : UnityEditor.Editor
    {
        [SerializeField] private StyleSheet style;

        public override VisualElement CreateInspectorGUI() {
            VisualElement root = new();

            root.styleSheets.Add(style);

            var hurtbox3D = (Hurtbox3D)target;

            VisualElement panel3D = new();
            panel3D.AddToClassList("panel");

            PropertyField overrideTrigger = new() {
                bindingPath = "overrideTrigger",
                label = "Override trigger",
            };

            ObjectField triggerOverride = new() {
                bindingPath = "trigger",
                objectType = typeof(Collider),
                label = "Trigger",
            };

            overrideTrigger.RegisterValueChangeCallback(_ =>
                triggerOverride.style.display = hurtbox3D.overrideTrigger ? DisplayStyle.Flex : DisplayStyle.None
            );

            panel3D.Add(overrideTrigger);
            panel3D.Add(triggerOverride);

            root.Add(panel3D);

            return root;
        }
    }
}