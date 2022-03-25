using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private LayerMask solidObjectsLayer;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private LayerMask grassLayer;

    public event Action OnEncountered;
    
    private bool isMoving;
    private Vector2 input;

    // Cache the reference to the Animator
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // HandleUpdate wont be called automatically by Unity like Update does
    public void HandleUpdate()
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
                
                // (!) targetPos == true or false (Whatever the machine isWalkable returns)
                if(IsWalkable(targetPos)) 
                    StartCoroutine(Move(targetPos));
            }
        }
        
        animator.SetBool("isMoving", isMoving);
        
        if(Input.GetKeyDown(KeyCode.Return))
            Interact();
    }

    void Interact()
    {
        // Find direction the player is facing
        var facingDir = new Vector3(animator.GetFloat("moveX"), animator.GetFloat("moveY"));
        
        // Find position of tile the player is facing
        var interactPos = transform.position + facingDir;
        
        //Debug.DrawLine(transform.position, interactPos, Color.red, 1.0f);

        var collider = Physics2D.OverlapCircle(interactPos, 0.3f, interactableLayer);
        if (collider != null)
        {
            // Call Interact() of IInteractable interface (mind blown)
            collider.GetComponent<IInteractable>()?.Interact();
        }
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

        CheckForEncounters();
    }

    private bool IsWalkable(Vector3 targetPos)
    {
        if (Physics2D.OverlapCircle(targetPos, 0.05f, solidObjectsLayer | interactableLayer) != null)
        {
            return false;
        }
        
        return true;
    }

    private void CheckForEncounters()
    {
        if (Physics2D.OverlapCircle(transform.position, 0.05f, grassLayer) != null)
        {
            // Generate a random battle
            if (Random.Range(1, 101) <= 10)
            {
                // Disable player walking animation
                animator.SetBool("isMoving", false);
                
                // Call OnEncountered event
                OnEncountered();
                
                Debug.Log("MONSTER ENCOUNTER!");
            }
        }
    }
}