This document contains information about the architecture of the blockchain-based self-sovereign identity application for electronic health records(EHR). This application has been designed for a bachelor project. This application has been designed for the purpose of self-sovereign handling of EHR. The application is based on Hyperledger Indy and is therefor not limited to EHR. The application can also be used for any type of credential sharing according to the Hyperledger Indy standard. The application provides a simple command line interface that enable the user to interact with its Indy wallet and with an Indy blockchain.

### Thesis
The thesis document provides extensive information about the project. This includes the goals, tradeoffs and design decision.

## Architecture
The application is structured into four folders; Interface, Controllers, Facilitators, and Models. Each folder contains classes for the specific purpose of that folder. The `Program.cs` class merely initiates the command line interface. The purpose and contents of each folder is explained below.

### Interface
- `CliLoop`
- `CliPrompt`

The interface folder only contains functionality to handle command line IO. The `CliLoop` class matches all input commands to the right functionality of the classes in the other folders and uses the `CliPrompt` class to aquire the right input paramaters for the corresponding function.

### Controllers
- `PoolController`
Handles the creation of pool configuration files and the connection to pools via such configuration files.
- `DidController`
- `WalletController`
- `LedgerController`
- `AnoncredsController`
- `OfflineSecretController`


### Facilitators

### Models
