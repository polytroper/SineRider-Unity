using UnityEngine;
using System.Collections;
using TMPro;

public class Benchmark01 : MonoBehaviour {

    public int BenchmarkType = 0;
    
    private TextMeshPro m_textMeshPro;
    private TextMesh m_textMesh;
     
    private string label01 = "The <#0050FF>count is:</color>{0}";
    private const string label02 = "The <color=#0050FF>count is:</color>";
   
    private string m_string;
    private int m_frame;

    IEnumerator Start () 
    {       
        
        if (BenchmarkType == 0) // TextMesh Pro Component
        {
            m_textMeshPro = gameObject.AddComponent<TextMeshPro>();
            m_textMeshPro.anchorDampening = true;
            //m_textMeshPro.font = Resources.Load("Fonts/IMPACT SDF", typeof(TextMeshProFont)) as TextMeshProFont; // Make sure the IMPACT SDF exists before calling this...           
            //m_textMeshPro.fontSharedMaterial = Resources.Load("Fonts/IMPACT SDF", typeof(Material)) as Material; // Same as above make sure this material exists.
            
            m_textMeshPro.fontSize = 48;
            m_textMeshPro.anchor = AnchorPositions.Center;

            //m_textMeshPro.outlineWidth = 0.25f;
            //m_textMeshPro.fontSharedMaterial.SetFloat("_OutlineWidth", 0.2f);
            //m_textMeshPro.fontSharedMaterial.EnableKeyword("UNDERLAY_ON");
            //m_textMeshPro.lineJustification = LineJustificationTypes.Center;
            //m_textMeshPro.enableWordWrapping = true;    
            //m_textMeshPro.lineLength = 60;          
            //m_textMeshPro.characterSpacing = 0.2f;
            //m_textMeshPro.fontColor = new Color32(255, 255, 255, 255);           
        }
        else if (BenchmarkType == 1) // TextMesh
        {
            m_textMesh = gameObject.AddComponent<TextMesh>();
            m_textMesh.font = Resources.Load("Fonts/ARIAL", typeof(Font)) as Font;
            //m_textMesh.font = Resources.Load("Fonts/IMPACT", typeof(Font)) as Font;
            m_textMesh.GetComponent<Renderer>().sharedMaterial = m_textMesh.font.material;
           
            m_textMesh.fontSize = 48;
            m_textMesh.anchor = TextAnchor.MiddleCenter;
          
            //m_textMesh.color = new Color32(255, 255, 0, 255);    
        }
        
        
        for (int i = 0; i <= 1000000; i++)
        {
            if (BenchmarkType == 0)
            {               
                m_textMeshPro.SetText(label01, i % 1000);               
                //m_textMeshPro.text = label01 + (i % 1000).ToString();             
            }
            else if (BenchmarkType == 1)
                m_textMesh.text = label02 + (i % 1000).ToString();

            yield return null;
        }
        
	}

    /*
    void Update()
    {
        if (BenchmarkType == 0)
        {
            m_textMeshPro.text = (m_frame % 1000).ToString();            
        }
        else if (BenchmarkType == 1)
        {
            m_textMesh.text = (m_frame % 1000).ToString();
        }

        m_frame += 1;
    }
    */
}

