using UnityEngine;
using System.Collections.Generic;

public class GuardAStarAI : MonoBehaviour
{
    [Header("Patrol Route")]
    public Transform patrolRoute;
    public float chaseRepathTimer;
    public float patrolSpeed = 1.2f;
    [Header("Movement")]
    public float moveSpeed = 2f;
    public float waitTime = 2f;

    [Header("Vision")]
    public Transform player;
    public float visionRange = 4f;
    [Header("Vision")]

    public float viewDistance = 4f;

    [Range(0, 360)]
    public float viewAngle = 70f;

    public LayerMask obstacleMask;

    public Transform visionCone;

    [Header("AI")]

    public float suspiciousTime = 0.75f;
    public float suspiciousTimer = 0f;
    public float suspiciousDuration = 1.5f;

    public float chaseSpeed = 1.8f;

    public Color patrolColor =
        new Color(0f, 1f, 0f, 0.35f);

    public Color suspiciousColor =
        new Color(1f, 1f, 0f, 0.4f);

    public Color alertColor =
        new Color(1f, 0f, 0f, 0.45f);

    SpriteRenderer coneRenderer;

    public float lostSightTimer = 0f;

    public float chaseMemory = 12f;
    public float searchTime = 4f;

    bool hasAlertedPlayer = false;
    float searchTimer;
    Rigidbody2D rb;
    Animator animator;

    List<Node> currentPath;
    int currentPathIndex;

    Vector2 moveDirection;
    Vector2 lastDirection = Vector2.down;

    Transform currentTarget;
    Transform previousTarget;

    Vector2 lastKnownPlayerPosition;

    float waitTimer;

    Vector2 lastPosition;
    float stuckTimer;

    enum State
    {
        Patrol,
        Wait,
        Suspicious,
        Chase,
        Search,
        Return
    }

    State currentState;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        if (player == null)
        {
            GameObject p =
                GameObject.FindGameObjectWithTag("Player");

            if (p != null)
                player = p.transform;
        }

        if (visionCone != null)
        {
            coneRenderer =
                visionCone.GetComponent<SpriteRenderer>();
        }
    }

    void Start()
    {
        chaseRepathTimer = 0f;
        PickNewPatrolPoint();
    }

    void Update()
    {
        bool seesPlayer =
            CanSeePlayer();

        if (seesPlayer)
        {
            suspiciousTimer +=
                Time.deltaTime;

            if (suspiciousTimer >= suspiciousTime)
            {
                lastKnownPlayerPosition =
                    player.position;

                lostSightTimer =
                    chaseMemory;

                currentState =
                    State.Chase;

                if (!hasAlertedPlayer)
                {
                    hasAlertedPlayer = true;

                    if (GameManager.Instance != null)
                    {
                        GameManager.Instance.GameOver();
                    }
                }
            }
            else
            {
                if (currentState != State.Suspicious)
                {
                    currentState =
                        State.Suspicious;
                }

                currentState =
                    State.Suspicious;
            }
        }
        else
        {
            if (currentState != State.Chase)
            {
                suspiciousTimer -= Time.deltaTime;
            }

            suspiciousTimer =
                Mathf.Clamp(
                    suspiciousTimer,
                    0f,
                    suspiciousTime);
        }

        UpdateVisionCone();

        switch (currentState)
        {
            case State.Patrol:

                moveSpeed =
                    patrolSpeed;

                FollowPath();

                break;

            case State.Wait:
                HandleWait();
                break;

            case State.Suspicious:

                HandleSuspicious();

                break;

            case State.Chase:
                HandleChase();
                break;

            case State.Search:

                FollowPath();

                searchTimer -= Time.deltaTime;

                if (searchTimer <= 0f)
                {
                    currentState =
                        State.Return;
                }

                break;

            case State.Return:

                BuildPatrolPath();

                currentState =
                    State.Patrol;

                break;
        }

        DetectStuck();
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

    void CheckPlayerDetection()
    {
        if (player == null)
            return;

        float distance =
            Vector2.Distance(
                transform.position,
                player.position);

        if (distance <= visionRange)
        {
            lastKnownPlayerPosition =
                player.position;

            currentState = State.Chase;
        }
    }

    void HandleChase()
    {
        moveSpeed = chaseSpeed;

        if (CanSeePlayer())
        {
            lostSightTimer = chaseMemory;

            lastKnownPlayerPosition =
                player.position;
        }
        else
        {
            lostSightTimer -= Time.deltaTime;
        }
        if (lostSightTimer <= 0f)
        {
            BuildSearchPath();

            searchTimer =
                searchTime;

            currentState =
                State.Search;

            return;
        }

        chaseRepathTimer -=
            Time.deltaTime;

        if (chaseRepathTimer <= 0f)
        {
            currentPath =
                Pathfinder.Instance.FindPath(
                    transform.position,
                    lastKnownPlayerPosition);

            currentPathIndex = 0;

            chaseRepathTimer = 0.4f;
        }

        FollowPath();
    }

    void BuildSearchPath()
    {
        currentPath =
            Pathfinder.Instance.FindPath(
                transform.position,
                lastKnownPlayerPosition);

        currentPathIndex = 0;
    }

    void PickNewPatrolPoint()
    {
        if (patrolRoute == null)
            return;

        if (patrolRoute.childCount == 0)
            return;

        Transform nextPoint;

        do
        {
            nextPoint =
                patrolRoute.GetChild(
                    Random.Range(
                        0,
                        patrolRoute.childCount));
        }
        while (
            patrolRoute.childCount > 1 &&
            nextPoint == previousTarget);

        previousTarget = nextPoint;
        currentTarget = nextPoint;

        BuildPatrolPath();

        currentState = State.Patrol;
    }

    void BuildPatrolPath()
    {
        if (currentTarget == null)
            return;

        currentPath =
            Pathfinder.Instance.FindPath(
                transform.position,
                currentTarget.position);

        currentPathIndex = 0;
    }

    void FollowPath()
    {
        if (currentPath == null ||
            currentPath.Count == 0)
        {
            moveDirection = Vector2.zero;

            if (currentState == State.Search)
            {
                currentState = State.Wait;
                waitTimer = 3f;
            }
            else
            {
                currentState = State.Wait;
                waitTimer = waitTime;
            }

            return;
        }

        if (currentPathIndex >= currentPath.Count)
        {
            moveDirection = Vector2.zero;

            if (currentState == State.Search)
            {
                waitTimer = 2f;
                currentState = State.Return;
            }
            else if (currentState == State.Return)
            {
                PickNewPatrolPoint();
            }
            else
            {
                currentState = State.Wait;
                waitTimer = waitTime;
            }

            return;
        }

        Vector2 targetPos =
            currentPath[currentPathIndex]
            .worldPosition;

        Vector2 direction =
            targetPos -
            (Vector2)transform.position;

        if (direction.magnitude < 0.35f)
        {
            currentPathIndex++;
            return;
        }

        moveDirection =
            direction.normalized;

        if (moveDirection != Vector2.zero)
            lastDirection = moveDirection;
    }

    void HandleWait()
    {
        moveDirection = Vector2.zero;

        waitTimer -= Time.deltaTime;

        if (waitTimer <= 0)
        {
            PickNewPatrolPoint();
        }
    }

    void DetectStuck()
    {
        float movedDistance =
            Vector2.Distance(
                transform.position,
                lastPosition);

        if (
            moveDirection.sqrMagnitude > 0.1f &&
            movedDistance < 0.02f
        )
        {
            stuckTimer += Time.deltaTime;

            if (stuckTimer > 1.5f)
            {
                if (currentState == State.Patrol)
                {
                    BuildPatrolPath();
                }
                else if (currentState == State.Search)
                {
                    BuildSearchPath();
                }

                stuckTimer = 0f;
            }
        }
        else
        {
            stuckTimer = 0f;
        }

        lastPosition = transform.position;
    }

    void UpdateAnimator()
    {
        animator.SetFloat(
            "MoveX",
            lastDirection.x);

        animator.SetFloat(
            "MoveY",
            lastDirection.y);

        animator.SetFloat(
            "Speed",
            moveDirection.sqrMagnitude);
    }

    bool CanSeePlayer()
    {
        if (player == null)
            return false;

        Vector2 dir =
            player.position -
            transform.position;

        float distance =
            dir.magnitude;

        float currentDistance =
        currentState == State.Chase
        ? viewDistance * 2.5f
        : viewDistance;

        if (distance > currentDistance)
            return false;

        float angle =
            Vector2.Angle(
                lastDirection,
                dir);

        float currentAngle =
        currentState == State.Chase
        ? 180f
        : viewAngle;

        if (angle > currentAngle * 0.5f)
            return false;

        RaycastHit2D hit =
            Physics2D.Raycast(
                transform.position,
                dir.normalized,
                distance,
                obstacleMask);

        if (hit.collider != null)
            return false;

        return true;
    }

    void UpdateVisionCone()
    {
        if (visionCone == null)
            return;

        float angle =
            Mathf.Atan2(
                lastDirection.y,
                lastDirection.x)
            * Mathf.Rad2Deg;

        visionCone.localRotation =
            Quaternion.Euler(
                0f,
                0f,
                angle
            );

        if (coneRenderer == null)
            return;

        switch (currentState)
        {
            case State.Patrol:
            case State.Wait:

                coneRenderer.color =
                    patrolColor;
                break;

            case State.Suspicious:

                coneRenderer.color =
                    suspiciousColor;

                break;


            case State.Chase:
            case State.Search:

                coneRenderer.color =
                    alertColor;

                if (!hasAlertedPlayer)
                {
                    hasAlertedPlayer = true;

                    if (GameManager.Instance != null)
                    {
                        GameManager.Instance.GameOver();
                    }
                }

                break;
        }

       

    }
    void HandleSuspicious()
    {
        moveDirection = Vector2.zero;
    }
}