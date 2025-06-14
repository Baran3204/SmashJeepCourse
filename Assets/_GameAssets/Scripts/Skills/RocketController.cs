using Unity.Netcode;
using UnityEngine;
public class RocketController : NetworkBehaviour
{
    [SerializeField] private Collider _rocketCollider;
    [SerializeField] private float _movementSpeed;
    [SerializeField] private float _rotationSpeed;
    private bool _isMoving;

    [Rpc(SendTo.Owner)]
    private void SetOwnerVisualsRpc()
    {
        _rocketCollider.enabled = false;
    }
    private void Update()
    {
        if (IsServer && _isMoving)
        {
            MoveRocket();
        }
    }
    public override void OnNetworkSpawn()
    {
        if (IsOwner) SetOwnerVisualsRpc();
        RequestForMovementRpc();
    }   
    [Rpc(SendTo.Server)]
    private void RequestForMovementRpc()
    {
        _isMoving = true;
    }
    private void MoveRocket()
    {
        transform.position += _movementSpeed * transform.forward * Time.deltaTime;
        transform.Rotate(Vector3.forward, _rotationSpeed * Time.deltaTime, Space.Self);
    }
}
