    public interface IRequestModel<T>
    {
        string Serialize();
        T Deserialize(string json);
    }