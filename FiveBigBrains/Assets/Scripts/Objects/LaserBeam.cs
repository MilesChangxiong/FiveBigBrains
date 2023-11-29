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
    private bool isLaserActive = true;
    private float timer = 0f;
    private float switchInterval = 3f;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= switchInterval)
        {
            isLaserActive = !isLaserActive;
            timer = 0f;
        }

        if (isLaserActive)
        {
            DrawLaser();
        }
        else
        {
            lineRenderer.positionCount = 0; // 关闭激光
        }
    }
    IEnumerator DamagePlayer(Player player)
    {
        isDamagingPlayer = true;
        player.TakeDamage(1);  // ????????????
        yield return new WaitForSeconds(1f);  // ????1??
        //player.TakeDamage(1);  // ????????????
        isDamagingPlayer = false;
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

                PlayerBodyParts playerBodyPart = hit.collider.GetComponent<PlayerBodyParts>();
                //Player player = hit.collider.GetComponent<Player>();

                /***if (player)
                {
                    player.TakeDamage(100); // Kills the player
                    break;
                }***/

                if (playerBodyPart)
                {
                    if (!isDamagingPlayer)
                    {
                        StartCoroutine(DamagePlayer(playerBodyPart.transform.parent.GetComponent<Player>()));  // ??????????????????
                    }
                    return;  // ?????????? return ?????? break????????????????????????
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
