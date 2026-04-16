USE [B2BCoffee_DeliveryDB];
IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'Location' AND Object_ID = Object_ID(N'DeliveryStatusHistories'))
    ALTER TABLE DeliveryStatusHistories ADD Location NVARCHAR(MAX) NULL;
