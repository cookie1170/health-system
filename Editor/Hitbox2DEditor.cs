using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Cookie.HealthSystem.Editor
{
    [CustomEditor(typeof(Hitbox2D))]
    public class Hitbox2DEditor : HitboxEditor
    {
        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = base.CreateInspectorGUI();

            var hitbox2D = (Hitbox2D)target;

            VisualElement panel2D = new();
            panel2D.AddToClassList("panel");

            Foldout title2D = new() { text = "2D", viewDataKey = "title2D" };

            title2D.AddToClassList("title");

            PropertyField overrideTrigger = new()
            {
                bindingPath = "overrideTrigger",
                label = "Override trigger",
            };

            ObjectField triggerOverride = new()
            {
                bindingPath = "trigger",
                objectType = typeof(Collider2D),
                label = "Trigger",
            };

            overrideTrigger.RegisterValueChangeCallback(_ =>
                triggerOverride.style.display = hitbox2D.overrideTrigger
                    ? DisplayStyle.Flex
                    : DisplayStyle.None
            );

            title2D.Add(overrideTrigger);
            title2D.Add(triggerOverride);

            panel2D.Add(title2D);

            root.Add(panel2D);

            return root;
        }
    }
}
