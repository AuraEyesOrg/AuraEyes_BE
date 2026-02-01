using Domain.Common;
using Domain.Enums.Network;

namespace Domain.Entities.Network;

/// <summary>
/// Follow relationship between users/organisations
/// </summary>
public class Follow : BaseEntity
{
    /// <summary>
    /// Follower ID (NO FK - just stores ID)
    /// </summary>
    public Guid FollowerId { get; private set; }
    
    /// <summary>
    /// Type of follower
    /// </summary>
    public FollowerType FollowerType { get; private set; }
    
    /// <summary>
    /// Following ID (NO FK - just stores ID)
    /// </summary>
    public Guid FollowingId { get; private set; }
    
    /// <summary>
    /// Type of entity being followed
    /// </summary>
    public FollowingType FollowingType { get; private set; }
    
    /// <summary>
    /// Whether notifications are enabled for this follow
    /// </summary>
    public bool NotificationsEnabled { get; private set; } = true;

    private Follow() { } // EF Core

    /// <summary>
    /// Create a follow relationship
    /// </summary>
    public Follow(
        Guid followerId,
        FollowerType followerType,
        Guid followingId,
        FollowingType followingType,
        bool notificationsEnabled = true)
    {
        if (followerId == followingId)
            throw new ArgumentException("Cannot follow yourself");

        FollowerId = followerId;
        FollowerType = followerType;
        FollowingId = followingId;
        FollowingType = followingType;
        NotificationsEnabled = notificationsEnabled;
    }

    /// <summary>
    /// Toggle notifications
    /// </summary>
    public void ToggleNotifications()
    {
        NotificationsEnabled = !NotificationsEnabled;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Enable notifications
    /// </summary>
    public void EnableNotifications()
    {
        NotificationsEnabled = true;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Disable notifications
    /// </summary>
    public void DisableNotifications()
    {
        NotificationsEnabled = false;
        UpdatedAt = DateTime.UtcNow;
    }
}
