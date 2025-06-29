namespace TimeTile.API.Common.Api.Json
{
    public readonly struct PatchOptionalProperty<T>
    {
        public bool WasProvided { get; }
        public T? Value { get; }

        public PatchOptionalProperty(T? value, bool wasProvided = true)
        {
            Value = value;
            WasProvided = wasProvided;
        }

        public static PatchOptionalProperty<T> NotProvided => new(default, false);

        public static implicit operator PatchOptionalProperty<T>(T? value) => new(value);

        public bool HasValue => WasProvided && Value is not null;
    }
}
