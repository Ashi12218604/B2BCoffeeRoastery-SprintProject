USE [B2BCoffee_DeliveryDB];
IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'ActualDeliveryDate' AND Object_ID = Object_ID(N'Deliveries'))
    ALTER TABLE Deliveries ADD ActualDeliveryDate DATETIME2 NULL;
IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'AgentPhone' AND Object_ID = Object_ID(N'Deliveries'))
    ALTER TABLE Deliveries ADD AgentPhone NVARCHAR(MAX) NULL;
IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'ApprovedByAdminName' AND Object_ID = Object_ID(N'Deliveries'))
    ALTER TABLE Deliveries ADD ApprovedByAdminName NVARCHAR(MAX) NULL;
IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'AssignedAgent' AND Object_ID = Object_ID(N'Deliveries'))
    ALTER TABLE Deliveries ADD AssignedAgent NVARCHAR(MAX) NULL;
IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'Location' AND Object_ID = Object_ID(N'DeliveryStatusHistory'))
    ALTER TABLE DeliveryStatusHistory ADD Location NVARCHAR(MAX) NULL;
