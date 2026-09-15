using BJJChain.Blockchain;
using BJJChain.Enums;
using BJJChain.Models;
using BJJChain.Services;
using BJJChain.Validators;

namespace BJJChain.Tests
{
    public class GraduationServiceTests
    {
        private Instructor CreateInstructor(string id = "IN001", Belt maxLevel = Belt.Black)
        {
            return new Instructor(id, "Test Professor", maxLevel, "AC001");
        }

        private Student CreateStudent(string id = "ST001")
        {
            return new Student(id, "Test Student");
        }

        private (BlockchainManager Blockchain, GraduationValidator Validator, GraduationService Service) CreateService()
        {
            var blockchain = new BlockchainManager(difficulty: 1);
            var validator = new GraduationValidator();
            blockchain.CreateGenesisBlock();

            var service = new GraduationService(blockchain, validator);

            return (blockchain, validator, service);
        }

        [Fact]
        public void PromoteStudent_ValidGraduation_ShouldReturnSuccessAndAddBlock()
        {
            var (blockchain, validator, service) = CreateService();
            validator.RegisterStudent(CreateStudent());
            validator.RegisterInstructor(CreateInstructor());

            var result = service.PromoteStudent("ST001", "Test Student", "AC001", "IN001", Belt.Blue);

            Assert.True(result.Success);
            Assert.NotNull(result.Block);
            Assert.Equal(2, blockchain.BlockCount);
            Assert.Equal(Belt.Blue, validator.GetStudentCurrentBelt("ST001"));
        }

        [Fact]
        public void PromoteStudent_InvalidGraduation_ShouldReturnFailureAndNotAddBlock()
        {
            var (blockchain, validator, service) = CreateService();
            validator.RegisterStudent(CreateStudent());
            validator.RegisterInstructor(CreateInstructor());

            var result = service.PromoteStudent("ST001", "Test Student", "AC001", "IN001", Belt.Purple);

            Assert.False(result.Success);
            Assert.Null(result.Block);
            Assert.Equal(1, blockchain.BlockCount);
            Assert.Equal(Belt.White, validator.GetStudentCurrentBelt("ST001"));
        }

        [Fact]
        public void PromoteStudent_StudentNotFound_ShouldReturnFailureAndNotAddBlock()
        {
            var (blockchain, validator, service) = CreateService();
            validator.RegisterInstructor(CreateInstructor());

            var result = service.PromoteStudent("NONEXISTENT", "Nobody", "AC001", "IN001", Belt.Blue);

            Assert.False(result.Success);
            Assert.Contains("Student not found", result.Message);
            Assert.Equal(1, blockchain.BlockCount);
        }

        [Fact]
        public void PromoteStudent_MultipleGraduations_ShouldChainBlocksCorrectly()
        {
            var (blockchain, validator, service) = CreateService();
            validator.RegisterStudent(CreateStudent());
            validator.RegisterInstructor(CreateInstructor());

            var result1 = service.PromoteStudent("ST001", "Test Student", "AC001", "IN001", Belt.Blue);
            var result2 = service.PromoteStudent("ST001", "Test Student", "AC001", "IN001", Belt.Purple);

            Assert.True(result1.Success);
            Assert.True(result2.Success);
            Assert.Equal(3, blockchain.BlockCount);
            Assert.Equal(result1.Block.Hash, result2.Block.PreviousHash);
            Assert.Equal(Belt.Purple, validator.GetStudentCurrentBelt("ST001"));
        }

        [Fact]
        public void PromoteStudent_ValidGraduation_ShouldInvokeOnMiningStartCallback()
        {
            var (blockchain, validator, service) = CreateService();
            validator.RegisterStudent(CreateStudent());
            validator.RegisterInstructor(CreateInstructor());

            var callbackInvoked = false;

            service.PromoteStudent("ST001", "Test Student", "AC001", "IN001", Belt.Blue,
                onMiningStart: () => callbackInvoked = true);

            Assert.True(callbackInvoked);
        }
    }
}