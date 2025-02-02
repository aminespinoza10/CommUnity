resource "azurerm_resource_group" "rg" {
  name = "${var.prefix}-vecinos-rg"
  location = var.location
}

resource "azurerm_container_app_environment" "container_app_environment" {
  name                           = "${var.prefix}-vecinos-env"
  location                       = azurerm_resource_group.rg.location
  resource_group_name            = azurerm_resource_group.rg.name
  zone_redundancy_enabled        = false
  
  workload_profile {
    name                  = "Consumption"
    workload_profile_type = "Consumption"
  }
}