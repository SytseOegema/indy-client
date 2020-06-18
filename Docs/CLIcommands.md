# CLI commands
This document contains a list with all commands provided via the command line interface of the application. For each command the necessary input fields are explained as well. Some methods require a connection to a hyperledger indy pool, an open wallet or an active DID in order to perform the command. For each command requirements are specified as well.

### `exit`
This command can be used to exit the client.

---
### `help`
This command can be used to view a list of available commands.

---
### `EHR environment setup`
**Requires**
- an active pool connection

This command can be used to initialize the client with wallets, schemas, credentials and emergency access secrets.

---
### `pool connect`
This command can be used to connect to a pool. The command connects via a genesis file in the `.indy_client` folder.

| inputs | example |
| ------ | ------- |
| pool name | sandbox |

---
### `wallet create`
This command can be used to create a new Wallet.

| inputs | example |
| ------ | ------- |
| wallet identifier | Anne |
| wallet key | Anne |

*wallet key may be empty. In that case the wallet key will be the same as the wallet identifier*

---
### `wallet open`
This command can be used to open an existing wallet

| inputs | example |
| ------ | ------- |
| wallet identifier | Anne |
| wallet key | Anne |

*wallet key may be empty. In that case the wallet key will be the same as the wallet identifier*

---
### `wallet close`
This command can be used to close the currently opened wallet. If there is no wallet currently opened nothing happens.

---
### `wallet list`
This command can be used to list the wallets that exist on your system

---
### `wallet record add`
**Requires**
- an opened wallet

This command can be used to create a record in the open wallet.

| inputs | example |
| ------ | ------- |
| record type | shared-secret |
| record id | 01-13561084561275182 |
| record value | anything |
| record tags(JSON) | {"json": "data", "more-json": "data"} |

---
### `wallet record get`
**Requires**
- an opened wallet

This command can be used to get records from the open wallet. These records can be queried in MongoDB style, see [here](https://hyperledger-indy.readthedocs.io/projects/sdk/en/latest/docs/design/011-wallet-query-language/README.html) for more information. Also wallet query option must be provided(default values are provided with via the CLI).

| inputs | example |
| ------ | ------- |
| record type | shared-secret |
| wallet query(JSON) | {"param": "value"} |
| query options(JSON) | {"retrieveTotalCount": true, "retrieveType": true, "retrieveTags": true} |

---
### `wallet record delete`
**Requires**
- an opened wallet

This command can be used to delete records from the opened wallet.

| inputs | example |
| ------ | ------- |
| record type | shared-secret |
| record id | 01-13561084561275182 |

---
### `wallet record update tag`
**Requires**
- an opened wallet

This command can be used to update the tag JSON of an existing record in the open wallet.

| inputs | example |
| ------ | ------- |
| record type | shared-secret |
| record id | 01-13561084561275182 |
| record tags(JSON) | {"json": "different-data", "more-json": "different-data"} |

---
### `wallet export local`
**Requires**
- an opened wallet

This command can be used to export the open wallet to a file on your system.

| inputs | example |
| ------ | ------- |
| export path | /home/export_wallet_file |
| export key | export-key |

*make sure the export path points to a file that does not exist yet.*

---
### `wallet export ipfs`
**Requires**
- an opened wallet

This command can be used to export the open wallet to a file on IPFS.

| inputs | example |
| ------ | ------- |
| export key | export-key |
| wallet key | Anne |

---
### `wallet import local`
**Requires**
- an opened wallet

This command can be used to import a wallet export file into the client. The wallet name specified on import can be used afterwards to open the wallet. The wallet key has to be the same as the wallet key that was originally set.

| inputs | example |
| ------ | ------- |
| wallet name | Anne_import |
| import path | /home/export_wallet_file |
| wallet key | Anne |
| export key | export-key |

---
### `wallet import ipfs`
**Requires**
- an opened wallet

This command can be used to import a wallet file from IPFS. Both the IPFS export JSON as the path towards the file containing the IPFS export JSON can be used to import a wallet from IPFS. The JSON input has been designed for emergency doctors in combination with the access methods `offline emergency secret obtain` and `emergency secret reconstruct`.

| inputs | example |
| ------ | ------- |
| wallet name | Anne_IPFS_import |
| wallet IPFS export JSON | {"ipfs_path": "QMasldhaslgsahdjgasldjgsahdgowej134lk", "wallet_key": "Anne", "export_key": "export-key"} |
| path to file with IPFS export JSON | /home/{user}/.indy_client/wallet_export/Anne_ipfs_export.json |

*Either provide the JSON or the path to the file containing the JSON.*

---
### `did create`
**Requires**
- an opened wallet

This command can be used to create a new DID in the opened wallet. **Be aware that this DID is not published.** To publish the DID use command `ledger send initial nym`. The DID can be created with a seed. This seed must always contain 32 characters! The command will always create the same DID for a specific seed.

| inputs | example |
| ------ | ------- |
| DID seed | 000000000000000000000000Trustee1 |

---
### `did activate`
**Requires**
- an opened wallet

This command can be used to activate one of the DIDs in the open wallet. This DID will be used to sign transactions until another DID is activated.

| inputs | example |
| ------ | ------- |
| DID | 7zExvrP1Qc5UQ6CZZUrG1e |

---
### `did list`
**Requires**
- an opened wallet

This command can be used to list all DIDs in the open wallet.

---
### `ledger send initial nym`
**Requires**
- an active pool connection
- an opened wallet
- an active DID
- requires at least an Endorser role

This command can be used to publish a new DID to the ledger. This command has to be executed with an active DID that is already on the ledger with at least an Endorser role. Typical used after creating a new wallet with a new DID (and verkey). Open the wallet of an Endorser, Steward or Trustee and publish the just create DID and verkey to the ledger.

| inputs | example |
| ------ | ------- |
| DID to publish | 7zExvrP1Qc5UQ6CZZUrG1e |
| Verkey to publish | 7zExvrP1Qc5UQ6CZZUrG1e7zExvrP1Qc5UQ6CZZUrG1e |
| alias | doctor |
| role | ENDORSER |

*Role is empty when creating a regular user.*

---
### `schema create`
**Requires**
- an active pool connection
- an opened wallet
- an active DID
- requires at least an Endorser role

This command can be used to create and publish a new schema. This schema can then be used to create credential definitions. The result of this command is the JSON representation of the schema.

| inputs | example |
| ------ | ------- |
| schema name | new_schema |
| schema version | 1.0.0 |
| schema attributes | ["attribute1", "attribute2", "attribute3"] |

---
### `schema list`
**Requires**
- an opened wallet

This command can be used to list all schema's in the open wallet. It contains the schema id and JSON representation.

---
### `schema get`
**Requires**
- an active pool connection
- an opened wallet
- an active DID

This command can be used to obtain an existing schema from the ledger.

| inputs | example |
| ------ | ------- |
| publisher DID | 7zExvrP1Qc5UQ6CZZUrG1e |
| schema name/identifier | new_schema |

---
### `master secret create`
**Requires**
- an opened wallet

This command can be used to create a master secret that is then stored in the wallet. A master secret is used for creating credential requests.

| inputs | example |
| ------ | ------- |
| identifier | masterkey-identifier |

---
### `credential definitions patient create`
**Requires**
- an active pool connection
- an opened wallet
- an active DID

This command can be used to create the basic credential definition for a patient. No input is required as this command will automatically use the schema definitions of the Gov-Health-Department that have been create by running `EHR environment setup`. The following credential definitions are created **EHR**(Electronic Health Record), **WBSS**(Wallet Backup Shared Secret), **ESS**(Emergency Shared Secret) and **ETP**(Emergency Trusted Parties).

---
### `credential definition create`
**Requires**
- an active pool connection
- an opened wallet
- an active DID

This command can be used to create a credential definition. The required input schema JSON can be obtained either directly from the command `schema create`, `schema get` or it can be found in the 'schema list' output. The tag input can be used to separate multiple credential definitions based on the same schema.

| inputs | example |
| ------ | ------- |
| schema JSON | ouput of for example `schema create` |
| credential definition tag | TAG1 |

---
### `credential definition list`
**Requires**
- an opened wallet
- an active DID

This command can be used to list all credential definitions in the open wallet.

---
### `credential definition get`
**Requires**
- an opened wallet

This command can be used to get an existing credential definition from the open wallet. The input requires a credential definition tag. This tag is matched against the existing credential definitions.

| inputs | example |
| ------ | ------- |
| credential definition tag | TAG1 |

---
### `credential offer create`
**Requires**
- an opened wallet

This command can be used by the issuer of a credential. The issuer creates an credential offer. This offer is send to someone that would like to obtain a certain credential.

| inputs | example |
| ------ | ------- |
| credential definition id |  |

---
### `credential request create`
**Requires**
- an opened wallet

This command can be used by the prover of a credential. The prover creates an credential request based on the credential offer it has received from the credential issuer. Also the credential definition is required, which can be found in the wallet of the issuer via the command `credential definition list`. The ouput of `credential definition list` contains these definitions. The last input is the master secret id, which is the id that is used to create a master secret with the command `master secret create`.

| inputs | example |
| ------ | ------- |
| credential offer JSON | JSON output of `credential offer create` |
| credential definition JSON | JSON output of `credential definition create` |
| master secret id | identifier used in `master secret create` |

*The command `master secret create` has to be used before a credential request can be created*

---
### `credential create`
**Requires**
- an opened wallet

This command can be used by the issuer to create a credential for someone. This someone has already provided the issuer with an credential request based on the credential offer that has earlier been send by the issuer. Both the credential offer JSON and credential request JSON are necessary to create a credential and have been provided by `credential offer create` and `credential request create`. The schema attributes have to correspond with the actual schema that is used in the credential definition. The credential values are in the end issued to the prover in the credential.

| inputs | example |
| ------ | ------- |
| credential offer JSON | JSON output of `credential offer create` |
| credential request JSON | part of the JSON output of `credential request create` |
| schema attributes | ["attribute1", "attribute2", "attribute3"] |
| credential values | ["value1", "value2", "value3"] |

---
### `credential store`
**Requires**
- an opened wallet

This command can be used by the prover of an credential to store the credential provided by the issuer in his own wallet. Part of the output of the command `credential request create` has to be used. As well as the actual credential in JSON and the initial credential definition in JSON.

| inputs | example |
| ------ | ------- |
| credential request meta JSON | meta part of JSON output of `credential request create` |
| credential JSON | output of `credential create` |
| credential definition JSON | output of `credential definition create` |

---
### `issuer shared secret list`
**Requires**
- an opened wallet

This command can be used by an issuer(patient) to list all the Shamir secrets he has create.

---
### `issuer shared secret list unused`
**Requires**
- an opened wallet

This command can be used by an issuer(patient) to list all the Shamir secrets he has not shared with trusted parties yet.

---
### `wallet backup shared secret create`
**Requires**
- an opened wallet
- an exported wallet to IPFS

This command can be used by an issuer(patient) to create Shamir shared secrets that together contain the JSON export config of the wallet stored on IPFS. This export config can be used to access the wallet in case of emergency. The minimum number to reconstructs specifies the minimum number of Shamir secrets that are necessary to reconstruct the JSON export config.

| inputs | example |
| ------ | ------- |
| minimum number to reconstruct | 4 |
| total number of secrets | 8 |
*The minimum number to reconstruct has to be at least 3*

---
### `issuer shared secret mark shared`
**Requires**
- an opened wallet
- an exported wallet to IPFS

This command can be used to mark one of the shared secrets in your wallet as shared with a trusted party. The record ID is the id of the record that you want to mark as shared. Use the command `issuer emergency shared secret list` to see all secrets and their record id. The id is the same as the shared secret.

| inputs | example |
| ------ | ------- |
| record ID | 01-Q2I4UWOEJFSDKGHOQ34TYEUGPFVHNGQ3P94WTUGIHSFBJQ94WRUOGSFVNKBQP3ERUTGOUIFSD |

---
### `holder emergency shared secret provide`
**Requires**
- an opened wallet
- an exported wallet to IPFS

This command can be used to share an emergency secret with a doctor in case of emergency. The input requires the emergency doctor proof JSON of that doctor and the name or identifier of the secret issuer. The doctor proof will only hold if the doctor requesting the secret has the correct credential as issued by the Gov-Health-Department.

| inputs | example |
| ------ | ------- |
| doctor proof JSON | JSON as returned by `doctor proof create` |
| wallet identifier | The identifier of the issuer of the shared secret |

---
### `offline emergency secret obtain`
**Requires**
- an opened wallet
- an exported wallet to IPFS

This command can be used in case of emergency by a doctor to gain access information to the IPFS backup. In the case that the device hosting the client was found on the patient, the doctor can gain the IPFS access information by providing his doctor proof JSON to the device.

| inputs | example |
| ------ | ------- |
| doctor proof JSON | JSON as returned by `doctor proof create` |
| wallet identifier | Patient1 |

### `shared secret reconstruct`
This command can be used to reconstruct the actual secret based on the shared secrets. Just paste in the secrets and enter a blank line to finish the input. If the correct number and correct secrets are provided the actual secret will be returned.

| inputs | example |
| ------ | ------- |
| Shamir shared secrets | 01-Q2I4UWOEJFSDKGHOQ34TYEUGPFVHNGQ3P94WTUGIHSFBJQ94WRUOGSFVNKBQP3ERUTGOUIFSD |


---
### `doctor proof request`
This command can be used to review the predefined request of a doctor certificate that has to be issued by the Gov-Health-Department.

---
### `doctor proof create`
This command can be used to create a proof that proofs the identity holder has a doctor certificate that has been issued by the Gov-Health-Department.  If `EHR environment setup` has been used. Wallets of *Doctor1*, *Doctor2* and *Doctor3* will be able to provide this proof. The command is configured to use the first credential in the wallet that meets the requirements as defined in `doctor proof request`.

---
### `doctor proof verify`
This command can be used to verify proofs to actually proof the requirements of an emergency doctor according to the proof request of `doctor proof request`.
