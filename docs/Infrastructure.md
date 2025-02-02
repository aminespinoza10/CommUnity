# Infrastructure deployment

In order to deploy the required infrastructure for this solution you can take a look at the [Infrastructure](../Infrastructure/) folder where the components can be deployed.

## List of components

|Component type|Component purpose|
|--------------|-----------------|
|Resource group|Hosts the whole set of components|
|Container Apps Environment|The expected environment for the whole set of microservices|

## Requirements

- You need to create a storage account to hold the remote state for Terraform.
## Roadmap

Create a CI/CD process to deploy the whole infrastructure