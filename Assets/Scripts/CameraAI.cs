using UnityEngine;

public class CameraAI : MonoBehaviour
{
    [Header("Detection")]
    public Transform player;

    public float viewDistance = 5f;

    [Range(0, 360)]
    public float viewAngle = 70f;

    public LayerMask obstacleMask;

    [Header("Vision Cone")]
    public SpriteRenderer visionConeRenderer;

    [Header("Colors")]
    public Color normalColor =
        new Color(0f, 1f, 0f, 0.35f);

    public Color suspiciousColor =
        new Color(1f, 1f, 0f, 0.4f);

    public Color alertColor =
        new Color(1f, 0f, 0f, 0.45f);

    [Header("Timers")]
    public float suspiciousTime = 0.5f;

    float detectionTimer;

    [Header("Rotation")]
    public float rotateSpeed = 25f;

    public float maxRotation = 60f;

    float currentRotation;

    bool rotateRight = true;

    Vector2 facingDirection =
        Vector2.up;

    bool gameOverTriggered;

    void Start()
    {
        if (player == null)
        {
            GameObject p =
                GameObject.FindGameObjectWithTag(
                    "Player");

            if (p != null)
                player = p.transform;
        }

        if (visionConeRenderer != null)
        {
            visionConeRenderer.color =
                normalColor;
        }
    }

    void Update()
    {
        RotateCamera();

        bool seesPlayer =
            CanSeePlayer();

        if (seesPlayer)
        {
            detectionTimer +=
                Time.deltaTime;

            if (detectionTimer >= suspiciousTime)
            {
                if (visionConeRenderer != null)
                {
                    visionConeRenderer.color =
                        alertColor;
                }

                if (!gameOverTriggered)
                {
                    gameOverTriggered = true;

                    if (GameManager.Instance != null)
                    {
                        GameManager.Instance
                            .GameOver();
                    }
                }
            }
            else
            {
                if (visionConeRenderer != null)
                {
                    visionConeRenderer.color =
                        suspiciousColor;
                }
            }
        }
        else
        {
            detectionTimer = 0f;

            if (visionConeRenderer != null)
            {
                visionConeRenderer.color =
                    normalColor;
            }
        }
    }

    bool CanSeePlayer()
    {
        if (player == null)
            return false;

        Vector2 dirToPlayer =
            player.position -
            transform.position;

        float distance =
            dirToPlayer.magnitude;

        if (distance > viewDistance)
            return false;

        float angle =
            Vector2.Angle(
                facingDirection,
                dirToPlayer);

        if (angle > viewAngle * 0.5f)
            return false;

        RaycastHit2D hit =
            Physics2D.Raycast(
                transform.position,
                dirToPlayer.normalized,
                distance,
                obstacleMask);

        if (hit.collider != null)
            return false;

        return true;
    }

    void RotateCamera()
    {
        float delta =
            rotateSpeed *
            Time.deltaTime;

        if (rotateRight)
        {
            currentRotation += delta;

            if (currentRotation >= maxRotation)
            {
                currentRotation =
                    maxRotation;

                rotateRight = false;
            }
        }
        else
        {
            currentRotation -= delta;

            if (currentRotation <= -maxRotation)
            {
                currentRotation =
                    -maxRotation;

                rotateRight = true;
            }
        }

        transform.rotation =
            Quaternion.Euler(
                0,
                0,
                currentRotation);

        float radians =
            transform.eulerAngles.z *
            Mathf.Deg2Rad;

        facingDirection =
            new Vector2(
                Mathf.Cos(
                    radians +
                    Mathf.PI / 2f),
                Mathf.Sin(
                    radians +
                    Mathf.PI / 2f));
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color =
            Color.cyan;

        Gizmos.DrawWireSphere(
            transform.position,
            viewDistance);
    }
}