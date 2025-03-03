using UnityEngine;

public class Attack : MonoBehaviour
{
    // public Enemy enemy;

    private void OnTriggerStay2D(Collider2D collision)
    {
        collision.GetComponent<Enemy>().TakeHit();
    }
}
