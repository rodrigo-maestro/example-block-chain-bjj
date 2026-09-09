using BJJChain.Enums;

namespace BJJChain.Models
{
    public class Instructor
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Belt MaxAuthorizationLevel { get; set; }
        public string AcademyId { get; set; }

        public Instructor(string id, string name, Belt maxAuthorizationLevel, string academyId)
        {
            Id = id;
            Name = name;
            MaxAuthorizationLevel = maxAuthorizationLevel;
            AcademyId = academyId;
        }

        public override string ToString()
        {
            return $"[{Id}] {Name} - Authorized up to: {MaxAuthorizationLevel}";
        }
    }
}