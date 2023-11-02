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
        //player.TakeDamage(1);  // ÿ�ε�һ��Ѫ
        yield return new WaitForSeconds(1f);  // �ȴ�1��
        player.TakeDamage(1);  // ÿ�ε�һ��Ѫ
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

                Player player = hit.collider.GetComponent<Player>();

                /***if (player)
                {
                    player.TakeDamage(100); // Kills the player
                    break;
                }***/

                if (player)
                {
                    if (!isDamagingPlayer)
                    {
                        StartCoroutine(DamagePlayer(player));  // ʹ��Э����ʵ����ʱ
                    }
                    return;  // ����Ӧ���� return ������ break���Ա��������������߼�
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
