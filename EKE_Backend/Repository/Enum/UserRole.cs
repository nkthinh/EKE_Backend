using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Enums
{
    public enum UserRole
    {
        Unspecified = 0,
        Student = 1,
        Tutor = 2,
        Admin = 3
    }

    public enum Gender
    {
        Male = 1,
        Female = 2,
        Other = 3
    }

    public enum SwipeActionType
    {
        Like = 1,
        Dislike = 2,
        SuperLike = 3
    }

    public enum MatchStatus
    {
        Active = 1,
        Inactive = 2,
        Blocked = 3
    }

    public enum MessageType
    {
        Text = 1,
        Image = 2,
        File = 3,
        System = 4
    }

    public enum BookingStatus
    {
        Pending = 1,
        Confirmed = 2,
        Completed = 3,
        Cancelled = 4
    }

    public enum LocationType
    {
        Online = 1,
        TutorPlace = 2,
        StudentPlace = 3,
        PublicPlace = 4
    }

    public enum NotificationType
    {
        Match = 1,
        Message = 2,
        Booking = 3,
        Review = 4,
        System = 5
    }

    public enum VerificationStatus
    {
        Pending = 1,
        Verified = 2,
        Rejected = 3
    }

    public enum ProficiencyLevel
    {
        Beginner = 1,
        Intermediate = 2,
        Advanced = 3,
        Expert = 4
    }

    public enum LearningStyle
    {
        Visual = 1,
        Auditory = 2,
        Kinesthetic = 3,
        Reading = 4
    }

    public enum SenderType
    {
        User = 1,
        Ai = 2
    }
}

