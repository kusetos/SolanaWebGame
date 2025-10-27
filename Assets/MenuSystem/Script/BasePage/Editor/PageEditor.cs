using System;
using System.Reflection;
using System.Linq;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Page), true), CanEditMultipleObjects]
public class PageEditor : Editor
{
    SerializedProperty IsDestroyWhenClosed;
    SerializedProperty IsOverlay;
    SerializedProperty IsWaitingPreviousPageAnimationComplete;

    SerializedProperty IsUsingAnimation;
    SerializedProperty pageAnimation;

    SerializedProperty pageContent;

    private Page page;
    private object currentBasePageAnimationObjType;

    private FieldInfo[] childFields;

    private void OnEnable()
    {
        page = (Page)target;
        childFields = target.GetType().GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        PopulatePropertyObject();
        OnEditorInitialize();
    }

    private void PopulatePropertyObject()
    {
        IsDestroyWhenClosed = serializedObject.FindProperty(nameof(IsDestroyWhenClosed));
        IsOverlay = serializedObject.FindProperty(nameof(IsOverlay));
        IsUsingAnimation = serializedObject.FindProperty(nameof(IsUsingAnimation));
        pageAnimation = serializedObject.FindProperty(nameof(pageAnimation));
        pageContent = serializedObject.FindProperty(nameof(pageContent));
        IsWaitingPreviousPageAnimationComplete = serializedObject.FindProperty(nameof(IsWaitingPreviousPageAnimationComplete));
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("Base Page Settings", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(IsDestroyWhenClosed);
        EditorGUILayout.PropertyField(IsOverlay);
        EditorGUILayout.PropertyField(IsWaitingPreviousPageAnimationComplete);
        DrawPageAnimationProperty();
        EditorGUILayout.PropertyField(pageContent);
        EditorGUI.indentLevel--;
        EditorGUILayout.EndVertical();

        DrawInheritanceField();
        serializedObject.ApplyModifiedProperties();
    }

    private void DrawPageAnimationProperty()
    {
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(IsUsingAnimation);
        if (EditorGUI.EndChangeCheck())
        {
            if (IsUsingAnimation.boolValue == false && currentBasePageAnimationObjType != null)
            {
                DestroyRequiredComponents(page.GetComponent<BasePageAnimation>().GetType());
            }
        }

        EditorGUILayout.BeginHorizontal();
        if (page.IsUsingAnimation)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField(pageAnimation.displayName, GUILayout.Width(EditorGUIUtility.labelWidth));
            var content = new GUIContent(currentBasePageAnimationObjType == null ? "None" : currentBasePageAnimationObjType.ToString());
            if (EditorGUILayout.DropdownButton(content, FocusType.Passive, GUILayout.MinWidth(0)))
            {
                PopulateMenuItem();
            }
            EditorGUI.indentLevel--;
        }
        EditorGUILayout.EndHorizontal();
    }

    private Type[] GetInheritedClasses(Type MyType)
    {
        //if you want the abstract classes drop the !TheType.IsAbstract but it is probably to instance so its a good idea to keep it.
        return Assembly.GetAssembly(MyType).GetTypes().Where(TheType => TheType.IsClass && !TheType.IsAbstract && TheType.IsSubclassOf(MyType)).ToArray();
    }

    private void OnEditorInitialize()
    {
        if (page.TryGetComponent<BasePageAnimation>(out BasePageAnimation basePageAnimation))
        {
            currentBasePageAnimationObjType = basePageAnimation.GetType();
        }
    }

    private void PopulateMenuItem()
    {
        GenericMenu menu = new GenericMenu();
        AddMenuItemForComponent(menu, "None", null);
        foreach (var type in GetInheritedClasses(typeof(BasePageAnimation)))
        {
            AddMenuItemForComponent(menu, type.Name, type);
        }
        menu.ShowAsContext();
    }

    private void AddMenuItemForComponent(GenericMenu menu, string name, Type componentType)
    {
        bool on;
        if (componentType == null && currentBasePageAnimationObjType == null)
            on = true;
        else
            on = currentBasePageAnimationObjType == null ? false : currentBasePageAnimationObjType.Equals(componentType);

        menu.AddItem(new GUIContent(name), on, OnComponentItemSelected, componentType);
    }

    private void OnComponentItemSelected(object componentTypeObj)
    {
        if (componentTypeObj == currentBasePageAnimationObjType) return;
        if (componentTypeObj == null)
        {
            //Destroy Component
            if (currentBasePageAnimationObjType != null)
            {
                DestroyRequiredComponents(page.GetComponent<BasePageAnimation>().GetType());
            }
            currentBasePageAnimationObjType = null;
            pageAnimation.objectReferenceValue = null;
            return;
        }
        currentBasePageAnimationObjType = componentTypeObj;

        //Destroy old component if exist and add new one
        if (page.TryGetComponent<BasePageAnimation>(out BasePageAnimation basePageAnimation))
        {
            if (!currentBasePageAnimationObjType.Equals(basePageAnimation.GetType()))
            {
                DestroyRequiredComponents(basePageAnimation.GetType());
            }
        }
        pageAnimation.objectReferenceValue = page.gameObject.AddComponent(currentBasePageAnimationObjType as Type);
        serializedObject.ApplyModifiedProperties();
    }

    private void DrawInheritanceField()
    {
        if (childFields.Length <= 0) return;

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField(page.GetType().Name + " Settings", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        foreach (FieldInfo field in childFields)
        {
            if (field.IsPublic || field.GetCustomAttribute(typeof(SerializeField)) != null)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty(field.Name));
            }
        }
        EditorGUI.indentLevel--;
        EditorGUILayout.EndVertical();
    }

    //To-Do : maybe add this to editorUtility class
    //This Only remove first type of Required component parameter (m_Type0).
    private void DestroyRequiredComponents(Type memberInfo)
    {
        DestroyImmediate(page.GetComponent<BasePageAnimation>(), true);
        RequireComponent[] requiredComponentsAtts = Attribute.GetCustomAttributes(memberInfo, typeof(RequireComponent), true) as RequireComponent[];
        if (requiredComponentsAtts.Length == 0) return;
        foreach (RequireComponent rc in requiredComponentsAtts)
        {
            if (rc != null && page.GetComponent(rc.m_Type0) != null)
            {
                DestroyImmediate(page.GetComponent(rc.m_Type0), true);
                DestroyRequiredComponents(rc.m_Type0); // Check if the required component type has required component too.
            }
        }
    }
}
