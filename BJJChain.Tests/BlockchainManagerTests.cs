using BJJChain.Blockchain;
using BJJChain.Enums;
using BJJChain.Models;

namespace BJJChain.Tests
{
    public class BlockchainManagerTests
    {
        private BlockchainManager CreateBlockchain(int difficulty = 3)
        {
            return new BlockchainManager(difficulty: difficulty);
        }

        [Fact]
        public void CreateGenesisBlock_ShouldCreateValidBlock()
        {
            // Arrange
            var blockchain = CreateBlockchain();

            // Act
            blockchain.CreateGenesisBlock();

            // Assert
            Assert.NotNull(blockchain.GetBlock(0));
            Assert.Equal(0, blockchain.GetBlock(0).Index);
            Assert.Equal("0", blockchain.GetBlock(0).PreviousHash);
        }

        [Fact]
        public void CreateGenesisBlock_ShouldHaveBlockCount1()
        {
            // Arrange
            var blockchain = CreateBlockchain();

            // Act
            blockchain.CreateGenesisBlock();

            // Assert
            Assert.Equal(1, blockchain.BlockCount);
        }

        [Fact]
        public void AddBlock_ShouldChainCorrectly()
        {
            // Arrange
            var blockchain = CreateBlockchain();
            blockchain.CreateGenesisBlock();

            var graduation = new GraduationEvent(
                studentId: "ST001",
                studentName: "Carlos Silva",
                academyId: "AC001",
                instructorId: "IN001",
                previousBelt: Belt.White,
                newBelt: Belt.Blue,
                date: DateTime.Now
            );

            var block = new Block(1, graduation, blockchain.GetBlock(0).Hash);

            // Act
            blockchain.AddBlock(block);

            // Assert
            Assert.Equal(2, blockchain.BlockCount);
            Assert.Equal(blockchain.GetBlock(0).Hash, block.PreviousHash);
        }

        [Fact]
        public void AddBlock_MultipleBlocks_ShouldMaintainChain()
        {
            // Arrange
            var blockchain = CreateBlockchain();
            blockchain.CreateGenesisBlock();

            var graduation1 = new GraduationEvent("ST001", "Carlos", "AC001", "IN001",
                Belt.White, Belt.Blue, DateTime.Now);
            var graduation2 = new GraduationEvent("ST001", "Carlos", "AC001", "IN001",
                Belt.Blue, Belt.Purple, DateTime.Now.AddMonths(6));

            var block1 = new Block(1, graduation1, blockchain.GetBlock(0).Hash);

            // Act
            blockchain.AddBlock(block1);
            blockchain.AddBlock(
                new Block(2, graduation2, blockchain.GetBlock(1).Hash));

            // Assert
            Assert.Equal(3, blockchain.BlockCount);
            Assert.Equal(block1.Hash, blockchain.GetBlock(1).Hash);
            Assert.Equal(block1.Hash, blockchain.GetBlock(2).PreviousHash);
        }

        [Fact]
        public void IsValid_IntactChain_ShouldReturnTrue()
        {
            // Arrange
            var blockchain = CreateBlockchain();
            blockchain.CreateGenesisBlock();

            var graduation = new GraduationEvent("ST001", "Carlos", "AC001", "IN001",
                Belt.White, Belt.Blue, DateTime.Now);
            var block = new Block(1, graduation, blockchain.GetBlock(0).Hash);
            blockchain.AddBlock(block);

            // Act
            var isValid = blockchain.IsValid();

            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public void IsValid_TamperedBlock_ShouldReturnFalse()
        {
            // Arrange
            var blockchain = CreateBlockchain();
            blockchain.CreateGenesisBlock();

            var graduation = new GraduationEvent("ST001", "Carlos", "AC001", "IN001",
                Belt.White, Belt.Blue, DateTime.Now);
            var block = new Block(1, graduation, blockchain.GetBlock(0).Hash);
            blockchain.AddBlock(block);

            // Act
            var tamperedBlock = blockchain.GetBlock(1);
            tamperedBlock.Event = new GraduationEvent("ST001", "Carlos", "AC001", "IN001",
                Belt.Blue, Belt.Black, DateTime.Now);

            var isValid = blockchain.IsValid();

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public void GetStudentHistory_ShouldReturnAllStudentBlocks()
        {
            // Arrange
            var blockchain = CreateBlockchain();
            blockchain.CreateGenesisBlock();

            var graduation1 = new GraduationEvent("ST001", "Carlos", "AC001", "IN001",
                Belt.White, Belt.Blue, DateTime.Now);
            var graduation2 = new GraduationEvent("ST001", "Carlos", "AC001", "IN001",
                Belt.Blue, Belt.Purple, DateTime.Now.AddMonths(6));

            var block1 = new Block(1, graduation1, blockchain.GetBlock(0).Hash);
            blockchain.AddBlock(block1);

            var block2 = new Block(2, graduation2, blockchain.GetBlock(1).Hash);
            blockchain.AddBlock(block2);

            // Act
            var history = blockchain.GetStudentHistory("ST001");

            // Assert
            Assert.Equal(2, history.Count);
            Assert.All(history, block => Assert.Equal("ST001", block.Event.StudentId));
        }

        [Fact]
        public void GetStudentHistory_NonexistentStudent_ShouldReturnEmpty()
        {
            // Arrange
            var blockchain = CreateBlockchain();
            blockchain.CreateGenesisBlock();

            // Act
            var history = blockchain.GetStudentHistory("NONEXISTENT");

            // Assert
            Assert.Empty(history);
        }
    }
}
