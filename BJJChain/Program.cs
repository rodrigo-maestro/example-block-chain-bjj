using System;
using System.Collections.Generic;
using BJJChain.Blockchain;
using BJJChain.Enums;
using BJJChain.Models;
using BJJChain.Validators;

class Program
{
    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        // EDUCATIONAL INTRODUCTION
        DisplayWelcome();

        // 1. INITIALIZE BLOCKCHAIN AND COMPONENTS
        Console.WriteLine("\n[STEP 1] Initializing blockchain with genesis block...\n");

        ExplainGenesisBlock();

        BlockchainManager blockchain = new BlockchainManager(difficulty: 3);
        GraduationValidator validator = new GraduationValidator();

        blockchain.CreateGenesisBlock();

        // 2. REGISTER ACADEMIES
        Console.WriteLine("\n[STEP 2] Registering academies...\n");

        Academy academy1 = new Academy("AC001", "Gracie Jiu-Jitsu Academy", "São Paulo");
        Academy academy2 = new Academy("AC002", "Alliance BJJ Club", "Rio de Janeiro");

        Console.WriteLine($"[OK] Academy registered: {academy1}");
        Console.WriteLine($"[OK] Academy registered: {academy2}");

        // 3. REGISTER INSTRUCTORS
        Console.WriteLine("\n[STEP 3] Registering instructors...\n");

        Instructor instructor1 = new Instructor("IN001", "Professor João", Belt.Black, "AC001");
        Instructor instructor2 = new Instructor("IN002", "Professor Maria", Belt.Brown, "AC002");

        validator.RegisterInstructor(instructor1);
        validator.RegisterInstructor(instructor2);

        Console.WriteLine($"[OK] Instructor registered: {instructor1}");
        Console.WriteLine($"[OK] Instructor registered: {instructor2}");

        // 4. REGISTER STUDENTS
        Console.WriteLine("\n[STEP 4] Registering students...\n");

        Student student1 = new Student("ST001", "Carlos Silva");
        Student student2 = new Student("ST002", "Ana Santos");

        validator.RegisterStudent(student1);
        validator.RegisterStudent(student2);

        Console.WriteLine($"[OK] Student registered: {student1}");
        Console.WriteLine($"[OK] Student registered: {student2}");

        // 5. FIRST GRADUATION: White → Blue
        Console.WriteLine("\n[STEP 5] Processing first graduation: White → Blue\n");

        ExplainValidation();

        var validation1 = validator.ValidateGraduation("ST001", "IN001", Belt.Blue);
        Console.WriteLine(validation1.Message);

        if (validation1.IsValid)
        {
            GraduationEvent graduation1 = new GraduationEvent(
                studentId: "ST001",
                studentName: "Carlos Silva",
                academyId: "AC001",
                instructorId: "IN001",
                previousBelt: Belt.White,
                newBelt: Belt.Blue,
                date: DateTime.Now
            );

            Block block1 = new Block(1, graduation1, blockchain.GetBlock(0).Hash);

            Console.WriteLine("\n--- Mining Process Starting ---\n");
            ExplainMining();

            blockchain.AddBlock(block1);
            validator.UpdateStudentBelt("ST001", "IN001", Belt.Blue);

            Console.WriteLine($"[OK] Block mined successfully!");
            block1.PrintBlock();
        }

        // 6. SECOND GRADUATION: Blue → Purple
        Console.WriteLine("\n[STEP 6] Processing second graduation: Blue → Purple\n");

        var validation2 = validator.ValidateGraduation("ST001", "IN001", Belt.Purple);
        Console.WriteLine(validation2.Message);

        if (validation2.IsValid)
        {
            GraduationEvent graduation2 = new GraduationEvent(
                studentId: "ST001",
                studentName: "Carlos Silva",
                academyId: "AC001",
                instructorId: "IN001",
                previousBelt: Belt.Blue,
                newBelt: Belt.Purple,
                date: DateTime.Now.AddMonths(6)
            );

            Block block2 = new Block(2, graduation2, blockchain.GetBlock(1).Hash);

            Console.WriteLine("\nMining block...");
            blockchain.AddBlock(block2);
            validator.UpdateStudentBelt("ST001", "IN001", Belt.Purple);

            Console.WriteLine($"[OK] Block mined successfully!");
            block2.PrintBlock();
        }

        // 7. THIRD GRADUATION: Student changes academy
        Console.WriteLine("\n[STEP 7] Student changes academy and continues training\n");

        ExplainBlockChaining();

        var validation3 = validator.ValidateGraduation("ST001", "IN002", Belt.Brown);
        Console.WriteLine(validation3.Message);

        if (validation3.IsValid)
        {
            GraduationEvent graduation3 = new GraduationEvent(
                studentId: "ST001",
                studentName: "Carlos Silva",
                academyId: "AC002",
                instructorId: "IN002",
                previousBelt: Belt.Purple,
                newBelt: Belt.Brown,
                date: DateTime.Now.AddMonths(18)
            );

            Block block3 = new Block(3, graduation3, blockchain.GetBlock(2).Hash);

            Console.WriteLine("\nMining block...");
            blockchain.AddBlock(block3);
            validator.UpdateStudentBelt("ST001", "IN002", Belt.Brown);

            Console.WriteLine($"[OK] Block mined successfully!");
            block3.PrintBlock();
        }

        // 8. DISPLAY COMPLETE BLOCKCHAIN
        blockchain.PrintChain();

        // 9. DISPLAY STUDENT HISTORY
        Console.WriteLine("\n╔══════════════════════════════════════════╗");
        Console.WriteLine("║        CARLOS SILVA - GRADUATION HISTORY ║");
        Console.WriteLine("╚══════════════════════════════════════════╝\n");

        List<Block> studentHistory = blockchain.GetStudentHistory("ST001");
        int graduationCount = 1;

        foreach (Block block in studentHistory)
        {
            Console.WriteLine($"Graduation #{graduationCount}:");
            Console.WriteLine($"  Event: {block.Event}");
            Console.WriteLine($"  Academy: {(block.Event.AcademyId == "AC001" ? "Gracie Jiu-Jitsu Academy" : "Alliance BJJ Club")}");
            Console.WriteLine($"  Block Hash: {block.Hash.Substring(0, 16)}...");
            Console.WriteLine();
            graduationCount++;
        }

        // 10. VALIDATE BLOCKCHAIN (Expected: VALID)
        Console.WriteLine("\n[STEP 8] Validating blockchain integrity...\n");

        ExplainValidation2();

        blockchain.IsValid();

        // 11. TAMPER WITH A BLOCK (Intentional modification)
        Console.WriteLine("\n[STEP 9] Attempting to tamper with block...\n");

        ExplainTampering();

        Console.WriteLine("!  Modifying graduation event data...\n");

        Block tamperedBlock = blockchain.GetBlock(2);
        tamperedBlock.Event = new GraduationEvent(
            studentId: "ST001",
            studentName: "Carlos Silva",
            academyId: "AC001",
            instructorId: "IN001",
            previousBelt: Belt.Blue,
            newBelt: Belt.Black,
            date: DateTime.Now
        );

        Console.WriteLine($"Modified Block #2:");
        Console.WriteLine($"  New event: {tamperedBlock.Event}");
        Console.WriteLine($"  Old hash: {tamperedBlock.Hash.Substring(0, 16)}...");
        Console.WriteLine($"  New hash: {tamperedBlock.CalculateHash().Substring(0, 16)}...");

        // 12. VALIDATE BLOCKCHAIN AGAIN (Expected: INVALID)
        Console.WriteLine("\n[STEP 10] Validating blockchain again after tampering...\n");
        blockchain.IsValid();

        // 13. FINAL SUMMARY
        Console.WriteLine("\n╔══════════════════════════════════════════╗");
        Console.WriteLine("║              FINAL SUMMARY               ║");
        Console.WriteLine("╚══════════════════════════════════════════╝\n");

        Console.WriteLine($"Total blocks in blockchain: {blockchain.BlockCount}");
        Console.WriteLine($"Current student belt: {validator.GetStudentCurrentBelt("ST001")}");
        Console.WriteLine($"\n[OK] Demo completed successfully!");
        Console.WriteLine("\nBlockchain concepts demonstrated:");
        Console.WriteLine("  • Genesis block creation");
        Console.WriteLine("  • Chained blocks with SHA-256 hashing");
        Console.WriteLine("  • Proof of Work (nonce mining)");
        Console.WriteLine("  • Chain validation");
        Console.WriteLine("  • Tamper detection and immutability");
        Console.WriteLine("  • Student history preservation across academies");

        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }

    // EDUCATIONAL EXPLANATIONS
    static void DisplayWelcome()
    {
        Console.WriteLine("\n╔══════════════════════════════════════════╗");
        Console.WriteLine("║         BJJ CHAIN - BLOCKCHAIN           ║");
        Console.WriteLine("║    Brazilian Jiu-Jitsu Graduation Demo   ║");
        Console.WriteLine("╚══════════════════════════════════════════╝\n");

        Console.WriteLine("Welcome to BJJ Chain - An educational blockchain demonstration");
        Console.WriteLine("using Brazilian Jiu-Jitsu belt graduations as transactions.\n");

        Console.WriteLine("This application will demonstrate core blockchain concepts:");
        Console.WriteLine("  - How blocks are created and linked");
        Console.WriteLine("  - What mining means and why it's needed");
        Console.WriteLine("  - How hashing ensures data integrity");
        Console.WriteLine("  - How the chain detects tampering\n");

        PauseForUser();
    }

    static void ExplainGenesisBlock()
    {
        Console.WriteLine("--- CONCEPT: GENESIS BLOCK ---\n");
        Console.WriteLine("The GENESIS BLOCK is the first block in any blockchain.");
        Console.WriteLine("It has no previous block to reference (PreviousHash = '0').");
        Console.WriteLine("All other blocks will be linked to it, forming a chain.\n");

        Console.WriteLine("In BJJ Chain:");
        Console.WriteLine("  - The genesis block represents the start of the certification system");
        Console.WriteLine("  - It contains a dummy graduation event (not a real student)");
        Console.WriteLine("  - Every real graduation block will eventually trace back to this");
        Console.WriteLine("  - If the genesis block is tampered with, the entire chain breaks\n");

        PauseForUser();
    }

    static void ExplainValidation()
    {
        Console.WriteLine("--- CONCEPT: VALIDATION ---\n");
        Console.WriteLine("Before a graduation can be recorded, it must pass validation rules:");
        Console.WriteLine("  1. The student must exist in the system");
        Console.WriteLine("  2. The instructor must exist and be authorized");
        Console.WriteLine("  3. The new belt must be higher than the current belt");
        Console.WriteLine("  4. The instructor's authorization level must match or exceed the new belt");
        Console.WriteLine("  5. No belt skipping is allowed (White -> Blue, not White -> Purple)\n");

        Console.WriteLine("This prevents fraud BEFORE it enters the blockchain.");
        Console.WriteLine("Once data is on the blockchain, it cannot be deleted,");
        Console.WriteLine("so preventing bad data from entering is critical.\n");

        PauseForUser();
    }

    static void ExplainMining()
    {
        Console.WriteLine("--- CONCEPT: MINING & PROOF OF WORK ---\n");

        Console.WriteLine("MINING is the process of finding a valid HASH for a block.");
        Console.WriteLine("This is the core security mechanism of blockchain.\n");

        Console.WriteLine("How mining works in BJJ Chain:\n");

        Console.WriteLine("Step 1: Take the block data");
        Console.WriteLine("  - Block index, timestamp, graduation event details, etc.\n");

        Console.WriteLine("Step 2: Create a SHA-256 hash");
        Console.WriteLine("  - SHA-256 converts any input into a fixed 64-character string");
        Console.WriteLine("  - Example: 'Carlos White->Blue' becomes:");
        Console.WriteLine("    a7f3e9d2c1b8f4a6e9c2d1b5f8a3e6c9...\n");

        Console.WriteLine("Step 3: Check if the hash matches the DIFFICULTY requirement");
        Console.WriteLine("  - Difficulty = 3 means the hash must start with '000'");
        Console.WriteLine("  - If it doesn't, we modify the NONCE and try again");
        Console.WriteLine("  - Nonce: 'Number used ONCE' - a counter we increment\n");

        Console.WriteLine("Step 4: Repeat until we find a valid hash");
        Console.WriteLine("  - This requires many attempts (computational work)");
        Console.WriteLine("  - The more difficult the requirement, the more work needed");
        Console.WriteLine("  - This makes tampering expensive (you'd need to re-mine)\n");

        Console.WriteLine("Why is this important?");
        Console.WriteLine("  - Mining proves work was done to secure the block");
        Console.WriteLine("  - Makes tampering costly (would need to re-mine entire chain)");
        Console.WriteLine("  - Creates a timestamp of when the block was secured");
        Console.WriteLine("  - In real blockchain, miners get rewarded for this work\n");

        PauseForUser();
    }

    static void ExplainBlockChaining()
    {
        Console.WriteLine("--- CONCEPT: BLOCK CHAINING ---\n");

        Console.WriteLine("Each block contains a reference to the PREVIOUS block's hash.");
        Console.WriteLine("This creates an unbreakable chain:\n");

        Console.WriteLine("Block 1 (Genesis):");
        Console.WriteLine("  Hash: a7f3e9d2c1b8f4a6e9c2d1b5f8a3e6c9");
        Console.WriteLine("  PreviousHash: 0\n");

        Console.WriteLine("Block 2 (First Graduation):");
        Console.WriteLine("  Hash: 000f4a6e9c2d1b5f8a3e6c9a7f3e9d2");
        Console.WriteLine("  PreviousHash: a7f3e9d2c1b8f4a6e9c2d1b5f8a3e6c9 <- References Block 1\n");

        Console.WriteLine("Block 3 (Second Graduation):");
        Console.WriteLine("  Hash: 000e6c9a7f3e9d2c1b5f8a3e6c9a7f3");
        Console.WriteLine("  PreviousHash: 000f4a6e9c2d1b5f8a3e6c9a7f3e9d2 <- References Block 2\n");

        Console.WriteLine("Why this matters:");
        Console.WriteLine("  - If someone tampers with Block 1, its hash changes");
        Console.WriteLine("  - Block 2's PreviousHash no longer matches");
        Console.WriteLine("  - Block 3's entire calculation becomes invalid");
        Console.WriteLine("  - The tampering breaks the ENTIRE chain\n");

        Console.WriteLine("In BJJ Chain:");
        Console.WriteLine("  - Carlos changes academies but keeps his history intact");
        Console.WriteLine("  - All his graduation blocks reference each other");
        Console.WriteLine("  - This proves the sequence and integrity of his progression");
        Console.WriteLine("  - No one can fake a belt promotion without detection\n");

        PauseForUser();
    }

    static void ExplainValidation2()
    {
        Console.WriteLine("--- CONCEPT: CHAIN VALIDATION ---\n");

        Console.WriteLine("Validation checks if the blockchain is still intact:");
        Console.WriteLine("  1. For EACH block:");
        Console.WriteLine("     - Recalculate what the hash SHOULD be");
        Console.WriteLine("     - Compare it to the stored hash");
        Console.WriteLine("     - If they don't match, the block was tampered with\n");

        Console.WriteLine("  2. For EACH block:");
        Console.WriteLine("     - Check if PreviousHash matches the actual previous block's hash");
        Console.WriteLine("     - If not, the chain is broken\n");

        Console.WriteLine("If all checks pass, the blockchain is VALID and trusted.");
        Console.WriteLine("If any check fails, tampering is detected immediately.\n");

        PauseForUser();
    }

    static void ExplainTampering()
    {
        Console.WriteLine("--- CONCEPT: TAMPER DETECTION ---\n");

        Console.WriteLine("Now we'll intentionally modify a block to show why blockchains are immutable.\n");

        Console.WriteLine("Scenario: A fraudster wants to change Carlos's Blue belt to Black.");
        Console.WriteLine("(This would skip the Purple and Brown belts - not allowed!)\n");

        Console.WriteLine("What happens:\n");

        Console.WriteLine("1. The fraudster changes the block data:");
        Console.WriteLine("   Before: previousBelt=Blue, newBelt=Purple");
        Console.WriteLine("   After:  previousBelt=Blue, newBelt=Black\n");

        Console.WriteLine("2. The block's hash BECOMES INVALID:");
        Console.WriteLine("   Old hash (before tampering):  000f4a6e9c2d1b5f8a3e6c9a7f3e9d2");
        Console.WriteLine("   New hash (after tampering):   a7f3e9d2c1b8f4a6e9c2d1b5f8a3e6c9");
        Console.WriteLine("   These don't match! (The new one doesn't even start with 000)\n");

        Console.WriteLine("3. The next block's PreviousHash no longer matches:");
        Console.WriteLine("   Block 3 expects: 000f4a6e9c2d1b5f8a3e6c9a7f3e9d2");
        Console.WriteLine("   But Block 2 now has: a7f3e9d2c1b8f4a6e9c2d1b5f8a3e6c9");
        Console.WriteLine("   MISMATCH! The entire chain breaks.\n");

        Console.WriteLine("4. When we validate, we immediately detect the problem:");
        Console.WriteLine("   'BLOCKCHAIN IS INVALID - Tampering detected!'\n");

        Console.WriteLine("To successfully tamper with the chain, a fraudster would need to:");
        Console.WriteLine("  1. Re-mine Block 2 (expensive - takes many calculations)");
        Console.WriteLine("  2. Re-mine Block 3 (another expensive effort)");
        Console.WriteLine("  3. Re-mine every subsequent block (nearly impossible)");
        Console.WriteLine("  4. Do all this before anyone validates the chain\n");

        Console.WriteLine("This is why blockchain is considered immutable - not because");
        Console.WriteLine("it's technically impossible to change, but because changing it");
        Console.WriteLine("is prohibitively expensive and immediately detectable.\n");

        PauseForUser();
    }

    static void PauseForUser()
    {
        Console.WriteLine("(Press ENTER to continue...)\n");
        Console.ReadLine();
    }
}