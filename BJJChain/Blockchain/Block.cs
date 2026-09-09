namespace BJJChain.Blockchain
{
    using System;
    using System.Security.Cryptography;
    using System.Text;
    using BJJChain.Models;

    public class Block
    {
        public int Index { get; set; }
        public DateTime Timestamp { get; set; }
        public GraduationEvent Event { get; set; }
        public string PreviousHash { get; set; }
        public string Hash { get; set; }
        public long Nonce { get; set; }

        public Block(int index, GraduationEvent graduationEvent, string previousHash)
        {
            Index = index;
            Event = graduationEvent;
            Timestamp = DateTime.Now;
            PreviousHash = previousHash;
            Hash = string.Empty;
            Nonce = 0;
        }

        /// <summary>
        /// Calculates the block hash with the formula: SHA256(index + timestamp + event + previous hash + nonce)
        /// </summary>
        public string CalculateHash()
        {
            string input = $"{Index}{Timestamp:O}{Event.StudentId}{Event.PreviousBelt}{Event.NewBelt}{PreviousHash}{Nonce}";

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
            }
        }

        /// <summary>
        /// Displays summarized block information
        /// </summary>
        public void PrintBlock()
        {
            Console.WriteLine($"\n--- Block {Index} ---");
            Console.WriteLine($"Timestamp: {Timestamp:dd/MM/yyyy HH:mm:ss}");
            Console.WriteLine($"Event: {Event}");
            Console.WriteLine($"Previous Hash: {PreviousHash.Substring(0, Math.Min(8, PreviousHash.Length))}...");
            Console.WriteLine($"Current Hash: {Hash.Substring(0, Math.Min(8, Hash.Length))}...");
            Console.WriteLine($"Nonce: {Nonce}");
        }
    }

}
