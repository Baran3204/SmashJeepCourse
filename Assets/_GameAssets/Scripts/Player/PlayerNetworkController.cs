using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetworkController : NetworkBehaviour
{
    [SerializeField] private CinemachineCamera _camera;
    private PlayerVehicleController _playerVehicleController;
    private PlayerSkillController _playerSkillController;
    private PlayerInteractionController _playerInteractionController;
    public override void OnNetworkSpawn()
    {
        _camera.gameObject.SetActive(IsOwner);

        if (!IsOwner) return;

        _playerVehicleController = GetComponent<PlayerVehicleController>();
        _playerSkillController = GetComponent<PlayerSkillController>();
        _playerInteractionController = GetComponent<PlayerInteractionController>();
    }

    public void OnPlayerRespawned()
    {
        _playerVehicleController.OnPlayerRespawned();
        _playerSkillController.OnPlayerRespawned();
        _playerInteractionController.OnPlayerRespawned();
    }
}
