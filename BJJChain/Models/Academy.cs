namespace BJJChain.Models
{
    public class Academy
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }

        public Academy(string id, string name, string location)
        {
            Id = id;
            Name = name;
            Location = location;
        }

        public override string ToString()
        {
            return $"[{Id}] {Name} ({Location})";
        }
    }
}