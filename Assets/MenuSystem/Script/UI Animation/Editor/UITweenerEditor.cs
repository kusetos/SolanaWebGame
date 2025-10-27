using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UITweener)), CanEditMultipleObjects]
public class UITweenerEditor : Editor
{
    private SerializedProperty canPlayOnEnable;
    private SerializedProperty tweenList;

    private UITweener uiTweener;

    private void OnEnable()
    {
        uiTweener = target as UITweener;

        canPlayOnEnable = serializedObject.FindProperty(nameof(canPlayOnEnable));
        tweenList = serializedObject.FindProperty(nameof(tweenList));
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        if (uiTweener.GetComponent<PageTweener>() == null)
        {
            EditorGUILayout.PropertyField(canPlayOnEnable);
            if (canPlayOnEnable.boolValue && !uiTweener.HasTweenIdentifier("OnEnable"))
            {
                UITweener.UITweenerData tweenData = new UITweener.UITweenerData();
                tweenData.TweenIdentifier = "OnEnable";
                uiTweener.InsertTweenData(0, tweenData);
            }
            else if (!canPlayOnEnable.boolValue && uiTweener.HasTweenIdentifier("OnEnable"))
            {
                tweenList.DeleteArrayElementAtIndex(0);
            }

            for (int i = 0; i < tweenList.arraySize; i++)
            {
                DrawTweenDataElement(tweenList.GetArrayElementAtIndex(i), i);
            }

            DrawAddTweenButton();
        }
        else
            EditorGUILayout.HelpBox("This Component Required for PageTweener", MessageType.Info);

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawAddTweenButton()
    {
        if (Selection.objects.Length > 1)
        {
            EditorGUILayout.HelpBox("Add new TweenData not supported on Multi-object editing.", MessageType.None);
            return;
        }
        EditorGUILayout.Space(5f);
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Add New TweenData", GUILayout.MaxWidth(200f)))
        {
            //Add new Tween Data to tweenData list
            uiTweener.AddTweenData(new UITweener.UITweenerData());
        }
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space(5f);
    }

    private void DrawTweenDataElement(SerializedProperty property, int index)
    {
        SerializedProperty TweenIdentifier = property.FindPropertyRelative(nameof(UITweener.UITweenerData.TweenIdentifier));
        SerializedProperty TweenSetting = property.FindPropertyRelative(nameof(UITweener.UITweenerData.TweenSetting));

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        {
            EditorGUILayout.BeginHorizontal();
            {
                GUIStyle foldoutStyle = new GUIStyle(EditorStyles.foldout);
                foldoutStyle.wordWrap = true;
                string identifier = string.IsNullOrEmpty(TweenIdentifier.stringValue) ? string.Empty : " - " + TweenIdentifier.stringValue;
                property.isExpanded = GUILayout.Toggle(property.isExpanded, "Tween Data" + identifier, foldoutStyle);
                if (GUILayout.Button(EditorGUIUtility.IconContent("d_winbtn_win_min", "Remove Tween Data"), EditorStyles.iconButton, GUILayout.MaxWidth(20f)))
                {
                    tweenList.DeleteArrayElementAtIndex(index);
                    return;
                }
            }
            EditorGUILayout.EndHorizontal();
            if (property.isExpanded)
            {
                EditorGUI.BeginDisabledGroup(TweenIdentifier.stringValue == "OnEnable");
                EditorGUILayout.PropertyField(TweenIdentifier);
                EditorGUI.EndDisabledGroup();
                EditorGUILayout.PropertyField(TweenSetting);
            }
        }
        EditorGUILayout.EndVertical();
    }
}
