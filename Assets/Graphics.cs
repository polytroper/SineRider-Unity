using UnityEngine;
using System.Collections;

public class Graphics : MonoBehaviour {
	public GUISkin skin;

	public Texture2D cartesianButtonIcon;
	public Texture2D polarButtonIcon;
	public Texture2D invertedButtonIcon;
	public Texture2D addButtonIcon;
	public Texture2D deleteButtonIcon;
	public Texture2D staticButtonIcon;
	public Texture2D dynamicButtonIcon;
	public Texture2D timerLabelIcon;
	public Texture2D orderLabelIcon;
	public Texture2D linkButtonIcon;
	public Texture2D recordButtonIcon;
	public Texture2D keypadButtonIcon;
	public Texture2D goButtonIcon;
	public Texture2D resetButtonIcon;
	public Texture2D stopButtonIcon;
	public Texture2D puzzleIcon;
	public Texture2D folderIcon;
	public Texture2D nextIcon;
	public Texture2D previousIcon;
	public Texture2D muteIcon;
	public Texture2D unmuteIcon;
	public Sprite staticObjectiveSprite;
	public Sprite dynamicObjectiveSprite;
	public Sprite staticHighlighterSprite;
	public Sprite dynamicHighlighterSprite;

	[HideInInspector]
	public int width;
	[HideInInspector]
	public int height;
	[HideInInspector]
	public float aspect;
	[HideInInspector]
	public bool portrait;
	[HideInInspector]
	public float a;

	public static GUIStyle inputCompactStyle;
	public static GUIStyle inputCompactDisabledStyle;
	public static GUIStyle inputStyle;
	public static GUIStyle inputInfoStyle;
	public static GUIStyle inputErrorStyle;
	public static GUIStyle inputDisabledStyle;
	public static GUIStyle variableStyle;
	public static GUIStyle infoStyle;
	public static GUIStyle blackStyle;
	public static GUIStyle guiStyle;
	public static GUIStyle listStyle;
	public static GUIStyle listGreenStyle;
	public static GUIStyle listBlueStyle;
	public static GUIStyle buttonStyle;
	public static GUIStyle buttonGreenStyle;
	public static GUIStyle buttonWorldGreenStyle;
	public static GUIStyle buttonRedStyle;
	public static GUIStyle buttonBlueStyle;
	public static GUIStyle buttonDisabledStyle;
	public static GUIStyle toggleStyle;
	public static GUIStyle toggleDisabledStyle;
	public static GUIStyle labelStyle;
	public static GUIStyle labelDisabledStyle;
	public static GUIStyle boxStyle;
	public static GUIStyle progressLabelStyle;
	public static GUIStyle progressBarStyle;
	public static GUIStyle keypadStyle256;
	public static GUIStyle keypadStyle192;
	public static GUIStyle keypadStyle128;
	public static GUIStyle keypadStyle64;
	public static GUIStyle keypadStyle0;

	public static Graphics instance;

	void Start(){
		int i;
		instance = this;

		Texture2D tex256 = new Texture2D(3, 3);
		Color[] colorsWhite = {Color.black, Color.black, Color.black,
				  			   Color.black, Color.white, Color.black,
							   Color.black, Color.black, Color.black};
		tex256.SetPixels(colorsWhite);
		tex256.filterMode = FilterMode.Point;
		tex256.Apply();

		Texture2D tex192 = new Texture2D(3, 3);
		Color[] colors192 = {Color.black, Color.black, Color.black,
				  			 Color.black, new Color(0.75f, 0.75f, 0.75f), Color.black,
				  			 Color.black, Color.black, Color.black};
		tex192.SetPixels(colors192);
		tex192.filterMode = FilterMode.Point;
		tex192.Apply();

		Texture2D tex128 = new Texture2D(3, 3);
		Color[] colors128 = {Color.black, Color.black, Color.black,
				  			 Color.black, new Color(0.5f, 0.5f, 0.5f), Color.black,
				  			 Color.black, Color.black, Color.black};
		tex128.SetPixels(colors128);
		tex128.filterMode = FilterMode.Point;
		tex128.Apply();
		
		Texture2D tex96 = new Texture2D(3, 3);
		Color[] colors96 = {Color.black, Color.black, Color.black,
							Color.black, new Color(0.375f, 0.375f, 0.375f), Color.black,
							Color.black, Color.black, Color.black};
		tex96.SetPixels(colors96);
		tex96.filterMode = FilterMode.Point;
		tex96.Apply();

		Texture2D tex64 = new Texture2D(3, 3);
		Color[] colors64 = {Color.black, Color.black, Color.black,
				  			Color.black, new Color(0.25f, 0.25f, 0.25f), Color.black,
				  			Color.black, Color.black, Color.black};
		tex64.SetPixels(colors64);
		tex64.filterMode = FilterMode.Point;
		tex64.Apply();

		Texture2D tex32 = new Texture2D(3, 3);
		Color[] colors32 = {Color.black, Color.black, Color.black,
				  			Color.black, new Color(0.125f, 0.125f, 0.125f), Color.black,
				  			Color.black, Color.black, Color.black};
		tex32.SetPixels(colors32);
		tex32.filterMode = FilterMode.Point;
		tex32.Apply();

		Texture2D tex0 = new Texture2D(3, 3);
		Color[] colors0 = {Color.black, Color.black, Color.black,
				  		   Color.black, Color.black, Color.black,
				  		   Color.black, Color.black, Color.black};
		tex0.SetPixels(colors0);
		tex0.filterMode = FilterMode.Point;
		tex0.Apply();
		
		Texture2D texError = new Texture2D(3, 3);
		Color[] colorsError = {Color.black, Color.black, Color.black,
							   Color.black, new Color(1, 0.5f, 0.5f), Color.black,
							   Color.black, Color.black, Color.black};
		texError.SetPixels(colorsError);
		texError.filterMode = FilterMode.Point;
		texError.Apply();
		
		Texture2D texInfo = new Texture2D(3, 3);
		Color[] colorsInfo = {Color.black, Color.black, Color.black,
			Color.black, new Color(0.5f, 1, 0.5f), Color.black,
			Color.black, Color.black, Color.black};
		texInfo.SetPixels(colorsInfo);
		texInfo.filterMode = FilterMode.Point;
		texInfo.Apply();

		Texture2D texGreen = new Texture2D(3, 3);
		Color[] colorsGreen = {Color.black, Color.black, Color.black,
							   Color.black, new Color(0.5f, 1, 0.5f), Color.black,
							   Color.black, Color.black, Color.black};
		texGreen.SetPixels(colorsGreen);
		texGreen.filterMode = FilterMode.Point;
		texGreen.Apply();
		
		Texture2D texRed = new Texture2D(3, 3);
		Color[] colorsRed = {Color.black, Color.black, Color.black,
			Color.black, new Color(1, 0.5f, 0.5f), Color.black,
			Color.black, Color.black, Color.black};
		texRed.SetPixels(colorsRed);
		texRed.filterMode = FilterMode.Point;
		texRed.Apply();
		
		Texture2D texBlue = new Texture2D(3, 3);
		Color[] colorsBlue = {Color.black, Color.black, Color.black,
			Color.black, new Color(0.5f, 0.5f, 1), Color.black,
			Color.black, Color.black, Color.black};
		texBlue.SetPixels(colorsBlue);
		texBlue.filterMode = FilterMode.Point;
		texBlue.Apply();

		//Alpha Textures

		Texture2D tex256_192 = new Texture2D(3, 3);
		Color[] colors256_192 = {Color.black, Color.black, Color.black,
				  			   Color.black, new Color(1, 1, 1, 0.75f), Color.black,
							   Color.black, Color.black, Color.black};
		tex256_192.SetPixels(colors256_192);
		tex256_192.filterMode = FilterMode.Point;
		tex256_192.Apply();

		Texture2D tex192_192 = new Texture2D(3, 3);
		Color[] colors192_192 = {Color.black, Color.black, Color.black,
				  			 Color.black, new Color(0.75f, 0.75f, 0.75f, 0.75f), Color.black,
				  			 Color.black, Color.black, Color.black};
		tex192_192.SetPixels(colors192_192);
		tex192_192.filterMode = FilterMode.Point;
		tex192_192.Apply();

		Texture2D tex128_192 = new Texture2D(3, 3);
		Color[] colors128_192 = {Color.black, Color.black, Color.black,
				  			 Color.black, new Color(0.5f, 0.5f, 0.5f, 0.75f), Color.black,
				  			 Color.black, Color.black, Color.black};
		tex128_192.SetPixels(colors128_192);
		tex128_192.filterMode = FilterMode.Point;
		tex128_192.Apply();

		Texture2D tex64_192 = new Texture2D(3, 3);
		Color[] colors64_192 = {Color.black, Color.black, Color.black,
				  			Color.black, new Color(0.25f, 0.25f, 0.25f, 0.75f), Color.black,
				  			Color.black, Color.black, Color.black};
		tex64_192.SetPixels(colors64_192);
		tex64_192.filterMode = FilterMode.Point;
		tex64_192.Apply();

		Texture2D tex32_192 = new Texture2D(3, 3);
		Color[] colors32_192 = {Color.black, Color.black, Color.black,
				  			Color.black, new Color(0.125f, 0.125f, 0.125f, 0.75f), Color.black,
				  			Color.black, Color.black, Color.black};
		tex32_192.SetPixels(colors32_192);
		tex32_192.filterMode = FilterMode.Point;
		tex32_192.Apply();
		
		Texture2D tex0_192 = new Texture2D(3, 3);
		Color[] colors0_192 = {Color.black, Color.black, Color.black,
							   Color.black, new Color(0, 0, 0, 0.75f), Color.black,
							   Color.black, Color.black, Color.black};
		tex0_192.SetPixels(colors0_192);
		tex0_192.filterMode = FilterMode.Point;
		tex0_192.Apply();

		guiStyle = new GUIStyle();

		listStyle = new GUIStyle();
		listStyle.normal.textColor = Color.white;
		listStyle.border = new RectOffset(1, 1, 1, 1);
		listStyle.padding = new RectOffset(4, 4, 0, 0);
		listStyle.alignment = TextAnchor.MiddleLeft;
		listStyle.normal.background = tex128;
		listStyle.hover.background = tex256;
		listStyle.onHover.background = tex256;
		//listStyle.padding.left = listStyle.padding.right = listStyle.padding.top = listStyle.padding.bottom = 4;

		listGreenStyle = new GUIStyle();
		listGreenStyle.normal.textColor = Color.black;
		listGreenStyle.border = new RectOffset(1, 1, 1, 1);
		listGreenStyle.padding = new RectOffset(4, 4, 0, 0);
		listGreenStyle.alignment = TextAnchor.MiddleLeft;
		listGreenStyle.normal.background = texGreen;
		listGreenStyle.hover.background = tex256;
		listGreenStyle.onHover.background = tex256;

		listBlueStyle = new GUIStyle();
		listBlueStyle.normal.textColor = Color.black;
		listBlueStyle.border = new RectOffset(1, 1, 1, 1);
		listBlueStyle.padding = new RectOffset(4, 4, 0, 0);
		listBlueStyle.alignment = TextAnchor.MiddleLeft;
		listBlueStyle.normal.background = texBlue;
		listBlueStyle.hover.background = tex256;
		listBlueStyle.onHover.background = tex256;

		inputCompactStyle = new GUIStyle();
		inputCompactStyle.clipping = TextClipping.Clip;
		inputCompactStyle.normal.textColor = Color.black;
		inputCompactStyle.alignment = TextAnchor.MiddleCenter;
		inputCompactStyle.border = new RectOffset(1, 1, 1, 1);
		inputCompactStyle.padding = new RectOffset(2, 2, 0, 0);
		//inputStyle.onFocused.background = tex;
		//inputStyle.focused.background = tex;
		inputCompactStyle.normal.background = tex256;
		
		inputCompactDisabledStyle = new GUIStyle();
		inputCompactDisabledStyle.clipping = TextClipping.Clip;
		inputCompactDisabledStyle.normal.textColor = Color.white;
		inputCompactDisabledStyle.alignment = TextAnchor.MiddleCenter;
		inputCompactDisabledStyle.border = new RectOffset(1, 1, 1, 1);
		inputCompactDisabledStyle.padding = new RectOffset(2, 2, 0, 0);
		//inputStyle.onFocused.background = tex;
		//inputStyle.focused.background = tex;
		inputCompactDisabledStyle.normal.background = tex64;

		inputStyle = new GUIStyle();
		inputStyle.clipping = TextClipping.Clip;
		inputStyle.normal.textColor = Color.black;
		inputStyle.alignment = TextAnchor.MiddleLeft;
		inputStyle.border = new RectOffset(1, 1, 1, 1);
		inputStyle.padding = new RectOffset(2, 2, 0, 0);
		//inputStyle.onFocused.background = tex;
		//inputStyle.focused.background = tex;
		inputStyle.normal.background = tex256;
		
		inputDisabledStyle = new GUIStyle();
		inputDisabledStyle.clipping = TextClipping.Clip;
		inputDisabledStyle.normal.textColor = Color.white;
		inputDisabledStyle.alignment = TextAnchor.MiddleLeft;
		inputDisabledStyle.border = new RectOffset(1, 1, 1, 1);
		inputDisabledStyle.padding = new RectOffset(2, 2, 0, 0);
		//inputStyle.onFocused.background = tex;
		//inputStyle.focused.background = tex;
		inputDisabledStyle.normal.background = tex64;
		
		inputInfoStyle = new GUIStyle();
		inputInfoStyle.clipping = TextClipping.Clip;
		inputInfoStyle.normal.textColor = Color.black;
		inputInfoStyle.alignment = TextAnchor.MiddleCenter;
		inputInfoStyle.border = new RectOffset(1, 1, 1, 1);
		inputInfoStyle.padding = new RectOffset(2, 2, 0, 0);
		//inputStyle.onFocused.background = tex;
		//inputStyle.focused.background = tex;
		inputInfoStyle.normal.background = texInfo;
		
		inputErrorStyle = new GUIStyle();
		inputErrorStyle.clipping = TextClipping.Clip;
		inputErrorStyle.normal.textColor = Color.black;
		inputErrorStyle.alignment = TextAnchor.MiddleCenter;
		inputErrorStyle.border = new RectOffset(1, 1, 1, 1);
		inputErrorStyle.padding = new RectOffset(2, 2, 0, 0);
		//inputStyle.onFocused.background = tex;
		//inputStyle.focused.background = tex;
		inputErrorStyle.normal.background = texError;

		//tex.SetPixel(1, 1, Color.white*3/4);
		//tex.Apply();
		variableStyle = new GUIStyle();
		variableStyle.normal.textColor = Color.white;
		variableStyle.alignment = TextAnchor.MiddleRight;
		variableStyle.border = new RectOffset(1, 1, 1, 1);
		variableStyle.padding = new RectOffset(2, 2, 0, 0);
		variableStyle.normal.background = tex32;
		
		infoStyle = new GUIStyle();
		infoStyle.normal.textColor = Color.white;
		infoStyle.alignment = TextAnchor.MiddleCenter;
		infoStyle.border = new RectOffset(1, 1, 1, 1);
		infoStyle.padding = new RectOffset(2, 2, 0, 0);
		infoStyle.normal.background = tex32;
		
		blackStyle = new GUIStyle();
		blackStyle.normal.textColor = Color.white;
		blackStyle.alignment = TextAnchor.MiddleCenter;
		blackStyle.border = new RectOffset(1, 1, 1, 1);
		blackStyle.padding = new RectOffset(2, 2, 0, 0);
		blackStyle.normal.background = tex0;
		
		buttonStyle = new GUIStyle();
		buttonStyle.alignment = TextAnchor.MiddleCenter;
		buttonStyle.border = new RectOffset(1, 1, 1, 1);
		buttonStyle.padding = new RectOffset(2, 2, 2, 2);
		//inputStyle.onFocused.background = tex;
		//inputStyle.focused.background = tex;
		buttonStyle.normal.textColor = Color.white;
		buttonStyle.active.textColor = Color.white;
		buttonStyle.hover.textColor = Color.black;
		buttonStyle.normal.background = tex128;
		buttonStyle.active.background = tex64;
		buttonStyle.hover.background = tex256;
		
		buttonGreenStyle = new GUIStyle();
		buttonGreenStyle.alignment = TextAnchor.MiddleCenter;
		buttonGreenStyle.border = new RectOffset(1, 1, 1, 1);
		buttonGreenStyle.padding = new RectOffset(2, 2, 2, 2);
		//inputStyle.onFocused.background = tex;
		//inputStyle.focused.background = tex;
		buttonGreenStyle.normal.textColor = Color.black;
		buttonGreenStyle.active.textColor = Color.white;
		buttonGreenStyle.hover.textColor = Color.black;
		buttonGreenStyle.normal.background = texGreen;
		buttonGreenStyle.active.background = tex64;
		buttonGreenStyle.hover.background = tex256;
		
		buttonBlueStyle = new GUIStyle();
		buttonBlueStyle.alignment = TextAnchor.MiddleCenter;
		buttonBlueStyle.border = new RectOffset(1, 1, 1, 1);
		buttonBlueStyle.padding = new RectOffset(2, 2, 2, 2);
		//inputStyle.onFocused.background = tex;
		//inputStyle.focused.background = tex;
		buttonBlueStyle.normal.textColor = Color.black;
		buttonBlueStyle.active.textColor = Color.white;
		buttonBlueStyle.hover.textColor = Color.black;
		buttonBlueStyle.normal.background = texBlue;
		buttonBlueStyle.active.background = tex64;
		buttonBlueStyle.hover.background = tex256;
		
		buttonRedStyle = new GUIStyle();
		buttonRedStyle.alignment = TextAnchor.MiddleCenter;
		buttonRedStyle.border = new RectOffset(1, 1, 1, 1);
		buttonRedStyle.padding = new RectOffset(2, 2, 2, 2);
		//inputStyle.onFocused.background = tex;
		//inputStyle.focused.background = tex;
		buttonRedStyle.normal.textColor = Color.black;
		buttonRedStyle.active.textColor = Color.white;
		buttonRedStyle.hover.textColor = Color.black;
		buttonRedStyle.normal.background = texRed;
		buttonRedStyle.active.background = tex64;
		buttonRedStyle.hover.background = tex256;

		buttonDisabledStyle = new GUIStyle();
		buttonDisabledStyle.alignment = TextAnchor.MiddleCenter;
		buttonDisabledStyle.border = new RectOffset(1, 1, 1, 1);
		buttonDisabledStyle.padding = new RectOffset(2, 2, 0, 0);
		//inputStyle.onFocused.background = tex;
		//inputStyle.focused.background = tex;
		buttonDisabledStyle.normal.textColor = Color.white;
		buttonDisabledStyle.normal.background = tex64;
		
		toggleStyle = new GUIStyle();
		toggleStyle.alignment = TextAnchor.MiddleCenter;
		toggleStyle.border = new RectOffset(1, 1, 1, 1);
		toggleStyle.padding = new RectOffset(2, 2, 2, 2);
		//inputStyle.onFocused.background = tex;
		//inputStyle.focused.background = tex;
		toggleStyle.normal.textColor = Color.black;
		toggleStyle.active.textColor = Color.black;
		toggleStyle.hover.textColor = Color.black;
		toggleStyle.normal.background = texBlue;
		toggleStyle.active.background = tex192;
		toggleStyle.onActive.background = tex192;
		toggleStyle.onNormal.background = tex192;
		toggleStyle.hover.background = tex256;
		toggleStyle.onHover.background = tex256;
		
		toggleDisabledStyle = new GUIStyle();
		toggleDisabledStyle.alignment = TextAnchor.MiddleCenter;
		toggleDisabledStyle.border = new RectOffset(1, 1, 1, 1);
		toggleDisabledStyle.padding = new RectOffset(2, 2, 2, 2);
		//inputStyle.onFocused.background = tex;
		//inputStyle.focused.background = tex;
		toggleDisabledStyle.normal.textColor = Color.white;
		toggleDisabledStyle.normal.background = tex64;
		
		labelStyle = new GUIStyle();
		labelStyle.alignment = TextAnchor.MiddleCenter;
		labelStyle.border = new RectOffset(1, 1, 1, 1);
		labelStyle.padding = new RectOffset(2, 2, 2, 2);
		//inputStyle.onFocused.background = tex;
		//inputStyle.focused.background = tex;
		labelStyle.normal.textColor = Color.white;
		labelStyle.normal.background = tex96;
		
		labelDisabledStyle = new GUIStyle();
		labelDisabledStyle.alignment = TextAnchor.MiddleCenter;
		labelDisabledStyle.border = new RectOffset(1, 1, 1, 1);
		labelDisabledStyle.padding = new RectOffset(2, 2, 2, 2);
		//inputStyle.onFocused.background = tex;
		//inputStyle.focused.background = tex;
		labelDisabledStyle.normal.textColor = Color.white;
		labelDisabledStyle.normal.background = tex64;

		boxStyle = new GUIStyle();
		boxStyle.alignment = TextAnchor.MiddleCenter;
		boxStyle.border = new RectOffset(1, 1, 1, 1);
		boxStyle.padding = new RectOffset(2, 2, 0, 0);
		//inputStyle.onFocused.background = tex;
		//inputStyle.focused.background = tex;
		boxStyle.normal.textColor = Color.white;
		boxStyle.active.textColor = Color.white;
		boxStyle.hover.textColor = Color.white;
		boxStyle.normal.background = tex64;
		boxStyle.active.background = tex128;
		boxStyle.hover.background = tex192;
		
		progressLabelStyle = new GUIStyle();
		progressLabelStyle.alignment = TextAnchor.MiddleCenter;
		progressLabelStyle.border = new RectOffset(1, 1, 1, 1);
		progressLabelStyle.padding = new RectOffset(2, 2, 2, 2);
		progressLabelStyle.normal.textColor = Color.white;
		
		progressBarStyle = new GUIStyle();
		progressBarStyle.alignment = TextAnchor.MiddleCenter;
		progressBarStyle.border = new RectOffset(1, 1, 1, 1);
		progressBarStyle.padding = new RectOffset(2, 2, 2, 2);
		progressBarStyle.normal.background = texBlue;

		keypadStyle256 = new GUIStyle();
		keypadStyle256.alignment = TextAnchor.MiddleCenter;
		keypadStyle256.border = new RectOffset(1, 1, 1, 1);
		keypadStyle256.padding = new RectOffset(2, 2, 0, 0);
		//inputStyle.onFocused.background = tex;
		//inputStyle.focused.background = tex;
		keypadStyle256.normal.textColor = Color.black;
		keypadStyle256.active.textColor = Color.white;
		keypadStyle256.hover.textColor = Color.white;
		keypadStyle256.normal.background = tex256_192;
		keypadStyle256.active.background = tex64;
		keypadStyle256.hover.background = tex0;

		keypadStyle192 = new GUIStyle();
		keypadStyle192.alignment = TextAnchor.MiddleCenter;
		keypadStyle192.border = new RectOffset(1, 1, 1, 1);
		keypadStyle192.padding = new RectOffset(2, 2, 0, 0);
		//inputStyle.onFocused.background = tex;
		//inputStyle.focused.background = tex;
		keypadStyle192.normal.textColor = Color.black;
		keypadStyle192.active.textColor = Color.white;
		keypadStyle192.hover.textColor = Color.white;
		keypadStyle192.normal.background = tex192_192;
		keypadStyle192.active.background = tex64;
		keypadStyle192.hover.background = tex0;

		keypadStyle128 = new GUIStyle();
		keypadStyle128.alignment = TextAnchor.MiddleCenter;
		keypadStyle128.border = new RectOffset(1, 1, 1, 1);
		keypadStyle128.padding = new RectOffset(2, 2, 0, 0);
		//inputStyle.onFocused.background = tex;
		//inputStyle.focused.background = tex;
		keypadStyle128.normal.textColor = Color.white;
		keypadStyle128.active.textColor = Color.white;
		keypadStyle128.hover.textColor = Color.white;
		keypadStyle128.normal.background = tex128_192;
		keypadStyle128.active.background = tex64;
		keypadStyle128.hover.background = tex0;

		keypadStyle64 = new GUIStyle();
		keypadStyle64.alignment = TextAnchor.MiddleCenter;
		keypadStyle64.border = new RectOffset(1, 1, 1, 1);
		keypadStyle64.padding = new RectOffset(2, 2, 0, 0);
		//inputStyle.onFocused.background = tex;
		//inputStyle.focused.background = tex;
		keypadStyle64.normal.textColor = Color.white;
		keypadStyle64.active.textColor = Color.white;
		keypadStyle64.hover.textColor = Color.white;
		keypadStyle64.normal.background = tex64_192;
		keypadStyle64.active.background = tex128;
		keypadStyle64.hover.background = tex0;

		keypadStyle0 = new GUIStyle();
		keypadStyle0.alignment = TextAnchor.MiddleCenter;
		keypadStyle0.border = new RectOffset(1, 1, 1, 1);
		keypadStyle0.padding = new RectOffset(2, 2, 0, 0);
		//inputStyle.onFocused.background = tex;
		//inputStyle.focused.background = tex;
		keypadStyle0.normal.textColor = Color.white;
		keypadStyle0.active.textColor = Color.white;
		keypadStyle0.hover.textColor = Color.white;
		keypadStyle0.normal.background = tex0_192;
		keypadStyle0.active.background = tex192;
		keypadStyle0.hover.background = tex128;
	}
	
	void Update () {
		width = Screen.width;
		height = Screen.height;
		aspect = (float)width/height;
		if (aspect > 1) portrait = false;
		else portrait = true;
		a = Mathf.Min(aspect, 1);
		variableStyle.fontSize = Mathf.RoundToInt(a*height/40);
		infoStyle.fontSize = Mathf.RoundToInt(a*height/40);
		inputCompactStyle.fontSize = Mathf.RoundToInt(a*height/40);
		inputCompactDisabledStyle.fontSize = Mathf.RoundToInt(a*height/40);
		inputStyle.fontSize = Mathf.RoundToInt(a*height/40);
		inputInfoStyle.fontSize = Mathf.RoundToInt(a*height/40);
		inputErrorStyle.fontSize = Mathf.RoundToInt(a*height/40);
		inputDisabledStyle.fontSize = Mathf.RoundToInt(a*height/40);
		listStyle.fontSize = Mathf.RoundToInt(a*height/40);
		listGreenStyle.fontSize = Mathf.RoundToInt(a*height/40);
		listBlueStyle.fontSize = Mathf.RoundToInt(a*height/40);
		buttonStyle.fontSize = Mathf.RoundToInt(a*height/40);
		buttonGreenStyle.fontSize = Mathf.RoundToInt(a*height/40);
		buttonBlueStyle.fontSize = Mathf.RoundToInt(a*height/40);
		buttonRedStyle.fontSize = Mathf.RoundToInt(a*height/40);
		buttonDisabledStyle.fontSize = Mathf.RoundToInt(a*height/40);
		toggleStyle.fontSize = Mathf.RoundToInt(a*height/70);
		toggleDisabledStyle.fontSize = Mathf.RoundToInt(a*height/70);
		labelStyle.fontSize = Mathf.RoundToInt(a*height/40);
		labelDisabledStyle.fontSize = Mathf.RoundToInt(a*height/40);
		progressLabelStyle.fontSize = Mathf.RoundToInt(a*height/40);
		keypadStyle256.fontSize = Mathf.RoundToInt(a*height/18);
		keypadStyle192.fontSize = Mathf.RoundToInt(a*height/18);
		keypadStyle128.fontSize = Mathf.RoundToInt(a*height/18);
		keypadStyle64.fontSize = Mathf.RoundToInt(a*height/18);
		keypadStyle0.fontSize = Mathf.RoundToInt(a*height/18);
	}
 
    public static bool List (Rect position, ref bool showList, ref int listEntry, GUIContent buttonContent, GUIContent[] listContent, GUIStyle buttonStyle, GUIStyle boxStyle, GUIStyle listStyle) {
        bool done = false;
 
        GUI.Label(position, buttonContent, buttonStyle);
        if (showList) {
        	int i;
        	Rect itemRect;
        	for (i = 0; i < listContent.Length; i++) {

        	}
            //Rect listRect = new Rect(position.x, position.y, position.width, listStyle.CalcHeight(listContent[0], 1.0f)*listContent.Length);
            //Rect listRect = new Rect(position.x, position.y-listStyle.CalcHeight(listContent[0], 1.0f), position.width, listStyle.CalcHeight(listContent[0], 1.0f)*listContent.Length);
            Rect listRect = new Rect(position.x, position.y-(listContent.Length-1)*Screen.height/20, position.width, (listContent.Length)*Screen.height/20);
            Debug.Log(listContent.Length);
            GUI.Box(listRect, "", boxStyle);
            listEntry = GUI.SelectionGrid(listRect, listEntry, listContent, 1, listStyle);
        }
        if (done) {
            showList = false;
        }
        return done;
    }

	public static string DrawKeypad (Rect padRect, bool enabled) {
		string toReturn = "";
		float padWidth = padRect.width;
		float padHeight = padRect.height;
		float padX = padRect.x;
		float padY = padRect.y;

		Rect rect0 = new Rect(padX+1*padWidth/5, padY+4*padHeight/5, padWidth/5, padHeight/5);
		Rect rect1 = new Rect(padX+1*padWidth/5, padY+3*padHeight/5, padWidth/5, padHeight/5);
		Rect rect2 = new Rect(padX+2*padWidth/5, padY+3*padHeight/5, padWidth/5, padHeight/5);
		Rect rect3 = new Rect(padX+3*padWidth/5, padY+3*padHeight/5, padWidth/5, padHeight/5);
		Rect rect4 = new Rect(padX+1*padWidth/5, padY+2*padHeight/5, padWidth/5, padHeight/5);
		Rect rect5 = new Rect(padX+2*padWidth/5, padY+2*padHeight/5, padWidth/5, padHeight/5);
		Rect rect6 = new Rect(padX+3*padWidth/5, padY+2*padHeight/5, padWidth/5, padHeight/5);
		Rect rect7 = new Rect(padX+1*padWidth/5, padY+1*padHeight/5, padWidth/5, padHeight/5);
		Rect rect8 = new Rect(padX+2*padWidth/5, padY+1*padHeight/5, padWidth/5, padHeight/5);
		Rect rect9 = new Rect(padX+3*padWidth/5, padY+1*padHeight/5, padWidth/5, padHeight/5);
		Rect rectPeriod = new Rect(padX+2*padWidth/5, padY+4*padHeight/5, padWidth/5, padHeight/5);
		Rect rectParenthOpen = new Rect(padX+3*padWidth/5, padY+4*padHeight/5, padWidth/5, padHeight/5);
		Rect rectParenthClose = new Rect(padX+4*padWidth/5, padY+4*padHeight/5, padWidth/5, padHeight/5);
		Rect rectPlus = new Rect(padX+4*padWidth/5, padY+0*padHeight/5, padWidth/5, padHeight/5);
		Rect rectMinus = new Rect(padX+4*padWidth/5, padY+1*padHeight/5, padWidth/5, padHeight/5);
		Rect rectTimes = new Rect(padX+4*padWidth/5, padY+2*padHeight/5, padWidth/5, padHeight/5);
		Rect rectDivide = new Rect(padX+4*padWidth/5, padY+3*padHeight/5, padWidth/5, padHeight/5);
		Rect rectMod = new Rect(padX+3*padWidth/5, padY+0*padHeight/5, padWidth/5, padHeight/5);
		Rect rectPow = new Rect(padX+2*padWidth/5, padY+0*padHeight/5, padWidth/5, padHeight/5);
		Rect rectLog = new Rect(padX+1*padWidth/5, padY+0*padHeight/5, padWidth/5, padHeight/5);
		Rect rectSin = new Rect(padX+0*padWidth/5, padY+1*padHeight/5, padWidth/5, padHeight/5);
		Rect rectCos = new Rect(padX+0*padWidth/5, padY+2*padHeight/5, padWidth/5, padHeight/5);
		Rect rectTan = new Rect(padX+0*padWidth/5, padY+3*padHeight/5, padWidth/5, padHeight/5);
		Rect rectX = new Rect(padX+0*padWidth/5, padY+0*padHeight/5, padWidth/5, padHeight/5);
		Rect rectT = new Rect(padX+1*padWidth/5, padY+0*padHeight/5, padWidth/5, padHeight/5);
		Rect rectBackspace = new Rect(padX+0*padWidth/5, padY+4*padHeight/5, padWidth/5, padHeight/5);

		if (enabled) {
			if (GUI.Button(rect0, "0", keypadStyle256)) toReturn = "0";
			if (GUI.Button(rect1, "1", keypadStyle256)) toReturn = "1";
			if (GUI.Button(rect2, "2", keypadStyle256)) toReturn = "2";
			if (GUI.Button(rect3, "3", keypadStyle256)) toReturn = "3";
			if (GUI.Button(rect4, "4", keypadStyle256)) toReturn = "4";
			if (GUI.Button(rect5, "5", keypadStyle256)) toReturn = "5";
			if (GUI.Button(rect6, "6", keypadStyle256)) toReturn = "6";
			if (GUI.Button(rect7, "7", keypadStyle256)) toReturn = "7";
			if (GUI.Button(rect8, "8", keypadStyle256)) toReturn = "8";
			if (GUI.Button(rect9, "9", keypadStyle256)) toReturn = "9";
			if (GUI.Button(rectPeriod, ".", keypadStyle256)) toReturn = ".";
			if (GUI.Button(rectParenthOpen, "(", keypadStyle64)) toReturn = "(";
			if (GUI.Button(rectParenthClose, ")", keypadStyle64)) toReturn = ")";
			if (GUI.Button(rectPlus, "+", keypadStyle128)) toReturn = "+";
			if (GUI.Button(rectMinus, "-", keypadStyle128)) toReturn = "-";
			if (GUI.Button(rectTimes, "*", keypadStyle128)) toReturn = "*";
			if (GUI.Button(rectDivide, "/", keypadStyle128)) toReturn = "/";
			if (GUI.Button(rectMod, "%", keypadStyle128)) toReturn = "%";
			if (GUI.Button(rectPow, "^", keypadStyle128)) toReturn = "^";
			//if (GUI.Button(rectLog, "log", keypadStyle128)) toReturn = "log(";
			if (GUI.Button(rectSin, "sin", keypadStyle64)) toReturn = "sin(";
			if (GUI.Button(rectCos, "cos", keypadStyle64)) toReturn = "cos(";
			if (GUI.Button(rectTan, "tan", keypadStyle64)) toReturn = "tan(";
			if (GUI.Button(rectX, "X", keypadStyle192)) toReturn = "x";
			if (GUI.Button(rectT, "T", keypadStyle192)) toReturn = "t";
			if (GUI.Button(rectBackspace, "DEL", keypadStyle0)) toReturn = "DEL";
		}
		else {
			if (GUI.Button(rect0, "0", keypadStyle256)) toReturn = "0";
			if (GUI.Button(rect1, "1", keypadStyle256)) toReturn = "1";
			if (GUI.Button(rect2, "2", keypadStyle256)) toReturn = "2";
			if (GUI.Button(rect3, "3", keypadStyle256)) toReturn = "3";
			if (GUI.Button(rect4, "4", keypadStyle256)) toReturn = "4";
			if (GUI.Button(rect5, "5", keypadStyle256)) toReturn = "5";
			if (GUI.Button(rect6, "6", keypadStyle256)) toReturn = "6";
			if (GUI.Button(rect7, "7", keypadStyle256)) toReturn = "7";
			if (GUI.Button(rect8, "8", keypadStyle256)) toReturn = "8";
			if (GUI.Button(rect9, "9", keypadStyle256)) toReturn = "9";
			if (GUI.Button(rectPeriod, ".", keypadStyle256)) toReturn = ".";
			if (GUI.Button(rectParenthOpen, "(", keypadStyle64)) toReturn = "(";
			if (GUI.Button(rectParenthClose, ")", keypadStyle64)) toReturn = ")";
			if (GUI.Button(rectPlus, "+", keypadStyle128)) toReturn = "+";
			if (GUI.Button(rectMinus, "-", keypadStyle128)) toReturn = "-";
			if (GUI.Button(rectTimes, "*", keypadStyle128)) toReturn = "*";
			if (GUI.Button(rectDivide, "/", keypadStyle128)) toReturn = "/";
			if (GUI.Button(rectMod, "%", keypadStyle128)) toReturn = "%";
			if (GUI.Button(rectPow, "^", keypadStyle128)) toReturn = "^";
			//if (GUI.Button(rectLog, "log", keypadStyle128)) toReturn = "log(";
			if (GUI.Button(rectSin, "sin", keypadStyle64)) toReturn = "sin(";
			if (GUI.Button(rectCos, "cos", keypadStyle64)) toReturn = "cos(";
			if (GUI.Button(rectTan, "tan", keypadStyle64)) toReturn = "tan(";
			if (GUI.Button(rectX, "X", keypadStyle192)) toReturn = "x";
			if (GUI.Button(rectT, "T", keypadStyle192)) toReturn = "t";
			if (GUI.Button(rectBackspace, "DEL", keypadStyle0)) toReturn = "DEL";
		}
		return toReturn;
	}
}
