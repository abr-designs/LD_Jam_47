using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class Vector3Converter : JsonConverter<Vector3>
{
    private struct jsonV3
    {
        public float x, y, z;
    }
    
    public override void WriteJson(JsonWriter writer, Vector3 value, JsonSerializer serializer)
    {
        var data = new jsonV3
        {
            x = value.x,
            y = value.y,
            z = value.z
        };
        
        serializer.Serialize(writer, data);
    }

    public override Vector3 ReadJson(JsonReader reader, Type objectType, Vector3 existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var data = serializer.Deserialize(reader, typeof(jsonV3));

        if (data == null)
            return Vector3.zero;

        var coord = (jsonV3) data;
        
        return new Vector3(coord.x, coord.y, coord.z);
    }
}