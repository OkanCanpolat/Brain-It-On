using UnityEngine;
using Zenject;

public class ExitStayMission : MonoBehaviour
{
    [SerializeField] private CollidableObjectType targetCollisionObject;
    [SerializeField] private int targetCount;
    private bool completed = false;
    private SignalBus signalBus;
    private int currentCount;

    [Inject]
    public void Construct(SignalBus signalBus)
    {
        this.signalBus = signalBus;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!completed) return;

        CollidableObject collidableObjectcollision = collision.gameObject.GetComponent<Collider2D>().gameObject.GetComponent<CollidableObject>();

        if (collidableObjectcollision != null && collidableObjectcollision.Type == targetCollisionObject)
        {

            currentCount--;

            if (currentCount == targetCount)
            {
                completed = true;
                signalBus.TryFire<MissionFinishedSignal>();
            }

            if (completed && currentCount < targetCount)
            {
                signalBus.TryFire<MissionFinishCancelSignal>();
                completed = false;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        CollidableObject collidableObjectcollision = collision.gameObject.GetComponent<Collider2D>().gameObject.GetComponent<CollidableObject>();
        
        if (collidableObjectcollision != null && collidableObjectcollision.Type == targetCollisionObject)
        {
            currentCount++;

            if (currentCount == targetCount)
            {
                completed = true;
                signalBus.TryFire<MissionFinishedSignal>();
            }

            if (completed && currentCount > targetCount)
            {
                signalBus.TryFire<MissionFinishCancelSignal>();
                completed = false;
            }
        }
    }
}
