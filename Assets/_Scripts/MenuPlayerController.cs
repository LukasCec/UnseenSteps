using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;


public class MenuPlayerController : MonoBehaviour
{
    Rigidbody2D rb;
    Animator animator;

    [Header("Movement")]
    public float targetX = -119f; // Koncová X pozícia
    public float startX = -122f; // Počiatočná X pozícia
    public float startY = 5f; // Nová: Počiatočná Y pozícia (vyššie, aby spadol)
    public float moveSpeed = 1.5f; // Rýchlosť pohybu

    [Header("Ground Check")]
    public Transform groundCheck; // Prázdny GameObject v spodnej časti postavy
    public float groundCheckDistance = 0.2f;
    public LayerMask groundLayer; // Vrstva, ktorá je považovaná za zem
    private bool isGrounded = false;

    [Header("Attack")] // NOVÉ NASTAVENIA ÚTOKU
    public float attackDuration = 0.5f; // Dĺžka trvania animácie útoku (upravte podľa dĺžky vašej animácie)
    private bool isAttacking = false;

    // Stavový príznak, ktorý hovorí, že úvodný pohyb je dokončený
    private bool isFinishedMoving = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        if (rb == null || animator == null || groundCheck == null)
        {
            // Kritická chyba: ak chýbajú kľúčové komponenty pre simuláciu pádu
            Debug.LogError("Chýba Rigidbody2D, Animator, alebo GroundCheck Transform. Pád a animácie nebudú fungovať!");
            enabled = false;
            return;
        }

        // Nastavíme počiatočnú pozíciu (získame aktuálnu Z, ale nastavíme X a Y)
        transform.position = new Vector3(startX, startY, transform.position.z);

        // Zapneme gravitáciu (ak nebola predtým zapnutá)
        rb.isKinematic = false;
        
        // Otočenie
        if (targetX > startX && transform.localScale.x < 0) Flip();
        else if (targetX < startX && transform.localScale.x > 0) Flip();

        // Spustíme rutinu pádu a pohybu
        StartCoroutine(FallAndMove());
    }

    void Update()
    {

        // Útok je povolený len po dokončení úvodného pohybu a keď postava neútočí
        if (isFinishedMoving && Input.GetMouseButtonDown(0) && !isAttacking)
        {
            StartCoroutine(AttackRoutine());
        }

        // Kontrola resetu scény
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetScene();
        }
    }

    void FixedUpdate()
    {
        // V MenuController zjednodušená fyzika: len kontrola zeme a aktualizácia animátora.
        CheckGround();
        UpdateAnimator();
    }
    
    private void CheckGround()
    {
        // Raycast kontroluje, či je pod hráčom zem
        RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayer);

        bool wasGrounded = isGrounded;
        isGrounded = hit.collider != null;

        // Ak Animator Controller používa parameter isGrounded, pošleme mu aktuálny stav
        if (isGrounded != wasGrounded && animator != null)
        {
            animator.SetBool(AnimationStrings.IsGrounded, isGrounded);
        }
    }

    void UpdateAnimator()
    {
        if (animator == null) return;
        
        // Nastavuje YVelocity pre animácie skok/pád
        animator.SetFloat(AnimationStrings.YVelocity, rb.linearVelocity.y);
        
        // IsMoving (pre chôdzu) sa nastavuje iba v korutine MoveToPosition() 
        //, keď je postava v horizontálnom pohybe
    }

    private IEnumerator FallAndMove()
    {
        // ====================================
        // KROK 1: PÁD (Čakáme, kým dopadne na zem)
        // ====================================
        
        // Ak nechceme, aby postava skákala hneď po dopade, môžeme jej vynulovať vertikálnu rýchlosť
        // po dopade. Tu ju necháme na gravitácii.
        
        Debug.Log("Začínam pád...");
        
        // Čakáme, kým sa isGrounded prepne na true
        yield return new WaitUntil(() => isGrounded);

        Debug.Log("Dopadol som na zem. Spúšťam pohyb.");
        
        // Po dopade (isGrounded = true) prepneme na Idle animáciu. 
        // Teraz môžeme začať horizontálny pohyb.

        // ====================================
        // KROK 2: POHYB DO CIEĽOVEJ POZÍCIE
        // ====================================
        
        if (animator != null)
        {
            // Spustenie animácie chôdze
            animator.SetBool(AnimationStrings.IsMoving, true);
        }
        
        Vector3 startPos = transform.position;
        Vector3 targetPos = new Vector3(targetX, transform.position.y, transform.position.z);
        float distance = Mathf.Abs(targetX - startX);
        float duration = distance / moveSpeed;
        float startTime = Time.time;
        
        while (transform.position.x != targetX)
        {
            float t = (Time.time - startTime) / duration;
            
            // Pohyb riadime priamo transformáciou, aby sme neovplyvňovali Rigidbody pre pád.
            // Rigidbody je potrebné hlavne pre YVelocity a isGrounded v UpdateAnimator/CheckGround.
            float newX = Mathf.Lerp(startPos.x, targetPos.x, t);
            transform.position = new Vector3(newX, transform.position.y, transform.position.z);


            // TIP: Ak by ste chceli použiť Rigidbody, použite rb.velocity = new Vector2(horizontal, rb.velocity.y);
            // Ale pre menu je transform.position jednoduchšie.
            
            if (t >= 1.0f)
            {
                transform.position = new Vector3(targetX, transform.position.y, transform.position.z); 
                break;
            }

            yield return null;
        }

        // ====================================
        // KROK 3: ZASTAVENIE A IDLE ANIMÁCIA
        // ====================================
        
        if (animator != null)
        {
            // Zastavenie animácie chôdze -> prechod na Idle
            animator.SetBool(AnimationStrings.IsMoving, false);
        }

        // Ak Rigidbody už nie je potrebné, môžeme ho prepnúť na IsKinematic
        rb.bodyType = RigidbodyType2D.Static; 

        // Hráč je v stave Idle a môže interagovať
        isFinishedMoving = true;
    }


    private IEnumerator AttackRoutine()
    {
        isAttacking = true;
        
        if (animator != null)
        {
            // Spustenie animácie útoku (vyžaduje parameter 'attack' typu Trigger)
            animator.SetTrigger(AnimationStrings.Attack);
        }
        
        // Čakáme na dokončenie animácie
        yield return new WaitForSeconds(attackDuration);

        // Reset stavu. Postava sa automaticky vráti do Idle, 
        // pretože isMoving je False a Animator Controller je správne nastavený
        isAttacking = false;
    }

    void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    void ResetScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    // Pre vizualizáciu v Editore
    void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(groundCheck.position, (Vector2)groundCheck.position + Vector2.down * groundCheckDistance);
        }
    }
}