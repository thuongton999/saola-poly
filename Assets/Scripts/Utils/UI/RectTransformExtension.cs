using System;
using UnityEngine;
using UnityEngine.UI;

public static class RectTransformExtension {
    public static RectTransform SetLeft(this RectTransform rectTransform, float left) {
        rectTransform.offsetMin = new Vector2(left, rectTransform.offsetMin.y);
        return rectTransform;
    }

    public static RectTransform SetRight(this RectTransform rectTransform, float right) {
        rectTransform.offsetMax = new Vector2(-right, rectTransform.offsetMax.y);
        return rectTransform;
    }

    public static RectTransform SetTop(this RectTransform rectTransform, float top) {
        rectTransform.offsetMax = new Vector2(rectTransform.offsetMax.x, -top);
        return rectTransform;
    }

    public static RectTransform SetBottom(this RectTransform rectTransform, float bottom) {
        rectTransform.offsetMin = new Vector2(rectTransform.offsetMin.x, bottom);
        return rectTransform;
    }

    

    public static RectTransform SetAnchorPreset(this RectTransform rectTransform, CAnchorPresets preset) {
        switch (preset) {
            case CAnchorPresets.TopLeft:
                rectTransform.anchorMin = new Vector2(0, 1);
                rectTransform.anchorMax = new Vector2(0, 1);
                break;
            case CAnchorPresets.TopCenter:
                rectTransform.anchorMin = new Vector2(0.5f, 1);
                rectTransform.anchorMax = new Vector2(0.5f, 1);
                break;
            case CAnchorPresets.TopRight:
                rectTransform.anchorMin = new Vector2(1, 1);
                rectTransform.anchorMax = new Vector2(1, 1);
                break;
            case CAnchorPresets.MiddleLeft:
                rectTransform.anchorMin = new Vector2(0, 0.5f);
                rectTransform.anchorMax = new Vector2(0, 0.5f);
                break;
            case CAnchorPresets.MiddleCenter:
                rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                break;
            case CAnchorPresets.MiddleRight:
                rectTransform.anchorMin = new Vector2(1, 0.5f);
                rectTransform.anchorMax = new Vector2(1, 0.5f);
                break;
            case CAnchorPresets.BottomLeft:
                rectTransform.anchorMin = new Vector2(0, 0);
                rectTransform.anchorMax = new Vector2(0, 0);
                break;
            case CAnchorPresets.BottomCenter:
                rectTransform.anchorMin = new Vector2(0.5f, 0);
                rectTransform.anchorMax = new Vector2(0.5f, 0);
                break;
            case CAnchorPresets.BottomRight:
                rectTransform.anchorMin = new Vector2(1, 0);
                rectTransform.anchorMax = new Vector2(1, 0);
                break;
            case CAnchorPresets.StretchAll:
                rectTransform.anchorMin = Vector2.zero;
                rectTransform.anchorMax = Vector2.one;
                break;
            case CAnchorPresets.StretchLeft:
                rectTransform.anchorMin = new Vector2(0, 0);
                rectTransform.anchorMax = new Vector2(0, 1);
                break;
            case CAnchorPresets.StretchRight:
                rectTransform.anchorMin = new Vector2(1, 0);
                rectTransform.anchorMax = new Vector2(1, 1);
                break;
            case CAnchorPresets.StretchTop:
                rectTransform.anchorMin = new Vector2(0, 1);
                rectTransform.anchorMax = new Vector2(1, 1);
                break;
            case CAnchorPresets.StretchBottom:
                rectTransform.anchorMin = new Vector2(0, 0);
                rectTransform.anchorMax = new Vector2(1, 0);
                break;
            case CAnchorPresets.StretchCenter:
                rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                break;
            case CAnchorPresets.StretchMiddle:
                rectTransform.anchorMin = new Vector2(0.5f, 0);
                rectTransform.anchorMax = new Vector2(0.5f, 1);
                break;
        }
        return rectTransform;
    }
}