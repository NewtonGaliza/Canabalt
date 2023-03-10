using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float gravity;
    private Vector2 _velocity;
    public Vector2 velocity
    {
        get { return _velocity; }
        set { _velocity = value; }
    }
    private float acceleration = 10;
    private float maxAcceleration = 10;
    private float maxXVelocity = 100;
    private float _distance;
    public float distance
    {
        get { return _distance; }
        set { _distance = value; }
    }

    [SerializeField] private float groundHeight;
    [SerializeField] private bool isGrounded = false;
    [SerializeField] private float jumpVelocity;


    private bool isHoldingJump = false;
    private float maxHoldJumpTime = 0.4f;
    private float holdJumpTimer = 0.0f;


    private float jumpGroundThreshold = 1;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 pos = transform.position;
        float groundDistance = Mathf.Abs(pos.y - groundHeight);

        if(isGrounded || groundDistance <= jumpGroundThreshold)
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                isGrounded = false;
                _velocity.y = jumpVelocity;
                isHoldingJump = true;
                holdJumpTimer = 0;
            }
        }

        if(Input.GetKeyUp(KeyCode.Space))
        {
             isHoldingJump = false;       
        }
    }

    private void FixedUpdate()
    {
        Vector2 pos = transform.position;

        if(!isGrounded)
        {
            if(isHoldingJump)
            {
                holdJumpTimer += Time.fixedDeltaTime;
                if(holdJumpTimer >= maxHoldJumpTime)
                {
                    isHoldingJump = false;
                }
            }

            pos.y += velocity.y * Time.fixedDeltaTime;

            if(!isHoldingJump)
            {
                _velocity.y += gravity * Time.fixedDeltaTime;
            }

            Vector2 rayOrigin = new Vector2(pos.x + 0.7f, pos.y);
            Vector2 rayDirection = Vector2.up;
            float rayDistance = _velocity.y * Time.fixedDeltaTime;
            RaycastHit2D hit2D = Physics2D.Raycast(rayOrigin, rayDirection, rayDistance);
            if(hit2D.collider != null)
            {
                Ground ground = hit2D.collider.GetComponent<Ground>();
                if(ground != null)
                {
                    groundHeight = ground.groundHeight;
                    pos.y = groundHeight;
                    _velocity.y = 0;
                    isGrounded = true;
                }
            }
        }


        distance += velocity.x * Time.fixedDeltaTime;

        if(isGrounded)
        {
            float velocityRatio = velocity.x / maxXVelocity;

            acceleration = maxAcceleration * (1 - velocityRatio);

            _velocity.x += acceleration * Time.fixedDeltaTime;            

            if(velocity.x >= maxXVelocity)
            {
                _velocity.x = maxXVelocity;
            }

            Vector2 rayOrigin = new Vector2(pos.x - 0.7f, pos.y);
            Vector2 rayDirection = Vector2.up;
            float rayDistance = _velocity.y * Time.fixedDeltaTime;
            RaycastHit2D hit2D = Physics2D.Raycast(rayOrigin, rayDirection, rayDistance);
            if(hit2D.collider == null)
            {
                isGrounded = true;
            }

        }

        transform.position = pos;
    }
}
