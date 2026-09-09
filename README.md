# BJJ Chain

A simple educational console application built in C# that demonstrates core **blockchain concepts** using **Brazilian Jiu-Jitsu (BJJ) belt graduations** as transactions.

## Features

* **Genesis Block Creation:** Initializes the blockchain with a root block.
* **SHA-256 Hashing:** Secures block data through unique cryptographic fingerprints.
* **Proof of Work (Mining):** Implements a difficulty target (nonce mining) required to seal blocks.
* **Block Chaining:** Connects sequential blocks together, ensuring immutability.
* **Graduation Validation Rules:** Enforces proper BJJ belt progression (e.g., preventing belt skipping, checking authorized instructors, tracking history across different academies).
* **Tamper Detection:** Demonstrates how modifying historical records breaks the chain and invalidates its integrity.

## Technologies Used

* **C# / .NET**
* **Object-Oriented Programming (OOP)**
* **SHA-256 Cryptography (`System.Security.Cryptography`)**

## How to Run

1. Make sure you have the [.NET SDK](https://dotnet.microsoft.com/) installed.
2. Clone the repository:
```bash
git clone https://github.com/rodrigo-maestro/example-block-chain-bjj.git

```


3. Navigate to the project folder and run the application:
```bash
dotnet run

```
