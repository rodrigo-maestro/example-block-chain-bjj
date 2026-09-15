using System;
using BJJChain.Blockchain;
using BJJChain.Enums;
using BJJChain.Models;
using BJJChain.Validators;

namespace BJJChain.Services
{
    public class GraduationService
    {
        private readonly BlockchainManager blockchain;
        private readonly GraduationValidator validator;

        public GraduationService(BlockchainManager blockchain, GraduationValidator validator)
        {
            this.blockchain = blockchain;
            this.validator = validator;
        }

        public (bool Success, string Message, Block Block) PromoteStudent(
            string studentId, string studentName, string academyId,
            string instructorId, Belt newBelt)
        {
            var currentBelt = validator.GetStudentCurrentBelt(studentId) ?? Belt.White;

            var updateResult = validator.UpdateStudentBelt(studentId, instructorId, newBelt);
            if (!updateResult.IsValid)
            {
                return (false, updateResult.Message, null);
            }

            var graduationEvent = new GraduationEvent(
                studentId, studentName, academyId, instructorId,
                currentBelt, newBelt, DateTime.Now);

            var block = new Block(index: 0, graduationEvent, previousHash: string.Empty);
            blockchain.AddBlock(block);

            return (true, updateResult.Message, block);
        }
    }
}