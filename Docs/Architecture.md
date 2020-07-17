This document contains information about the architecture of the blockchain-based self-sovereign identity application for electronic health records(EHR). This application has been designed for a bachelor project. This application has been designed for the purpose of self-sovereign handling of EHR. The application is based on Hyperledger Indy and is therefor not limited to EHR. The application can also be used for any type of credential sharing according to the Hyperledger Indy standard. The application provides a simple command line interface that enable the user to interact with its Indy wallet and with an Indy blockchain.

### Thesis
The [thesis document](https://github.com/SytseOegema/indy-client/blob/master/Docs/Thesis.pdf) provides extensive information about the project. This includes the goals, tradeoffs and design decision.

## Architecture
The application is structured into four folders; Interface, Controllers, Facilitators, and Models. Each folder contains classes for the specific purpose of that folder. The `Program.cs` class merely initiates the command line interface. The purpose and contents of each folder is explained below.

### Interface
- `CliLoop`
- `CliPrompt`

The interface folder only contains functionality to handle command line IO. The `CliLoop` class matches all input commands to the right functionality of the classes in the other folders and uses the `CliPrompt` class to aquire the right input paramaters for the corresponding function. The `CliLoop` uses all Controllers to achieve this functionality. Because some functionality has specific requirement, the `CliLoop` implements check functions that make use of Exceptions.

### Controllers
- `PoolController`
Handles the creation of pool configuration files and the connection to pools via such configuration files.
- `DidController`
Handles all interactions with DIDs in the opened wallet. This controller cannot be used without a wallet controller and is for that reason only used by the WalletController via composition.
- `WalletController`
Handles all interactions with the users wallet and thus identity. This includes the creation of records, credential defintions, and credentials. Also listing and storing of this information is handled by this controller. The interactions with the (emergency) medical record are handled by this controller as well. Finally, all interaction with the `DidController` are handled by this controller via composition.
- `LedgerController`
Handles all requests made to the blockchain. These include the publishing of a new DID(identity), schema, and credential definition.
- `OfflineSecretController`
This controller handles the offline emergency secret. It ensures that the offline emergency secret can only be obtained by providing a valid doctor proof.

### Facilitators
- BigNumberFacilitator
Provides a class that handles big number arithmetic using strings.
- CipherFacilitator
Provides a class that handles encryption and decryption of strings.
- CredDefFacilitator
Provides a static class that handles CredDefs.
- DoctorProofFacilitator
Provides a static class that can create and verify doctors proofs.
- IOFacilitator
Provides a static class for filesystem IO.
- IpfsFacilitator
Provides a class for IPFS interaction.
- PrettyPrintFacilitator
Provides a class for JSON printing.
- SecretSharingFacilitator
Provides a static class that handles Shamir's Shared Secret integration.
- SetupFacilitator
Provides a class that can setup a basic EHR environment. It is encouraged to use this command the first time the application is started.
- ShellFacilitator
Provides a static class to execute bash commands.
- StringFacilitator
Provides a static class to check a string only contains digits.


Most Facilitator functions are used by Controllers. Some are however directly approached by the `CliLoop`.

### Models
- EHRBackupModel
- EmergencyDocotorCredentialModel
- GovernmentSchemasModel
- WalletBackupModel

All models are based on the schemas that are created in the 'SetupFacilitator'. These schemas are stored in files on the files system and are approachable via these models. The models are used by Controllers and Facilitators to prevent code duplication or annoying input fields.

- DoctorProofRequest.json
Contains the proof JSON that is used to verify a doctor credential's validity. For more information on proofs check Indy's [documentation](https://hyperledger-indy.readthedocs.io/projects/sdk/en/latest/docs/how-tos/negotiate-proof/README.html).
