using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static PlayerData;

public class DialogueEventUI : MonoBehaviour
{
    public GameObject _allObjects;
    public Transform _sceneTransform;
    public int sceneDirection; // 1 ==> Right, -1 == Left

    public Image _skyImage;

    public Image _cloudsImage;
    private Image cloudsParallaxImage;
    private float cloudSpeed;

    public Image _groundImage;
    private Image groundParallaxImage;
    private float groundSpeed;

    public Image _interiorImage;
    private bool vehicleMoving;
    private float timeSinceLastBump;
    public Button _finishEventButton;


    public Button _tempStartNewDialogueButton;

    [Header("Dialogue UI")]
    //  Dialogue UI

    public GameObject UIDialogObj;
    public bool DialogueShown;
    public float timeSinceLastDialogueStarted;
    [Range(1, 16)]
    public int dialogueSpeed;
    private int FramesBetweenCharacters;
    public float speakerHeight;
    [SerializeField]
    private Queue<Line> sentences = new Queue<Line>();
    private Line currentSentence;
    private float timeSinceLastLine;
    private bool mouseHoveringOverText;
    private bool typing;
    private enum Speaker { Left, Right }

    //  Shared

    public GameObject _dialoguePrompt;
    public Button _dialogueButton;
    public Text _dialogueText;
    public Image _dialogueTimeFill;
    public static bool playingConversation;

    //  Left

    public GameObject _speakerLeft;
    public Image _speakerLeftImage;
    public Text _speakerLeftName;

    //  Right

    public GameObject _speakerRight;
    public Image _speakerRightImage;
    public Text _speakerRightName;

    private void Awake()
    {
        typing = false;
        timeSinceLastLine = 0;
        FramesBetweenCharacters = 16 / dialogueSpeed; // 0 frames between text is max dialogue speed
        speakerHeight = _speakerRightImage.rectTransform.sizeDelta.y;

        _dialogueButton.onClick.AddListener(() => DisplayNextSentence());
        _tempStartNewDialogueButton.onClick.AddListener(() => StartNewRandomSentence());
        _finishEventButton.onClick.AddListener(() => FinishEvent());

        _finishEventButton.gameObject.SetActive(false);
        TurnOffDialogue();
    }


    private void Update()
    {
        if (playingConversation)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                DisplayNextSentence();
            }
        }
    }
    public void Show(bool show)
    {
        _allObjects.SetActive(show);
    }
    public void Init()
    {

        cloudsParallaxImage = Instantiate(_cloudsImage, _sceneTransform, false);
        groundParallaxImage = Instantiate(_groundImage, _sceneTransform, false);

        _groundImage.transform.SetAsLastSibling();
        _interiorImage.transform.SetAsLastSibling();

        sceneDirection = 1;
        vehicleMoving = true;

        groundSpeed = 10;
        cloudSpeed = groundSpeed / 2f;

        groundParallaxImage.transform.localPosition = sceneDirection * Vector3.right * _groundImage.preferredWidth;
        cloudsParallaxImage.transform.localPosition = sceneDirection * Vector3.right * _cloudsImage.preferredWidth;
        StartCoroutine(StartVehicleBumps());
        StartCoroutine(ParallaxEffect());
    }

    //  Dialogue

    private void StartNewRandomSentence()
    {
        int convoType = UnityEngine.Random.Range(0, 2);
        ConversationScriptObj newConvo;
        List<WizardData> _wizardsToDisplay = DataStorage.Singleton.playerData.WizardList;
        if (_wizardsToDisplay == null)
        {
            _finishEventButton.gameObject.SetActive(true);
            return;
        }
        int wizIndex = UnityEngine.Random.Range(0, _wizardsToDisplay.Count);
        WizardData wizData = _wizardsToDisplay[wizIndex];

        if (convoType == 0)
        {

            newConvo = Resources.Load(GS.Conversations("PositiveConvoTemplate"), typeof(ConversationScriptObj)) as ConversationScriptObj;
            DataStorage.Singleton.AdjustWizardHappiness(wizIndex, 1);
        }
        else
        {
            newConvo = Resources.Load(GS.Conversations("NegativeConvoTemplate"), typeof(ConversationScriptObj)) as ConversationScriptObj;
            DataStorage.Singleton.AdjustWizardHappiness(wizIndex, -1);
        }

        int i = 0;
        foreach (Line l in newConvo.lines)
        {
            newConvo.lines[i] = new Line
            {
                characterLeft = Resources.Load(GS.DialogueCharacters(wizData.WizType.ToString()), typeof(DialogueCharacterScriptObj)) as DialogueCharacterScriptObj,
                LineTimer = l.LineTimer,
                textLeft = l.textLeft,
                textRight = l.textRight
            };
            i++;
        };
        StartDialogue(newConvo);
        _finishEventButton.gameObject.SetActive(true);
    }

    public void StartDialogue(ConversationScriptObj convo, bool showDialogueTime = true, bool buttonsInteractable = true)
    {
        DialogueShown = true;
        TurnOnDialogue();
        if (!playingConversation)
        {
            playingConversation = true;
            sentences.Clear();

            foreach (Line line in convo.lines)
            {
                sentences.Enqueue(line);
            }
            DisplayNextSentence();
        }
        ShowDialogueTimes(showDialogueTime);
        ButtonsInteractable(buttonsInteractable);
    }

    public void DisplayNextSentence()
    {
        mouseHoveringOverText = false;
        timeSinceLastLine = 0;
        HideDialoguePrompt();

        _speakerLeft.SetActive(false);
        _speakerRight.SetActive(false);
        _dialogueText.gameObject.SetActive(false);
        _dialogueText.gameObject.SetActive(false);

        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }
        Line line = sentences.Dequeue();
        currentSentence = line;
        StopAllCoroutines();
        StartCoroutine(ShowDialoguePrompt());

        if (line.characterLeft)
        {
            _speakerLeft.SetActive(true);
            _speakerLeftImage.sprite = line.characterLeft.portrait;
            if (line.characterLeft.width > 0 && line.characterLeft.height > 0)
            {
                _speakerLeftImage.rectTransform.sizeDelta = new Vector2(((float)line.characterLeft.width / line.characterLeft.height) * speakerHeight, speakerHeight);
            }
            if (line.characterLeft.fullName != null)
            {
                _speakerLeftName.text = line.characterLeft.fullName;
                //_speakerLeftNameBG.sizeDelta = new Vector2(Mathf.Max(_speakerLeftImageBackground.GetComponent<RectTransform>().sizeDelta.x, 30 + line.characterLeft.fullName.Length * 28), _speakerLeftNameBG.rect.height);
            }

            if (line.textLeft != "")
            {
                _dialogueText.gameObject.SetActive(true);
                StartCoroutine(TypeSentence(line.textLeft, Speaker.Left));
            }
        }
        if (line.characterRight)
        {
            _speakerRight.SetActive(true);
            _speakerRightImage.sprite = line.characterRight.portrait;
            if (line.characterRight.width > 0 && line.characterRight.height > 0)
            {
                _speakerRightImage.rectTransform.sizeDelta = new Vector2(((float)line.characterRight.width / line.characterRight.height) * speakerHeight, speakerHeight);
            }
            if (line.characterRight.fullName != null)
            {
                _speakerRightName.text = line.characterRight.fullName;
                //_speakerRightNameBG.sizeDelta = new Vector2(Mathf.Max( _speakerRightImageBackground.GetComponent<RectTransform>().sizeDelta.x, 30 + line.characterRight.fullName.Length * 28), _speakerRightNameBG.rect.height);
            }

            if (line.textRight != "")
            {
                _dialogueText.gameObject.SetActive(true);
                StartCoroutine(TypeSentence(line.textRight, Speaker.Right));
            }
        }
    }
    private IEnumerator TypeSentence(string sentence, Speaker speaker)
    {
        timeSinceLastLine = 0;
        typing = true;
        _dialogueText.text = "";

        foreach (char letter in sentence.ToCharArray())
        {
            if (speaker == Speaker.Left) _dialogueText.text += letter;
            else _dialogueText.text += letter;

            for (int i = 0; i <= FramesBetweenCharacters; i++)
            {
                yield return new WaitForFixedUpdate();
            }
        }

        typing = false;
        timeSinceLastLine = 0;
    }

    public void TurnOnDialogue()
    {
        UIDialogObj.SetActive(true);
    }
    public void TurnOffDialogue()
    {
        UIDialogObj.SetActive(false);
    }
    public void EndDialogue()
    {
        currentSentence = new Line();
        playingConversation = false;
        TurnOffDialogue();
        DialogueShown = false;
        timeSinceLastDialogueStarted = 0f;
        mouseHoveringOverText = false;
    }
    public IEnumerator ShowDialoguePrompt()
    {
        yield return new WaitForSeconds(5f);
        for (int i = 0; i < 50; i++)
        {
            foreach (Image img in _dialoguePrompt.GetComponentsInChildren<Image>())
            {
                img.color = new Color(1, 1, 1, (i / 50f));
            }
            yield return new WaitForFixedUpdate();
        }
    }
    public void HideDialoguePrompt()
    {
        StopCoroutine(ShowDialoguePrompt());
        foreach (Image img in _dialoguePrompt.GetComponentsInChildren<Image>())
        {
            img.color = new Color(1, 1, 1, 0);
        }
    }
    private void ShowDialogueTimes(bool showDialogueTime)
    {
        _dialogueTimeFill.transform.parent.gameObject.SetActive(showDialogueTime);
    }
    private void ButtonsInteractable(bool interactable)
    {
        _dialogueButton.interactable = interactable;
    }
    public void MouseHoveringOverText()
    {
        mouseHoveringOverText = true;
    }
    public void MouseNoLongerHoveringOverText()
    {
        mouseHoveringOverText = false;
    }

    //  Misc

    private void FinishEvent()
    {
        SavePlayerData.SavePlayer(DataStorage.Singleton.saveSlot, DataStorage.Singleton.playerData);
        DataStorage.Singleton.FinishEvent();
    }
    private IEnumerator ParallaxEffect()
    {
        while (vehicleMoving)
        {
            _cloudsImage.transform.localPosition -= sceneDirection * Vector3.right * cloudSpeed * Time.deltaTime;
            cloudsParallaxImage.transform.localPosition -= sceneDirection * Vector3.right * cloudSpeed * Time.deltaTime;

            _groundImage.transform.localPosition -= sceneDirection * Vector3.right * groundSpeed * Time.deltaTime;
            groundParallaxImage.transform.localPosition -= sceneDirection * Vector3.right * groundSpeed * Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }
    }
    private IEnumerator StartVehicleBumps()
    {
        while (vehicleMoving)
        {
            timeSinceLastBump += Time.deltaTime;
            if (timeSinceLastBump > 3f)
            {
                _cloudsImage.transform.localPosition += Vector3.up * 1;
                cloudsParallaxImage.transform.localPosition += Vector3.up * 1;
                _groundImage.transform.localPosition += Vector3.up * 1;
                groundParallaxImage.transform.localPosition += Vector3.up * 1;

                yield return new WaitForSeconds(0.5f);

                _cloudsImage.transform.localPosition -= Vector3.up * 1;
                cloudsParallaxImage.transform.localPosition -= Vector3.up * 1;
                _groundImage.transform.localPosition -= Vector3.up * 1;
                groundParallaxImage.transform.localPosition -= Vector3.up * 1;

                timeSinceLastBump = 0;
            }
            yield return new WaitForFixedUpdate();
        }
    }
}
