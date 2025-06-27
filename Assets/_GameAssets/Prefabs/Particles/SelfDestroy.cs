using Unity.Netcode;
using UnityEngine;

public class SelfDestroy : NetworkBehaviour
{
    [SerializeField] private float _destroyDur;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            Destroy(gameObject, _destroyDur);
        }
    }
}
