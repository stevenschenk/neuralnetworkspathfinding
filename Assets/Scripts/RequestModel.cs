
    using UnityEngine;

public class RequestModel<T> : IRequestModel<T>
    {
        public string Serialize()
        {
            return JsonUtility.ToJson(this);
        }

        public T Deserialize(string json)
        {
            return JsonUtility.FromJson<T>(json);
        }
    }