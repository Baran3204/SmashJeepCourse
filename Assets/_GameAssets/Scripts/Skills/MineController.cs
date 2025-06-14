using Unity.Netcode;
using UnityEditor;
using UnityEngine;

public class MineController : NetworkBehaviour
{
    [SerializeField] private Collider _mineCollider;
    [SerializeField] private float _fallsSpeed;
    [SerializeField] private float _rayDistance;
    [SerializeField] private LayerMask _groundLayer;

    private bool _hasLanded;
    private Vector3 _lastSendPos;
    public override void OnNetworkSpawn()
    {
        if (IsOwner) SetOwnerVisualsRpc();
    }

    private void Update()
    {
        if (!IsServer && _hasLanded) return;

        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit ht, _rayDistance, _groundLayer))
        {
            _hasLanded = true;
            transform.position = ht.point + new Vector3(0f, 0.1f,0f);
            if (_lastSendPos != transform.position)
            {
                SyncPosRpc(transform.position);
                _lastSendPos = transform.position;
            }
        }
        else
        {
            transform.position += _fallsSpeed * Vector3.down * Time.deltaTime;
            if (_lastSendPos != transform.position)
            {
                SyncPosRpc(transform.position);
                _lastSendPos = transform.position;
            }
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void SyncPosRpc(Vector3 pos)
    {
        if (IsServer) return;
        transform.position = pos;
    }

    [Rpc(SendTo.Owner)]
    private void SetOwnerVisualsRpc()
    {
        _mineCollider.enabled = false;
    }
}
