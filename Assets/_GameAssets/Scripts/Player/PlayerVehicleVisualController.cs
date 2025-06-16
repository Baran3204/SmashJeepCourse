using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerVehicleVisualController : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerVehicleController _playerVehicleController;
    [SerializeField] private Transform _wheelFrontLeft, _wheelFrontRight, _wheelBackLeft, _wheelBackRight;
    [SerializeField] private float _wheelSpinSpeed, _wheelYWhenSpringMin, _wheelYWhenSpringMax;
    [SerializeField] private Transform _jeepVisualTransform;
    [SerializeField] private Collider _jeepCollider;

    private Quaternion _wheelFrontLeftRoll;
    private Quaternion _wheelFrontRigthRoll;
    private float _forwardSpeed;
    private float _steerInput;
    private float _steerAngle => _playerVehicleController.setting.SteerAngle;
    private float _springRestLength => _playerVehicleController.setting.SpringRestLenght;
    private Dictionary<WheelType, float> _springsCurrentLength = new Dictionary<WheelType, float>
    {
        {WheelType.FrontLeft, 0f},
        {WheelType.FrontRight, 0f},
        {WheelType.BackLeft, 0f},
        {WheelType.BackRight, 0f}
    };
    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        _playerVehicleController.OnVehicleCrashed += () =>
        {
            enabled = false;
        };
    }
    private void Start()
    {
        _wheelFrontLeftRoll = _wheelFrontLeft.localRotation;
        _wheelFrontRigthRoll = _wheelFrontRight.localRotation;
    }
    private void Update()
    {
        if (!IsOwner) return;
        if (GameManager.Instance.GetCurrentGameState() != GameState.Playing) return;
        UpdateVisualStates();
        RotateWheels();
        SetSuspension();
    }
    private void UpdateVisualStates()
    {
        _steerInput = Input.GetAxisRaw("Horizontal");

        _forwardSpeed = Vector3.Dot(_playerVehicleController.forward, _playerVehicleController.Velocity);

        _springsCurrentLength[WheelType.FrontLeft] = _playerVehicleController.GetSpringCurrentLength(WheelType.FrontLeft);
        _springsCurrentLength[WheelType.FrontRight] = _playerVehicleController.GetSpringCurrentLength(WheelType.FrontRight);
        _springsCurrentLength[WheelType.BackLeft] = _playerVehicleController.GetSpringCurrentLength(WheelType.BackLeft);
        _springsCurrentLength[WheelType.BackRight] = _playerVehicleController.GetSpringCurrentLength(WheelType.BackRight);
    }

    private void RotateWheels()
    {
        if (_springsCurrentLength[WheelType.FrontLeft] < _springRestLength)
        {
            _wheelFrontLeftRoll *= Quaternion.AngleAxis(_forwardSpeed * _wheelSpinSpeed * Time.deltaTime, Vector3.right);
        }

        if (_springsCurrentLength[WheelType.FrontRight] < _springRestLength)
        {
            _wheelFrontRigthRoll *= Quaternion.AngleAxis(_forwardSpeed * _wheelSpinSpeed * Time.deltaTime, Vector3.right);
        }

        if (_springsCurrentLength[WheelType.BackLeft] < _springRestLength)
        {
            _wheelBackLeft.localRotation *= Quaternion.AngleAxis(_forwardSpeed * _wheelSpinSpeed * Time.deltaTime, Vector3.right);
        }

        if (_springsCurrentLength[WheelType.BackRight] < _springRestLength)
        {
            _wheelBackRight.localRotation *= Quaternion.AngleAxis(_forwardSpeed * _wheelSpinSpeed * Time.deltaTime, Vector3.right);
        }

        _wheelFrontLeft.localRotation = Quaternion.AngleAxis(_steerInput * _steerAngle, Vector3.up) * _wheelFrontLeftRoll;
        _wheelFrontRight.localRotation = Quaternion.AngleAxis(_steerInput * _steerAngle, Vector3.up) * _wheelFrontRigthRoll;
    }

    private void SetSuspension()
    {
        float springFrontLeftRatio = _springsCurrentLength[WheelType.FrontLeft] / _springRestLength;
        float springFrontRigthRatio = _springsCurrentLength[WheelType.FrontRight] / _springRestLength;
        float springBackLeftRatio = _springsCurrentLength[WheelType.BackLeft] / _springRestLength;
        float springBackRigthRatio = _springsCurrentLength[WheelType.BackRight] / _springRestLength;

        _wheelFrontLeft.localPosition = new Vector3(_wheelFrontLeft.localPosition.x,
        _wheelYWhenSpringMin + (_wheelYWhenSpringMax - _wheelYWhenSpringMin) * springFrontLeftRatio, _wheelFrontLeft.localPosition.z);

        _wheelFrontRight.localPosition = new Vector3(_wheelFrontRight.localPosition.x,
        _wheelYWhenSpringMin + (_wheelYWhenSpringMax - _wheelYWhenSpringMin) * springFrontRigthRatio, _wheelFrontRight.localPosition.z);

        _wheelBackLeft.localPosition = new Vector3(_wheelBackLeft.localPosition.x,
        _wheelYWhenSpringMin + (_wheelYWhenSpringMax - _wheelYWhenSpringMin) * springBackLeftRatio, _wheelBackLeft.localPosition.z);

        _wheelBackRight.localPosition = new Vector3(_wheelBackRight.localPosition.x,
        _wheelYWhenSpringMin + (_wheelYWhenSpringMax - _wheelYWhenSpringMin) * springBackRigthRatio, _wheelBackRight.localPosition.z);
    }
    [Rpc(SendTo.ClientsAndHost)]
    private void SetJeepVisualActiveRpc(bool active) => _jeepVisualTransform.gameObject.SetActive(active);

    private IEnumerator SetVehicleVisualActiveCourtine(float delay)
    {
        SetJeepVisualActiveRpc(false);
        _jeepCollider.enabled = false;

        yield return new WaitForSeconds(delay);

        SetJeepVisualActiveRpc(true);
        _jeepCollider.enabled = true;
        enabled = true;
    }

    public void SetvehicleActiveCourtine(float delay)
    {
        StartCoroutine(SetVehicleVisualActiveCourtine(delay));
    }
}
