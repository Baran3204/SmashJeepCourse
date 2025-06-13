using UnityEngine;

[CreateAssetMenu(fileName = "Vehicle Settings", menuName = "Scriptable Objects/Vehicle Settings")]
public class VehicleSettingSO : ScriptableObject
{
    [Header("Wheel Paddings")]
    [SerializeField] private float _wheelsPaddingX;
    [SerializeField] private float _wheelsPaddingZ;

    [Header("Süspasyon")]
    [SerializeField] private float _springRestLenght;
    [SerializeField] private float _springStrength;
    [SerializeField] private float _springDamper;

    [Header("Handling")]
    [SerializeField] private float _steerAngle;
    [SerializeField] private float _frontWheelGripFactor;
    [SerializeField] private float _rearWheelGripFactor;

    [Header("Body")]
    [SerializeField] private float _tireMass;

    [Header("Power")]
    [SerializeField] private float _acceleratePower;
    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _maxReverseSpeed;
    [SerializeField] private float _brakesPower;
    
    [Header("Air Resistance")]
    [SerializeField] private float _airResistance;
    public float WheelPaddingX => _wheelsPaddingX;
    public float WheelPaddingZ => _wheelsPaddingZ;
    public float SpringRestLenght => _springRestLenght;
    public float SpringStrength => _springStrength;
    public float SpringDamper => _springDamper;
    public float SteerAngle => _steerAngle;
    public float FrontWheelGripFactor => _frontWheelGripFactor;
    public float RearWheelGripFactor => _rearWheelGripFactor;
    public float TierMass => _tireMass;
    public float AcceleratePower => _acceleratePower;
    public float MaxSpeed => _maxSpeed;
    public float MaxReverseSpeed => _maxReverseSpeed;
    public float BrakesPower => _brakesPower;
    public float AirResistance => _airResistance;

}
