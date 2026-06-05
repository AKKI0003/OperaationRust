using UnityEngine;
using System.Collections.Generic;

public class GuardAI : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 1.2f;

    [Header("Vision")]
    public float visionDistance = 6f;
    public float visionAngle = 45f;

    [Header("Patrol")]
    public Transform patrolParent;
    public float waitTime = 3f;

    Rigidbody2D rb;
    Animator animator;

    Transform player;

    Vector2 moveDirection;
    Vector2 lastDirection = Vector2.down;

    Transform currentPoint;

    float waitTimer;

    enum State
    {
        Patrol,
        Wait,
        Chase,
        Investigate
    }

    State currentState;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Start()
    {
        PickRandomPoint();
        currentState = State.Patrol;
    }

    void Update()
    {
        DetectPlayer();

        switch (currentState)
        {
            case State.Patrol:
                Patrol();
                break;

            case State.Wait:
                Wait();
                break;

            case State.Chase:
                Chase();
                break;
        }

        UpdateAnimator();
    }

    void FixedUpdate()
    {
        rb.MovePosition(
            rb.position +
            moveDirection *
            moveSpeed *
            Time.fixedDeltaTime
        );
    }

    void Patrol()
    {
        if (currentPoint == null)
            return;

        Vector2 dir =
            (currentPoint.position - transform.position);

        if (dir.magnitude < 0.2f)
        {
            currentState = State.Wait;
            waitTimer = waitTime;
            moveDirection = Vector2.zero;
            return;
        }

        moveDirection = dir.normalized;

        if (moveDirection != Vector2.zero)
            lastDirection = moveDirection;
    }

    void Wait()
    {
        moveDirection = Vector2.zero;

        waitTimer -= Time.deltaTime;

        if (waitTimer <= 0)
        {
            PickRandomPoint();
            currentState = State.Patrol;
        }
    }

    void Chase()
    {
        Vector2 dir =
            (player.position - transform.position);

        moveDirection = dir.normalized;

        lastDirection = moveDirection;
    }

    void DetectPlayer()
    {
        Vector2 toPlayer =
            player.position - transform.position;

        if (toPlayer.magnitude > visionDistance)
            return;

        float angle =
            Vector2.Angle(lastDirection, toPlayer);

        if (angle > visionAngle)
            return;

        RaycastHit2D hit =
            Physics2D.Raycast(
                transform.position,
                toPlayer.normalized,
                visionDistance,
                LayerMask.GetMask(
                    "Obstacle",
                    "Player"
                )
            );

        if (hit.collider != null &&
           hit.collider.CompareTag("Player"))
        {
            currentState = State.Chase;
        }
    }

    void PickRandomPoint()
    {
        List<Transform> points =
            new List<Transform>();

        foreach (Transform point in patrolParent)
        {
            points.Add(point);
        }

        currentPoint =
            points[Random.Range(0, points.Count)];
    }

    void UpdateAnimator()
    {
        animator.SetFloat("MoveX", lastDirection.x);
        animator.SetFloat("MoveY", lastDirection.y);
        animator.SetFloat("Speed", moveDirection.sqrMagnitude);
    }
}