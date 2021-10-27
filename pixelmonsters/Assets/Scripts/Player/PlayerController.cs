using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    
    private bool isMoving;
    private Vector2 input;

    // Cache the reference to the Animator
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!isMoving)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            // Keep the player from moving diagonally 
            if (input.x != 0) input.y = 0;

            if (input != Vector2.zero)
            {
                // Accessing Animator Blendtree parameters moveX and moveY
                animator.SetFloat("moveX", input.x);
                animator.SetFloat("moveY", input.y);
                
                Vector2 targetPos = transform.position;
                targetPos.x += input.x;
                targetPos.y += input.y;

                StartCoroutine(Move(targetPos));
            }
        }
        
        animator.SetBool("isMoving", isMoving);
    }

    // Coroutine - Move the Player from its starting position to its target position, over a period of time
    IEnumerator Move(Vector3 targetPos)
    {
        isMoving = true;
        
        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            // Break out of the coroutine
            yield return null;
        }
        transform.position = targetPos;
        
        isMoving = false;
    }
    
}
