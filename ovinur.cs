using UnityEngine;
using UnityEngine.Tilemaps;

public class Enemy : MonoBehaviour
{
    public float speed = 2.0f; // Hraði óvinar
    public float timeToChange = 2.0f; // Tími áður en óvinur skiptir um átt
    public bool horizontal = true; // Ef satt, hreyfist lárétt; annars lóðrétt
    public GameObject smokeParticleEffect; // Eiturpartíklar
    public ParticleSystem fixedParticleEffect; // Partíklar þegar óvinur er lagaður
    public Tilemap tilemap; // Tilvísun í flísakort

    private Rigidbody2D rigidbody2d; // Tilvísun í Rigidbody2D
    private Vector2 direction; // Núverandi átt óvinar
    private bool repaired = false; // Ef óvinur hefur verið lagaður
    private Animator animator; // Tilvísun í Animator

    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        direction = horizontal ? Vector2.right : Vector2.down; // Velur upphafs átt
        animator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        if (!repaired) // Athugar hvort óvinur er lagaður
        {
            Vector2 position = rigidbody2d.position + direction * speed * Time.fixedDeltaTime;

            // Athugar hvort næsta staðsetning er innan flísakorts
            if (IsPositionInsideTilemap(position))
            {
                rigidbody2d.MovePosition(position);
            }
            else
            {
                // Skipti um átt ef óvinur nær út fyrir flísakort
                direction *= -1;
            }
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        Playercontroller player = other.collider.GetComponent<Playercontroller>();
        if (player != null) // Ef leikmaður snertir óvin
        {
            player.ChangePoints(-1); // Dregur stig af leikmanni
        }
    }

    public void Fix()
    {
        repaired = true; // Stillir sem lagað
        rigidbody2d.simulated = false; // Fjarlægir eðlisfræði frá óvin
        animator.SetTrigger("Fixed"); // Spilar lagaðan hreyfimynd
    }

    private bool IsPositionInsideTilemap(Vector2 position)
    {
        Vector3Int tilePosition = tilemap.WorldToCell(position); // Breytir heiminum í flísastöðu
        return tilemap.HasTile(tilePosition); // Athugar hvort flís sé á stöðu
    }
}
