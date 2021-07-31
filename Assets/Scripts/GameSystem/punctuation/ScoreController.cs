using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreController : MonoBehaviour
{
    Text scoreLabel;
    PlayerController playerController;

    [SerializeField]
    private GameObject player;

    Canvas canvas;


    private enum UpDown { Down = -1, Start = 0, Up = 1 };
    private Text text;
    private UpDown textChanged = UpDown.Start;

    GameObject goCanvas;
    GameObject goGlobalScore;
    

    bool flagStar;
    bool flagMush;
    bool flagFlower;
    Font arial;

    int globalScore;
    int totalCoinsInt;

    void Awake()
    {
        arial = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
        flagStar = true;

        // Create Canvas GameObject.
        goCanvas = new GameObject();
        goCanvas.name = "Canvas_punctualScoreText";
        goCanvas.AddComponent<Canvas>();
        goCanvas.AddComponent<CanvasScaler>();
        goCanvas.AddComponent<GraphicRaycaster>();

        // Get canvas from the GameObject.
        Canvas canvas;
        canvas = goCanvas.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        goGlobalScore = new GameObject("acumScore");
        
        goGlobalScore.transform.parent = GameObject.Find("ScreenCanvas").transform;
        goGlobalScore.AddComponent<Text>();
        goGlobalScore.GetComponent<Text>().text = "0";
        goGlobalScore.GetComponent<Text>().font = arial;
        goGlobalScore.GetComponent<Text>().fontStyle = FontStyle.Italic;
        goGlobalScore.GetComponent<Text>().fontSize = 18;
        goGlobalScore.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
        goGlobalScore.GetComponent<RectTransform>().localScale = new Vector3(1,1,1);
        goGlobalScore.GetComponent<RectTransform>().localPosition = new Vector3(-150, Screen.height / 2 - 15, 0);


        GameObject goCoinUI = new GameObject("animCoinUI");
        GameObject goTextCoin = new GameObject("textCoin");

        goTextCoin.transform.parent = GameObject.Find("ScreenCanvas").transform;
        goTextCoin.transform.localScale = new Vector3(1, 1, 1);
        goTextCoin.transform.localPosition = new Vector3(-40, Screen.height / 2 - 53, 0);
        goTextCoin.AddComponent<Text>();
        goTextCoin.GetComponent<Text>().text = "x 00";
        goTextCoin.GetComponent<Text>().font = arial;
        goTextCoin.GetComponent<Text>().fontStyle = FontStyle.Italic;
        goTextCoin.GetComponent<Text>().fontSize = 18;

        goCoinUI.transform.parent = GameObject.Find("ScreenCanvas").transform;
        goCoinUI.transform.localScale = new Vector3(0.2f, 0.2f, 1);
        goCoinUI.transform.localPosition = new Vector3(-100, Screen.height / 2 - 12, 0);
        goCoinUI.AddComponent<Image>();

        StartCoroutine(renderCoinUIAnim(goCoinUI));

 

        totalCoinsInt = 0;


    }

    public void sumCoinsOnCanvas()
    {
        totalCoinsInt = totalCoinsInt + 1;

        if (totalCoinsInt <= 9)
        {
            GameObject.Find("textCoin").GetComponent<Text>().text = "x 0" + totalCoinsInt.ToString();
        }
        else if (totalCoinsInt <= 99)
        {
            GameObject.Find("textCoin").GetComponent<Text>().text = "x " + totalCoinsInt.ToString();
        }
        else
        {
            GameObject.Find("textCoin").GetComponent<Text>().text = "x 00";
            totalCoinsInt = 0;
        }

    }

    IEnumerator renderCoinUIAnim(GameObject goCoinUI)
    {
        int spriteIndex = 0;
        while (true)
        {
            goCoinUI.GetComponent<Image>().sprite = Resources.Load<UnityEngine.Tilemaps.AnimatedTile>("Animations/Tiles/Coin_anim").m_AnimatedSprites[spriteIndex] as Sprite;
            spriteIndex++;

            if (spriteIndex == 3)
            {
                spriteIndex = 0;
            }

            yield return new WaitForSeconds(0.18f);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        playerController = player.GetComponent<PlayerController>();
        //Canvas.ForceUpdateCanvases();
        //canvas = new Canvas();
        //canvas.worldCamera = GameObject.Find("MainCamera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    /*
     * Se llama desde MapManager para generar las diferentes puntuaciones que se consiguen del entorno.
     * scoreType = -1: fireball death
     * scoreType = 1: star
     * scoreType = 2: mush
     * scoreType = 3: yoshiCoin
     * scoreType = 4: coin
     * scoreType = 5: goomba
     * scoreType = 6: koopa troopa (al saltar sobre un koopa troopa se convierte en un caparazon)
     * scoreType = 7: koopa troopa volador (al saltar sobre un koopa troopa volador, se convierte en un koopa troopa normal (scoreType = 6))
     * scoreType = 8: planta carnivora
     * scoreType = 9: break brick block
     */
    public void scoreDispatcher(bool floatingTextEnabled, int scoreType)
    {
        switch (scoreType)
        {
            
            case 1:
            case 2:
            case 3:
                StartCoroutine(generateScore(floatingTextEnabled, 20));
                break;
            case -1:
            case 5:
                StartCoroutine(generateScore(floatingTextEnabled, 2));
                break;
            case 4:
            case 6:
                StartCoroutine(generateScore(floatingTextEnabled, 4));
                break;
            case 7:
                StartCoroutine(generateScore(floatingTextEnabled, 10));
                break;
            default:
                break;
        }

    }

    IEnumerator generateScore(bool floatingTextEnabled, int multiplier)
    {
        int punctBase = 50;
        float counter = 0;
        globalScore = globalScore + (punctBase * multiplier);
        goGlobalScore.GetComponent<Text>().text = globalScore.ToString();

        // Load the Arial font from the Unity Resources folder.

        if (floatingTextEnabled)
        {

            // Create the Text GameObject.
            GameObject goPunctualScore = new GameObject("Text");
            goPunctualScore.transform.parent = goCanvas.transform;
            goPunctualScore.AddComponent<Text>();

            // Set Text component properties.
            text = goPunctualScore.GetComponent<Text>();
            text.font = arial;
            text.fontStyle = FontStyle.Italic;
            text.text = (punctBase * multiplier).ToString();
            text.fontSize = 14;
            text.alignment = TextAnchor.MiddleCenter;

            // Provide Text position and size using RectTransform.
            RectTransform rtPunctualScore;
            rtPunctualScore = text.GetComponent<RectTransform>();

            while (true)
            {
                rtPunctualScore.localPosition = new Vector3(0, counter, 0);
                rtPunctualScore.sizeDelta = new Vector2(500, 200);

                counter = counter + Time.deltaTime * 200;
            

                if (counter >= 110)
                {
                    yield return new WaitForSeconds(0.2f);
                    Destroy(goPunctualScore);
                    break;
                }

                yield return null;

            }

        }




    }

    void OnGUI()
    {
        if (playerController.isDetectedStar && flagStar)
        {
            scoreDispatcher(true, 1);
            flagStar = false;
        }
        if (playerController.isDetectedMush && flagMush)
        {
            scoreDispatcher(true, 2);
            flagMush = false;
        }
        if (playerController.isDetectedFlower && flagFlower)
        {
            scoreDispatcher(true, 3);
            flagFlower = false;
        }


        if (!playerController.isDetectedStar)
        {
            flagStar = true;
        }
        if (!playerController.isDetectedMush)
        {
            flagMush = true;
        }
        if (!playerController.isDetectedFlower)
        {
            flagFlower = true;
        }


    }




}
