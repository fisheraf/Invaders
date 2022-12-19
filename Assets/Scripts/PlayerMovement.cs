using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float movementSpeed;


    private void Awake()
    {
        
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.x > -19 && Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Translate(-movementSpeed * Time.deltaTime,0,0);
        }
        if (transform.position.x < 19 && Input.GetKey(KeyCode.RightArrow))
        {
            transform.Translate(movementSpeed * Time.deltaTime, 0, 0);
        }
    }
}