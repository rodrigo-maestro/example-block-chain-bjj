using System;
using BJJChain.Enums;

namespace BJJChain.Models
{
    public class GraduationEvent
    {
        public string StudentId { get; set; }
        public string StudentName { get; set; }
        public string AcademyId { get; set; }
        public string InstructorId { get; set; }
        public Belt PreviousBelt { get; set; }
        public Belt NewBelt { get; set; }
        public DateTime Date { get; set; }

        public GraduationEvent(
            string studentId,
            string studentName,
            string academyId,
            string instructorId,
            Belt previousBelt,
            Belt newBelt,
            DateTime date)
        {
            StudentId = studentId;
            StudentName = studentName;
            AcademyId = academyId;
            InstructorId = instructorId;
            PreviousBelt = previousBelt;
            NewBelt = newBelt;
            Date = date;
        }

        public override string ToString()
        {
            return $"{StudentName}: {PreviousBelt} → {NewBelt} em {Date:dd/MM/yyyy}";
        }
    }

}
