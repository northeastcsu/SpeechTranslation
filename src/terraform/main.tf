locals {
  prefix        = "${var.projectname}${var.environment}"
  rgname        = "${local.prefix}rg"
  nsgname       = "${local.prefix}nsg"
  stoname       = "${local.prefix}sto"
  srname        = "${local.prefix}signalR"
  appPlan       = "${local.prefix}appplan"
  speechFn      = "${local.prefix}azfn"
  webappplan    = "${local.prefix}webappplan"
  webappname    = "${local.prefix}webapp"
  speechsvcname = "${local.prefix}speechsvc"
}


//azure provider
terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "3.47.0"
    }
  }
}

provider "azurerm" {
  # Configuration options
  features {

  }
}

//resource group
resource "azurerm_resource_group" "speechrg" {
  name     = local.rgname
  location = var.location

  tags = {
    environment = var.environment
    projectname = var.projectname
  }

}

//signal r service
resource "azurerm_signalr_service" "speechsignalr" {
  name                = local.srname
  location            = azurerm_resource_group.speechrg.location
  resource_group_name = azurerm_resource_group.speechrg.name

  sku {
    name     = "Standard_S1"
    capacity = 1
  }

  cors {
    allowed_origins = ["*"]
  }

  connectivity_logs_enabled = true
  messaging_logs_enabled    = true
  service_mode              = "Serverless"

  tags = {
    environment = var.environment
    projectname = var.projectname
  }

}

/*output "signalrconnectionstring" {
  value = azurerm_signalr_service.speechsignalr.primary_connection_string
}*/

//storate account
resource "azurerm_storage_account" "speechsto" {
  name                     = "speechtranslatedevsto"
  resource_group_name      = azurerm_resource_group.speechrg.name
  location                 = azurerm_resource_group.speechrg.location
  account_tier             = "Standard"
  account_replication_type = "LRS"

  tags = {
    environment = var.environment
    projectname = var.projectname
  }

}

//app service plan - Windows
resource "azurerm_service_plan" "speechappplan" {
  name                = local.appPlan
  location            = azurerm_resource_group.speechrg.location
  resource_group_name = azurerm_resource_group.speechrg.name
  sku_name            = "S1"
  os_type             = "Windows"

}

//azure function
resource "azurerm_function_app" "speechfnapp" {
  name                       = local.speechFn
  location                   = azurerm_resource_group.speechrg.location
  resource_group_name        = azurerm_resource_group.speechrg.name
  app_service_plan_id        = azurerm_service_plan.speechappplan.id
  storage_account_name       = azurerm_storage_account.speechsto.name
  storage_account_access_key = azurerm_storage_account.speechsto.primary_access_key
  version                    = "~4"
  app_settings = {
    "AzureSignalRConnectionString"             = azurerm_signalr_service.speechsignalr.primary_connection_string
    "FUNCTIONS_WORKER_RUNTIME"                 = "dotnet"
    "WEBSITE_CONTENTAZUREFILECONNECTIONSTRING" = azurerm_storage_account.speechsto.primary_connection_string
  }

  tags = {
    environment = var.environment
    projectname = var.projectname
  }
}


//app service plan - Linux
resource "azurerm_service_plan" "webappappplan" {
  name                = local.webappplan
  resource_group_name = azurerm_resource_group.speechrg.name
  location            = azurerm_resource_group.speechrg.location
  os_type             = "Linux"
  sku_name            = "S1"

  tags = azurerm_resource_group.speechrg.tags
}

//azure app service
resource "azurerm_linux_web_app" "example" {
  name                = local.webappname
  resource_group_name = azurerm_resource_group.speechrg.name
  location            = azurerm_resource_group.speechrg.location
  service_plan_id     = azurerm_service_plan.webappappplan.id

  site_config {
    application_stack {
      node_version = "18-lts"
    }
    app_command_line = "pm2 serve /home/site/wwwroot/dist --no-daemon --spa"
  }

  tags = azurerm_resource_group.speechrg.tags

}

//speech service
resource "azurerm_cognitive_account" "cognitivespeech" {
  name                = local.speechsvcname
  location            = azurerm_resource_group.speechrg.location
  resource_group_name = azurerm_resource_group.speechrg.name
  kind                = "SpeechServices"

  sku_name = "S0"

  tags = azurerm_resource_group.speechrg.tags
}