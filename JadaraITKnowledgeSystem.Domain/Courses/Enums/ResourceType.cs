namespace JadaraITKnowledgeSystem.Domain.Courses.Enums
{
    public enum ResourceType
    {
        // Our platform courses
        ECampusCourse = 1,

        // External video resources
        YouTubeVideo = 2,
        YouTubePlaylist = 3,

        // Written content
        Article = 4,
        BlogPost = 5,

        // Other popular platforms
        UdemyCourse = 10,
        CourseraCourse = 11,
        EdXCourse = 12,
        LinkedInLearning = 13,
        PluralSight = 14,

        // General
        OtherPlatformCourse = 98,
        Other = 99
    }
}