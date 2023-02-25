namespace datingApp.Entities;

public class UserLike
{
    public AppUser SourceUser { get; set; }
    public int SourseUserId { get; set; }
    public AppUser TargetUser { get; set; }
    public int TargetUserId { get; set; }
}