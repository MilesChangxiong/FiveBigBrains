using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LaserBeam : MonoBehaviour
{
    private LineRenderer lineRenderer;
    public float laserRange = 100f;
    public int maxReflections = 5;
    public LayerMask collisionLayers;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        DrawLaser();
    }

    private void DrawLaser()
    {
        lineRenderer.positionCount = 1;
        lineRenderer.SetPosition(0, transform.position);

        Vector2 lastLaserPosition = transform.position;
        Vector2 currentDirection = transform.right;

        for (int i = 0; i < maxReflections; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(lastLaserPosition, currentDirection, laserRange, collisionLayers);
            if (hit)
            {
                Debug.DrawLine(hit.point, hit.point + (Vector2)hit.normal, Color.blue, 1f);

                lineRenderer.positionCount++;
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, hit.point);
                lastLaserPosition = hit.point + hit.normal * 0.01f;

                Player player = hit.collider.GetComponent<Player>();
                if (player)
                {
                    player.TakeDamage(100); // Kills the player
                    break;
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
    }
}
