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
| record type | emergency-shared-secret |
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
| record type | emergency-shared-secret |
| wallet query(JSON) | {"param": "value"} |
| query options(JSON) | {"retrieveTotalCount": true, "retrieveType": true, "retrieveTags": true} |

---
### `wallet record delete`
**Requires**
- an opened wallet

This command can be used to delete records from the opened wallet.

| inputs | example |
| ------ | ------- |
| record type | emergency-shared-secret |
| record id | 01-13561084561275182 |

---
### `wallet record update tag`
**Requires**
- an opened wallet

This command can be used to update the tag JSON of an existing record in the open wallet.

| inputs | example |
| ------ | ------- |
| record type | emergency-shared-secret |
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
