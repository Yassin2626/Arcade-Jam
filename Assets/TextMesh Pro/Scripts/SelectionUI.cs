using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectionUI : MonoBehaviour
{
    [Header("Egyptian Theme Colors")]
    public Color goldColor = new Color(0.83f, 0.69f, 0.22f);
    public Color darkGold = new Color(0.60f, 0.45f, 0.10f);
    public Color sandColor = new Color(0.96f, 0.87f, 0.70f);
    public Color darkSand = new Color(0.76f, 0.67f, 0.50f);
    public Color egyptianBlue = new Color(0.05f, 0.20f, 0.53f);
    public Color darkBg = new Color(0.10f, 0.06f, 0.04f);
    public Color parchment = new Color(0.94f, 0.90f, 0.82f);

    [Header("Player Colors")]
    public Color playerOneColor = new Color(0.47f, 0.82f, 0.55f);
    public Color playerTwoColor = new Color(0.93f, 0.51f, 0.47f);
    public Color arrowInactiveColor = new Color(0.60f, 0.50f, 0.30f);

    [Header("Character Sprites")]
    public Sprite[] characterSprites;

    [Header("Gun Sprites")]
    public Sprite[] gunSprites;

    [Header("Box Settings")]
    public float boxWidth = 260f;
    public float boxHeight = 280f;
    public float selectedScaleMultiplier = 1.15f;

    [Header("Controls")]
    public string p1Horizontal = "Horizontal_1";
    public string p1Vertical = "Vertical_1";
    public string p1Confirm = "Jump_1";
    public string p2Horizontal = "Horizontal_2";
    public string p2Vertical = "Vertical_2";
    public string p2Confirm = "Jump_2";

    private Canvas _canvas;
    private GameObject _selectionPanel;
    private AudioSource _audioSource;
    private AudioClip _selectClick;
    private AudioClip _readyChime;
    private AudioClip _unreadyTap;

    private string[] _characterNames = { "Pharaoh" };
    private string[] _gunNames = { "Gun", "Sickle" };

    private class PlayerSelections
    {
        public int charIndex = 0;
        public int gunIndex = 0;
        public int currentRow = 1;
        public bool ready = false;
        public float lastH = 0f;
        public float lastV = 0f;

        public TextMeshProUGUI charLeftArrow;
        public TextMeshProUGUI charRightArrow;
        public TextMeshProUGUI gunLeftArrow;
        public TextMeshProUGUI gunRightArrow;
        public GameObject charBox;
        public GameObject gunBox;
        public TextMeshProUGUI charBoxLabel;
        public TextMeshProUGUI gunBoxLabel;
        public Image charBoxImage;
        public TextMeshProUGUI charBoxText;
        public Image gunBoxImage;
        public TextMeshProUGUI gunBoxText;
        public TextMeshProUGUI charLabel;
        public TextMeshProUGUI gunLabel;
        public TextMeshProUGUI readyLabel;
        public Image readyBorder;
    }

    private PlayerSelections _p1 = new PlayerSelections();
    private PlayerSelections _p2 = new PlayerSelections();
    private bool _bothReady = false;

    private void Start()
    {
        _canvas = FindObjectOfType<Canvas>();
        if (_canvas == null)
        {
            Debug.LogError("SelectionUI: No Canvas found in scene!");
            return;
        }
        _audioSource = gameObject.GetComponent<AudioSource>();
        if (_audioSource == null)
            _audioSource = gameObject.AddComponent<AudioSource>();
        _audioSource.playOnAwake = false;
        LoadSprites();
        LoadAudio();
        CreateUI();
        RandomizeWeapons();
    }

    private void LoadAudio()
    {
        _selectClick = Resources.Load<AudioClip>("select_click");
        _readyChime = Resources.Load<AudioClip>("ready_chime");
        _unreadyTap = Resources.Load<AudioClip>("unready_tap");
    }

    private void LoadSprites()
    {
        if (characterSprites == null || characterSprites.Length == 0)
        {
            characterSprites = new Sprite[_characterNames.Length];
            Sprite head = Resources.Load<Sprite>("The-Head-1");
            if (head != null)
                characterSprites[0] = head;
        }
        if (gunSprites == null || gunSprites.Length == 0)
        {
            gunSprites = new Sprite[_gunNames.Length];
            Sprite gun = Resources.Load<Sprite>("gun");
            if (gun != null)
                gunSprites[0] = gun;
            Sprite sickle = Resources.Load<Sprite>("The-Sickle");
            if (sickle != null)
                gunSprites[1] = sickle;
        }
    }

    private void CreateUI()
    {
        _selectionPanel = new GameObject("EgyptianSelectionUI", typeof(RectTransform));
        RectTransform pr = _selectionPanel.GetComponent<RectTransform>();
        pr.SetParent(_canvas.transform, false);
        pr.anchorMin = Vector2.zero;
        pr.anchorMax = Vector2.one;
        pr.offsetMin = Vector2.zero;
        pr.offsetMax = Vector2.zero;

        Image bg = _selectionPanel.AddComponent<Image>();
        bg.color = new Color(0.08f, 0.04f, 0.02f, 0.95f);

        CreateTitle();
        CreatePlayerArea(_p1, -1, playerOneColor, "PLAYER 1");
        CreatePlayerArea(_p2, 1, playerTwoColor, "PLAYER 2");
        CreateReadyBar();
    }

    private void RandomizeWeapons()
    {
        int rnd = UnityEngine.Random.Range(0, _gunNames.Length);
        _p1.gunIndex = rnd;
        _p2.gunIndex = 1 - rnd;
        UpdateBox(_p1);
        UpdateBox(_p2);
    }

    private void CreateTitle()
    {
        GameObject obj = new GameObject("Title", typeof(TextMeshProUGUI));
        obj.transform.SetParent(_selectionPanel.transform, false);
        TextMeshProUGUI t = obj.GetComponent<TextMeshProUGUI>();
        RectTransform r = obj.GetComponent<RectTransform>();
        r.anchorMin = new Vector2(0.5f, 1f);
        r.anchorMax = new Vector2(0.5f, 1f);
        r.pivot = new Vector2(0.5f, 1f);
        r.sizeDelta = new Vector2(900, 70);
        r.anchoredPosition = new Vector2(0, -30f);
        t.text = "ARENA OF THE PHARAOHS";
        t.fontSize = 48;
        t.fontStyle = FontStyles.Bold;
        t.alignment = TextAlignmentOptions.Center;
        t.color = goldColor;

        GameObject sep = new GameObject("Sep", typeof(Image));
        sep.transform.SetParent(_selectionPanel.transform, false);
        RectTransform sr = sep.GetComponent<RectTransform>();
        sr.anchorMin = new Vector2(0.5f, 1f);
        sr.anchorMax = new Vector2(0.5f, 1f);
        sr.pivot = new Vector2(0.5f, 1f);
        sr.sizeDelta = new Vector2(800, 3);
        sr.anchoredPosition = new Vector2(0, -95f);
        sep.GetComponent<Image>().color = darkGold;
    }

    private void CreatePlayerArea(PlayerSelections p, int side, Color playerColor, string label)
    {
        float xOff = side < 0 ? -480f : 480f;
        float topY = -200f;

        CreateSelector(p, true, side, xOff, topY - 100f);
        CreateSelector(p, false, side, xOff, topY + 280f);
    }

    private void CreateSelector(PlayerSelections p, bool isCharacter, int side, float xOff, float yPos)
    {
        string[] items = isCharacter ? _characterNames : _gunNames;
        string label = isCharacter ? "CHARACTER" : "WEAPON";

        TextMeshProUGUI lbl = MakeLabel(label, xOff, yPos + boxHeight / 2f + 40f, side);
        TextMeshProUGUI leftArrow = MakeArrow("<", xOff - boxWidth / 2f - 70f, yPos, side);
        TextMeshProUGUI rightArrow = MakeArrow(">", xOff + boxWidth / 2f + 70f, yPos, side);

        GameObject box = new GameObject(isCharacter ? "CharBox" : "GunBox", typeof(RectTransform), typeof(Image));
        box.transform.SetParent(_selectionPanel.transform, false);
        RectTransform br = box.GetComponent<RectTransform>();
        br.anchorMin = new Vector2(0.5f, 0.5f);
        br.anchorMax = new Vector2(0.5f, 0.5f);
        br.pivot = new Vector2(0.5f, 0.5f);
        br.sizeDelta = new Vector2(boxWidth, boxHeight);
        br.anchoredPosition = new Vector2(xOff, yPos);

        Image bi = box.GetComponent<Image>();
        bi.sprite = MakeRounded((int)boxWidth, (int)boxHeight, 14, parchment, 0.12f);
        bi.type = Image.Type.Sliced;

        int idx = isCharacter ? p.charIndex : p.gunIndex;

        if (isCharacter)
        {
            GameObject iconImg = new GameObject("IconImage", typeof(RectTransform), typeof(Image));
            iconImg.transform.SetParent(box.transform, false);
            Image img = iconImg.GetComponent<Image>();
            RectTransform iir = iconImg.GetComponent<RectTransform>();
            iir.anchorMin = new Vector2(0.5f, 0.5f);
            iir.anchorMax = new Vector2(0.5f, 0.5f);
            iir.pivot = new Vector2(0.5f, 0.5f);
            iir.sizeDelta = new Vector2(160, 160);
            iir.anchoredPosition = new Vector2(0, 16f);
            img.preserveAspect = true;
            p.charBoxImage = img;

            GameObject iconTxt = new GameObject("IconText", typeof(TextMeshProUGUI));
            iconTxt.transform.SetParent(box.transform, false);
            TextMeshProUGUI txt = iconTxt.GetComponent<TextMeshProUGUI>();
            RectTransform tir = iconTxt.GetComponent<RectTransform>();
            tir.anchorMin = new Vector2(0.5f, 0.5f);
            tir.anchorMax = new Vector2(0.5f, 0.5f);
            tir.pivot = new Vector2(0.5f, 0.5f);
            tir.sizeDelta = new Vector2(80, 80);
            tir.anchoredPosition = new Vector2(0, 16f);
            txt.text = Initial(items[idx]);
            txt.fontSize = 50;
            txt.alignment = TextAlignmentOptions.Center;
            txt.color = new Color(0.12f, 0.06f, 0.03f);
            p.charBoxText = txt;

            bool showImage = characterSprites != null && idx < characterSprites.Length && characterSprites[idx] != null;
            img.gameObject.SetActive(showImage);
            txt.gameObject.SetActive(!showImage);
            if (showImage) img.sprite = characterSprites[idx];
        }
        else
        {
            GameObject iconImg = new GameObject("IconImage", typeof(RectTransform), typeof(Image));
            iconImg.transform.SetParent(box.transform, false);
            Image img = iconImg.GetComponent<Image>();
            RectTransform iir = iconImg.GetComponent<RectTransform>();
            iir.anchorMin = new Vector2(0.5f, 0.5f);
            iir.anchorMax = new Vector2(0.5f, 0.5f);
            iir.pivot = new Vector2(0.5f, 0.5f);
            iir.sizeDelta = new Vector2(160, 160);
            iir.anchoredPosition = new Vector2(0, 16f);
            img.preserveAspect = true;
            p.gunBoxImage = img;

            GameObject iconTxt = new GameObject("IconText", typeof(TextMeshProUGUI));
            iconTxt.transform.SetParent(box.transform, false);
            TextMeshProUGUI txt = iconTxt.GetComponent<TextMeshProUGUI>();
            RectTransform tir = iconTxt.GetComponent<RectTransform>();
            tir.anchorMin = new Vector2(0.5f, 0.5f);
            tir.anchorMax = new Vector2(0.5f, 0.5f);
            tir.pivot = new Vector2(0.5f, 0.5f);
            tir.sizeDelta = new Vector2(80, 80);
            tir.anchoredPosition = new Vector2(0, 16f);
            txt.text = Initial(items[idx]);
            txt.fontSize = 50;
            txt.alignment = TextAlignmentOptions.Center;
            txt.color = new Color(0.12f, 0.06f, 0.03f);
            p.gunBoxText = txt;

            bool showImage = gunSprites != null && idx < gunSprites.Length && gunSprites[idx] != null;
            img.gameObject.SetActive(showImage);
            txt.gameObject.SetActive(!showImage);
            if (showImage) img.sprite = gunSprites[idx];
        }

        GameObject nameObj = new GameObject("Name", typeof(TextMeshProUGUI));
        nameObj.transform.SetParent(box.transform, false);
        TextMeshProUGUI nameText = nameObj.GetComponent<TextMeshProUGUI>();
        RectTransform nr = nameObj.GetComponent<RectTransform>();
        nr.anchorMin = new Vector2(0.5f, 0f);
        nr.anchorMax = new Vector2(0.5f, 0f);
        nr.pivot = new Vector2(0.5f, 0f);
        nr.sizeDelta = new Vector2(boxWidth - 10, 30);
        nr.anchoredPosition = new Vector2(0, 8f);
        nameText.fontSize = 22;
        nameText.fontStyle = FontStyles.Bold;
        nameText.alignment = TextAlignmentOptions.Center;
        nameText.color = new Color(0.12f, 0.06f, 0.03f);
        nameText.text = items[idx];

        if (isCharacter)
        {
            p.charLeftArrow = leftArrow;
            p.charRightArrow = rightArrow;
            if (items.Length <= 1)
            {
                leftArrow.gameObject.SetActive(false);
                rightArrow.gameObject.SetActive(false);
            }
            p.charBox = box;
            p.charBoxLabel = nameText;
            p.charLabel = lbl;
        }
        else
        {
            p.gunLeftArrow = leftArrow;
            p.gunRightArrow = rightArrow;
            p.gunBox = box;
            p.gunBoxLabel = nameText;
            p.gunLabel = lbl;
        }
    }

    private TextMeshProUGUI MakeLabel(string text, float x, float y, int side)
    {
        GameObject obj = new GameObject("Label_" + text, typeof(TextMeshProUGUI));
        obj.transform.SetParent(_selectionPanel.transform, false);
        TextMeshProUGUI t = obj.GetComponent<TextMeshProUGUI>();
        RectTransform r = obj.GetComponent<RectTransform>();
        r.anchorMin = new Vector2(0.5f, 0.5f);
        r.anchorMax = new Vector2(0.5f, 0.5f);
        r.pivot = new Vector2(0.5f, 0.5f);
        r.sizeDelta = new Vector2(300, 32);
        r.anchoredPosition = new Vector2(x, y);
        t.text = text;
        t.fontSize = 28;
        t.fontStyle = FontStyles.Bold;
        t.alignment = TextAlignmentOptions.Center;
        t.color = sandColor;
        return t;
    }

    private TextMeshProUGUI MakeArrow(string symbol, float x, float y, int side)
    {
        GameObject obj = new GameObject("Arrow_" + symbol, typeof(TextMeshProUGUI));
        obj.transform.SetParent(_selectionPanel.transform, false);
        TextMeshProUGUI t = obj.GetComponent<TextMeshProUGUI>();
        RectTransform r = obj.GetComponent<RectTransform>();
        r.anchorMin = new Vector2(0.5f, 0.5f);
        r.anchorMax = new Vector2(0.5f, 0.5f);
        r.pivot = new Vector2(0.5f, 0.5f);
        r.sizeDelta = new Vector2(70, 90);
        r.anchoredPosition = new Vector2(x, y);
        t.text = symbol;
        t.fontSize = 80;
        t.fontStyle = FontStyles.Bold;
        t.alignment = TextAlignmentOptions.Center;
        t.color = arrowInactiveColor;
        return t;
    }

    private void CreateReadyBar()
    {
        float yPos = 360f;

        CreateReadyBox(_p1, -480f, yPos, playerOneColor, "WASD+SPACE  |  XBOX B");
        CreateReadyBox(_p2, 480f, yPos, playerTwoColor, "ARROWS+SHIFT  |  XBOX B");
    }

    private void CreateReadyBox(PlayerSelections p, float x, float y, Color color, string controls)
    {
        GameObject border = new GameObject("ReadyBorder", typeof(RectTransform), typeof(Image));
        border.transform.SetParent(_selectionPanel.transform, false);
        RectTransform br = border.GetComponent<RectTransform>();
        br.anchorMin = new Vector2(0.5f, 0.5f);
        br.anchorMax = new Vector2(0.5f, 0.5f);
        br.pivot = new Vector2(0.5f, 0.5f);
        br.sizeDelta = new Vector2(360, 95);
        br.anchoredPosition = new Vector2(x, y);
        Image bi = border.GetComponent<Image>();
        bi.sprite = MakeRounded(360, 95, 12, new Color(0.25f, 0.18f, 0.08f, 0.9f), 0.15f);
        bi.type = Image.Type.Sliced;
        p.readyBorder = bi;

        GameObject labelObj = new GameObject("Label", typeof(TextMeshProUGUI));
        labelObj.transform.SetParent(border.transform, false);
        TextMeshProUGUI lt = labelObj.GetComponent<TextMeshProUGUI>();
        RectTransform lr = labelObj.GetComponent<RectTransform>();
        lr.anchorMin = Vector2.zero;
        lr.anchorMax = Vector2.one;
        lr.sizeDelta = Vector2.zero;
        lt.text = "NOT READY";
        lt.fontSize = 30;
        lt.fontStyle = FontStyles.Bold;
        lt.alignment = TextAlignmentOptions.Center;
        lt.color = darkSand;
        p.readyLabel = lt;

        GameObject hint = new GameObject("Hint", typeof(TextMeshProUGUI));
        hint.transform.SetParent(border.transform, false);
        TextMeshProUGUI ht = hint.GetComponent<TextMeshProUGUI>();
        RectTransform hr = hint.GetComponent<RectTransform>();
        hr.anchorMin = new Vector2(0.5f, 0f);
        hr.anchorMax = new Vector2(0.5f, 0f);
        hr.pivot = new Vector2(0.5f, 0f);
        hr.sizeDelta = new Vector2(340, 24);
        hr.anchoredPosition = new Vector2(0, 6f);
        ht.text = controls;
        ht.fontSize = 18;
        ht.alignment = TextAlignmentOptions.Center;
        ht.color = new Color(darkSand.r, darkSand.g, darkSand.b, 0.85f);
    }

    private Sprite MakeRounded(int width, int height, int radius, Color color, float borderRatio = 0f)
    {
        Texture2D tex = new Texture2D(width, height);
        tex.filterMode = FilterMode.Bilinear;
        Color bc = Color.Lerp(color, Color.black, 0.3f);
        Color cl = new Color(0, 0, 0, 0);

        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
            {
                bool tl = x < radius && y < radius;
                bool tr = x >= width - radius && y < radius;
                bool bl = x < radius && y >= height - radius;
                bool br = x >= width - radius && y >= height - radius;

                if (tl && Vector2.Distance(new Vector2(x, y), new Vector2(radius, radius)) > radius)
                { tex.SetPixel(x, y, cl); continue; }
                if (tr && Vector2.Distance(new Vector2(x, y), new Vector2(width - radius - 1, radius)) > radius)
                { tex.SetPixel(x, y, cl); continue; }
                if (bl && Vector2.Distance(new Vector2(x, y), new Vector2(radius, height - radius - 1)) > radius)
                { tex.SetPixel(x, y, cl); continue; }
                if (br && Vector2.Distance(new Vector2(x, y), new Vector2(width - radius - 1, height - radius - 1)) > radius)
                { tex.SetPixel(x, y, cl); continue; }

                bool onBorder = x < borderRatio * width || x >= width * (1 - borderRatio) ||
                                y < borderRatio * height || y >= height * (1 - borderRatio);
                tex.SetPixel(x, y, onBorder ? bc : color);
            }

        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f), 100, 0, SpriteMeshType.FullRect, new Vector4(radius, radius, radius, radius));
    }

    private string Initial(string name)
    {
        if (string.IsNullOrEmpty(name)) return "?";
        if (name.Length >= 4) return name.Substring(0, 2);
        return name.Substring(0, 1);
    }

    private void UpdateBox(PlayerSelections p)
    {
        p.charBoxLabel.text = _characterNames[p.charIndex];
        if (p.charBoxImage != null && p.charBoxText != null)
        {
            bool showImage = characterSprites != null && p.charIndex < characterSprites.Length && characterSprites[p.charIndex] != null;
            p.charBoxImage.gameObject.SetActive(showImage);
            p.charBoxText.gameObject.SetActive(!showImage);
            if (showImage)
                p.charBoxImage.sprite = characterSprites[p.charIndex];
            else
                p.charBoxText.text = Initial(_characterNames[p.charIndex]);
        }
        p.gunBoxLabel.text = _gunNames[p.gunIndex];
        if (p.gunBoxImage != null && p.gunBoxText != null)
        {
            bool showImage = gunSprites != null && p.gunIndex < gunSprites.Length && gunSprites[p.gunIndex] != null;
            p.gunBoxImage.gameObject.SetActive(showImage);
            p.gunBoxText.gameObject.SetActive(!showImage);
            if (showImage)
                p.gunBoxImage.sprite = gunSprites[p.gunIndex];
            else
                p.gunBoxText.text = Initial(_gunNames[p.gunIndex]);
        }
    }

    private void Update()
    {
        if (GameState.Instance == null) return;
        if (GameState.Instance.gameState != GameState.GameStateEnum.GetReady)
        {
            if (_selectionPanel != null && _selectionPanel.activeSelf)
                _selectionPanel.SetActive(false);
            return;
        }

        if (_selectionPanel != null && !_selectionPanel.activeSelf)
            _selectionPanel.SetActive(true);

        HandlePlayer(_p1, p1Horizontal, p1Vertical, p1Confirm);
        HandlePlayer(_p2, p2Horizontal, p2Vertical, p2Confirm);

        UpdateVisuals(_p1, playerOneColor);
        UpdateVisuals(_p2, playerTwoColor);

        if (_p1.ready && _p2.ready && !_bothReady)
        {
            _bothReady = true;
            Save();
        }
    }

    private void HandlePlayer(PlayerSelections p, string hAxis, string vAxis, string confirm)
    {
        float h = Input.GetAxisRaw(hAxis);
        float v = Input.GetAxisRaw(vAxis);
        bool hit = Input.GetButtonDown(confirm);

        if (!p.ready)
        {
            if (Mathf.Abs(h) > 0.3f && Mathf.Abs(p.lastH) < 0.3f)
            {
                int dir = h > 0 ? 1 : -1;
                if (p.currentRow == 0)
                {
                    int len = _characterNames.Length;
                    p.charIndex = (p.charIndex + dir + len) % len;
                }
                else
                {
                    int len = _gunNames.Length;
                    p.gunIndex = (p.gunIndex + dir + len) % len;
                }
                UpdateBox(p);
                if (_selectClick != null)
                    _audioSource.PlayOneShot(_selectClick);
            }

            if (Mathf.Abs(v) > 0.3f && Mathf.Abs(p.lastV) < 0.3f)
            {
                int dir = v > 0 ? -1 : 1;
                p.currentRow = (p.currentRow + dir + 2) % 2;
            }
        }

        if (hit)
        {
            p.ready = !p.ready;
            if (!p.ready) _bothReady = false;
            if (p.ready)
            {
                if (_readyChime != null)
                    _audioSource.PlayOneShot(_readyChime);
            }
            else
            {
                if (_unreadyTap != null)
                    _audioSource.PlayOneShot(_unreadyTap);
            }
        }

        p.lastH = h;
        p.lastV = v;
    }

    private void UpdateVisuals(PlayerSelections p, Color playerColor)
    {
        Color active = playerColor;

        SetArrow(p.charLeftArrow, p.currentRow == 0 ? active : arrowInactiveColor);
        SetArrow(p.charRightArrow, p.currentRow == 0 ? active : arrowInactiveColor);
        SetArrow(p.gunLeftArrow, p.currentRow == 1 ? active : arrowInactiveColor);
        SetArrow(p.gunRightArrow, p.currentRow == 1 ? active : arrowInactiveColor);

        float cs = p.currentRow == 0 ? selectedScaleMultiplier : 1f;
        float gs = p.currentRow == 1 ? selectedScaleMultiplier : 1f;
        p.charBox.transform.localScale = Vector3.Lerp(p.charBox.transform.localScale, Vector3.one * cs, Time.deltaTime * 10f);
        p.gunBox.transform.localScale = Vector3.Lerp(p.gunBox.transform.localScale, Vector3.one * gs, Time.deltaTime * 10f);

        p.charBox.GetComponent<Image>().color = Color.Lerp(p.charBox.GetComponent<Image>().color, p.currentRow == 0 ? active : parchment, Time.deltaTime * 8f);
        p.gunBox.GetComponent<Image>().color = Color.Lerp(p.gunBox.GetComponent<Image>().color, p.currentRow == 1 ? active : parchment, Time.deltaTime * 8f);

        p.charLabel.color = Color.Lerp(p.charLabel.color, p.currentRow == 0 ? active : sandColor, Time.deltaTime * 8f);
        p.gunLabel.color = Color.Lerp(p.gunLabel.color, p.currentRow == 1 ? active : sandColor, Time.deltaTime * 8f);

        if (p.readyLabel != null)
        {
            bool goingReady = p.ready;
            p.readyLabel.text = goingReady ? "READY!" : "NOT READY";
            p.readyLabel.color = Color.Lerp(p.readyLabel.color, goingReady ? Color.white : darkSand, Time.deltaTime * 6f);
        }
        if (p.readyBorder != null)
        {
            Color targetBorder = p.ready ? new Color(0.2f, 0.5f, 0.2f, 0.9f) : new Color(0.25f, 0.18f, 0.08f, 0.9f);
            p.readyBorder.color = Color.Lerp(p.readyBorder.color, targetBorder, Time.deltaTime * 6f);
        }
    }

    private void SetArrow(TextMeshProUGUI arrow, Color color)
    {
        if (arrow != null)
            arrow.color = Color.Lerp(arrow.color, color, Time.deltaTime * 8f);
    }

    private void Save()
    {
        if (GameState.Instance != null)
        {
            GameState.Instance.SetCharacterSelection("1", _p1.charIndex);
            GameState.Instance.SetGunSelection("1", _p1.gunIndex);
            GameState.Instance.SetCharacterSelection("2", _p2.charIndex);
            GameState.Instance.SetGunSelection("2", _p2.gunIndex);
            GameState.Instance.SetReady("1");
            GameState.Instance.SetReady("2");
        }
    }

    public void Hide()
    {
        if (_selectionPanel != null)
            _selectionPanel.SetActive(false);
    }
}