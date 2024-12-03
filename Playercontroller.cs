using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro; // Fyrir TextMeshPro
using UnityEngine.SceneManagement; // Fyrir senu stjórn

public class Playercontroller : MonoBehaviour
{
    public float speed = 4.0f; // Hraði leikmanns
    public float sprintMultiplier = 1.5f; // Margföldun fyrir hraða í spretthlaupi
    public int maxPoints = 5; // Hámarks stig leikmanns
    public TMP_Text pointsText; // TextMeshPro fyrir stigaskjá
    public Transform respawnPosition; // Endurræsistaðsetning leikmanns
    public GameObject projectilePrefab; // Fyrirmynd fyrir skot
    public float projectileForce = 300.0f; // Kraftur fyrir skot
    public Tilemap tilemap; // Tilvísun í flísakort fyrir mörk
    public Camera mainCamera; // Tilvísun í myndavél

    private int currentPoints; // Núverandi stig leikmanns
    private Rigidbody2D rigidbody2d; // Rigidbody2D leikmanns
    private Animator animator; // Animator fyrir hreyfimyndir
    private AudioSource audioSource; // Hljóðspilari fyrir hljóð

    private Vector2 movementInput; // Hreyfistefna
    private Vector2 lookDirection = new Vector2(1, 0); // Sjálfgefin stefna sem leikmaður horfir í

    void Start()
    {
        // Athugar hvort leikmaður byrjar í senu 0 og endurræsir stöðu
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            if (respawnPosition != null)
            {
                transform.position = respawnPosition.position; // Setur upphafsstöðu
            }
            else
            {
                Debug.LogWarning("Engin endurræsistaðsetning er stillt!"); // Vísbending ef vantar
            }
        }

        rigidbody2d = GetComponent<Rigidbody2D>(); // Nær í Rigidbody2D
        animator = GetComponent<Animator>(); // Nær í Animator
        audioSource = GetComponent<AudioSource>(); // Nær í hljóðspilara

        currentPoints = maxPoints; // Stillir upphafsstig leikmanns
        UpdatePointsText(); // Uppfærir stigaskjá
    }

    void Update()
    {
        HandleMovement(); // Sér um hreyfingu leikmanns
        UpdateCameraPosition(); // Uppfærir myndavélastöðu

        if (Input.GetKeyDown(KeyCode.Space)) // Athugar hvort leikmaður skýtur
        {
            Shoot();
        }
    }

    void HandleMovement()
    {
        Vector2 position = rigidbody2d.position;

        // Meðhöndlun hreyfingar
        Vector2 inputDirection = Vector2.zero;
        if (Input.GetKey(KeyCode.W)) inputDirection.y += 1; // Upp
        if (Input.GetKey(KeyCode.S)) inputDirection.y -= 1; // Niður
        if (Input.GetKey(KeyCode.A)) inputDirection.x -= 1; // Vinstri
        if (Input.GetKey(KeyCode.D)) inputDirection.x += 1; // Hægri

        if (inputDirection.magnitude > 1) inputDirection.Normalize(); // Normalíserar átt ef hún er ská

        float currentSpeed = speed;
        if (Input.GetKey(KeyCode.LeftShift)) // Ef leikmaður sprettir
        {
            currentSpeed *= sprintMultiplier;
        }

        Vector2 newPosition = position + inputDirection * currentSpeed * Time.deltaTime;

        if (IsPositionInsideTilemap(newPosition)) // Athugar hvort ný staðsetning er innan flísakorts
        {
            rigidbody2d.MovePosition(newPosition); // Uppfærir stöðu leikmanns
        }

        // Uppfærir hreyfimyndir og horfustefnu
        if (inputDirection != Vector2.zero)
        {
            lookDirection = inputDirection;
            animator.SetFloat("Look X", lookDirection.x);
            animator.SetFloat("Look Y", lookDirection.y);
        }

        animator.SetFloat("Speed", inputDirection.magnitude * (Input.GetKey(KeyCode.LeftShift) ? sprintMultiplier : 1));
    }

    void UpdateCameraPosition()
    {
        if (mainCamera != null)
        {
            Vector3 newCameraPosition = transform.position;
            newCameraPosition.z = -10; // Heldur myndavél í 2D-Z plani
            mainCamera.transform.position = newCameraPosition;
        }
    }

    void Shoot()
    {
        GameObject projectile = Instantiate(projectilePrefab, rigidbody2d.position + lookDirection * 0.5f, Quaternion.identity);
        Rigidbody2D projectileRigidBody = projectile.GetComponent<Rigidbody2D>();
        projectileRigidBody.AddForce(lookDirection * projectileForce);
        animator.SetTrigger("Shoot");
    }

    public void ChangePoints(int amount)
    {
        currentPoints = Mathf.Clamp(currentPoints + amount, 0, maxPoints); // Uppfærir stig
        UpdatePointsText();

        if (currentPoints == 0) // Ef stig eru núll, fer í senu 0
        {
            RespawnInScene0();
        }
    }

    void UpdatePointsText()
    {
        if (pointsText != null)
        {
            pointsText.text = "Stig: " + currentPoints; // Uppfærir stigaskjá
        }
    }

    void RespawnInScene0()
    {
        SceneManager.LoadScene(0); // Endurræsir leikmann í senu 0
    }

    private bool IsPositionInsideTilemap(Vector2 position)
    {
        Vector3Int tilePosition = tilemap.WorldToCell(position); // Breytir heiminum í flísastöðu
        return tilemap.HasTile(tilePosition); // Athugar hvort flís sé á staðsetningu
    }
}
