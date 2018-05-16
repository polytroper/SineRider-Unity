// Copyright (C) 2014 Stephan Bouchard - All Rights Reserved
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms


using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;



namespace TMPro
{
   
    public enum AnchorPositions { TopLeft, Top, TopRight, Left, Center, Right, BottomLeft, Bottom, BottomRight, BaseLine };
    public enum AlignmentTypes { Left, Center, Right, Justified };
    //public enum VerticalCharacterMappingOptions { Character = 0, Line = 1, Paragraph = 2, MatchHorizontal = 3 };
    public enum TextureMappingOptions { Character = 0, Line = 1, Paragraph = 2, MatchAspect = 3 };
    public enum FontStyles { Normal = 0, Bold = 1, Italic = 2, Underline = 4, BoldItalic = Bold + Italic, BoldUnderline = Bold + Underline, BoldItalicUnderline = BoldUnderline + Italic, ItalicUnderline = Italic + Underline };


    [ExecuteInEditMode]
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshFilter))]
    [AddComponentMenu("Mesh/TextMesh Pro")]
    public partial class TextMeshPro : MonoBehaviour 
    {
        // Public Properties & Serializable Properties  
        
        /// <summary>
        /// A string containing the text to be displayed.
        /// </summary>
        public string text
        {
            get { return m_text; }
            set { if (m_text != value) { m_inputSource = TextInputSources.Text; havePropertiesChanged = true; isAffectingWordWrapping = true; isInputParsingRequired = true; m_text = value; /* ScheduleUpdate(); */ } }
        }


        /// <summary>
        /// The TextMeshPro font asset to be assigned to this text object.
        /// </summary>
        public TextMeshProFont font
        {
            get { return m_fontAsset; }
            set { m_fontAsset = value; LoadFontAsset(); havePropertiesChanged = true; /* hasFontAssetChanged = true;*/ isAffectingWordWrapping = true; /* ScheduleUpdate(); */ }
        }


        /// <summary>
        /// The material to be assigned to this text object. An instance of the material will be assigned to the object's renderer.
        /// </summary>
        public Material fontMaterial
        {
            get { return GetComponent<Renderer>().material; }
            set { if (m_fontMaterial != value) { SetFontMaterial(value); havePropertiesChanged = true; m_fontMaterial = value; /* ScheduleUpdate(); */ } }
        }


        /// <summary>
        /// The material to be assigned to this text object.
        /// </summary>
        public Material fontSharedMaterial
        {
            get { return GetComponent<Renderer>().sharedMaterial; }
            set { if (m_sharedMaterial != value) { SetSharedFontMaterial(value); havePropertiesChanged = true; m_sharedMaterial = value; /* ScheduleUpdate(); */ } }
        }


        /// <summary>
        /// Sets the RenderQueue along with Ztest to force the text to be drawn last and on top of scene elements.
        /// </summary>
        public bool isOverlay
        {
            get { return m_isOverlay; }
            set { if (m_isOverlay != value) { m_isOverlay = value; SetShaderType(); havePropertiesChanged = true; /* ScheduleUpdate(); */ } }
        }


        /// <summary>
        /// This is the default vertex color assigned to each vertices. Color tags will override vertex colors unless the overrideColorTags is set.
        /// </summary>
        public Color32 color
        {
            get { return m_fontColor; }
            set { if (m_fontColor.Compare(value) == false) { havePropertiesChanged = true; m_fontColor = value; /* ScheduleUpdate(); */ } }
        }


        /// <summary>
        /// Sets the color of the _FaceColor property of the assigned material. Changing face color will result in an instance of the material.
        /// </summary>
        public Color32 faceColor
        {
            get { return m_faceColor; }
            set { if (m_faceColor.Compare(value) == false) { SetFaceColor(value); havePropertiesChanged = true; m_faceColor = value; /* ScheduleUpdate(); */ } }
        }


        /// <summary>
        /// Sets the color of the _OutlineColor property of the assigned material. Changing outline color will result in an instance of the material.
        /// </summary>
        public Color32 outlineColor
        {
            get { return m_outlineColor; }
            set { if (m_outlineColor.Compare(value) == false) { SetOutlineColor(value); havePropertiesChanged = true; m_outlineColor = value; /* ScheduleUpdate(); */ } }
        }


        /// <summary>
        /// Sets the thickness of the outline of the font. Setting this value will result in an instance of the material.
        /// </summary>
        public float outlineWidth
        {
            get { return m_outlineWidth; }
            set { SetOutlineThickness(value); havePropertiesChanged = true; checkPaddingRequired = true; m_outlineWidth = value; /* ScheduleUpdate(); */ }
        }


        /// <summary>
        /// The size of the font.
        /// </summary>
        public float fontSize
        {
            get { return m_fontSize; }
            set { if (m_fontSize != value) { havePropertiesChanged = true; isAffectingWordWrapping = true; /* hasFontScaleChanged = true; */ m_fontSize = value; /* ScheduleUpdate(); */ } }
        }


        /// <summary>
        /// The amount of additional spacing between characters.
        /// </summary>
        public float characterSpacing
        {
            get { return m_characterSpacing; }
            set { if (m_characterSpacing != value) { havePropertiesChanged = true; isAffectingWordWrapping = true; m_characterSpacing = value; /* ScheduleUpdate(); */ } }
        }


        /// <summary>
        /// Determines where word wrap will occur.
        /// </summary>
        public float lineLength
        {
            get { return m_lineLength; }
            set { if (m_lineLength != value) { havePropertiesChanged = true; isAffectingWordWrapping = true; m_lineLength = value; /* ScheduleUpdate(); */ } }
        }


        /// <summary>
        /// Contains the bounds of the text object.
        /// </summary>
        public Bounds bounds
        {
            get { if (m_mesh != null) return m_mesh.bounds; return new Bounds(); }
            //set { if (_meshExtents != value) havePropertiesChanged = true; _meshExtents = value; }
        }

        /// <summary>
        /// The amount of additional spacing to add between each lines of text.
        /// </summary>
        public float lineSpacing
        {
            get { return m_lineSpacing; }
            set { if (m_lineSpacing != value) { havePropertiesChanged = true; m_lineSpacing = value; /* ScheduleUpdate(); */ } }
        }


        /// <summary>
        /// Determines the anchor position of the text object.  
        /// </summary>
        public AnchorPositions anchor
        {
            get { return m_anchor; }
            set { if (m_anchor != value) { havePropertiesChanged = true; m_anchor = value; /* ScheduleUpdate(); */ } }
        }


        /// <summary>
        /// Line justification options.
        /// </summary>
        public AlignmentTypes alignment
        {
            get { return m_lineJustification; }
            set { if (m_lineJustification != value) { havePropertiesChanged = true; m_lineJustification = value; /* ScheduleUpdate(); */ } }
        }


        /// <summary>
        /// Determines if kerning is enabled or disabled.
        /// </summary>
        public bool enableKerning
        {
            get { return m_enableKerning; }
            set { if (m_enableKerning != value) { havePropertiesChanged = true; isAffectingWordWrapping = true; m_enableKerning = value; /* ScheduleUpdate(); */ } }
        }


        /// <summary>
        /// Anchor dampening prevents the anchor position from being adjusted unless the positional change exceeds about 40% of the width of the underline character. This essentially stabilizes the anchor position.
        /// </summary>
        public bool anchorDampening
        {
            get { return m_anchorDampening; }
            set { if (m_anchorDampening != value) { havePropertiesChanged = true; m_anchorDampening = value; /* ScheduleUpdate(); */ } }
        }


        /// <summary>
        /// This overrides the color tags forcing the vertex colors to be the default font color.
        /// </summary>
        public bool overrideColorTags
        {
            get { return m_overrideHtmlColors; }
            set { if (m_overrideHtmlColors != value) { havePropertiesChanged = true; m_overrideHtmlColors = value; /* ScheduleUpdate(); */ } }
        }


        /// <summary>
        /// Adds extra padding around each character. This may be necessary when the displayed text is very small to prevent clipping.
        /// </summary>
        public bool extraPadding
        {
            get { return m_enableExtraPadding; }
            set { if (m_enableExtraPadding != value) { havePropertiesChanged = true; checkPaddingRequired = true; isAffectingWordWrapping = true; m_enableExtraPadding = value; /* ScheduleUpdate(); */ } }
        }


        /// <summary>
        /// Controls whether or not word wrapping is applied. When disabled, the text will be displayed on a single line.
        /// </summary>
        public bool enableWordWrapping
        {
            get { return m_enableWordWrapping; }
            set { if (m_enableWordWrapping != value) { havePropertiesChanged = true; isAffectingWordWrapping = true; isInputParsingRequired = true; m_enableWordWrapping = value; /* ScheduleUpdate(); */ } }
        }


        /// <summary>
        /// Controls how the face and outline textures will be applied to the text object.
        /// </summary>
        public TextureMappingOptions horizontalMapping
        {
            get { return m_horizontalMapping; }
            set { if (m_horizontalMapping != value) { havePropertiesChanged = true; m_horizontalMapping = value; /* ScheduleUpdate(); */ } }
        }


        /// <summary>
        /// Controls how the face and outline textures will be applied to the text object.
        /// </summary>
        public TextureMappingOptions verticalMapping
        {
            get { return m_verticalMapping; }
            set { if (m_verticalMapping != value) { havePropertiesChanged = true; m_verticalMapping = value; /* ScheduleUpdate(); */ } }
        }

        /// <summary>
        /// Forces objects that are not visible to get refreshed.
        /// </summary>
        public bool ignoreVisibility
        {
            get { return m_ignoreCulling; }
            set { if (m_ignoreCulling != value) { havePropertiesChanged = true; m_ignoreCulling = value; /* ScheduleUpdate(); */ } }
        }


        /// <summary>
        /// Sets Perspective Correction to Zero for Orthographic Camera mode & 0.875f for Perspective Camera mode.
        /// </summary>
        public bool isOrthographic
        {
            get { return m_isOrthographic; }
            set { havePropertiesChanged = true; m_isOrthographic = value; SetPerspectiveCorrection(); /* ScheduleUpdate(); */ }
        }



        /// <summary>
        /// Sets the Renderer's sorting Layer ID
        /// </summary>
        public int sortingLayerID
        {
            get { return m_sortingLayerID; }
            set { m_sortingLayerID = value; m_renderer.sortingLayerID = value; }
        }


        /// <summary>
        /// Sets the Renderer's sorting order within the assigned layer.
        /// </summary>
        public int sortingOrder
        {
            get { return m_sortingOrder; }
            set { m_sortingOrder = value; m_renderer.sortingOrder = value; }
        }


        public bool isAdvancedLayoutComponentPresent
        {
            //get { return m_isAdvanceLayoutComponentPresent; }
            set
            {
                if (m_isAdvanceLayoutComponentPresent != value)
                {
                    m_advancedLayoutComponent = value == true ? GetComponent<TMPro_AdvancedLayout>() : null;
                    havePropertiesChanged = true;
                    m_isAdvanceLayoutComponentPresent = value;
                }
            }
        }


        public TMPro_MeshInfo MeshInfo
        {
            get { return m_meshInfo; }
        }


        /// <summary>
        /// Function to be used to force recomputing of character padding when Shader / Material properties have been changed via script.
        /// </summary>
        public void UpdateMeshPadding()
        {
            m_padding = ShaderUtilities.GetPadding(m_renderer.sharedMaterials, m_enableExtraPadding, m_isUsingBold);
            havePropertiesChanged = true;
            /* ScheduleUpdate(); */
        }


        /// <summary>
        /// Function to force regeneration of the mesh before its normal process time. This is useful when changes to the text object properties need to be applied immediately.
        /// </summary>
        public void ForceMeshUpdate()
        {      
            OnWillRenderObject();
        }


        public void UpdateFontAsset()
        {           
            LoadFontAsset();
        }

        /// <summary>
        /// <para>Formatted string containing a pattern and a value representing the text to be rendered.</para>
        /// <para>ex. TextMeshPro.SetText ("Number is {0:1}.", 5.56f);</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="text">String containing the pattern."</param>
        /// <param name="arg0">Value can be an integer or float.</param>
        public void SetText<T>(string text, T arg0) where T : IConvertible
        {
            SetText(text, arg0, 255, 255);
        }

        /// <summary>
        /// <para>Formatted string containing a pattern and a value representing the text to be rendered.</para>
        /// <para>ex. TextMeshPro.SetText ("First number is {0} and second is {1:2}.", 10, 5.756f);</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="text">String containing the pattern."</param>
        /// <param name="arg0">Value can be an integer or float.</param>
        /// <param name="arg1">Value can be an integer or float.</param>
        public void SetText<T, U>(string text, T arg0, U arg1)
            where T : IConvertible
            where U : IConvertible
        {
            SetText(text, arg0, arg1, 255);
        }

        /// <summary>
        /// <para>Formatted string containing a pattern and a value representing the text to be rendered.</para>
        /// <para>ex. TextMeshPro.SetText ("A = {0}, B = {1} and C = {2}.", 2, 5, 7);</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="text">String containing the pattern."</param>
        /// <param name="arg0">Value can be an integer or float.</param>
        /// <param name="arg1">Value can be an integer or float.</param>
        /// <param name="arg2">Value can be an integer or float.</param>
        public void SetText<T, U, V>(string text, T arg0, U arg1, V arg2)
            where T : IConvertible
            where U : IConvertible
            where V : IConvertible
        {
            // Early out if nothing has been changed from previous invocation.
            if (text == old_text && arg0.ToSingle(NumberFormat) == old_arg0 && arg1.ToSingle(NumberFormat) == old_arg1 && arg2.ToSingle(NumberFormat) == old_arg2)
            {
                return;
            }

            old_text = text;
            old_arg1 = 255;
            old_arg2 = 255;

            int decimalPrecision = 2;
            int index = 0;

            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];

                if (c == 123) // '{'
                {
                    // Check if user is requesting some decimal precision. Format is {0:2}
                    if (text[i + 2] == 58) // ':'
                    {
                        decimalPrecision = text[i + 3] - 48;
                    }

                    switch (text[i + 1] - 48)
                    {
                        case 0: // 1st Arg                        
                            switch (arg0.GetTypeCode())
                            {
                                case TypeCode.Int32:
                                    int someInt = arg0.ToInt32(NumberFormat);
                                    old_arg0 = someInt;
                                    AddIntToCharArray(someInt, ref index, decimalPrecision);
                                    break;
                                case TypeCode.Single:
                                    float someFloat = arg0.ToSingle(NumberFormat);
                                    old_arg0 = someFloat;
                                    AddFloatToCharArray(someFloat, ref index, decimalPrecision);
                                    break;
                            }
                            break;
                        case 1: // 2nd Arg
                            switch (arg1.GetTypeCode())
                            {
                                case TypeCode.Int32:
                                    int someInt = arg1.ToInt32(NumberFormat);
                                    old_arg1 = someInt;
                                    AddIntToCharArray(someInt, ref index, decimalPrecision);
                                    break;
                                case TypeCode.Single:
                                    float someFloat = arg1.ToSingle(NumberFormat);
                                    old_arg1 = someFloat;
                                    AddFloatToCharArray(someFloat, ref index, decimalPrecision);
                                    break;
                            }
                            break;
                        case 2: // 3rd Arg
                            switch (arg2.GetTypeCode())
                            {
                                case TypeCode.Int32:
                                    int someInt = arg2.ToInt32(NumberFormat);
                                    old_arg2 = someInt;
                                    AddIntToCharArray(someInt, ref index, decimalPrecision);
                                    break;
                                case TypeCode.Single:
                                    float someFloat = arg2.ToSingle(NumberFormat);
                                    old_arg2 = someFloat;
                                    AddFloatToCharArray(someFloat, ref index, decimalPrecision);
                                    break;
                            }
                            break;
                    }

                    if (text[i + 2] == 58)
                        i += 4;
                    else
                        i += 2;

                    continue;
                }
                m_input_CharArray[index] = c;
                index += 1;
            }

            m_input_CharArray[index] = (char)0;
            m_charArray_Length = index; // Set the length to where this '0' termination is.

#if UNITY_EDITOR
            // Create new string to be displayed in the Input Text Box of the Editor Panel.
            m_text = new string(m_input_CharArray, 0, index);
#endif

            m_inputSource = TextInputSources.SetText;
            isInputParsingRequired = true;
            havePropertiesChanged = true;
            /* ScheduleUpdate(); */
        }




        /// <summary>
        /// Character array containing the text to be displayed.
        /// </summary>
        /// <param name="charArray"></param>
        public void SetCharArray(char[] charArray)
        {
            if (charArray == null || charArray.Length == 0)
                return;

            // Check to make sure chars_buffer is large enough to hold the content of the string.
            if (m_char_buffer.Length <= charArray.Length)
            {
                int newSize = Mathf.NextPowerOfTwo(charArray.Length + 1);
                m_char_buffer = new char[newSize];
            }

            int index = 0;

            for (int i = 0; i < charArray.Length; i++)
            {
                if (charArray[i] == 92 && i < charArray.Length - 1)
                {
                    switch ((int)charArray[i + 1])
                    {
                        case 116: // \t Tab
                            m_char_buffer[index] = (char)9;
                            i += 1;
                            index += 1;
                            continue;
                        case 110: // \n LineFeed
                            m_char_buffer[index] = (char)10;
                            i += 1;
                            index += 1;
                            continue;
                    }
                }

                m_char_buffer[index] = charArray[i];
                index += 1;
            }
            m_char_buffer[index] = (char)0;

            m_inputSource = TextInputSources.SetCharArray;
            havePropertiesChanged = true;
            isInputParsingRequired = true;
        }

    }
}