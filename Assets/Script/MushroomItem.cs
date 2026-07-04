using UnityEngine;

public class MushroomItem : MonoBehaviour
{
    [SerializeField] float moveSpeed = 3f;
    private int moveDirection = 1;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        rb.linearVelocityX = moveDirection * moveSpeed;
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerMove player = collision.gameObject.GetComponent<PlayerMove>();
            
            if (player != null)
                player.GrowUp();

            Destroy(gameObject);
        }
        else
        {
            if (Mathf.Abs(collision.contacts[0].normal.x) > 0.5f)
            {
                moveDirection *= -1; 
            }
        }
    }
}
