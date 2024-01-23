using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class TypewriterEffect : MonoBehaviour
{
	[SerializeField]
	[Min(0f)]
	private float textDisplaySpeed;

	[SerializeField]
	private TextMeshProUGUI textBoxText;

	public TextMeshProUGUI SourceText => textBoxText;

	public bool IsUpdatingText { get; set; }

	private float displayLength;

	private int lastDisplayLength;

	private int displayLengthMax;

	private string evaluationString;

	private Color32[] vertexColours;

	private bool IsNextCharacterBlank => evaluationString[Mathf.Min((int)displayLength, displayLengthMax)] == ' ';

	private bool IsRenderSameAsLast => lastDisplayLength == (int)displayLength;

	public delegate void TypewriterDelegate();

	public event TypewriterDelegate TypewriterStart;

	public event TypewriterDelegate TypewriterEnd;

	// Update is called once per frame
	void Update()
	{
		if(IsUpdatingText)
			UpdateTypewriter();
	}

	public void StartText(string text)
	{
		if (text == null)
			return;

		textBoxText.SetText(text);
		// Typewriter counter
		displayLength = 0f;
		lastDisplayLength = -1;
		// Reset dialogue box
		textBoxText.renderMode = TextRenderFlags.DontRender;
		IsUpdatingText = true;
		TypewriterStart?.Invoke();
		if (textDisplaySpeed == 0f)
			SkipToEnd();
	}

	public void SkipToEnd()
	{
		SetupNewRender();
		displayLength = displayLengthMax;
		IsUpdatingText = false;
		TypewriterEnd?.Invoke();
	}

	private void SetupNewRender()
	{
		textBoxText.renderMode = TextRenderFlags.Render;
		textBoxText.ForceMeshUpdate();
		evaluationString = textBoxText.GetParsedText();
		displayLengthMax = evaluationString.Length - 1;
	}

	private void UpdateTypewriter()
	{
		bool firstRender = textBoxText.renderMode == TextRenderFlags.DontRender;

		if (firstRender)
			SetupNewRender();

		if (displayLength >= displayLengthMax)
		{
			SkipToEnd();
			return;
		}

		displayLength = Mathf.Min(displayLength + textDisplaySpeed * Time.deltaTime, displayLengthMax);

		if (IsRenderSameAsLast)
			return;

		if (IsNextCharacterBlank)
			++displayLength;

		if(!firstRender)
			textBoxText.ForceMeshUpdate();

		SetCharacterVisibility();

		lastDisplayLength = (int)displayLength;
	}

	private void SetCharacterVisibility()
	{
		int meshIndex;
		int vertexIndex;
		TMP_CharacterInfo charInfo;

		for (int i = displayLengthMax; i >= displayLength; --i)
		{
			charInfo = textBoxText.textInfo.characterInfo[i];

			if (!charInfo.isVisible) { continue; }

			meshIndex = charInfo.materialReferenceIndex;
			vertexIndex = charInfo.vertexIndex;

			vertexColours = textBoxText.textInfo.meshInfo[meshIndex].colors32;
			vertexColours[vertexIndex + 0].a = 0;
			vertexColours[vertexIndex + 1].a = 0;
			vertexColours[vertexIndex + 2].a = 0;
			vertexColours[vertexIndex + 3].a = 0;
		}

		textBoxText.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
	}

	public void SetTextDisplaySpeed(float speed) => textDisplaySpeed = speed;
}