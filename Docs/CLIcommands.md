# CLI commands
This document contains a list with all commands provided via the command line interface of the application. For each command the necessary input fields are explained as well. Some methods require a connection to a hyperledger indy pool, an open wallet or an active DID in order to perform the command. For each command requirements are specified as well.

### `exit`
This command can be used to exit the client.

### `EHR environment setup`
**Requires**
- an active pool connection

This command can be used to initialize the client with wallets, schemas, credentials and emergency access secrets.

### `pool connect`
This command can be used to connect to a pool. The command connects via a genesis file in the `.indy_client` folder.

| inputs | example |
| ------ | ------- |
| pool name | sandbox |

### `wallet create`
This command can be used to create a new Wallet.

| inputs | example |
| ------ | ------- |
| wallet identifier | Anne |
| wallet key | Anne |
*wallet key may be empty. In that case the wallet key will be the same as the wallet identifier*

### `wallet open`
This command can be used to open an existing wallet

| inputs | example |
| ------ | ------- |
| wallet identifier | Anne |
| wallet key | Anne |
*wallet key may be empty. In that case the wallet key will be the same as the wallet identifier*

### `wallet close`
This command can be used to close the currently opened wallet. If there is no wallet currently opened nothing happens.

### `wallet list`
This command can be used to list the wallets that exist on your system

### `wallet open`
This command can be used to

| inputs | example |
| ------ | ------- |
| | |
