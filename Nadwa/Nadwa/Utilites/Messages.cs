namespace Nadwa.Utilites;

public class Messages {
    public static class Success {
        public static string UserAdd = "User added successfully.";
        public static string UserDelete = "User added successfully.";
        public static string BookingEvent = "Congratulations, Event Booked Successfully";
        public static string CancelEvent = "Event canceled Successfully";
    }

    public static class Fail {
        public static string UserFound = "User cannot be found.";
        public static string UserAdd = "User cannot be added.";
        public static string UserDelete = "User cannot be deleted.";
        public static string BookingEvent = "User cannot book this event";
        public static string BookingHighCostEvent = "User cannot book this event due to low balance";
        public static string CancelEvent = "Event cannot be canceled";
    }
}