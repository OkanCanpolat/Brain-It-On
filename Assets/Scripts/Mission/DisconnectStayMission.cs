using UnityEngine;
using Zenject;

public class DisconnectStayMission : MonoBehaviour
{
    [SerializeField] private CollidableObjectType targetCollisionObject;
    private bool disconnected = false;
    private SignalBus signalBus;

    [Inject]
    public void Construct(SignalBus signalBus)
    {
        this.signalBus = signalBus;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (disconnected) return;

        CollidableObject collidableObjectcollision = collision.collider.gameObject.GetComponent<CollidableObject>();

        if (collidableObjectcollision != null)
        {
            if (collidableObjectcollision.Type == targetCollisionObject)
            {
                disconnected = true;
                signalBus.TryFire<MissionFinishedSignal>();
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!disconnected) return;

        CollidableObject collidableObjectcollision = collision.collider.gameObject.GetComponent<CollidableObject>();

        if (collidableObjectcollision != null)
        {
            if (collidableObjectcollision.Type == targetCollisionObject)
            {
                disconnected = false;
                signalBus.TryFire<MissionFinishCancelSignal>();
            }
        }
    }
}
