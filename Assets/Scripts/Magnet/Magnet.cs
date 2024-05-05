using System.Collections.Generic;
using UnityEngine;

public enum MagnetSign
{
    Positive, Negative
}
public class Magnet : MonoBehaviour
{
    [SerializeField] private MagnetSign sign;
    [SerializeField] private MagnetSign targetMagnetSign;
    [SerializeField] private Rigidbody2D magnetRB;
    [SerializeField] private float force;
    private List<Rigidbody2D> targetMagnets;

    public MagnetSign Sign => sign;
    public Rigidbody2D MagnetRigidbody => magnetRB;

    private void Awake()
    {
        targetMagnets = new List<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        foreach(Rigidbody2D rb in targetMagnets)
        {
            rb.AddForce((transform.position - rb.transform.position) * force * Time.deltaTime);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Magnet target = collision.gameObject.GetComponent<Magnet>();

        if (target != null && target.Sign == targetMagnetSign)
        {
            targetMagnets.Add(target.MagnetRigidbody);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        Magnet target = collision.gameObject.GetComponent<Magnet>();

        if (target != null && target.Sign == targetMagnetSign)
        {
            targetMagnets.Remove(target.MagnetRigidbody);
        }
    }
}
