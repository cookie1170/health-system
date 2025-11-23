using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Cookie.HealthSystem.Editor
{
    [CustomEditor(typeof(Hurtbox2D))]
    public class Hurtbox2DEditor : UnityEditor.Editor
    {
        [SerializeField] private StyleSheet style;

        public override VisualElement CreateInspectorGUI() {
            VisualElement root = new();

            root.styleSheets.Add(style);

            var hurtbox2D = (Hurtbox2D)target;

            VisualElement panel2D = new();
            panel2D.AddToClassList("panel");

            PropertyField overrideTrigger = new() {
                bindingPath = "overrideTrigger",
                label = "Override trigger",
            };

            ObjectField triggerOverride = new() {
                bindingPath = "trigger",
                objectType = typeof(Collider2D),
                label = "Trigger",
            };

            overrideTrigger.RegisterValueChangeCallback(_ =>
                triggerOverride.style.display = hurtbox2D.overrideTrigger ? DisplayStyle.Flex : DisplayStyle.None
            );

            panel2D.Add(overrideTrigger);
            panel2D.Add(triggerOverride);

            root.Add(panel2D);

            return root;
        }
    }
}