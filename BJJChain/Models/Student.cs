using System;
using BJJChain.Enums;

namespace BJJChain.Models
{
    public class Student
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Belt CurrentBelt { get; set; }
        public DateTime RegistrationDate { get; set; }

        public Student(string id, string name)
        {
            Id = id;
            Name = name;
            CurrentBelt = Belt.White;
            RegistrationDate = DateTime.Now;
        }

        public override string ToString()
        {
            return $"[{Id}] {Name} - Current belt: {CurrentBelt}";
        }
    }
}