namespace TimeTile.API.Users.Requests
{
    public interface ICreateUserRequest
    {
        string Firstname { get; }
        string Lastname { get; }
        string PhoneNumber { get; }
        string HomeAddress { get; }
        DateOnly BirthDate { get; }
    }
}
