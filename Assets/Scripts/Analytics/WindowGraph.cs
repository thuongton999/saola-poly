using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WindowGraph : MonoBehaviour
{
    public KeyCode keyCode;
    public Canvas canvas;
    public int valueSize = 36;
    public Sprite circleSprite;
    public RectTransform graphContainer;
    public Color axisColor = Color.white;
    public Color lineColor = Color.white;
    public Color textColor = Color.white;
    public float lineWidth = 6f;
    public bool viewDot = true;
    public bool viewValue = false;
    public float dotSize = 10f;
    public int frequency = 10;
    float graphHeight = 0f;
    float graphWidth = 0f;

    List<int> values;

    protected virtual void Init()
    {
        graphHeight = graphContainer.sizeDelta.y;
        graphWidth = graphContainer.sizeDelta.x;
        values = new List<int>();
    }

    private void Start()
    {
        Init();
        ShowGraph();
    }

    private void Update() {
        if (Input.GetKeyDown(keyCode)) {
            canvas.enabled = !canvas.enabled;
        }
    }

    private enum CTextAlignment
    {
        Top,
        Bottom,
        Left,
        Right
    }

    public void AddDataPoint(int value)
    {
        values.Add(value);
        ShowGraph();
    }

    private GameObject CreateNewGraphContainer()
    {
        GameObject container = new GameObject("Graph Container");
        container.transform.SetParent(graphContainer.parent, false);
        RectTransform rectTransform = container.AddComponent<RectTransform>();
        rectTransform.SetAnchorPreset(CAnchorPresets.StretchAll);
        rectTransform.SetLeft(0);
        rectTransform.SetRight(0);
        rectTransform.SetTop(0);
        rectTransform.SetBottom(0);
        return container;
    }

    private void CleanGraph()
    {
        RectTransform newGraphContainer = CreateNewGraphContainer().GetComponent<RectTransform>();
        if (Application.isPlaying)
            Destroy(graphContainer.gameObject);
        else
            DestroyImmediate(graphContainer.gameObject);
        graphContainer = newGraphContainer;
    }

    private TextMeshProUGUI CreateText(string text, CTextAlignment alignment, int size)
    {
        TextMeshProUGUI textValue = new GameObject("value").AddComponent<TextMeshProUGUI>();
        RectTransform textRectTransform = textValue.GetComponent<RectTransform>();
        textValue.alignment = TextAlignmentOptions.Center;
        textValue.fontSize = size;
        switch (alignment)
        {
            case CTextAlignment.Top:
                textValue.rectTransform.anchoredPosition = new Vector2(0, textRectTransform.sizeDelta.y / 2);
                break;
            case CTextAlignment.Bottom:
                textValue.rectTransform.anchoredPosition = new Vector2(0, -textRectTransform.sizeDelta.y / 2);
                break;
            case CTextAlignment.Left:
                textValue.rectTransform.anchoredPosition = new Vector2(-textRectTransform.sizeDelta.x / 4, 0);
                break;
            case CTextAlignment.Right:
                textValue.rectTransform.anchoredPosition = new Vector2(textRectTransform.sizeDelta.x / 4, 0);
                break;
        }
        textValue.text = text;
        var textWidth = textValue.text.Length * size;
        textRectTransform.sizeDelta = new Vector2(textWidth, size);
        textValue.color = textColor;
        return textValue;
    }

    private GameObject CreateDot(Vector2 anchoredPosition, float value, bool cViewValue = true, bool cViewDot = true)
    {
        GameObject dotObject = new GameObject(value.ToString(), typeof(Image));
        dotObject.transform.SetParent(graphContainer, false);
        dotObject.GetComponent<Image>().sprite = circleSprite;
        if (!viewDot && !cViewDot)
            dotObject.GetComponent<Image>().color = new Color(1, 1, 1, 0); // transparent
        if (viewValue && cViewValue)
            CreateText(value.ToString(), CTextAlignment.Top, valueSize).transform.SetParent(dotObject.transform, false);
        RectTransform rectTransform = dotObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = new Vector2(dotSize, dotSize);
        rectTransform.SetAnchorPreset(CAnchorPresets.BottomLeft);
        return dotObject;
    }

    private GameObject CreateAxis(string name = "axis")
    {
        GameObject axis = new GameObject(name, typeof(Image));
        axis.transform.SetParent(graphContainer, false);
        axis.GetComponent<Image>().color = axisColor;
        return axis;
    }

    private GameObject[] DrawAxis()
    {
        GameObject rootDot = CreateDot(Vector2.zero, 0, cViewValue: false);

        GameObject xAxis = CreateAxis("xAxis");
        RectTransform rectTransform = xAxis.GetComponent<RectTransform>();
        rectTransform.SetAnchorPreset(CAnchorPresets.StretchBottom);
        rectTransform.sizeDelta = new Vector2(0, lineWidth);
        rectTransform.SetLeft(0);
        rectTransform.SetRight(0);
        rectTransform.anchoredPosition = Vector2.zero;

        GameObject yAxis = CreateAxis("yAxis");
        rectTransform = yAxis.GetComponent<RectTransform>();
        rectTransform.SetAnchorPreset(CAnchorPresets.StretchLeft);
        rectTransform.sizeDelta = new Vector2(lineWidth, 0);
        rectTransform.SetTop(0);
        rectTransform.SetBottom(0);
        rectTransform.localPosition = new Vector3(-graphWidth/2, 0, 0);

        return new GameObject[] { xAxis, yAxis };
    }

    public void ShowGraph()
    {
        graphHeight = graphContainer.rect.height;
        graphWidth = graphContainer.rect.width;
        CleanGraph();

        float xSize = graphWidth / (values.Count - 1);
        float ySize = values.Max();

        var xyAxis = DrawAxis();

        GameObject lastCircleGameObject = null;
        for (int i = 0; i < values.Count; i++)
        {
            float xPosition = i * xSize;
            float yPosition = (values[i] / ySize) * graphHeight;
            GameObject circleGameObject = CreateDot(new Vector2(xPosition, yPosition), values[i]);
            if (lastCircleGameObject != null)
            {
                var lastCircleAnchor = lastCircleGameObject.GetComponent<RectTransform>().anchoredPosition;
                var circleAnchor = circleGameObject.GetComponent<RectTransform>().anchoredPosition;
                CreateDotConnection(lastCircleAnchor, circleAnchor);
            }
            lastCircleGameObject = circleGameObject;
        }
    }

    private float GetAngleFromVectorFloat(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        if (angle < 0)
            angle += 360;
        return angle;
    }

    private void CreateDotConnection(Vector2 dotPositionA, Vector2 dotPositionB)
    {
        GameObject gameObject = new GameObject("dotConnection", typeof(Image));
        gameObject.transform.SetParent(graphContainer, false);
        gameObject.GetComponent<Image>().color = lineColor;
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        Vector2 dir = (dotPositionB - dotPositionA).normalized;
        float distance = Vector2.Distance(dotPositionA, dotPositionB);
        rectTransform.SetAnchorPreset(CAnchorPresets.BottomLeft);
        rectTransform.sizeDelta = new Vector2(distance, lineWidth);
        rectTransform.anchoredPosition = dotPositionA + dir * distance * .5f;
        rectTransform.localEulerAngles = new Vector3(0, 0, GetAngleFromVectorFloat(dir));
    }
}