using System.Collections;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LaserBeam : MonoBehaviour
{
    private LineRenderer lineRenderer;
    public float laserRange = 100f;
    public int maxReflections = 5;
    public LayerMask collisionLayers;
    private bool isDamagingPlayer = false;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        DrawLaser();
    }

    IEnumerator DamagePlayer(Player player)
    {
        isDamagingPlayer = true;

        float elapsedTime = 0f;
        while (elapsedTime < 1f)
        {
            if (player == null || !player.isBeingDamagedByLaser)
            {
                // leave laser in 1s = no deduction for health
                isDamagingPlayer = false;
                yield break;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // take damage if stay for a whole second
        player.TakeDamage(1);
        isDamagingPlayer = false;
    }


    private void DrawLaser()
    {
        lineRenderer.positionCount = 1;
        lineRenderer.SetPosition(0, transform.position);

        Vector2 lastLaserPosition = transform.position;
        Vector2 currentDirection = transform.right;

        Player playerHit = null;

        for (int i = 0; i < maxReflections; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(lastLaserPosition, currentDirection, laserRange, collisionLayers);
            if (hit)
            {
                lineRenderer.positionCount++;
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, hit.point);
                lastLaserPosition = hit.point + hit.normal * 0.01f;

                Player player = hit.collider.GetComponent<Player>();

                if (player)
                {
                    player.isBeingDamagedByLaser = true;  // the player is being hit

                    if (!isDamagingPlayer)
                    {
                        StartCoroutine(DamagePlayer(player));  // Counting time
                    }
                    return;  // should not be break
                }
                else
                {
                    if (player)
                    {
                        player.isBeingDamagedByLaser = false;  // not hit
                    }
                }

                Mirror mirror = hit.collider.GetComponent<Mirror>();
                if (mirror)
                {
                    currentDirection = Vector2.Reflect(currentDirection, hit.normal);
                }
                else
                {
                    break;
                }
            }
            else
            {
                lineRenderer.positionCount++;
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, lastLaserPosition + currentDirection * laserRange);
                break;
            }
        }
        if (playerHit == null)
        {
            // if not hit, set isBeingDamagedByLaser to false
            foreach (var player in FindObjectsOfType<Player>())
            {
                player.isBeingDamagedByLaser = false;
            }
        }
    }

}
