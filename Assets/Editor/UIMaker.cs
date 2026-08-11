using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Bonus3DUIMaker : EditorWindow
{
    [MenuItem("Tools/FOG/Generate 3D Bonus UI")]
    public static void GenerateUI()
    {
        // Prevent accidental duplicates
        if (GameObject.Find("Bonus3D_UI") != null)
        {
            Debug.LogWarning("Bonus3D_UI already exists.");
            Selection.activeGameObject = GameObject.Find("Bonus3D_UI");
            return;
        }

        // --------------------------------------------------
        // CANVAS
        // --------------------------------------------------

        GameObject canvasObj = new GameObject("Bonus3D_UI");

        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;

        canvasObj.AddComponent<GraphicRaycaster>();

        // --------------------------------------------------
        // ROOT
        // --------------------------------------------------

        GameObject root = CreateUIObject("UIRoot", canvasObj.transform);

        RectTransform rootRect = root.GetComponent<RectTransform>();
        rootRect.anchorMin = Vector2.zero;
        rootRect.anchorMax = Vector2.one;
        rootRect.offsetMin = Vector2.zero;
        rootRect.offsetMax = Vector2.zero;

        // --------------------------------------------------
        // TOP BAR
        // --------------------------------------------------

        GameObject topBar = CreateUIObject("TopBar", root.transform);

        RectTransform topRect = topBar.GetComponent<RectTransform>();

        topRect.anchorMin = new Vector2(0, 1);
        topRect.anchorMax = new Vector2(1, 1);

        topRect.pivot = new Vector2(0.5f, 1);

        topRect.sizeDelta = new Vector2(0, 130);
        topRect.anchoredPosition = Vector2.zero;

        // --------------------------------------------------
        // RESTART
        // --------------------------------------------------

        GameObject restart = CreateButton(
            "RestartButton",
            "Restart",
            topBar.transform
        );

        RectTransform restartRect = restart.GetComponent<RectTransform>();

        restartRect.anchorMin = new Vector2(0, 0.5f);
        restartRect.anchorMax = new Vector2(0, 0.5f);
        restartRect.pivot = new Vector2(0, 0.5f);

        restartRect.sizeDelta = new Vector2(190, 75);
        restartRect.anchoredPosition = new Vector2(25, 0);

        // --------------------------------------------------
        // TIMER
        // --------------------------------------------------

        GameObject timer = CreatePanel("Timer", topBar.transform);

        RectTransform timerRect = timer.GetComponent<RectTransform>();

        timerRect.anchorMin = new Vector2(0.5f, 0.5f);
        timerRect.anchorMax = new Vector2(0.5f, 0.5f);
        timerRect.sizeDelta = new Vector2(230, 85);
        timerRect.anchoredPosition = new Vector2(-350, 0);

        CreateText(
            "TimerText",
            "30",
            timer.transform,
            42
        );

        // --------------------------------------------------
        // DIAMONDS
        // --------------------------------------------------

        GameObject diamonds = CreatePanel("DiamondCounter", topBar.transform);

        RectTransform diamondRect = diamonds.GetComponent<RectTransform>();

        diamondRect.anchorMin = new Vector2(0.5f, 0.5f);
        diamondRect.anchorMax = new Vector2(0.5f, 0.5f);

        diamondRect.sizeDelta = new Vector2(300, 85);
        diamondRect.anchoredPosition = new Vector2(0, 0);

        // Diamond image placeholder
        GameObject diamondImage = CreateUIObject(
            "DiamondImage",
            diamonds.transform
        );

        Image diamondImg = diamondImage.AddComponent<Image>();

        RectTransform diamondImageRect =
            diamondImage.GetComponent<RectTransform>();

        diamondImageRect.anchorMin = new Vector2(0, 0.5f);
        diamondImageRect.anchorMax = new Vector2(0, 0.5f);
        diamondImageRect.sizeDelta = new Vector2(65, 65);
        diamondImageRect.anchoredPosition = new Vector2(45, 0);

        CreateText(
            "DiamondText",
            "0 / 28",
            diamonds.transform,
            36
        );

        // --------------------------------------------------
        // LIVES
        // --------------------------------------------------

        GameObject lives = CreatePanel("Lives", topBar.transform);

        RectTransform livesRect = lives.GetComponent<RectTransform>();

        livesRect.anchorMin = new Vector2(0.5f, 0.5f);
        livesRect.anchorMax = new Vector2(0.5f, 0.5f);

        livesRect.sizeDelta = new Vector2(250, 85);
        livesRect.anchoredPosition = new Vector2(350, 0);

        for (int i = 0; i < 3; i++)
        {
            GameObject heart = CreateUIObject(
                "Heart_" + i,
                lives.transform
            );

            Image image = heart.AddComponent<Image>();

            RectTransform heartRect =
                heart.GetComponent<RectTransform>();

            heartRect.anchorMin = new Vector2(0.5f, 0.5f);
            heartRect.anchorMax = new Vector2(0.5f, 0.5f);

            heartRect.sizeDelta = new Vector2(65, 65);

            heartRect.anchoredPosition =
                new Vector2((i - 1) * 75, 0);
        }

        // --------------------------------------------------
        // QUIT
        // --------------------------------------------------

        GameObject quit = CreateButton(
            "QuitButton",
            "Quit",
            topBar.transform
        );

        RectTransform quitRect = quit.GetComponent<RectTransform>();

        quitRect.anchorMin = new Vector2(1, 0.5f);
        quitRect.anchorMax = new Vector2(1, 0.5f);

        quitRect.pivot = new Vector2(1, 0.5f);

        quitRect.sizeDelta = new Vector2(190, 75);
        quitRect.anchoredPosition = new Vector2(-25, 0);

        // --------------------------------------------------
        // FINISH
        // --------------------------------------------------

        Selection.activeGameObject = canvasObj;

        Debug.Log("🔥 FOG 3D Bonus UI generated!");
    }

    private static GameObject CreateUIObject(
        string name,
        Transform parent)
    {
        GameObject obj = new GameObject(
            name,
            typeof(RectTransform)
        );

        obj.transform.SetParent(parent, false);

        return obj;
    }

    private static GameObject CreatePanel(
        string name,
        Transform parent)
    {
        GameObject panel = CreateUIObject(name, parent);

        Image image = panel.AddComponent<Image>();

        image.color = new Color(
            0.75f,
            0.75f,
            0.75f,
            0.85f
        );

        return panel;
    }

    private static GameObject CreateButton(
        string name,
        string text,
        Transform parent)
    {
        GameObject buttonObj =
            CreateUIObject(name, parent);

        Image image =
            buttonObj.AddComponent<Image>();

        image.color = new Color(
            0.8f,
            0.8f,
            0.8f,
            0.9f
        );

        Button button =
            buttonObj.AddComponent<Button>();

        GameObject textObj =
            CreateText(
                "Text",
                text,
                buttonObj.transform,
                32
            );

        return buttonObj;
    }

    private static GameObject CreateText(
        string name,
        string text,
        Transform parent,
        float fontSize)
    {
        GameObject obj =
            CreateUIObject(name, parent);

        TextMeshProUGUI tmp =
            obj.AddComponent<TextMeshProUGUI>();

        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;

        RectTransform rect =
            obj.GetComponent<RectTransform>();

        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        return obj;
    }
}