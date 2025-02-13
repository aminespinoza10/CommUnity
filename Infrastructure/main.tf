resource "azurerm_resource_group" "rg" {
  name = "${var.prefix}-commUnity-rg"
  location = var.location
}

resource "azurerm_container_app_environment" "container_app_environment" {
  name                           = "${var.prefix}-commUnity-env"
  location                       = azurerm_resource_group.rg.location
  resource_group_name            = azurerm_resource_group.rg.name
  zone_redundancy_enabled        = false
  
  workload_profile {
    name                  = "Consumption"
    workload_profile_type = "Consumption"
  }
}

resource "azurerm_servicebus_namespace" "servicebus_namespace" {
  name                = "${var.prefix}-commUnity-servicebus-namespace"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  sku                 = "Standard"
}

resource "azurerm_servicebus_queue" "general_servicebus_queue" {
  name         = "general_balance_queue"
  namespace_id = azurerm_servicebus_namespace.servicebus_namespace.id

  partitioning_enabled = true
}

resource "azurerm_servicebus_queue" "year_servicebus_queue" {
  name         = "year_balance_queue"
  namespace_id = azurerm_servicebus_namespace.servicebus_namespace.id

  partitioning_enabled = true
}

resource "azurerm_servicebus_queue" "monthly_servicebus_queue" {
  name         = "monthly_balance_queue"
  namespace_id = azurerm_servicebus_namespace.servicebus_namespace.id

  partitioning_enabled = true
}