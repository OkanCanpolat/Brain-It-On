using UnityEngine;
using Zenject;

public class TouchStayMission : MonoBehaviour
{
    [SerializeField] private CollidableObjectType targetCollisionObject;
    private bool collisionOccured = false;
    private SignalBus signalBus;

    [Inject]
    public void Construct(SignalBus signalBus)
    {
        this.signalBus = signalBus;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collisionOccured) return;

        CollidableObject collidableObjectcollision = collision.collider.gameObject.GetComponent<CollidableObject>();

        if (collidableObjectcollision != null)
        {
            if (collidableObjectcollision.Type == targetCollisionObject)
            {
                collisionOccured = true;
                signalBus.TryFire<MissionFinishedSignal>();
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        CollidableObject collidableObjectcollision = collision.collider.gameObject.GetComponent<CollidableObject>();

        if (collidableObjectcollision != null)
        {
            if (collidableObjectcollision.Type == targetCollisionObject)
            {
                collisionOccured = false;
                signalBus.TryFire<MissionFinishCancelSignal>();
            }
        }
    }
}
