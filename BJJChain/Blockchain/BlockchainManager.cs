namespace BJJChain.Blockchain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using BJJChain.Enums;
    using BJJChain.Models;

    public class BlockchainManager
    {
        private List<Block> Chain { get; set; }
        private int Difficulty { get; set; } // Number of leading zeros in the hash

        public BlockchainManager(int difficulty = 3)
        {
            Chain = new List<Block>();
            Difficulty = difficulty;
        }

        /// <summary>
        /// Creates and adds the genesis block (initial block)
        /// </summary>
        public void CreateGenesisBlock()
        {
            GraduationEvent genesisEvent = new GraduationEvent(
                "GENESIS",
                "Genesis Block",
                "GENESIS",
                "GENESIS",
                Belt.White,
                Belt.White,
                new DateTime(2020, 1, 1)
            );

            Block genesisBlock = new Block(0, genesisEvent, "0");
            MineBlock(genesisBlock);
            Chain.Add(genesisBlock);

            Console.WriteLine("✓ Genesis block created and mined.");
        }

        /// <summary>
        /// Performs simplified proof-of-work
        /// Increments nonce until finding a hash that starts with "difficulty" zeros
        /// </summary>
        public void MineBlock(Block block)
        {
            string target = new string('0', Difficulty);

            while (!block.CalculateHash().StartsWith(target))
            {
                block.Nonce++;
            }

            block.Hash = block.CalculateHash();
        }

        /// <summary>
        /// Adds a new block to the blockchain
        /// </summary>
        public void AddBlock(Block block)
        {
            block.Index = Chain.Count;
            block.PreviousHash = Chain.Last().Hash;
            MineBlock(block);
            Chain.Add(block);
        }

        /// <summary>
        /// Validates the entire chain by checking:
        /// 1. If each hash was calculated correctly
        /// 2. If each block correctly references the previous hash
        /// </summary>
        public bool IsValid()
        {
            for (int i = 1; i < Chain.Count; i++)
            {
                Block currentBlock = Chain[i];
                Block previousBlock = Chain[i - 1];

                // Verifies if the current block hash is correct
                if (currentBlock.Hash != currentBlock.CalculateHash())
                {
                    Console.WriteLine($"X Invalid hash in block {i}");
                    return false;
                }

                // Verifies if the referenced previous hash is correct
                if (currentBlock.PreviousHash != previousBlock.Hash)
                {
                    Console.WriteLine($"X Invalid previous hash reference in block {i}");
                    return false;
                }
            }

            Console.WriteLine("✓ Blockchain is valid!");
            return true;
        }

        /// <summary>
        /// Gets the graduation history of a student
        /// </summary>
        public List<Block> GetStudentHistory(string studentId)
        {
            return Chain
                .Where(block => block.Event.StudentId == studentId && block.Event.StudentId != "GENESIS")
                .ToList();
        }

        /// <summary>
        /// Displays the entire blockchain
        /// </summary>
        public void PrintChain()
        {
            Console.WriteLine("\n╔═════════════════════════════════════════╗");
            Console.WriteLine("║          COMPLETE BLOCKCHAIN HISTORY    ║");
            Console.WriteLine("╚═════════════════════════════════════════╝\n");

            foreach (Block block in Chain)
            {
                block.PrintBlock();
            }
        }

        /// <summary>
        /// Gets the total number of blocks
        /// </summary>
        public int BlockCount => Chain.Count;

        /// <summary>
        /// Gets a specific block by index
        /// </summary>
        public Block GetBlock(int index)
        {
            return index >= 0 && index < Chain.Count ? Chain[index] : null;
        }
    }

}
