# BB SSI EHR CLI Client
This repository holds the source code for a blockchain-based self-sovereign identity application for electronic health records. The .NET client interacts with the Hyperledger Indy identity blockchain utilizing Indy SDK. The client provides a CLI that enables users to connect to a pool and create an identity on that pool.

Documentation on the project can be found in the Docs folder.

# Quickstart
Follow the instructions to setup the environment

## run a indy pool
To start a local indy pool look [here](https://github.com/hyperledger/indy-node/blob/master/docs/source/start-nodes.md) or via [docker](https://github.com/hyperledger/indy-sdk/blob/master/README.md#how-to-start-local-nodes-pool-with-docker).

### start a local indy pool
```
sudo apt-key adv --keyserver keyserver.ubuntu.com --recv-keys CE7709D068DB5E88
sudo bash -c 'echo "deb https://repo.sovrin.org/deb xenial stable" >> /etc/apt/sources.list'
sudo apt-get update
sudo apt-get install indy-node
```

Now adjust the file /etc/indy/indy_config.py and replace the line
```
NETWORK_NAME = None
```

with the line
```
NETWORK_NAME = sandbox
```

Run the following command to generate keys and transactions for a network containing 4 nodes.
```
generate_indy_pool_transactions --nodes 4 --clients 0 --nodeNum 1 2 3 4
```

Now you can run the 4 nodes(in 4 different terminals) as
```
start_indy_node Node1 0.0.0.0 9701 0.0.0.0 9702
```
```
start_indy_node Node2 0.0.0.0 9703 0.0.0.0 9704
```
```
start_indy_node Node3 0.0.0.0 9705 0.0.0.0 9706
```
```
start_indy_node Node4 0.0.0.0 9707 0.0.0.0 9708
```

## setup client
The client is test on Ubuntu 16.04 with .NET 2.1 core SDK.
```
# add repository
wget https://packages.microsoft.com/config/ubuntu/18.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb

# install dotnet
sudo add-apt-repository universe
sudo apt-get update
sudo apt-get install apt-transport-https
sudo apt-get update
sudo apt-get install dotnet-sdk-2.1
```

### install libindy for ubuntu is shown below. For other OS see [here](https://github.com/hyperledger/indy-sdk/blob/master/README.md#binaries).
```
sudo apt-key adv --keyserver keyserver.ubuntu.com --recv-keys CE7709D068DB5E88
# xenial for 16.04, bionic for 18.04
# sudo add-apt-repository "deb https://repo.sovrin.org/sdk/deb (xenial|bionic) master"
sudo add-apt-repository "deb https://repo.sovrin.org/sdk/deb xenial master"
sudo apt-get update
sudo apt-get install -y libindy
```

### Now we need to add the Indy .NET SDK nd the NewtonSoft.Json library for handling Json.
```
dotnet add package Hyperledger.Indy.Sdk --version 1.11.1
dotnet add package Newtonsoft.Json --version 12.0.3
dotnet add package Ipfs.Http.Client
dotnet add package SecretSharingDotNet --version 0.3.0
```

### Install IPFS daemon
```
wget https://github.com/ipfs/go-ipfs/releases/download/v0.5.0/go-ipfs_v0.5.0_linux-amd64.tar.gz
tar -xvzf go-ipfs_v0.5.0_linux-amd64.tar.gz
cd go-ipfs
sudo bash install.sh
ipfs --version

cd ..
rm go-ipfs_v0.5.0_linux-amd64.tar.gz
rm -r go-ipfs


# create systemd service

# create the file /etc/systemd/system/ipfs.service
# with the following content
# this assumes there is a user hyper in the system
[Unit]
Description=IPFS daemon
After=network.target

[Service]
User=hyper
ExecStart=/usr/local/bin/ipfs daemon

[Install]
WantedBy=multiuser.target



# after creating run
ipfs init

# finally run
sudo systemctl start ipfs
sudo systemctl enable ipfs
```


### Start the application
Fork this repository and clone it. Make sure that you are in the Code repository and run the command
```
dotnet run
```
