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
            var blockchain = CreateBlockchain();

            blockchain.CreateGenesisBlock();

            Assert.NotNull(blockchain.GetBlock(0));
            Assert.Equal(0, blockchain.GetBlock(0).Index);
            Assert.Equal("0", blockchain.GetBlock(0).PreviousHash);
        }

        [Fact]
        public void CreateGenesisBlock_ShouldHaveBlockCount1()
        {
            var blockchain = CreateBlockchain();

            blockchain.CreateGenesisBlock();

            Assert.Equal(1, blockchain.BlockCount);
        }

        [Fact]
        public void AddBlock_MultipleBlocks_ShouldMaintainChain()
        {
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

            Assert.Equal(3, blockchain.BlockCount);
            Assert.Equal(blockchain.GetBlock(0).Hash, block1.PreviousHash);
            Assert.Equal(block1.Hash, block2.PreviousHash);
        }

        [Fact]
        public void IsValid_IntactChain_ShouldReturnTrue()
        {
            var blockchain = CreateBlockchain();
            blockchain.CreateGenesisBlock();

            var graduation = new GraduationEvent("ST001", "Carlos", "AC001", "IN001",
                Belt.White, Belt.Blue, DateTime.Now);
            var block = new Block(1, graduation, blockchain.GetBlock(0).Hash);
            blockchain.AddBlock(block);

            var isValid = blockchain.IsValid();

            Assert.True(isValid);
        }

        [Fact]
        public void IsValid_TamperedBlock_ShouldReturnFalse()
        {
            var blockchain = CreateBlockchain();
            blockchain.CreateGenesisBlock();

            var graduation = new GraduationEvent("ST001", "Carlos", "AC001", "IN001",
                Belt.White, Belt.Blue, DateTime.Now);
            var block = new Block(1, graduation, blockchain.GetBlock(0).Hash);
            blockchain.AddBlock(block);

            var tamperedBlock = blockchain.GetBlock(1);
            tamperedBlock.Event = new GraduationEvent("ST001", "Carlos", "AC001", "IN001",
                Belt.Blue, Belt.Black, DateTime.Now);

            var isValid = blockchain.IsValid();

            Assert.False(isValid);
        }

        [Fact]
        public void GetStudentHistory_ShouldReturnAllStudentBlocks()
        {
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

            var history = blockchain.GetStudentHistory("ST001");

            Assert.Equal(2, history.Count);
            Assert.All(history, block => Assert.Equal("ST001", block.Event.StudentId));
        }

        [Fact]
        public void GetStudentHistory_NonexistentStudent_ShouldReturnEmpty()
        {
            var blockchain = CreateBlockchain();
            blockchain.CreateGenesisBlock();

            var history = blockchain.GetStudentHistory("NONEXISTENT");

            Assert.Empty(history);
        }
    }
}