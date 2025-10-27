using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(TweenSettingData))]
public class TweenDataDrawer : PropertyDrawer
{
    private SerializedProperty TweenType;
    private SerializedProperty UsingCustomValue;
    private SerializedProperty From;
    private SerializedProperty To;
    private SerializedProperty TweenDirection;
    private SerializedProperty TweenMode;
    private SerializedProperty TweenEase;
    private SerializedProperty Duration;
    private SerializedProperty StartDelay;
    private SerializedProperty OnTweenStart;
    private SerializedProperty OnTweenComplete;

    private float previousPropertyHeights = EditorGUIUtility.singleLineHeight;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        Rect helpboxRect = new Rect(position.x, position.y, position.width, GetPropertyHeight(property, label));
        EditorGUI.HelpBox(helpboxRect, "", MessageType.None);

        Rect rectFoldout = new Rect(position.min.x + 18f, position.min.y, position.width, EditorGUIUtility.singleLineHeight);
        GUIStyle foldoutStyle = EditorStyles.foldout;
        foldoutStyle.fontStyle = FontStyle.Bold;

        property.isExpanded = EditorGUI.Foldout(rectFoldout, property.isExpanded, property.displayName, true, foldoutStyle);
        if (property.isExpanded)
        {
            EditorGUI.indentLevel++;
            DrawContent(position);
            EditorGUI.indentLevel--;
        }
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        TweenType = property.FindPropertyRelative(nameof(TweenSettingData.TweenType));
        UsingCustomValue = property.FindPropertyRelative(nameof(TweenSettingData.UsingCustomValue));
        From = property.FindPropertyRelative(nameof(TweenSettingData.From));
        To = property.FindPropertyRelative(nameof(TweenSettingData.To));
        TweenDirection = property.FindPropertyRelative(nameof(TweenSettingData.TweenDirection));
        TweenMode = property.FindPropertyRelative(nameof(TweenSettingData.TweenMode));
        TweenEase = property.FindPropertyRelative(nameof(TweenSettingData.TweenEase));
        Duration = property.FindPropertyRelative(nameof(TweenSettingData.Duration));
        StartDelay = property.FindPropertyRelative(nameof(TweenSettingData.StartDelay));
        OnTweenStart = property.FindPropertyRelative(nameof(TweenSettingData.OnTweenStart));
        OnTweenComplete = property.FindPropertyRelative(nameof(TweenSettingData.OnTweenComplete));

        float height = EditorGUI.GetPropertyHeight(property) + EditorGUIUtility.standardVerticalSpacing;
        if (!property.isExpanded)
            return height;

        if ((TweenTypes)TweenType.intValue == TweenTypes.None)
            height = (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing * 2) * 2;

        if (!UsingCustomValue.boolValue)
        {
            if (EqualsToAll((TweenTypes)TweenType.intValue, TweenTypes.Scale, TweenTypes.ScaleX, TweenTypes.ScaleY, TweenTypes.Fade))
                height -= GetPropertiesTotalHeight(From, To, TweenDirection);
            if ((TweenTypes)TweenType.intValue == TweenTypes.Slide)
                height -= GetPropertiesTotalHeight(From, To);
        }
        else
            height -= GetPropertiesTotalHeight(TweenDirection, TweenMode);

        return height;
    }

    private float GetPropertiesTotalHeight(params SerializedProperty[] properties)
    {
        float height = 0;
        foreach (var property in properties)
        {
            height += EditorGUI.GetPropertyHeight(property) + EditorGUIUtility.standardVerticalSpacing;
        }
        return height;
    }

    private void DrawContent(Rect position)
    {
        previousPropertyHeights = EditorGUIUtility.singleLineHeight;
        DrawField(TweenType, position, 0f, 0f, -5f);
        DrawCustomValueContent(position);

        TweenDirection.enumValueIndex = (TweenTypes)TweenType.intValue != TweenTypes.Slide ? 0 : TweenDirection.enumValueIndex;
        TweenEase.enumValueIndex = (TweenTypes)TweenType.intValue == TweenTypes.None ? 0 : TweenEase.enumValueIndex;
        UsingCustomValue.boolValue = (TweenTypes)TweenType.intValue == TweenTypes.None ? false : UsingCustomValue.boolValue;
        if (!UsingCustomValue.boolValue && (TweenTypes)TweenType.intValue == TweenTypes.Slide)
            DrawField(TweenDirection, position, 0f, 0f, -5f);
        else
            TweenDirection.enumValueIndex = 0;
        if ((TweenTypes)TweenType.intValue != TweenTypes.None)
        {
            if (!UsingCustomValue.boolValue)
                DrawField(TweenMode, position, 0f, 0f, -5f);
            DrawField(TweenEase, position, 0f, 0f, -5f);
            DrawField(Duration, position, 0f, 0f, -5f);
            DrawField(StartDelay, position, 0f, 0f, -5f);
            DrawField(OnTweenStart, position, 5f, 0f, -10f);
            DrawField(OnTweenComplete, position, 5f, 0f, -10f);
        }
    }

    private void DrawCustomValueContent(Rect position)
    {
        if ((TweenTypes)TweenType.intValue == TweenTypes.None) return;
        DrawField(UsingCustomValue, position, 0f, 0f, -5f);
        Vector3 newFromValue = From.vector3Value;
        Vector3 newToValue = To.vector3Value;

        if (!UsingCustomValue.boolValue) return;
        switch ((TweenTypes)TweenType.intValue)
        {
            case TweenTypes.Slide:
            case TweenTypes.Scale:
                DrawField(From, position, 0f, 0f, -5f);
                DrawField(To, position, 0f, 0f, -5f);
                break;
            case TweenTypes.Fade:
            case TweenTypes.ScaleX:
                newFromValue.x = DrawFloatField(From, position, "From", newFromValue.x);
                newToValue.x = DrawFloatField(To, position, "To", newToValue.x);

                From.vector3Value = newFromValue;
                To.vector3Value = newToValue;
                break;
            case TweenTypes.ScaleY:
                newFromValue.y = DrawFloatField(From, position, "From", newToValue.y);
                newToValue.y = DrawFloatField(To, position, "To", newToValue.y);

                From.vector3Value = newFromValue;
                To.vector3Value = newToValue;
                break;
        }
    }

    private float DrawFloatField(SerializedProperty property, Rect position, string label, float value)
    {
        float posY = position.min.y + previousPropertyHeights;

        Rect rect = new Rect(position.min.x, posY, position.width - 5f, EditorGUIUtility.singleLineHeight);
        previousPropertyHeights += EditorGUI.GetPropertyHeight(property) + EditorGUIUtility.standardVerticalSpacing;
        return EditorGUI.FloatField(rect, label, value);
    }

    private void DrawField(SerializedProperty property, Rect position, float xOffset, float yOffset, float widthOffset)
    {
        float posY = position.min.y + previousPropertyHeights;
        Rect rect = new Rect(position.min.x + xOffset, posY + yOffset, position.width + widthOffset, EditorGUI.GetPropertyHeight(property));
        EditorGUI.PropertyField(rect, property);

        previousPropertyHeights += EditorGUI.GetPropertyHeight(property) + EditorGUIUtility.standardVerticalSpacing;
    }

    private bool EqualsToAll<T>(T type, params T[] conditions)
    {
        foreach (var condition in conditions)
        {
            if (type.Equals(condition))
                return true;
        }
        return false;
    }
}
