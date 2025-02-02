terraform {
  backend "azurerm" {
    storage_account_name = "generalstorageamin"
    container_name       = "tfstate"
    key                  = "vecinos.tfstate"
  }
}