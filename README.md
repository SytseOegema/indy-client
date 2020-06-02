# Indy Client
This repository holds the source code for a .NET client that interacts with the Hyperledger Indy identity blockchain utilizing Indy SDK. The client provides a CLI that enables users to connect to a pool and create an identity on that pool.

## run a indy pool
To start a local indy pool look [here](https://github.com/hyperledger/indy-sdk/blob/master/README.md#how-to-start-local-nodes-pool-with-docker).

## setup client
The client is test on Ubuntu 16.04 with .NET 2.1 core SDK.
```
add repository
wget https://packages.microsoft.com/config/ubuntu/18.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb

instal dotnet
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

sudo systemctl start ipfs
sudo systemctl enable ipfs
```
