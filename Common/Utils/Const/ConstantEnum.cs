namespace Common.Utils.Const;

public static class ConstantEnum
{
    public enum Role
    {
        Student = 1,
        Consultant = 2,
        Admin = 3,
    }
    
    public enum ScheduleStatus
    {
        Available = 1,
        Pending = 2,
        Booked = 3,
    }
    
    public enum AppointmentStatus
    {
        Pending = 1,
        Booked = 2,
        Completed = 3,
        Cancelled = 4,
    }

    public enum PaymentId
    {
        Success = 00,
    }
    
    public enum PaymentStatus
    {
        Pending = 1,
        Success = 2,
        Failed = 3,
    }
    
    public enum PaymentMethod
    {
        PayOs = 1,
        MoMo = 2,
        ZaloPay = 3,
        BankTransfer = 4,
    }
    
}