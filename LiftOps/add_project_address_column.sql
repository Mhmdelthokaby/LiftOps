-- Migration: Add ProjectAddress column to MaintenanceContracts table
-- Generated from: 20260112133940_AddProjectAddressToMaintenanceContract

ALTER TABLE [MaintenanceContracts]
ADD [ProjectAddress] nvarchar(max) NULL;
