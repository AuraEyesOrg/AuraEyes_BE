namespace Domain.Enums.Network;

/// <summary>
/// Category of professional network posts
/// </summary>
public enum PostCategory
{
    /// <summary>
    /// 🔬 Case presentation - sharing treatment experiences
    /// </summary>
    CasePresentation,
    
    /// <summary>
    /// 💬 Peer discussion - professional discussions among colleagues
    /// </summary>
    PeerDiscussion,
    
    /// <summary>
    /// 📚 Knowledge share - sharing knowledge, research, documents
    /// </summary>
    KnowledgeShare,
    
    /// <summary>
    /// 📢 Announcement - announcements from organisations
    /// </summary>
    Announcement
}

/// <summary>
/// Type of author for posts and comments
/// </summary>
public enum AuthorType
{
    Ophthalmologist,
    Organisation
}

/// <summary>
/// Type of reaction on posts
/// </summary>
public enum ReactionType
{
    /// <summary>
    /// 💡 Insightful
    /// </summary>
    Insightful,
    
    /// <summary>
    /// ✅ Agree
    /// </summary>
    Agree,
    
    /// <summary>
    /// 🙏 Helpful
    /// </summary>
    Helpful,
    
    /// <summary>
    /// ❓ Question
    /// </summary>
    Question,
    
    /// <summary>
    /// 🎉 Celebrate
    /// </summary>
    Celebrate
}

/// <summary>
/// Type of attachment in posts
/// </summary>
public enum AttachmentType
{
    /// <summary>
    /// Medical images
    /// </summary>
    Image,
    
    /// <summary>
    /// PDF, documents
    /// </summary>
    Document,
    
    /// <summary>
    /// Research papers
    /// </summary>
    Research
}


