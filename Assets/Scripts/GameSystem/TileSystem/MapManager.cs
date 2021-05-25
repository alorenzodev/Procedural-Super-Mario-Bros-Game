using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class MapManager : MonoBehaviour
{
    [SerializeField]
    private Tilemap foreGround;
    [SerializeField]
    private Tilemap detail;

    [SerializeField]
    private GameObject player;

    private Tile UsedSurpriseBox;
    private TileBase voidTileBase;

    private PlayerController playerController;

    [SerializeField]
    private List<TileData> tileDatas;
    
    private Dictionary<TileBase, TileData> dataFromTiles;

    private bool flagForHeadHit = true;

    //surprisebox bouncer control
    [Header("SurpriseBox Bouncer Control")]
    public float bounceHeight = 0.5f;
    public float bounceSpeed = 4f;

    private Vector2 blockOriginalPos;
    private Vector2 blockCoinOriginalPos;
    private Vector2 blockYoshiCoinOriginalPos;
    private Vector2 blockMushOriginalPos;

    [Header("Audio")]
    public AudioSource bumpAtHit;
    public AudioSource coinObtain;
    public AudioSource yoshiCoinObtain;
    public AudioSource itemEmergeFromBlock;
    public AudioSource growUpClip;
    public AudioSource brickBreakClip;

    private GameObject goMushOrFlower;
    public bool isDetectAnythingHitMush;

    //[Space(10)]


    private void Awake()
    {
        dataFromTiles = new Dictionary<TileBase, TileData>();

        foreach (var tileData in tileDatas)
        {
            foreach (var tile in tileData.tiles)
            {
                dataFromTiles.Add(tile, tileData);
            }
        }

    }

    private void Start()
    {
        playerController = player.GetComponent<PlayerController>();

        UsedSurpriseBox = Resources.Load<Tile>("Tiles/overworld_tileset_46") as Tile;

        voidTileBase = ScriptableObject.CreateInstance<Tile>();

    }

    private void FixedUpdate()
    {

        if (playerController.isDetectedGroundOnHead && flagForHeadHit)
        {
            Vector3Int gridUpperPlayerPosition;
            if (!playerController.isBig || playerController.currentAnim == playerController.goingDown || playerController.currentAnim == playerController.onFireGoingDown)
            {
                gridUpperPlayerPosition = foreGround.WorldToCell(playerController.transform.position + new Vector3(0, 0.5f, 0));
                
            }
            else
            {
                gridUpperPlayerPosition = foreGround.WorldToCell(playerController.transform.position + new Vector3(0, 1.5f, 0));
            }

            

            TileBase hitTileBase = foreGround.GetTile(gridUpperPlayerPosition);

            if (hitTileBase && !(playerController.isDetectedGroundOnHead && playerController.isDetectedGroundOnFeet) && (Input.GetKeyDown(KeyCode.Space) || Input.GetKey(KeyCode.Space)))
            {
                bumpAtHit.Play();
                print("En la posicion de grid" + gridUpperPlayerPosition + "existe el siguiente tile: [" + hitTileBase + "]. En el TileMap: " + foreGround.name);

                if (hitTileBase.name.Equals("SurpriseBox_anim"))
                {
                    blockHeadHitAnim(hitTileBase, voidTileBase, gridUpperPlayerPosition);
                    randomItemSurpriseBox(gridUpperPlayerPosition);
                }
                //overworld_tileset_69: correspondiente al tile de ladrillo
                if (hitTileBase.name.Equals("overworld_tileset_69"))
                {
                    if (!playerController.isBig)
                    {
                        blockHeadHitAnim(hitTileBase, voidTileBase, gridUpperPlayerPosition);
                    }
                    else
                    {
                        blockDestroyHitAnim(hitTileBase, voidTileBase, gridUpperPlayerPosition);
                    }
                }

            }
            else if (hitTileBase && (playerController.isDetectedGroundOnHead && playerController.isDetectedGroundOnFeet) && Input.GetKeyDown(KeyCode.Space))
            {
                bumpAtHit.Play();
                playerController.setAnim(2, playerController.isBig, playerController.isOnFire);
                print("En la posicion de grid" + gridUpperPlayerPosition + "existe el siguiente tile: [" + hitTileBase + "]. En el TileMap: " + foreGround.name);
                if (hitTileBase.name.Equals("SurpriseBox_anim"))
                {
                    blockHeadHitAnim(hitTileBase, voidTileBase, gridUpperPlayerPosition);
                    randomItemSurpriseBox(gridUpperPlayerPosition);
                }
                //overworld_tileset_69: correspondiente al tile de ladrillo
                if (hitTileBase.name.Equals("overworld_tileset_69"))
                {
                    if (!playerController.isBig)
                    {
                        blockHeadHitAnim(hitTileBase, voidTileBase, gridUpperPlayerPosition);
                    }
                    else
                    {
                        blockDestroyHitAnim(hitTileBase, voidTileBase, gridUpperPlayerPosition);
                    }
                }
            }
            else if (!hitTileBase)
            {
                gridUpperPlayerPosition = foreGround.WorldToCell(playerController.transform.position - new Vector3(0, 0.5f, 0));
                hitTileBase = foreGround.GetTile(gridUpperPlayerPosition);
                print("En la posicion de grid" + gridUpperPlayerPosition + "existe el siguiente tile: [" + hitTileBase + "]. En el TileMap: " + foreGround.name);

                if (hitTileBase && hitTileBase.name.Equals("SurpriseBox_anim"))
                {
                    blockHeadHitAnim(hitTileBase, voidTileBase, gridUpperPlayerPosition);
                    randomItemSurpriseBox(gridUpperPlayerPosition);
                }
                //overworld_tileset_69: correspondiente al tile de ladrillo
                if (hitTileBase && hitTileBase.name.Equals("overworld_tileset_69"))
                {
                    if (!playerController.isBig)
                    {
                        blockHeadHitAnim(hitTileBase, voidTileBase, gridUpperPlayerPosition);
                    }
                    else
                    {
                        blockDestroyHitAnim(hitTileBase, voidTileBase, gridUpperPlayerPosition);
                    }
                    
                }
            
            }
            flagForHeadHit = false;

        }

        if (playerController.isDetectedGroundOnFeet)
        {
            flagForHeadHit = true;
        }


        Vector3Int playerPosition;

        if (playerController.isBig)
        {
            playerPosition = detail.WorldToCell(playerController.transform.position);

            TileBase tileBaseCoin = detail.GetTile(playerPosition);

            if (tileBaseCoin && tileBaseCoin.name.Equals("Coin_anim"))
            {
                print("En la posicion de grid" + playerPosition + "existe el siguiente tile: [" + tileBaseCoin + "]. En el TileMap: " + detail.name);
                coinObtain.Play();
                detail.SetTile(playerPosition, voidTileBase);
            }

            playerPosition = detail.WorldToCell(playerController.transform.position + new Vector3(0, 0.5f));
            tileBaseCoin = detail.GetTile(playerPosition);

            if (tileBaseCoin && tileBaseCoin.name.Equals("Coin_anim"))
            {
                print("En la posicion de grid" + playerPosition + "existe el siguiente tile: [" + tileBaseCoin + "]. En el TileMap: " + detail.name);
                coinObtain.Play();
                detail.SetTile(playerPosition, voidTileBase);
            }

        }
        else
        {
            playerPosition = detail.WorldToCell(playerController.transform.position);

            TileBase tileBaseCoin = detail.GetTile(playerPosition);

            if (tileBaseCoin && tileBaseCoin.name.Equals("Coin_anim"))
            {
                print("En la posicion de grid" + playerPosition + "existe el siguiente tile: [" + tileBaseCoin + "]. En el TileMap: " + detail.name);
                coinObtain.Play();
                detail.SetTile(playerPosition, voidTileBase);
            }
        }

        

        if (playerController.isDetectedMush && GameObject.FindGameObjectWithTag("mush"))
        {
            Destroy(GameObject.FindGameObjectWithTag("mush"));
            if (!playerController.isBig)
            {
                growUpClip.Play();
            }
            else
            {
                yoshiCoinObtain.Play();
            }
                
        }

        if (playerController.isDetectedFlower && GameObject.FindGameObjectWithTag("flower"))
        {
            Destroy(GameObject.FindGameObjectWithTag("flower"));
            growUpClip.Play();
        }

    }

    private void blockDestroyHitAnim(TileBase hitTileBase, TileBase voidTileBase, Vector3Int gridPosition)
    {
        brickBreakClip.Play();

        GameObject goBrickBlockFrag1 = new GameObject("brickblock_frag1");
        GameObject goBrickBlockFrag2 = new GameObject("brickblock_frag2");
        GameObject goBrickBlockFrag3 = new GameObject("brickblock_frag3");
        GameObject goBrickBlockFrag4 = new GameObject("brickblock_frag4");

        Tile brickBlockFragTile = Resources.Load<Tile>("Tiles/items_objects_NPC_spritesheet_31") as Tile;
        Sprite spriteItemTile = brickBlockFragTile.sprite;

        goBrickBlockFrag1.transform.position = foreGround.CellToWorld(gridPosition) + new Vector3(0.25f, 0.75f, 0);
        goBrickBlockFrag2.transform.position = foreGround.CellToWorld(gridPosition) + new Vector3(0.75f, 0.75f, 0);
        goBrickBlockFrag3.transform.position = foreGround.CellToWorld(gridPosition) + new Vector3(0.25f, 0.25f, 0);
        goBrickBlockFrag4.transform.position = foreGround.CellToWorld(gridPosition) + new Vector3(0.75f, 0.25f, 0);

        goBrickBlockFrag1.AddComponent<SpriteRenderer>();
        goBrickBlockFrag2.AddComponent<SpriteRenderer>();
        goBrickBlockFrag3.AddComponent<SpriteRenderer>();
        goBrickBlockFrag4.AddComponent<SpriteRenderer>();

        goBrickBlockFrag1.AddComponent<Rigidbody2D>();
        goBrickBlockFrag2.AddComponent<Rigidbody2D>();
        goBrickBlockFrag3.AddComponent<Rigidbody2D>();
        goBrickBlockFrag4.AddComponent<Rigidbody2D>();

        goBrickBlockFrag1.GetComponent<Rigidbody2D>().gravityScale = 3;
        goBrickBlockFrag2.GetComponent<Rigidbody2D>().gravityScale = 3;
        goBrickBlockFrag3.GetComponent<Rigidbody2D>().gravityScale = 3;
        goBrickBlockFrag4.GetComponent<Rigidbody2D>().gravityScale = 3;

        goBrickBlockFrag1.GetComponent<SpriteRenderer>().sprite = spriteItemTile;
        goBrickBlockFrag2.GetComponent<SpriteRenderer>().sprite = spriteItemTile;
        goBrickBlockFrag3.GetComponent<SpriteRenderer>().sprite = spriteItemTile;
        goBrickBlockFrag4.GetComponent<SpriteRenderer>().sprite = spriteItemTile;

        goBrickBlockFrag1.GetComponent<Rigidbody2D>().AddForce(new Vector2(-2, 2), ForceMode2D.Impulse);
        goBrickBlockFrag2.GetComponent<Rigidbody2D>().AddForce(new Vector2(2, 2), ForceMode2D.Impulse);
        goBrickBlockFrag3.GetComponent<Rigidbody2D>().AddForce(new Vector2(-1, 1), ForceMode2D.Impulse);
        goBrickBlockFrag4.GetComponent<Rigidbody2D>().AddForce(new Vector2(1, 1), ForceMode2D.Impulse);

        //Añadir rotacion de los fragmentos.

        StartCoroutine(brickBlockFragRotation(goBrickBlockFrag1));
        StartCoroutine(brickBlockFragRotation(goBrickBlockFrag2));
        StartCoroutine(brickBlockFragRotation(goBrickBlockFrag3));
        StartCoroutine(brickBlockFragRotation(goBrickBlockFrag4));

        foreGround.SetTile(gridPosition, voidTileBase);

    }

    private void blockHeadHitAnim(TileBase hitTileBase, TileBase voidTileBase, Vector3Int gridPosition)
    {
        
        Sprite spriteHitTile = foreGround.GetSprite(gridPosition);

        GameObject goSpriteHitTile = new GameObject("BlockBox");

        goSpriteHitTile.AddComponent<SpriteRenderer>();
        goSpriteHitTile.AddComponent<BoxCollider2D>();
        goSpriteHitTile.AddComponent<Rigidbody2D>();

        goSpriteHitTile.transform.position = foreGround.CellToWorld(gridPosition) + new Vector3(0.5f, 0.5f, 0);

        goSpriteHitTile.GetComponent<SpriteRenderer>().sprite = spriteHitTile;
        goSpriteHitTile.GetComponent<BoxCollider2D>().size = new Vector2(1, 1);

        goSpriteHitTile.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        blockOriginalPos = goSpriteHitTile.transform.position;

        StartCoroutine(blockBounce(goSpriteHitTile, gridPosition, hitTileBase));
        goSpriteHitTile.tag = "Ground";
        goSpriteHitTile.layer = 8;

        foreGround.SetTile(gridPosition, voidTileBase);

    }
    //Randomiza que es lo que sale de una surprisebox. 10% Seta/Flor, 60% moneda, 25% monedas entre 2 y 8, 5% Estrella
    private void randomItemSurpriseBox(Vector3Int gridPosition)
    {
        var random = UnityEngine.Random.Range(0f, 1f);

        if (random <= 0.02f)
        {
            starItem(gridPosition);
        }
        else if (random <= 1f)
        {
            mushItem(gridPosition);
        }
        else if (random <= 0.2f)
        {
            yoshiCoinItem(gridPosition);
        }
        else if (random <= 1)
        {
            coinItem(gridPosition);
        }





    }

    private void mushItem(Vector3Int gridPosition)
    {

        if (!playerController.isBig)
        {
            bool flagExistingMush = false;

            if (GameObject.Find("mushroom"))
            {
                flagExistingMush = true;
            }

            goMushOrFlower = new GameObject("mushroom");
            Tile mushItem = Resources.Load<Tile>("Tiles/items_objects_NPC_spritesheet_0") as Tile;
            Sprite spriteItemTile = mushItem.sprite;

            goMushOrFlower.AddComponent<SpriteRenderer>();

            goMushOrFlower.transform.position = foreGround.CellToWorld(gridPosition) + new Vector3(0.5f, 0.7f, 0);

            blockMushOriginalPos = goMushOrFlower.transform.position;

            goMushOrFlower.GetComponent<SpriteRenderer>().sprite = spriteItemTile;
            goMushOrFlower.GetComponent<SpriteRenderer>().sortingLayerName = "Foreground";
            goMushOrFlower.GetComponent<SpriteRenderer>().sortingOrder = 9;
            goMushOrFlower.tag = "mush";
            goMushOrFlower.layer = 9;

            StartCoroutine(mushMovementAndDetect(goMushOrFlower, flagExistingMush));
        }
        else if(playerController.isBig)
        {
            bool flagExistingFlower = false;

            if (GameObject.Find("flower"))
            {
                flagExistingFlower = true;
            }

            goMushOrFlower = new GameObject("flower");
            AnimatedTile mushItem = Resources.Load<AnimatedTile>("Animations/Tiles/FireFlower_anim") as AnimatedTile;
            Sprite[] spriteItemTile = mushItem.m_AnimatedSprites;

            goMushOrFlower.AddComponent<SpriteRenderer>();

            goMushOrFlower.transform.position = foreGround.CellToWorld(gridPosition) + new Vector3(0.5f, 0.7f, 0);

            blockMushOriginalPos = goMushOrFlower.transform.position;

            goMushOrFlower.GetComponent<SpriteRenderer>().sprite = spriteItemTile[0];
            goMushOrFlower.GetComponent<SpriteRenderer>().sortingLayerName = "Foreground";
            goMushOrFlower.GetComponent<SpriteRenderer>().sortingOrder = 9;
            goMushOrFlower.tag = "flower";
            goMushOrFlower.layer = 10;

            StartCoroutine(flowerMovementAndDetect(goMushOrFlower, flagExistingFlower));
            StartCoroutine(flowerAnim(goMushOrFlower, spriteItemTile));

        }



        return;
    }

    private void coinItem(Vector3Int gridPosition)
    {
        coinObtain.Play();
        GameObject goSpriteItemTile = new GameObject("coinSpriteBounded");

        Tile coinItem = Resources.Load<Tile>("Tiles/items_objects_NPC_spritesheet_204") as Tile;
        Sprite spriteItemTile = coinItem.sprite;
        
        goSpriteItemTile.AddComponent<SpriteRenderer>();

        goSpriteItemTile.transform.position = foreGround.CellToWorld(gridPosition) + new Vector3(0.5f, 1.5f, 0);

        blockCoinOriginalPos = goSpriteItemTile.transform.position;

        goSpriteItemTile.GetComponent<SpriteRenderer>().sprite = spriteItemTile;


        StartCoroutine(coinBlockBounce(goSpriteItemTile));


    }

    private void yoshiCoinItem(Vector3Int gridPosition)
    {
        yoshiCoinObtain.Play();
        GameObject goSpriteItemTile = new GameObject("yoshiCoinSpriteBounded");

        Tile yoshiCoinItem = Resources.Load<Tile>("Tiles/yoshi_coin_0") as Tile;
        Sprite spriteItemTile = yoshiCoinItem.sprite;

        goSpriteItemTile.AddComponent<SpriteRenderer>();

        goSpriteItemTile.transform.position = foreGround.CellToWorld(gridPosition) + new Vector3(0.5f, 1.5f, 0);

        blockYoshiCoinOriginalPos = goSpriteItemTile.transform.position;

        goSpriteItemTile.GetComponent<SpriteRenderer>().sprite = spriteItemTile;


        StartCoroutine(yoshiCoinBlockAnim(goSpriteItemTile));
    }



    private void starItem(Vector3Int gridPosition)
    {
        return;
    }

    IEnumerator growUpBlockAndAnim()
    {

        while (true)
        {
            UnityEngine.WaitForSeconds waitTime = new WaitForSeconds(3f);
            playerController.growUp();
            
            yield return waitTime;
        }
    }

    IEnumerator brickBlockFragRotation(GameObject goSpriteItemTile)
    {

        float startRotation = goSpriteItemTile.transform.eulerAngles.y;
        float endRotation = startRotation + 360.0f;
        float t = 0.0f;

        while (true)
        {
            t += Time.deltaTime;
            float zRotation = Mathf.Lerp(startRotation, endRotation, t);
            goSpriteItemTile.transform.eulerAngles = new Vector3(goSpriteItemTile.transform.eulerAngles.x, goSpriteItemTile.transform.eulerAngles.y, zRotation);

            if (t >= 2)
            {
                Destroy(goSpriteItemTile);
                break;
            }

            yield return null;
        }
        
    }

    IEnumerator flowerAnim(GameObject goMushOrFlower, Sprite[] spriteItemTile)
    {
        var t = 0f;
        var indexAnim = 0;
        UnityEngine.WaitForSeconds waitTime = new WaitForSeconds(0.05f);

        while (true)
        {
            t += Time.deltaTime;

            if (goMushOrFlower)
            {
                goMushOrFlower.GetComponent<SpriteRenderer>().sprite = spriteItemTile[indexAnim];

                if (indexAnim == spriteItemTile.Length - 1)
                {
                    indexAnim = 0;
                }
                else
                {
                    indexAnim++;
                }
            }
            else
            {
                break;
            }
                


            yield return waitTime;
        }

    }

    IEnumerator flowerMovementAndDetect(GameObject goSpriteItemTile, bool flagExistingFlower)
    {
        while (true)
        {
            goSpriteItemTile.transform.position = new Vector2(goMushOrFlower.transform.position.x, goSpriteItemTile.transform.position.y + (bounceSpeed / 2.2f) * Time.deltaTime);

            if (goSpriteItemTile.transform.position.y.CompareTo((blockMushOriginalPos + new Vector2(0, bounceHeight * 1.8f)).y) >= 0)
            {
                goSpriteItemTile.AddComponent<Rigidbody2D>();
                goSpriteItemTile.AddComponent<BoxCollider2D>();

                goSpriteItemTile.GetComponent<Rigidbody2D>().freezeRotation = true;
                goSpriteItemTile.GetComponent<Rigidbody2D>().gravityScale = 3f;
                goSpriteItemTile.GetComponent<Rigidbody2D>().mass = 10;

                if (flagExistingFlower)
                {
                    UnityEngine.WaitForSeconds waitTime = new WaitForSeconds(0.15f);

                    yield return waitTime;
                    Destroy(goSpriteItemTile);
                }

                break;
            }

            yield return null;
        }


        yield return null;
    }
    IEnumerator mushMovementAndDetect(GameObject goSpriteItemTile, bool flagExistingMush)
    {
        var mushDir = 1;

        itemEmergeFromBlock.Play();
        //movimiento en Y para salir de la surprise box
        while (true)
        {
            goSpriteItemTile.transform.position = new Vector2(goSpriteItemTile.transform.position.x, goSpriteItemTile.transform.position.y + (bounceSpeed/2.2f) * Time.deltaTime);

            if (goSpriteItemTile.transform.position.y.CompareTo((blockMushOriginalPos + new Vector2(0, bounceHeight * 1.8f)).y) >= 0)
            {
                goSpriteItemTile.AddComponent<Rigidbody2D>();
                goSpriteItemTile.AddComponent<BoxCollider2D>();

                goSpriteItemTile.GetComponent<Rigidbody2D>().freezeRotation = true;
                goSpriteItemTile.GetComponent<Rigidbody2D>().gravityScale = 3f;
                goSpriteItemTile.GetComponent<Rigidbody2D>().mass = 10;

                if (flagExistingMush)
                {
                    UnityEngine.WaitForSeconds waitTime = new WaitForSeconds(0.15f);

                    yield return waitTime;
                    Destroy(goSpriteItemTile);
                }

                if (playerController.transform.position.x.CompareTo(goSpriteItemTile.transform.position.x) >= 0)
                {
                    mushDir = -1;
                }

                break;
            }

            yield return null;
        }


        //cambiar condicion para que se mueva, si detecta colision, cambie de direccion.
        while (true)
        {
            if (!goSpriteItemTile.Equals(null))
            {
                isDetectAnythingHitMush = Physics2D.OverlapBox(new Vector2(goSpriteItemTile.transform.position.x, goSpriteItemTile.transform.position.y), new Vector2(goSpriteItemTile.GetComponent<BoxCollider2D>().size.x * 0.9f, goSpriteItemTile.GetComponent<BoxCollider2D>().size.y / 2.5f), 0.0f, playerController.whatIsGround);
                goSpriteItemTile.transform.position = new Vector2(goSpriteItemTile.transform.position.x + (bounceSpeed / 1.5f) * Time.deltaTime * mushDir, goSpriteItemTile.transform.position.y);
            }

            if (!goSpriteItemTile.Equals(null) && isDetectAnythingHitMush)
            {
                if (mushDir == 1)
                {
                    mushDir = -1;
                }
                else
                {
                    mushDir = 1;
                }

                if (mushDir == -1)
                {
                    mushDir = 1;
                }
                else
                {
                    mushDir = -1;
                }
            }



            yield return null;
        }
    }

    IEnumerator yoshiCoinBlockAnim(GameObject goSpriteItemTile)
    {
        while (true)
        {
            goSpriteItemTile.transform.position = new Vector2(goSpriteItemTile.transform.position.x, goSpriteItemTile.transform.position.y + bounceSpeed * 2 * Time.deltaTime);

            if (goSpriteItemTile.transform.position.y.CompareTo((blockOriginalPos + new Vector2(0, bounceHeight * 1.5f)).y) >= 0)
            {
                Tile coinItem = Resources.Load<Tile>("Tiles/yoshi_coin_3") as Tile;
                Sprite spriteItemTile = coinItem.sprite;
                goSpriteItemTile.GetComponent<SpriteRenderer>().sprite = spriteItemTile;
                
            }

            if (goSpriteItemTile.transform.position.y.CompareTo((blockOriginalPos + new Vector2(0, bounceHeight * 3f)).y) >= 0)
            {
                Tile coinItem = Resources.Load<Tile>("Tiles/yoshi_coin_4") as Tile;
                Sprite spriteItemTile = coinItem.sprite;
                goSpriteItemTile.GetComponent<SpriteRenderer>().sprite = spriteItemTile;
            }

            if (goSpriteItemTile.transform.position.y.CompareTo((blockOriginalPos + new Vector2(0, bounceHeight * 4.5f)).y) >= 0)
            {
                Tile coinItem = Resources.Load<Tile>("Tiles/yoshi_coin_6") as Tile;
                Sprite spriteItemTile = coinItem.sprite;
                goSpriteItemTile.GetComponent<SpriteRenderer>().sprite = spriteItemTile;
                
            }

            if (goSpriteItemTile.transform.position.y.CompareTo((blockOriginalPos + new Vector2(0, bounceHeight * 6f)).y) >= 0)
            {
                Tile coinItem = Resources.Load<Tile>("Tiles/yoshi_coin_5") as Tile;
                Sprite spriteItemTile = coinItem.sprite;
                goSpriteItemTile.GetComponent<SpriteRenderer>().sprite = spriteItemTile;
                break;
            }


            yield return null;

        }

        while (true)
        {
            goSpriteItemTile.transform.position = new Vector2(goSpriteItemTile.transform.position.x, goSpriteItemTile.transform.position.y - bounceSpeed * 2 * Time.deltaTime);

            if (goSpriteItemTile.transform.position.y.CompareTo((blockOriginalPos + new Vector2(0, bounceHeight * 4.5f)).y) <= 0)
            {
                Tile coinItem = Resources.Load<Tile>("Tiles/yoshi_coin_3") as Tile;
                Sprite spriteItemTile = coinItem.sprite;
                goSpriteItemTile.GetComponent<SpriteRenderer>().sprite = spriteItemTile;
            }

            if (goSpriteItemTile.transform.position.y.CompareTo((blockOriginalPos + new Vector2(0, bounceHeight * 3f)).y) <= 0)
            {
                Tile coinItem = Resources.Load<Tile>("Tiles/yoshi_coin_4") as Tile;
                Sprite spriteItemTile = coinItem.sprite;
                goSpriteItemTile.GetComponent<SpriteRenderer>().sprite = spriteItemTile;
            }

            if (goSpriteItemTile.transform.position.y.CompareTo((blockOriginalPos + new Vector2(0, bounceHeight * 1.5f)).y) <= 0)
            {
                Tile coinItem = Resources.Load<Tile>("Tiles/yoshi_coin_6") as Tile;
                Sprite spriteItemTile = coinItem.sprite;
                goSpriteItemTile.GetComponent<SpriteRenderer>().sprite = spriteItemTile;
            }

            if (goSpriteItemTile.transform.position.y.CompareTo(blockYoshiCoinOriginalPos.y) <= 0)
            {
                Tile coinItem = Resources.Load<Tile>("Tiles/yoshi_coin_5") as Tile;
                Sprite spriteItemTile = coinItem.sprite;
                goSpriteItemTile.GetComponent<SpriteRenderer>().sprite = spriteItemTile;
                goSpriteItemTile.transform.position = blockCoinOriginalPos;

                Destroy(goSpriteItemTile);
                break;
            }

            yield return null;

        }


        yield return null;
    }

    IEnumerator coinBlockBounce(GameObject goSpriteItemTile)
    {
        while (true)
        {
            goSpriteItemTile.transform.position = new Vector2(goSpriteItemTile.transform.position.x, goSpriteItemTile.transform.position.y + bounceSpeed * 2 * Time.deltaTime);

            if (goSpriteItemTile.transform.position.y.CompareTo((blockOriginalPos + new Vector2(0, bounceHeight * 1.5f)).y) >= 0)
            {
                Tile coinItem = Resources.Load<Tile>("Tiles/items_objects_NPC_spritesheet_205") as Tile;
                Sprite spriteItemTile = coinItem.sprite;
                goSpriteItemTile.GetComponent<SpriteRenderer>().sprite = spriteItemTile;
            }

            if (goSpriteItemTile.transform.position.y.CompareTo((blockOriginalPos + new Vector2(0, bounceHeight * 3f)).y) >= 0)
            {
                Tile coinItem = Resources.Load<Tile>("Tiles/items_objects_NPC_spritesheet_206") as Tile;
                Sprite spriteItemTile = coinItem.sprite;
                goSpriteItemTile.GetComponent<SpriteRenderer>().sprite = spriteItemTile;
            }

            if (goSpriteItemTile.transform.position.y.CompareTo((blockOriginalPos + new Vector2(0, bounceHeight * 4.5f)).y) >= 0)
            {
                Tile coinItem = Resources.Load<Tile>("Tiles/items_objects_NPC_spritesheet_207") as Tile;
                Sprite spriteItemTile = coinItem.sprite;
                goSpriteItemTile.GetComponent<SpriteRenderer>().sprite = spriteItemTile;
            }

            if (goSpriteItemTile.transform.position.y.CompareTo((blockOriginalPos + new Vector2(0, bounceHeight * 6f)).y) >= 0)
            {
                Tile coinItem = Resources.Load<Tile>("Tiles/items_objects_NPC_spritesheet_204") as Tile;
                Sprite spriteItemTile = coinItem.sprite;
                goSpriteItemTile.GetComponent<SpriteRenderer>().sprite = spriteItemTile;
                break;
            }

            yield return null;

        }

        while (true)
        {
            goSpriteItemTile.transform.position = new Vector2(goSpriteItemTile.transform.position.x, goSpriteItemTile.transform.position.y - bounceSpeed * 2 * Time.deltaTime);

            if (goSpriteItemTile.transform.position.y.CompareTo((blockOriginalPos + new Vector2(0, bounceHeight * 4.5f)).y) <= 0)
            {
                Tile coinItem = Resources.Load<Tile>("Tiles/items_objects_NPC_spritesheet_205") as Tile;
                Sprite spriteItemTile = coinItem.sprite;
                goSpriteItemTile.GetComponent<SpriteRenderer>().sprite = spriteItemTile;
            }

            if (goSpriteItemTile.transform.position.y.CompareTo((blockOriginalPos + new Vector2(0, bounceHeight * 3f)).y) <= 0)
            {
                Tile coinItem = Resources.Load<Tile>("Tiles/items_objects_NPC_spritesheet_206") as Tile;
                Sprite spriteItemTile = coinItem.sprite;
                goSpriteItemTile.GetComponent<SpriteRenderer>().sprite = spriteItemTile;
            }

            if (goSpriteItemTile.transform.position.y.CompareTo((blockOriginalPos + new Vector2(0, bounceHeight * 1.5f)).y) <= 0)
            {
                Tile coinItem = Resources.Load<Tile>("Tiles/items_objects_NPC_spritesheet_207") as Tile;
                Sprite spriteItemTile = coinItem.sprite;
                goSpriteItemTile.GetComponent<SpriteRenderer>().sprite = spriteItemTile;
            }

            if (goSpriteItemTile.transform.position.y.CompareTo(blockCoinOriginalPos.y) <= 0)
            {
                Tile coinItem = Resources.Load<Tile>("Tiles/items_objects_NPC_spritesheet_204") as Tile;
                Sprite spriteItemTile = coinItem.sprite;
                goSpriteItemTile.GetComponent<SpriteRenderer>().sprite = spriteItemTile;
                goSpriteItemTile.transform.position = blockCoinOriginalPos;
                


                Destroy(goSpriteItemTile);
                break;
            }

            yield return null;

        }


        yield return null;
    }

    IEnumerator blockBounce(GameObject goSpriteHitTile, Vector3Int gridPosition, TileBase hitTileBase)
    {
        while (true)
        {
            goSpriteHitTile.transform.position = new Vector2(goSpriteHitTile.transform.position.x, goSpriteHitTile.transform.position.y + bounceSpeed * Time.deltaTime);

            if (goSpriteHitTile.transform.position.y.CompareTo((blockOriginalPos + new Vector2(0, bounceHeight)).y) >= 0)
            {
                break;
            }

            yield return null;

        }

        while (true)
        {
            goSpriteHitTile.transform.position = new Vector2(goSpriteHitTile.transform.position.x, goSpriteHitTile.transform.position.y - bounceSpeed * Time.deltaTime);

            if (goSpriteHitTile.transform.position.y.CompareTo(blockOriginalPos.y) <= 0)
            {
                goSpriteHitTile.transform.position = blockOriginalPos;

                if (hitTileBase && hitTileBase.name.Equals("SurpriseBox_anim"))
                {
                    foreGround.SetTile(gridPosition, UsedSurpriseBox);
                }else if ((hitTileBase && hitTileBase.name.Equals("overworld_tileset_69")))
                {
                    foreGround.SetTile(gridPosition, hitTileBase);
                }
                Destroy(goSpriteHitTile);
                break;
            }

            yield return null;

        }
    }
}

