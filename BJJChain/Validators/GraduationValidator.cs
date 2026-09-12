using System.Collections.Generic;
using BJJChain.Enums;
using BJJChain.Models;

namespace BJJChain.Validators
{
    public class GraduationValidator
    {
        private Dictionary<string, Instructor> instructors;
        private Dictionary<string, Student> students;

        public GraduationValidator()
        {
            instructors = new Dictionary<string, Instructor>();
            students = new Dictionary<string, Student>();
        }

        /// <summary>
        /// Registers an instructor
        /// </summary>
        public void RegisterInstructor(Instructor instructor)
        {
            instructors[instructor.Id] = instructor;
        }

        /// <summary>
        /// Registers a student
        /// </summary>
        public void RegisterStudent(Student student)
        {
            students[student.Id] = student;
        }

        /// <summary>
        /// Validates if a graduation is allowed
        /// </summary>
        public (bool IsValid, string Message) ValidateGraduation(
            string studentId,
            string instructorId,
            Belt newBelt)
        {
            // Checks if the student exists
            if (!students.ContainsKey(studentId))
            {
                return (false, "X Student not found.");
            }

            // Checks if the instructor exists
            if (!instructors.ContainsKey(instructorId))
            {
                return (false, "X Instructor not found.");
            }

            Student student = students[studentId];
            Instructor instructor = instructors[instructorId];

            // Checks if the new belt is higher than the current belt
            if (newBelt <= student.CurrentBelt)
            {
                return (false, $"X The new belt ({newBelt}) must be higher than the current belt ({student.CurrentBelt}).");
            }

            // Checks if the instructor is authorized for this belt
            if (newBelt > instructor.MaxAuthorizationLevel)
            {
                return (false, $"X The instructor is not authorized to grant the {newBelt} belt. Maximum authorized: {instructor.MaxAuthorizationLevel}.");
            }

            // Checks for belt sequence (no skipping belts)
            if ((int)newBelt != (int)student.CurrentBelt + 1)
            {
                return (false, $"X Belt skipping is not allowed. Next expected belt: {(Belt)((int)student.CurrentBelt + 1)}.");
            }

            return (true, "✓ Graduation authorized!");
        }

        /// <summary>
        /// Updates the student's current belt
        /// </summary>
        public (bool IsValid, string Message) UpdateStudentBelt(string studentId, string instructorId, Belt newBelt)
        {
            var validation = ValidateGraduation(studentId, instructorId, newBelt);

            if (!validation.IsValid)
            {
                return validation;
            }

            students[studentId].CurrentBelt = newBelt;
            return validation;
        }

        /// <summary>
        /// Gets the student's current belt
        /// </summary>
        public Belt? GetStudentCurrentBelt(string studentId)
        {
            return students.ContainsKey(studentId) ? students[studentId].CurrentBelt : null;
        }
    }
}