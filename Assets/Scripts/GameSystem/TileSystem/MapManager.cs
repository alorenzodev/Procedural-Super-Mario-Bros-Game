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

    private GameObject goMush;

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
            
            Vector3Int gridUpperPlayerPosition = foreGround.WorldToCell(playerController.transform.position + new Vector3(0, 0.5f, 0));

            TileBase hitTileBase = foreGround.GetTile(gridUpperPlayerPosition);

            if (hitTileBase && !(playerController.isDetectedGroundOnHead && playerController.isDetectedGroundOnFeet))
            {
                bumpAtHit.Play();
                //print("En la posicion de grid" + gridUpperPlayerPosition + "existe el siguiente tile: [" + hitTileBase + "]. En el TileMap: " + map.name);

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
                        //blockDestroyHitAnim(hitTileBase, voidTileBase, gridPosition);
                    }
                }

            }
            else if (hitTileBase && (playerController.isDetectedGroundOnHead && playerController.isDetectedGroundOnFeet) && Input.GetKeyDown(KeyCode.Space))
            {
                bumpAtHit.Play();
                playerController.setAnim(2, playerController.isBig);
                //print("En la posicion de grid" + gridUpperPlayerPosition + "existe el siguiente tile: [" + hitTileBase + "]. En el TileMap: " + map.name);
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
                        //blockDestroyHitAnim(hitTileBase, voidTileBase, gridPosition);
                    }
                }
            }
            else if (!hitTileBase)
            {
                gridUpperPlayerPosition = foreGround.WorldToCell(playerController.transform.position - new Vector3(0, 0.5f, 0));
                hitTileBase = foreGround.GetTile(gridUpperPlayerPosition);
                //print("En la posicion de grid" + gridUpperPlayerPosition + "existe el siguiente tile: [" + hitTileBase + "]. En el TileMap: " + map.name);

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
                        //blockDestroyHitAnim(hitTileBase, voidTileBase, gridPosition);
                    }
                    
                }
            
            }
            flagForHeadHit = false;

        }

        if (playerController.isDetectedGroundOnFeet)
        {
            flagForHeadHit = true;
        }


        Vector3Int playerPosition = detail.WorldToCell(playerController.transform.position);
        TileBase tileBaseCoin = detail.GetTile(playerPosition);

        if (tileBaseCoin && tileBaseCoin.name.Equals("Coin_anim"))
        {
            print("En la posicion de grid" + playerPosition + "existe el siguiente tile: [" + tileBaseCoin + "]. En el TileMap: " + detail.name);
            coinObtain.Play();
            detail.SetTile(playerPosition, voidTileBase);
        }

        if (playerController.isDetectedMush && GameObject.FindGameObjectWithTag("mush"))
        {
            Destroy(GameObject.FindGameObjectWithTag("mush"));
        }

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
        bool flagExistingMush = false;

        if (GameObject.Find("mushroom"))
        {
            flagExistingMush = true;
        }

        goMush = new GameObject("mushroom");
        Tile mushItem = Resources.Load<Tile>("Tiles/items_objects_NPC_spritesheet_0") as Tile;
        Sprite spriteItemTile = mushItem.sprite;

        goMush.AddComponent<SpriteRenderer>();

        goMush.transform.position = foreGround.CellToWorld(gridPosition) + new Vector3(0.5f, 0.7f, 0);

        blockMushOriginalPos = goMush.transform.position;

        goMush.GetComponent<SpriteRenderer>().sprite = spriteItemTile;
        goMush.GetComponent<SpriteRenderer>().sortingLayerName = "Foreground";
        goMush.GetComponent<SpriteRenderer>().sortingOrder = 9;
        goMush.tag = "mush";
        goMush.layer = 9;

        StartCoroutine(mushMovementAndDetect(goMush, flagExistingMush));

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

    IEnumerator mushMovementAndDetect(GameObject goSpriteItemTile, bool flagExistingMush)
    {
        //movimiento en Y para salir de la surprise box
        while (true)
        {
            goSpriteItemTile.transform.position = new Vector2(goSpriteItemTile.transform.position.x, goSpriteItemTile.transform.position.y + (bounceSpeed/2) * Time.deltaTime);

            if (goSpriteItemTile.transform.position.y.CompareTo((blockMushOriginalPos + new Vector2(0, bounceHeight * 1.8f)).y) >= 0)
            {
                goSpriteItemTile.AddComponent<Rigidbody2D>();
                goSpriteItemTile.AddComponent<BoxCollider2D>();

                goSpriteItemTile.GetComponent<Rigidbody2D>().freezeRotation = true;

                if (flagExistingMush)
                {
                    Destroy(goSpriteItemTile);
                }

                break;
            }

            yield return null;
        }

        //cambiar condicion para que se mueva, si detecta colision, cambie de direccion, y cuando detecte al personaje, desaparezca.
        //while (true)
        //{



        //    yield return null;
        //}

        yield return null;
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

