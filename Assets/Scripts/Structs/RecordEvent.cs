using Newtonsoft.Json;
using UnityEngine;

public struct RecordEvent
{
    public ABILITY Ability;
    [JsonConverter(typeof(Vector3Converter))]
    public Vector3 Position;
    [JsonConverter(typeof(Vector3Converter))]
    public Vector3 Direction;
    public STATE State;
    public float Time;
}