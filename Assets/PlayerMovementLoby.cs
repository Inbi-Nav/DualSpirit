using UnityEngine;

public class PlayerMovementLoby : MonoBehaviour
{
    public float moveSpeed = 5f; 
    public Animator animator;    
    public float stopDistance = 0.1f; 
    public Vector3 playerScale = new Vector3(1.5f, 1.5f, 1.5f); // Escala deseada del jugador

    private float minX, maxX; 
    private int lastDirection = 1; // 1 = derecha, -1 = izquierda

    void Start()
    {
        if (animator == null)
        {
            Debug.LogError("Animator no está asignado en el Inspector.");
        }

        CalculateScreenBounds();

        // Aplica la escala inicial al jugador
        transform.localScale = playerScale;
    }

    void Update()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0; 

        // Calcula la dirección hacia el ratón (solo en el eje X)
        Vector2 direction = new Vector2(mousePosition.x - transform.position.x, 0);

        // Si el ratón está lo suficientemente cerca, detén al jugador
        if (direction.magnitude <= stopDistance)
        {
            animator.SetFloat("Direction", 0); // Quieto (Idle)
            return; // No hagas nada más
        }

        // Normaliza la dirección para el movimiento
        direction.Normalize();

        // Mueve al personaje (solo en el eje X)
        Vector3 newPosition = transform.position + (Vector3)direction * moveSpeed * Time.deltaTime;

        // Restringe el movimiento dentro de los límites horizontales
        newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);

        // Aplica la nueva posición
        transform.position = newPosition;

        // Cambia la dirección y la escala solo si es necesario
        if (direction.x > 0 && lastDirection != 1)
        {
            transform.localScale = new Vector3(playerScale.x, playerScale.y, playerScale.z); // Mirando a la derecha
            animator.SetFloat("Direction", 1); // Camina hacia la derecha
            lastDirection = 1; // Actualiza la dirección actual
        }
        else if (direction.x < 0 && lastDirection != -1)
        {
            transform.localScale = new Vector3(-playerScale.x, playerScale.y, playerScale.z); // Mirando a la izquierda
            animator.SetFloat("Direction", -1); // Camina hacia la izquierda
            lastDirection = -1; // Actualiza la dirección actual
        }
    }

    void CalculateScreenBounds()
    {
        // Obtén el tamaño de la pantalla en coordenadas del mundo
        float screenAspect = (float)Screen.width / (float)Screen.height;
        float cameraHeight = Camera.main.orthographicSize * 2;
        float cameraWidth = cameraHeight * screenAspect;

        // Calcula los límites horizontales
        minX = Camera.main.transform.position.x - (cameraWidth / 2);
        maxX = Camera.main.transform.position.x + (cameraWidth / 2);
    }
}