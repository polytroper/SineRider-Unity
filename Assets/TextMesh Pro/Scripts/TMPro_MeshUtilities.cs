// Copyright (C) 2014 Stephan Bouchard - All Rights Reserved
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms


using UnityEngine;


namespace TMPro
{
    
    public class TMPro_MeshInfo
    {
        public TMPro_CharacterInfo[] characterInfo;
        public int characterCount;
        public Mesh mesh;
        public Vector3[] vertices;
        public Vector2[] uv0s;
        public Vector2[] uv2s;
        public Color32[] vertexColors;
        public Vector3[] normals;
        public Vector4[] tangents;
    }
       

    // Structure containing information about each Character & releated Mesh info for the text object.   
    public struct TMPro_CharacterInfo 
    {     
        public char Character;
        public short LineNumber;
        public short CharNumber;
        public short index;
        public short vertexIndex;
        public Vector3 BottomLeft;
        public Vector3 TopRight;
        public float TopLine;
        //public float CapLine;
        //public float CenterLine;
        public float BaseLine;
        public float BottomLine;
        public float AspectRatio;
        public float Scale;
        public Color32 color;
        public FontStyles style;
        //public bool isUnderlined;
        public bool isVisible;
    }


    public struct Mesh_Extents
    {
        public Vector2 Min;
        public Vector2 Max;
        public short NumberOfChars;
        public short NumberOfSpaces;

        public Mesh_Extents(Vector2 min, Vector2 max)
        {
            Min = min;
            Max = max;
            NumberOfChars = 0;
            NumberOfSpaces = 0;
        }

        public override string ToString()
        {
            //string s = "Min X: (" + Min.x + ")  Y: (" + Min.y + ")   Max X: (" + Max.x + ")  Y: (" + Max.y + ")";
            string s = "Center: (" + ")" + "  Extents: (" + ((Max.x - Min.x) / 2).ToString("f2") + "," + ((Max.y - Min.y) / 2).ToString("f2") + ").";
            return s;
        }
    }


    // Structure used for Word Wrapping which tracks the state of execution when the last space or carriage return character was encountered. 
    public struct WordWrapState
    {
        public int previous_WordBreak; 
        public int total_CharacterCount;
        public int visible_CharacterCount;
        public FontStyles fontStyle;
        public float fontScale;
        public float xAdvance; 
        public float baselineOffset;
        
        public Color32 vertexColor;
        public Mesh_Extents meshExtents;
        public Mesh_Extents lineExtents;    
    }

}
