// Copyright (C) 2014 Stephan Bouchard - All Rights Reserved
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms


using UnityEngine;
using UnityEditor;
using System.Collections;


namespace TMPro.EditorUtilities
{

    [CustomEditor(typeof(TextMeshPro)), CanEditMultipleObjects]
    public class TMPro_EditorPanel : Editor
    {

        private struct m_foldout
        { // Track Inspector foldout panel states, globally.
            public static bool textInput = true;
            public static bool fontSettings = true;
            public static bool shadowSetting = false;
            public static bool materialEditor = true;
        }

        private static int m_eventID;


        private const string k_UndoRedo = "UndoRedoPerformed";

        private GUISkin mySkin;
        //private GUIStyle Group_Label;
        private GUIStyle textAreaBox;
        private GUIStyle Section_Label;

        private SerializedProperty text_prop;   
        private SerializedProperty fontAsset_prop;
        private SerializedProperty fontColor_prop;
        private SerializedProperty fontSize_prop;
        private SerializedProperty characterSpacing_prop;
        private SerializedProperty lineLength_prop;
        private SerializedProperty lineSpacing_prop;
        private SerializedProperty lineJustification_prop;
        private SerializedProperty anchorPosition_prop;
        private SerializedProperty horizontalMapping_prop;
        private SerializedProperty verticalMapping_prop;

        private SerializedProperty enableWordWrapping_prop;
        private SerializedProperty wordWrappingRatios_prop;
        private SerializedProperty enableKerning_prop;

        private SerializedProperty overrideHtmlColor_prop;

        private SerializedProperty inputSource_prop;
        private SerializedProperty havePropertiesChanged_prop;
        private SerializedProperty isInputPasingRequired_prop;
        private SerializedProperty isAffectingWordWrapping_prop;

        private SerializedProperty hasFontAssetChanged_prop;

        private SerializedProperty enableExtraPadding_prop;
        private SerializedProperty checkPaddingRequired_prop;

        private SerializedProperty sortingLayerID_prop;
        private SerializedProperty sortingOrder_prop;

     


        private bool havePropertiesChanged = false;


        private TextMeshPro m_textMeshProScript;
        private Transform m_transform;
        private Renderer m_renderer;
        //private TMPro_UpdateManager m_updateManager;

        private Vector3[] handlePoints = new Vector3[4]; // { new Vector3(-10, -10, 0), new Vector3(-10, 10, 0), new Vector3(10, 10, 0), new Vector3(10, -10, 0) };
        private float prev_lineLenght;



        public void OnEnable()
        {
            // Initialize the Event Listener for Undo Events.
            Undo.undoRedoPerformed += OnUndoRedo;
            //Undo.postprocessModifications += OnUndoRedoEvent;   
      
            text_prop = serializedObject.FindProperty("m_text");
            fontAsset_prop = serializedObject.FindProperty("m_fontAsset");

            fontSize_prop = serializedObject.FindProperty("m_fontSize");
            fontColor_prop = serializedObject.FindProperty("m_fontColor");
            characterSpacing_prop = serializedObject.FindProperty("m_characterSpacing");
            lineLength_prop = serializedObject.FindProperty("m_lineLength");
            lineSpacing_prop = serializedObject.FindProperty("m_lineSpacing");
            lineJustification_prop = serializedObject.FindProperty("m_lineJustification");
            anchorPosition_prop = serializedObject.FindProperty("m_anchor");
            horizontalMapping_prop = serializedObject.FindProperty("m_horizontalMapping");
            verticalMapping_prop = serializedObject.FindProperty("m_verticalMapping");
            enableKerning_prop = serializedObject.FindProperty("m_enableKerning");
            overrideHtmlColor_prop = serializedObject.FindProperty("m_overrideHtmlColors");
            enableWordWrapping_prop = serializedObject.FindProperty("m_enableWordWrapping");
            wordWrappingRatios_prop = serializedObject.FindProperty("m_wordWrappingRatios");

            havePropertiesChanged_prop = serializedObject.FindProperty("havePropertiesChanged");
            inputSource_prop = serializedObject.FindProperty("m_inputSource");
            isInputPasingRequired_prop = serializedObject.FindProperty("isInputParsingRequired");
            isAffectingWordWrapping_prop = serializedObject.FindProperty("isAffectingWordWrapping");
            enableExtraPadding_prop = serializedObject.FindProperty("m_enableExtraPadding");
            checkPaddingRequired_prop = serializedObject.FindProperty("checkPaddingRequired");

            sortingLayerID_prop = serializedObject.FindProperty("m_sortingLayerID");
            sortingOrder_prop = serializedObject.FindProperty("m_sortingOrder");

            hasFontAssetChanged_prop = serializedObject.FindProperty("hasFontAssetChanged");


            if (EditorGUIUtility.isProSkin)
                mySkin = AssetDatabase.LoadAssetAtPath("Assets/TextMesh Pro/GUISkins/TMPro_DarkSkin.guiskin", typeof(GUISkin)) as GUISkin;
            else
                mySkin = AssetDatabase.LoadAssetAtPath("Assets/TextMesh Pro/GUISkins/TMPro_LightSkin.guiskin", typeof(GUISkin)) as GUISkin;

            if (mySkin != null)
            {
                Section_Label = mySkin.FindStyle("Section Label");
                //Group_Label = mySkin.FindStyle("Group Label");
                textAreaBox = mySkin.FindStyle("Text Area Box (Editor)");
            }

            m_textMeshProScript = (TextMeshPro)target;
            m_transform = Selection.activeGameObject.transform;
            m_renderer = Selection.activeGameObject.GetComponent<Renderer>();

            //m_updateManager = Camera.main.gameObject.GetComponent<TMPro_UpdateManager>();
        }


        public void OnDisable()
        {
            Undo.undoRedoPerformed -= OnUndoRedo;
            //Undo.postprocessModifications -= OnUndoRedoEvent;  
        }


        public override void OnInspectorGUI()
        {

            serializedObject.Update();

            GUILayout.Label("<b>TEXT INPUT BOX</b>  <i>(Type your text below.)</i>", Section_Label, GUILayout.Height(23));

            GUI.changed = false;

            text_prop.stringValue = EditorGUILayout.TextArea(text_prop.stringValue, textAreaBox, GUILayout.Height(75), GUILayout.ExpandWidth(true));
            if (GUI.changed)
            {
                GUI.changed = false;
                inputSource_prop.enumValueIndex = 0;
                isInputPasingRequired_prop.boolValue = true;
                isAffectingWordWrapping_prop.boolValue = true;
                havePropertiesChanged = true;         
            }


            GUILayout.Label("<b>FONT SETTINGS</b>", Section_Label);

            EditorGUIUtility.fieldWidth = 30;

            // FONT ASSET
            EditorGUILayout.PropertyField(fontAsset_prop);
            if (GUI.changed)
            {
                GUI.changed = false;
                Undo.RecordObject(m_renderer, "Asset & Material Change");              
                havePropertiesChanged = true;
                hasFontAssetChanged_prop.boolValue = true;
                isAffectingWordWrapping_prop.boolValue = true;               
            }

            // FACE VERTEX COLOR
            EditorGUILayout.PropertyField(fontColor_prop, new GUIContent("Face Color"));
            if (GUI.changed)
            {
                GUI.changed = false;
                havePropertiesChanged = true;
            }

            // FONT SIZE & CHARACTER SPACING GROUP
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(fontSize_prop);
            EditorGUILayout.PropertyField(characterSpacing_prop);
            EditorGUILayout.EndHorizontal();


            // LINE LENGHT & LINE SPACING GROUP
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(lineLength_prop);
            lineLength_prop.floatValue = Mathf.Round(lineLength_prop.floatValue * 100) / 100f; // Rounding Line Length to 2 decimal. 
            if (GUI.changed)
            {
                GUI.changed = false;
                havePropertiesChanged = true;
                isAffectingWordWrapping_prop.boolValue = true;
            }

            EditorGUILayout.PropertyField(lineSpacing_prop);
            EditorGUILayout.EndHorizontal();


            EditorGUILayout.PropertyField(lineJustification_prop);
            if (lineJustification_prop.enumValueIndex == 3)
                EditorGUILayout.Slider(wordWrappingRatios_prop, 0.0f, 1.0f, new GUIContent("Wrap Ratios (W <-> C)"));


            EditorGUILayout.PropertyField(anchorPosition_prop, new GUIContent("Anchor Position:"));


            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("UV Mapping Options");
            EditorGUILayout.PropertyField(horizontalMapping_prop, GUIContent.none, GUILayout.MinWidth(70f));
            EditorGUILayout.PropertyField(verticalMapping_prop, GUIContent.none, GUILayout.MinWidth(70f));
            EditorGUILayout.EndHorizontal();

            if (GUI.changed)
            {
                GUI.changed = false;
                havePropertiesChanged = true;
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(enableWordWrapping_prop, new GUIContent("Enable Word Wrap?"));
            if (GUI.changed)
            {
                GUI.changed = false;
                havePropertiesChanged = true;
                isAffectingWordWrapping_prop.boolValue = true;
                isInputPasingRequired_prop.boolValue = true;
            }
            EditorGUILayout.PropertyField(overrideHtmlColor_prop, new GUIContent("Override Color Tags?"));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(enableKerning_prop, new GUIContent("Enable Kerning?"));
            if (GUI.changed)
            {
                GUI.changed = false;
                isAffectingWordWrapping_prop.boolValue = true;
                havePropertiesChanged = true;
            }
            EditorGUILayout.PropertyField(enableExtraPadding_prop, new GUIContent("Extra Padding?"));
            if (GUI.changed)
            {
                GUI.changed = false;
                havePropertiesChanged = true;
                checkPaddingRequired_prop.boolValue = true;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.PropertyField(sortingLayerID_prop);
            EditorGUILayout.PropertyField(sortingOrder_prop);
           

            if (havePropertiesChanged)
            {
                havePropertiesChanged_prop.boolValue = true;
                havePropertiesChanged = false;              
                //m_updateManager.ScheduleObjectForUpdate(m_textMeshProScript);
            }

            EditorGUILayout.Space();

            serializedObject.ApplyModifiedProperties();
          
            /*
            Editor materialEditor = Editor.CreateEditor(m_renderer.sharedMaterial);
            if (materialEditor != null)
            {
                if (GUILayout.Button("<b>MATERIAL SETTINGS</b>     - <i>Click to expand</i> -", Section_Label))
                    m_foldout.materialEditor= !m_foldout.materialEditor;

                if (m_foldout.materialEditor)
                {
                    materialEditor.OnInspectorGUI();
                }
            }
            */
        }


        public void OnSceneGUI()
        {
            if (enableWordWrapping_prop.boolValue)
            {                             
                // Show Handles to represent Line Lenght settings      
                Bounds meshExtents = m_textMeshProScript.bounds;
                Vector3 lossyScale = m_transform.lossyScale;
             
                handlePoints[0] = m_transform.TransformPoint(new Vector3(meshExtents.min.x * lossyScale.x, meshExtents.min.y, 0));
                handlePoints[1] = m_transform.TransformPoint(new Vector3(meshExtents.min.x * lossyScale.x, meshExtents.max.y, 0));
                handlePoints[2] = handlePoints[1] + m_transform.TransformDirection(new Vector3(m_textMeshProScript.lineLength * lossyScale.x, 0, 0));
                handlePoints[3] = handlePoints[0] + m_transform.TransformDirection(new Vector3(m_textMeshProScript.lineLength * lossyScale.x, 0, 0));

                Handles.DrawSolidRectangleWithOutline(handlePoints, new Color32(0, 0, 0, 0), new Color32(255, 255, 0, 255));

                Vector3 old_right = (handlePoints[2] + handlePoints[3]) * 0.5f;
                Vector3 new_right = Handles.FreeMoveHandle(old_right, Quaternion.identity, HandleUtility.GetHandleSize(m_transform.position) * 0.05f, Vector3.zero, Handles.DotCap);
                
                if (old_right != new_right)
                {
                    float delta = new_right.x - old_right.x;                   
                    m_textMeshProScript.lineLength += delta / lossyScale.x;
                }
            }
        }

        // Special Handling of Undo / Redo Events.
        private void OnUndoRedo()
        {
            int undoEventID = Undo.GetCurrentGroup();
            int LastUndoEventID = m_eventID;

            if (undoEventID != LastUndoEventID)
            {
                for (int i = 0; i < targets.Length; i++)
                {
                    //Debug.Log("Undo & Redo Performed detected in Editor Panel. Event ID:" + Undo.GetCurrentGroup());
                    TMPro_EventManager.ON_TEXTMESHPRO_PROPERTY_CHANGED(true, targets[i] as TextMeshPro);
                    m_eventID = undoEventID;
                }
            }
        }

        /*
        private UndoPropertyModification[] OnUndoRedoEvent(UndoPropertyModification[] modifications)
        {
            int eventID = Undo.GetCurrentGroup();
            PropertyModification modifiedProp = modifications[0].propertyModification;      
            System.Type targetType = modifiedProp.target.GetType();
              
            if (targetType == typeof(Material))
            {
                //Debug.Log("Undo / Redo Event Registered in Editor Panel on Target: " + targetObject);
           
                //TMPro_EventManager.ON_MATERIAL_PROPERTY_CHANGED(true, targetObject as Material);
                //EditorUtility.SetDirty(targetObject);        
            }
  
            //string propertyPath = modifications[0].propertyModification.propertyPath;  
            //if (propertyPath == "m_fontAsset")
            //{
                //int currentEvent = Undo.GetCurrentGroup();
                //Undo.RecordObject(Selection.activeGameObject.renderer.sharedMaterial, "Font Asset Changed");
                //Undo.CollapseUndoOperations(currentEvent);
                //Debug.Log("Undo / Redo Event: Font Asset changed. Event ID:" + Undo.GetCurrentGroup());
            
            //}

            //Debug.Log("Undo / Redo Event Registered in Editor Panel on Target: " + modifiedProp.propertyPath + "  Undo Event ID:" + eventID + "  Stored ID:" + TMPro_EditorUtility.UndoEventID);
            //TextMeshPro_EventManager.ON_TEXTMESHPRO_PROPERTY_CHANGED(true, target as TextMeshPro);
            return modifications;
        }
        */
    }
}