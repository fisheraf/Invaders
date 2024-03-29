using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Shooting")]
    [SerializeField] float reloadTime;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] float bulletSpeed;
    [SerializeField] Transform fireLocation;

    [SerializeField] int maxBulletCount;
    [SerializeField] List<GameObject> bulletList = new List<GameObject>();

    public float reloadTimer;

    [Header("Movement")]
    [SerializeField] float horizontalMovementSpeed;
    [SerializeField] float forwardMovementSpeed;
    float lastSpeed;
    [SerializeField] float rotationSpeed;
    bool canMoveLeft = true;
    bool canMoveRight = true;

    [Header("Health")]
    [SerializeField] int health;

    [Header("Planet")]
    [SerializeField] Planet planetInRange;


    public bool cancelFirstShoot;  //Input bug - separate input mangager?
    Rigidbody2D playerRigidbody2D;
    [SerializeField] ParticleSystem engineParticleSystem;
    [SerializeField] GameObject shipTravelParticleSystem;

    private void Awake()
    {
        engineParticleSystem.Stop();
    }

    // Start is called before the first frame update
    void Start()
    {
        bulletPrefab.GetComponent<Bullet>().SetBulletSpeed(bulletSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        //ShootingReloadTimer();
        if (GameManager.Instance.gameState == GameManager.GameState.Invaders || GameManager.Instance.gameState == GameManager.GameState.MainMenu)
        {
            ShootingBulletCount();
        }
        if (GameManager.Instance.gameState == GameManager.GameState.OverWorld)
        {
            MovementForOverWorld();
            ShootingReloadTimer();
            Interact();
        }

        if (forwardMovementSpeed != lastSpeed)
        {
            Debug.Log("Resetting Particle System - Ship Travel position");
            shipTravelParticleSystem.transform.localPosition = new Vector3(0, forwardMovementSpeed * 1.5f); // move based on movement speed?
            lastSpeed = forwardMovementSpeed;
        }
    }
    private void LateUpdate()
    {
        if (GameManager.Instance.gameState == GameManager.GameState.Invaders || GameManager.Instance.gameState == GameManager.GameState.MainMenu)
        {
            MovementForInvaders();
        }
    }

    private void ShootingReloadTimer()
    {
        if (Input.GetKey(KeyCode.Space) && (reloadTimer >= reloadTime))
        {
            reloadTimer = 0f;
            Shoot();
        }

        reloadTimer += Time.deltaTime;
    }

    private void ShootingBulletCount()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W))
        {
            if (cancelFirstShoot) { Debug.Log("Cancel true"); cancelFirstShoot = false; return; }
            if (bulletList.Count < maxBulletCount)
            {
                Shoot();
            }
        }
    }

    public void SetPlayerToInvadersMovement()
    {
        playerRigidbody2D = GetComponent<Rigidbody2D>(); // called here rather than awake as GameManger fires first
        playerRigidbody2D.bodyType = RigidbodyType2D.Kinematic;
        playerRigidbody2D.velocity = Vector2.zero;
        playerRigidbody2D.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
    }

    public void SetPlayerToOverworldMovement()
    { 
        playerRigidbody2D.bodyType = RigidbodyType2D.Dynamic;
        playerRigidbody2D.constraints = RigidbodyConstraints2D.None;
    }

    private void MovementForInvaders()
    {
        
        if (canMoveLeft && (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)))
        {
            transform.Translate(-horizontalMovementSpeed * Time.deltaTime, 0, 0);
        }
        if (canMoveRight && (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)))
        {
            transform.Translate(horizontalMovementSpeed * Time.deltaTime, 0, 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("InvadersEdge"))
        {
            if(collision.gameObject.name == "Left")
            {
                canMoveLeft = false;
            }
            if (collision.gameObject.name == "Right")
            {
                canMoveRight = false;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("InvadersEdge"))
        {
            if (collision.gameObject.name == "Left")
            {
                canMoveLeft = true;
            }
            if (collision.gameObject.name == "Right")
            {
                canMoveRight = true;
            }
        }
    }

    private void MovementForOverWorld()
    {
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            playerRigidbody2D.AddRelativeForce(Vector2.up * forwardMovementSpeed, ForceMode2D.Impulse);
            if (engineParticleSystem.isStopped) { engineParticleSystem.Play(); }
        }
        else
        {
            if (engineParticleSystem.isPlaying) { engineParticleSystem.Stop(); }
        }
        float rotation = Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime;
        //transform.Rotate(0, 0, -rotation);
        playerRigidbody2D.AddTorque(-rotation);
    }


    private void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, fireLocation.position, fireLocation.rotation);
        bulletList.Add(bullet);
        if (GameManager.Instance.gameState == GameManager.GameState.Invaders)
        {
            GameManager.Instance.ScoreManager.PlayerBulletsFired += 1;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TakeDamage();
    }

    private void TakeDamage()
    {
        health -= 1;
        GameManager.Instance.UIManager.DisplayHealthIcons(health);
    }

    public void RemoveBullet(GameObject bullet)
    {
        bulletList.Remove(bullet);
    }

    public void SetPlantInRange(Planet planet)
    {
        planetInRange = planet;
    }

    private void Interact()
    {
        if (Input.GetKeyDown(KeyCode.E) && planetInRange != null)
        {
            planetInRange.InteractWithPlanet();
        }
    }
}
