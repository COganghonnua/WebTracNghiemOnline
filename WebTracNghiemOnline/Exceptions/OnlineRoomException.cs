namespace WebTracNghiemOnline.Exceptions
{
    public class OnlineRoomException
    {
    }
    public class RoomNotFoundException : Exception
    {
        public RoomNotFoundException() : base("Room not found") { }
    }

    public class UserAlreadyInRoomException : Exception
    {
        public UserAlreadyInRoomException() : base("User already in the room") { }
    }
}
