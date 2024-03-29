#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UI;

namespace Code.Core.Toggle
{
[CustomEditor(typeof(ToggleButton))]
public class ToggleButtonEditor : ButtonEditor
{
    private SerializedProperty _isOn;
    private SerializedProperty _toggleImagesGroup;
    private SerializedProperty _interactableImagesGroup;
    private SerializedProperty _notInteractableImagesGroup;
    
    protected override void OnEnable()
    {
        base.OnEnable();
        _isOn = serializedObject.FindProperty("_isOn");
        _toggleImagesGroup = serializedObject.FindProperty("_toggleImagesGroup");
        _interactableImagesGroup = serializedObject.FindProperty("_interactableImagesGroup");
        _notInteractableImagesGroup = serializedObject.FindProperty("_notInteractableImagesGroup");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();

        EditorGUILayout.PropertyField(_isOn);
        EditorGUILayout.PropertyField(_toggleImagesGroup);
        EditorGUILayout.PropertyField(_interactableImagesGroup);
        EditorGUILayout.PropertyField(_notInteractableImagesGroup);

        serializedObject.ApplyModifiedProperties();
    }
}
}
#endif
