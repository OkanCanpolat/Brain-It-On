using UnityEngine;
using Zenject;

public class DelayedPhysic : MonoBehaviour
{
    private SignalBus signalBus;
    private Rigidbody2D rb;

    [Inject]
    public void Construct(SignalBus signalBus)
    {
        this.signalBus = signalBus;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        signalBus.Subscribe<OnDrawSignal>(EnablePhysic);
    }


    private void EnablePhysic()
    {
        rb.isKinematic = false;
        signalBus.Unsubscribe<OnDrawSignal>(EnablePhysic);
    }
}
