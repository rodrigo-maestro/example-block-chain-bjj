using BJJChain.Enums;
using BJJChain.Models;
using BJJChain.Validators;

namespace BJJChain.Tests
{
    public class GraduationValidatorTests
    {
        private GraduationValidator CreateValidator()
        {
            return new GraduationValidator();
        }

        private Instructor CreateInstructor(string id = "IN001", Belt maxLevel = Belt.Black)
        {
            return new Instructor(id, "Test Professor", maxLevel, "AC001");
        }

        private Student CreateStudent(string id = "ST001", Belt currentBelt = Belt.White)
        {
            var student = new Student(id, "Test Student");
            student.CurrentBelt = currentBelt;
            return student;
        }

        [Fact]
        public void ValidateGraduation_StudentNotFound_ShouldReturnFalse()
        {
            var validator = CreateValidator();
            validator.RegisterInstructor(CreateInstructor());

            var result = validator.ValidateGraduation("NONEXISTENT", "IN001", Belt.Blue);

            Assert.False(result.IsValid);
            Assert.Contains("Student not found", result.Message);
        }

        [Fact]
        public void ValidateGraduation_InstructorNotFound_ShouldReturnFalse()
        {
            var validator = CreateValidator();
            validator.RegisterStudent(CreateStudent());

            var result = validator.ValidateGraduation("ST001", "NONEXISTENT", Belt.Blue);

            Assert.False(result.IsValid);
            Assert.Contains("Instructor not found", result.Message);
        }

        [Fact]
        public void ValidateGraduation_NewBeltNotHigherThanCurrent_ShouldReturnFalse()
        {
            var validator = CreateValidator();
            validator.RegisterStudent(CreateStudent(currentBelt: Belt.Blue));
            validator.RegisterInstructor(CreateInstructor());

            var result = validator.ValidateGraduation("ST001", "IN001", Belt.White);

            Assert.False(result.IsValid);
            Assert.Contains("must be higher", result.Message);
        }

        [Fact]
        public void ValidateGraduation_SameBelt_ShouldReturnFalse()
        {
            var validator = CreateValidator();
            validator.RegisterStudent(CreateStudent(currentBelt: Belt.Blue));
            validator.RegisterInstructor(CreateInstructor());

            var result = validator.ValidateGraduation("ST001", "IN001", Belt.Blue);

            Assert.False(result.IsValid);
        }

        [Fact]
        public void ValidateGraduation_InstructorNotAuthorized_ShouldReturnFalse()
        {
            var validator = CreateValidator();
            validator.RegisterStudent(CreateStudent(currentBelt: Belt.Purple));
            validator.RegisterInstructor(CreateInstructor(maxLevel: Belt.Purple));

            var result = validator.ValidateGraduation("ST001", "IN001", Belt.Brown);

            Assert.False(result.IsValid);
            Assert.Contains("not authorized", result.Message);
        }

        [Fact]
        public void ValidateGraduation_BeltSkipping_ShouldReturnFalse()
        {
            var validator = CreateValidator();
            validator.RegisterStudent(CreateStudent(currentBelt: Belt.White));
            validator.RegisterInstructor(CreateInstructor());

            var result = validator.ValidateGraduation("ST001", "IN001", Belt.Purple);

            Assert.False(result.IsValid);
            Assert.Contains("Belt skipping", result.Message);
        }

        [Fact]
        public void ValidateGraduation_ValidSequence_ShouldReturnTrue()
        {
            var validator = CreateValidator();
            validator.RegisterStudent(CreateStudent(currentBelt: Belt.White));
            validator.RegisterInstructor(CreateInstructor());

            var result = validator.ValidateGraduation("ST001", "IN001", Belt.Blue);

            Assert.True(result.IsValid);
            Assert.Contains("authorized", result.Message);
        }

        [Fact]
        public void UpdateStudentBelt_ValidSequence_ShouldUpdateBeltAndReturnTrue()
        {
            var validator = CreateValidator();
            validator.RegisterStudent(CreateStudent(currentBelt: Belt.White));
            validator.RegisterInstructor(CreateInstructor());

            var result = validator.UpdateStudentBelt("ST001", "IN001", Belt.Blue);

            Assert.True(result.IsValid);
            Assert.Equal(Belt.Blue, validator.GetStudentCurrentBelt("ST001"));
        }

        [Fact]
        public void UpdateStudentBelt_StudentNotFound_ShouldReturnFalse()
        {
            var validator = CreateValidator();
            validator.RegisterInstructor(CreateInstructor());

            var result = validator.UpdateStudentBelt("NONEXISTENT", "IN001", Belt.Blue);

            Assert.False(result.IsValid);
            Assert.Contains("Student not found", result.Message);
        }

        [Fact]
        public void UpdateStudentBelt_InvalidGraduation_ShouldNotUpdateBelt()
        {
            var validator = CreateValidator();
            validator.RegisterStudent(CreateStudent(currentBelt: Belt.White));
            validator.RegisterInstructor(CreateInstructor());

            var result = validator.UpdateStudentBelt("ST001", "IN001", Belt.Purple);

            Assert.False(result.IsValid);
            Assert.Equal(Belt.White, validator.GetStudentCurrentBelt("ST001"));
        }

        [Fact]
        public void UpdateStudentBelt_InstructorNotAuthorized_ShouldNotUpdateBelt()
        {
            var validator = CreateValidator();
            validator.RegisterStudent(CreateStudent(currentBelt: Belt.Purple));
            validator.RegisterInstructor(CreateInstructor(maxLevel: Belt.Purple));

            var result = validator.UpdateStudentBelt("ST001", "IN001", Belt.Brown);

            Assert.False(result.IsValid);
            Assert.Equal(Belt.Purple, validator.GetStudentCurrentBelt("ST001"));
        }

        [Fact]
        public void GetStudentCurrentBelt_NonexistentStudent_ShouldReturnNull()
        {
            var validator = CreateValidator();

            var belt = validator.GetStudentCurrentBelt("NONEXISTENT");

            Assert.Null(belt);
        }
    }
}