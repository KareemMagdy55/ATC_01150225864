namespace Nadwa.Utilites;

public class Messages {
    public static class Success {
        public static string UserAdd = "User added successfully.";
        public static string UserDelete = "User added successfully.";
        public static string BookingEvent = "Congratulations, Event Booked Successfully";
        public static string CancelEvent = "Event canceled Successfully";
        public static string BalanceUpdate = "Balance updated Successfully";
        
        
        public static string EventUpdate = "Event updated Successfully";
        public static string AddEvent = "Event added Successfully";
        public static string EventDelete = "Event deleted Successfully";
        
    }

    public static class Fail {
        public static string UserFound = "User cannot be found.";
        public static string UserAdd = "User cannot be added.";
        public static string UserDelete = "User cannot be deleted.";
        public static string BookingEvent = "User cannot book this event";
        public static string BookingHighCostEvent = "User cannot book this event due to low balance";
        public static string CancelEvent = "Event cannot be canceled";
        public static string BalanceUpdate = "Balance cannot be updated";
        
        public static string EventUpdate = "Event cannot be updated";
        public static string AddEvent = "Event cannot be added";
        public static string EventDelete = "Event cannot be deleted";


        public static string AddEventMonth = "Date is invalid";
        public static string EventAlreadyExist { get; set; } = "Event already exist in user events";
        public static string EventNotExist { get; set; } = "Event not exit in user events";
    }
}