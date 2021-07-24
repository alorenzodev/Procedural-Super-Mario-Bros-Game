using UnityEngine;
using UnityEngine.Audio;

public class PlayerController : MonoBehaviour
{
    //Render que nos pintará los sprites
#pragma warning disable CS0108 // El miembro oculta el miembro heredado. Falta una contraseña nueva
    public SpriteRenderer renderer;
    [Space(10)]
#pragma warning restore CS0108 // El miembro oculta el miembro heredado. Falta una contraseña nueva

    //Variables relacionadas con el movimiento y las físicas de Mario con respecto del mundo
    [Header("Movimiento")]
    public float speed = 5.1f;
    public float speedUp = 5f;
    public float maxSpeed = 12.0f;
    public float velocity;
    [Space(5)]

    //variables relacionadas con el salto
    [Header("Salto")]
    public float jumpForce;
    public float jumpTime;
    private float jumpTimeCounter;
    private bool isJumping;
    [Space(10)]
    //Componente del player que utilizaremos para diferentes cálculos
    private Rigidbody2D rb;

    //para saber si esta en el suelo
    [Header("Detectores de movimiento")]
    public bool isDetectedGroundOnFeet;
    public bool isDetectedGroundOnLeft;
    public bool isDetectedGroundOnRight;
    public bool isDetectedGroundOnHead;
    [Header("Detectores de elementos en la capa Default")]
    public bool isDetectedMush;
    public bool isDetectedMushOnFeet;
    public bool isDetectedFlower;
    public bool isDetectedStar;

    [Space(5)]
    //objetos detectores
    [Header("Objetos detectores")]
    public Transform feetPos;
    public Transform headPos;
    public Transform leftPos;
    public Transform rightPos;
    public float circleRadius;
    [Space(5)]
    //Tamaño de detectores
    [Header("Tamaño detectores")]
    public Vector2 boxSizeY;
    public Vector2 boxSizeFeet;
    public Vector2 boxSizeHead;
    public LayerMask whatIsGround;
    public LayerMask whatIsMush;
    public LayerMask whatIsFlower;
    public LayerMask whatIsStar;
    [Space(5)]
    //audio
    [Header("Audio")]
    public AudioSource smallJumpClip;
    public AudioSource bigJumpClip;
    [Space(10)]

    //Indicador de dirección
    [Header("Indicador de dirección")]
    public static bool changeDir = false; //indica el cambio de dirección para diferentes cálculos
    private float horizontalDirection = 0;
    [Space(10)]
    //Para saber si Mario es grande
    [Header("Indicador isBig")]
    public bool isBig = false;
    public bool isOnFire = false;
    public bool isStar = false;
    [Space(10)]
    //Sets de sprites preestablecidos
    public Sprite[] smallIdle, smallWalk, smallJump;
    [Space(5)]
    public Sprite[] bigIdle, bigWalk, bigJump;
    public Sprite[] grow;
    public Sprite[] goingDown;
    public Sprite[] onFireGoingDown;
    public Sprite[] onFireGrow;
    public Sprite[] onFireIdle, onFireWalk, onFireJump;
    public Sprite[] onFireBall;
    [Space(10)]
    //Cola de sprites a renderizar
    public Sprite[] currentAnim;
    private int animState = -1;
    public int currentSprite;
    private bool flagGrowAnim;
    private bool flagOnFireAnim;
    private bool flagShootingFireBall;
    private float counterGrowAnim = 0;
    private float counterOnFireAnim = 0;
    private float counterShootingFireBall = 0;
    private bool isDown = false;
    private bool isGoingDown = false;
    private bool isGoingUp = false;
    private static bool canMove = true;
    public bool shootingFireBall = false;


    //Reloj que cuenta el tiempo entre frames
    private float animClock = 0.0f;

    private static Transform p;

    //Este método nos permite acceder a la posición, la rotación y la escala de Mario, dado el código en Start().
    public static Transform getTransform()
    {
        return p;
    }

    // Start is called before the first frame update
    void Start()
    {
        p = this.transform; //Asignamos el transform de Mario a una variable.
        setAnim(0, isBig, isOnFire); //sprite por defecto
        rb = GetComponent<Rigidbody2D>(); //Asignamos el Rigidbody del player a una variable
    }

    // Update is called once per frame
    void Update()
    {

        //Control de detecciones
        isDetectedGroundOnFeet = Physics2D.OverlapBox(feetPos.position, boxSizeFeet, 0.0f, whatIsGround);
        isDetectedGroundOnLeft = Physics2D.OverlapBox(leftPos.position, boxSizeY, 0.0f, whatIsGround);
        isDetectedGroundOnRight = Physics2D.OverlapBox(rightPos.position, boxSizeY, 0.0f, whatIsGround);
        isDetectedGroundOnHead = Physics2D.OverlapBox(headPos.position, boxSizeHead, 0.0f, whatIsGround);

        if (isBig)
        {
            isDetectedMush = Physics2D.OverlapBox(new Vector2(this.transform.position.x, this.transform.position.y), new Vector2(this.GetComponent<BoxCollider2D>().size.x, this.GetComponent<BoxCollider2D>().size.y * 1.2f), 0.0f, whatIsMush);
            isDetectedFlower = Physics2D.OverlapBox(new Vector2(this.transform.position.x, this.transform.position.y), new Vector2(this.GetComponent<BoxCollider2D>().size.x, this.GetComponent<BoxCollider2D>().size.y * 1.2f), 0.0f, whatIsFlower);
        }
        else
        {
            isDetectedMush = Physics2D.OverlapBox(new Vector2(this.transform.position.x, this.transform.position.y - 0.5f), new Vector2(this.GetComponent<BoxCollider2D>().size.x, this.GetComponent<BoxCollider2D>().size.y), 0.0f, whatIsMush);
            isDetectedFlower = Physics2D.OverlapBox(new Vector2(this.transform.position.x, this.transform.position.y - 0.5f), new Vector2(this.GetComponent<BoxCollider2D>().size.x, this.GetComponent<BoxCollider2D>().size.y), 0.0f, whatIsFlower);
        }

        isDetectedStar = Physics2D.OverlapBox(new Vector2(this.transform.position.x, this.transform.position.y), new Vector2(this.GetComponent<BoxCollider2D>().size.x, this.GetComponent<BoxCollider2D>().size.y * 1.2f), 0.0f, whatIsStar);



        //isDetectedMushOnFeet = Physics2D.OverlapBox(feetPos.position, boxSizeFeet, 0.0f, whatIsMush);

        //Salto
        if (canMove && isDetectedGroundOnFeet && Input.GetKeyDown(KeyCode.Space))
        {
            isJumping = true;
            jumpTimeCounter = jumpTime;
            GetComponent<Rigidbody2D>().AddForce(new Vector2(0, 1) * (speed / 1.8888f), ForceMode2D.Impulse);

            if (isBig)
            {
                bigJumpClip.Play();
            }
            else
            {
                smallJumpClip.Play();
            }

        }

        if (canMove && Input.GetKeyUp(KeyCode.Space))
        {
            isJumping = false;
        }

        //Agacharse

        if (canMove && isDetectedGroundOnFeet && Input.GetKeyDown(KeyCode.DownArrow))
        {
            isGoingDown = true;
        }

        if (canMove && isDetectedGroundOnFeet && (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKey(KeyCode.DownArrow)) && isBig)
        {
            isDown = true;
        }

        if (canMove && Input.GetKeyUp(KeyCode.DownArrow))
        {
            isDown = false;
            isGoingUp = true;
        }


        if (isBig && isOnFire && Input.GetKeyDown(KeyCode.R))
        {
            shootingFireBall = true;
        }


        var pos = transform.position;


        if (canMove) { 
            if ((Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKey(KeyCode.DownArrow)) && isDetectedGroundOnFeet && (isBig || isOnFire))
            {
                Debug.Log("ENTRO VELOCITY 0");
                velocity = Mathf.MoveTowards(velocity, 0, speedUp * Time.deltaTime);
            }
            else
            {
                velocity = Mathf.MoveTowards(velocity, horizontalDirection, speedUp * Time.deltaTime);
            }
        



            //TODO: encontrar mejora de codificación (modularidad)
            if (!isDetectedGroundOnLeft && (horizontalDirection == -1 || horizontalDirection == 0)) //direccion izq
            {
                pos.x += (speed * velocity) * Time.deltaTime;
            }
            else if (!isDetectedGroundOnRight && (horizontalDirection == 1 || horizontalDirection == 0))
            {
                pos.x += (speed * velocity) * Time.deltaTime;
            }
            else if (isDetectedGroundOnLeft && (horizontalDirection == 1 || horizontalDirection == 0))
            {
                pos.x += (speed * velocity) * Time.deltaTime;
            }
            else if (isDetectedGroundOnRight && (horizontalDirection == -1 || horizontalDirection == 0))
            {
                pos.x += (speed * velocity) * Time.deltaTime;
            }

        }


        transform.position = pos;

        //Direccionamiento
        if (canMove && Input.GetKey(KeyCode.LeftArrow))
        {
            horizontalDirection = -1;
        }
        else if (canMove && Input.GetKey(KeyCode.RightArrow))
        {
            horizontalDirection = 1;
        }
        else
        {
            horizontalDirection = 0;
        }


        //Cambios de dirección
        if (horizontalDirection == 1)
        {
            //control de cambio de dirección
            if (renderer.flipX)
            {
                changeDir = true;
            }
            renderer.flipX = false;
        }
        else if (horizontalDirection == -1)
        {
            if (!renderer.flipX)
            {
                changeDir = true;
            }
            renderer.flipX = true;
        }

        if (isDetectedStar)
        {
            isStar = true;
        }

        //Cambio de sprites para animaciones
        if ((isDetectedMush || flagGrowAnim) && counterGrowAnim < 1 && !isBig)
        {
            canMove = false;
            flagGrowAnim = true;
            setAnim(3, isBig, isOnFire);
            counterGrowAnim += Time.deltaTime;

            if (counterGrowAnim >= 1)
            {
                flagGrowAnim = false;
                counterGrowAnim = 0;
                isBig = true;

                this.GetComponent<BoxCollider2D>().offset = new Vector2(this.GetComponent<BoxCollider2D>().offset.x, -0.02f);
                this.GetComponent<BoxCollider2D>().size = new Vector2(this.GetComponent<BoxCollider2D>().size.x, 1.8f);

                leftPos.localPosition = new Vector3(leftPos.localPosition.x, 0f);
                rightPos.localPosition = new Vector3(rightPos.localPosition.x, 0f);
                headPos.localPosition = new Vector3(headPos.localPosition.x, 1f);

                boxSizeY = new Vector2(boxSizeY.x, 1.7f);
                canMove = true;
            }

        } else if ((isDetectedFlower || flagOnFireAnim) && !isOnFire)
        {
            canMove = false;
            flagOnFireAnim = true;
            setAnim(5, isBig, isOnFire);
            counterOnFireAnim += Time.deltaTime;

            if (counterOnFireAnim >= 1)
            {
                flagOnFireAnim = false;
                counterOnFireAnim = 0;
                isOnFire = true;
                canMove = true;
            }
        }
        else if (canMove && (shootingFireBall || flagShootingFireBall))
        {
            flagShootingFireBall = true;
            setAnim(6, isBig, isOnFire);
            counterShootingFireBall += Time.deltaTime;

            if (counterShootingFireBall >= 0.06f)
            {
                flagShootingFireBall = false;
                counterShootingFireBall = 0;
            }

        }
        else if (canMove && isBig && isDown)
        {
            setAnim(4, isBig, isOnFire);
        }
        else if (canMove && velocity != 0 && (rb.velocity.y <= 0.001 && rb.velocity.y >= -0.001) && isDetectedGroundOnFeet) //Se mueve en X
        {
            setAnim(1, isBig, isOnFire);
        }
        else if (canMove && velocity == 0 && (rb.velocity.y <= 0.001 && rb.velocity.y >= -0.001) && isDetectedGroundOnFeet) //No se mueve en X
        {
            setAnim(0, isBig, isOnFire);
        }
        else if (canMove && rb.velocity.y != 0 && !isDetectedGroundOnFeet) //Se mueve en Y
        {
            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKey(KeyCode.DownArrow) && (isBig || isOnFire))
            {
                setAnim(4, isBig, isOnFire);
            }
            else
            {
                setAnim(2, isBig, isOnFire);
            }

        }

        //Cuando intenta cruzar colliders, la animación con los sprites va más lenta.
        if ((isDetectedGroundOnLeft && horizontalDirection == -1)
            || (isDetectedGroundOnRight && horizontalDirection == 1)
            || (!isDetectedGroundOnLeft && horizontalDirection == 1)
            || (!isDetectedGroundOnRight && horizontalDirection == -1))
        {
            //reloj de animaciones
            animClock += (Time.deltaTime * Mathf.Abs(velocity) * (speed * 3)) / 3;
        }else if (isDetectedMush || flagGrowAnim || isDetectedFlower || flagOnFireAnim)
        {
            animClock += Time.deltaTime * 13;
        }
        else
        {
            animClock += (Time.deltaTime * Mathf.Abs(velocity) * (speed * 3));
        }

        if (animClock >= 1)
        {
            currentSprite += 1;
            animClock = 0;
        }

        if (currentSprite >= currentAnim.Length)
        {
            currentSprite = 0;
        }

        renderer.sprite = currentAnim[currentSprite];

        //Control de la velocidad y la forma de correr del player
        if (canMove && Input.GetKey(KeyCode.E) && velocity != 0)
        {

            speed = Mathf.Clamp(speed + Time.deltaTime * 6, 5.1f, maxSpeed);
        }

        if (canMove && Input.GetKeyUp(KeyCode.E) || !Input.GetKey(KeyCode.E))
        {

            speed = Mathf.Clamp(speed - Time.deltaTime * 7, 5.1f, maxSpeed);    
        }

        if ((velocity == 0 || changeDir || isDetectedGroundOnLeft || isDetectedGroundOnRight)
            && !(isDetectedGroundOnLeft && isDetectedGroundOnRight) || Input.GetKey(KeyCode.DownArrow)) // Esta parte de la condicion elimina un error de deteccion de colisiones.
        {
            speed = 5.1f;
        }


        if (isGoingDown && isBig)
        {
            this.GetComponent<BoxCollider2D>().offset = new Vector2(this.GetComponent<BoxCollider2D>().offset.x, -0.5f);
            this.GetComponent<BoxCollider2D>().size = new Vector2(this.GetComponent<BoxCollider2D>().size.x, 0.96f);

            leftPos.localPosition = new Vector3(leftPos.localPosition.x, -0.5f);
            rightPos.localPosition = new Vector3(rightPos.localPosition.x, -0.5f);
            headPos.localPosition = new Vector3(headPos.localPosition.x, 0f);


            boxSizeY = new Vector2(boxSizeY.x, 0.4f);
        }
        else if (isGoingUp && isBig)
        {
            this.GetComponent<BoxCollider2D>().offset = new Vector2(this.GetComponent<BoxCollider2D>().offset.x, -0.02f);
            this.GetComponent<BoxCollider2D>().size = new Vector2(this.GetComponent<BoxCollider2D>().size.x, 1.9f);

            leftPos.localPosition = new Vector3(leftPos.localPosition.x, 0f);
            rightPos.localPosition = new Vector3(rightPos.localPosition.x, 0f);
            headPos.localPosition = new Vector3(headPos.localPosition.x, 1f);


            boxSizeY = new Vector2(boxSizeY.x, 1.7f);
        }

        shootingFireBall = false;
        isGoingUp = false;
        isGoingDown = false;
        changeDir = false;
    }

    void FixedUpdate()
    {
        //Salto
        if (Input.GetKey(KeyCode.Space) && isJumping)
        {
            if (jumpTimeCounter > 0)
            {
                GetComponent<Rigidbody2D>().AddForce(new Vector2(0, 1) * jumpForce, ForceMode2D.Impulse);
                jumpTimeCounter -= Time.deltaTime * 2;
            }
            else
            {
                isJumping = false;
            }
        }
    }

    //Gestiona que tipo de sprite se tiene que renderizar
    public void setAnim(int state, bool big, bool isOnFire)
    {
        if (state != animState)
        {
            currentSprite = 0;
            animClock = 0;

            //Animaciones para cuando Mario NO se mueve
            if (state == 0 && big && !isOnFire)
            {
                currentAnim = bigIdle;
            }
            else if (state == 0 && !big && !isOnFire)
            {
                currentAnim = smallIdle;
            }
            else if (state == 0 && isOnFire)
            {
                currentAnim = onFireIdle;
            }

            //Animaciones para cuando Mario SI se mueve
            if (state == 1 && big && !isOnFire)
            {
                currentAnim = bigWalk;
            }
            else if (state == 1 && !big && !isOnFire)
            {
                currentAnim = smallWalk;
            }
            else if (state == 1 && isOnFire)
            {
                currentAnim = onFireWalk;
            }

            if (state == 2 && big && !isOnFire)
            {
                currentAnim = bigJump;
            }
            else if (state == 2 && !big && !isOnFire)
            {
                currentAnim = smallJump;
            }
            else if (state == 2 && isOnFire)
            {
                currentAnim = onFireJump;
            }

            if (state == 3)
            {
                currentAnim = grow;
            }

            if (state == 4 && !isOnFire)
            {
                currentAnim = goingDown;
            }
            else if (state == 4 && isOnFire)
            {
                currentAnim = onFireGoingDown;
            }

            if (state == 5)
            {
                currentAnim = onFireGrow;
            }

            if (state == 6 && isOnFire)
            {
                currentAnim = onFireBall;
            }



            animState = state;
            
        }
    }

    public void growUp()
    {   

        animClock += (Time.deltaTime * Mathf.Abs(velocity) * (speed * 3));
        

        if (animClock >= 1)
        {
            currentSprite += 1;
            animClock = 0;
        }

        if (currentSprite >= currentAnim.Length)
        {
            currentSprite = 0;
        }

        renderer.sprite = currentAnim[currentSprite];

    }



}
