namespace MonarchMediaLLC.Shared
{
    public enum PackageType
    {
        Core,
        Professional,
        Premium
    }

    public enum Industry
    {
        General, 
        Construction, 
        Landscaping,
        Restaurant, 
        Healthcare,
        LawFirm,
    }

    public class TeamMember
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
        public string ImagePath { get; set; } = string.Empty;
    }

    public class ProjectSummary
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string TechStack { get; set; } = string.Empty;
        public string LiveUrl { get; set; } = string.Empty;
        public string ImagePath { get; set; } = string.Empty;
        public string ImageAlt { get; set; } = string.Empty;
        public PackageType Package { get; set; } = PackageType.Core;
        public bool Featured { get; set; } = false;
        public int DisplayOrder { get; set; } = 0;
        //Use a deterministic default to avoid EF Core reporting pending model changes
        //caused by a date initializer that changes daily.
        public DateOnly CompletedOn { get; set; } = DateOnly.MinValue;
        public bool IsPublic { get; set; } = true;
        public Industry Industry { get; set; } = Industry.General;
        public string ClientName { get; set; } = string.Empty;

        public ProjectSummary(ProjectSummary other)
        {
            Id = other.Id;
            Title = other.Title;
            Description = other.Description;
            TechStack = other.TechStack;
            LiveUrl = other.LiveUrl;
            ImagePath = other.ImagePath;
            ImageAlt = other.ImageAlt;
            Package = other.Package;
            Featured = other.Featured;
            DisplayOrder = other.DisplayOrder;
            CompletedOn = other.CompletedOn;
            IsPublic = other.IsPublic;
            Industry = other.Industry;
            ClientName = other.ClientName;
        }

        public ProjectSummary()
        {
        }
    }

    public class AdminLoginRequest
    {
        public string UserName { get; set; } = string.Empty;
        public string Passkey { get; set; } = string.Empty;
    }
}
