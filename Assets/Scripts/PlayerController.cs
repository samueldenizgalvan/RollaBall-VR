using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour
{
    public float speed;
    private Rigidbody rb;
    private int count;

    // Used for jumping
    private bool isGrounded;
    public float jumpSpeed = 10f; // Ajuste para salto más controlado

    float objectHeight = 0;
    int winCount = 0;

    public TextMeshProUGUI countText;
    public TextMeshProUGUI winText;

    public Vector3 lastVelocity = Vector3.zero;

    // Punto de respawn parametrizado
    public Vector3 respawnPoint = new Vector3(0f, 0.5f, 0f);

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        count = 0;
        SetCountText();
        winText.text = "";

        MeshRenderer mr = GetComponent<MeshRenderer>();
        objectHeight = mr.bounds.extents.y;

        // Contar la cantidad de objetos con la etiqueta "Pick Up"
        winCount = GameObject.FindGameObjectsWithTag("Pick Up").Length;
    }

    private void Update()
    {
        lastVelocity = rb.velocity;
    }

    void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        RaycastHit hit;

        // Raycast para detectar si está en el suelo
        Physics.Raycast(transform.position, Vector3.down, out hit, objectHeight + 0.1f);

        if (hit.collider != null && 
            (hit.collider.gameObject.CompareTag("Ground") || hit.collider.gameObject.CompareTag("Bridge")))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }

        // Movimiento horizontal y vertical
        Vector3 movement = new Vector3(moveHorizontal, 0, moveVertical);

        // Saltar solo si está en el suelo
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            rb.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);
        }

        rb.AddForce(movement * speed);

        // Reducir velocidad solo si no hay input del jugador
        if (moveHorizontal == 0 && moveVertical == 0)
        {
            rb.velocity /= 1.1f;
        }

        // Respawn si el jugador cae
        if (transform.position.y <= -10)
        {
            transform.position = respawnPoint;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Pick Up"))
        {
            other.gameObject.SetActive(false);
            count = count + 1;
            SetCountText();
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Bridge"))
        {
            isGrounded = true;
        }
    }

    void SetCountText()
    {
        countText.text = "Count: " + count.ToString();
        if (count >= winCount)
        {
            winText.text = "You Win!";
        }
    }
}
