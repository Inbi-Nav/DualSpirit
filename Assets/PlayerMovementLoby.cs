using UnityEngine;

public class PlayerMovementLoby : MonoBehaviour
{
    public float moveSpeed = 5f; 
    public Animator animator;    
    public float stopDistance = 0.1f; 
    public Vector3 playerScale = new Vector3(1f, 1f, 1f); 

    private float minX, maxX; 
    private int lastDirection = 1; 

    void Start()
    {
        if (animator == null)
        {
            Debug.LogError("Animator no está asignado en el Inspector.");
        }

        CalculateScreenBounds();

        transform.localScale = playerScale;
    }

    void Update()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0; 

        Vector2 direction = new Vector2(mousePosition.x - transform.position.x, 0);

        if (direction.magnitude <= stopDistance)
        {
            animator.SetFloat("Direction", 0); 
            return; 
        }

        direction.Normalize();

        Vector3 newPosition = transform.position + (Vector3)direction * moveSpeed * Time.deltaTime;

        newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);

        transform.position = newPosition;

        if (direction.x > 0 && lastDirection != 1)
        {
            transform.localScale = new Vector3(playerScale.x, playerScale.y, playerScale.z); 
            animator.SetFloat("Direction", 1); 
            lastDirection = 1; 
        }
        else if (direction.x < 0 && lastDirection != -1)
        {
            transform.localScale = new Vector3(-playerScale.x, playerScale.y, playerScale.z); 
            animator.SetFloat("Direction", -1); 
            lastDirection = -1;
        }
    }

    void CalculateScreenBounds()
    {
        float screenAspect = (float)Screen.width / (float)Screen.height;
        float cameraHeight = Camera.main.orthographicSize * 2;
        float cameraWidth = cameraHeight * screenAspect;

        minX = Camera.main.transform.position.x - (cameraWidth / 2);
        maxX = Camera.main.transform.position.x + (cameraWidth / 2);
    }
}