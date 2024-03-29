using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float existenceTime;
    [SerializeField] float bulletSpeed;
    [SerializeField] bool playerBullet;


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DeathTimer(existenceTime));
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(new Vector3(0, bulletSpeed * Time.deltaTime, 0), Space.Self);
    }

    IEnumerator DeathTimer(float time)
    {
        yield return new WaitForSeconds(time);
        if (playerBullet)
        {
            FindObjectOfType<Player>().RemoveBullet(gameObject);
        }
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (playerBullet)
        {
            FindObjectOfType<Player>().RemoveBullet(gameObject);
        }
        Destroy(gameObject);
    }

    public void SetBulletSpeed(float speed)
    {
        bulletSpeed = speed;
    }
}
