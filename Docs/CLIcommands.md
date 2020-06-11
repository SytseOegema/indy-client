# CLI commands
This document contains a list with all commands provided via the command line interface of the application. For each command the necessary input fields are explained as well. Some methods require a connection to a hyperledger indy pool, an open wallet or an active DID in order to perform the command. For each command requirements are specified as well.

### `EHR environment setup`
**Requires**
- an active pool connection

This command can be used to initialize the client with wallets, schemas, credentials and emergency access secrets.

### `pool connect`
This command can be used to connect to a pool. The command connects via a genesis file in the `.indy_client` folder.

| inputs | format | example |
| ------ | ------ | ------- |
| pool name| string | sandbox |
