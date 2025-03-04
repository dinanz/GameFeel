using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{

    public float hitCooldown = 0.5f;
    private Dictionary<Enemy, float> lastHitTimeDict = new Dictionary<Enemy, float>();

    private void OnTriggerStay2D(Collider2D collision)
    {
        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy != null)
        {
            if (!lastHitTimeDict.ContainsKey(enemy) || Time.time - lastHitTimeDict[enemy] >= hitCooldown)
            {
                enemy.TakeHit();
                lastHitTimeDict[enemy] = Time.time;
            }
        }
    }
}
