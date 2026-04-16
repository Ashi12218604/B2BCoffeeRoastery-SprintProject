-- ═══════════════════════════════════════════════════════════
-- EMBER & BEAN — Seed 10 Coffee Products + Inventory
-- Run this against SQLEXPRESS to populate both databases
-- ═══════════════════════════════════════════════════════════

-- Shared Product IDs (same GUID used in both Product and Inventory DBs)
DECLARE @P1 UNIQUEIDENTIFIER = 'A1B2C3D4-1111-4AAA-BBBB-000000000001';
DECLARE @P2 UNIQUEIDENTIFIER = 'A1B2C3D4-2222-4AAA-BBBB-000000000002';
DECLARE @P3 UNIQUEIDENTIFIER = 'A1B2C3D4-3333-4AAA-BBBB-000000000003';
DECLARE @P4 UNIQUEIDENTIFIER = 'A1B2C3D4-4444-4AAA-BBBB-000000000004';
DECLARE @P5 UNIQUEIDENTIFIER = 'A1B2C3D4-5555-4AAA-BBBB-000000000005';
DECLARE @P6 UNIQUEIDENTIFIER = 'A1B2C3D4-6666-4AAA-BBBB-000000000006';
DECLARE @P7 UNIQUEIDENTIFIER = 'A1B2C3D4-7777-4AAA-BBBB-000000000007';
DECLARE @P8 UNIQUEIDENTIFIER = 'A1B2C3D4-8888-4AAA-BBBB-000000000008';
DECLARE @P9 UNIQUEIDENTIFIER = 'A1B2C3D4-9999-4AAA-BBBB-000000000009';
DECLARE @P10 UNIQUEIDENTIFIER = 'A1B2C3D4-AAAA-4AAA-BBBB-000000000010';

DECLARE @SuperAdminId UNIQUEIDENTIFIER = '00000000-0000-0000-0000-000000000001';
DECLARE @Now DATETIME2 = GETUTCDATE();

-- ═══════════════════════════════════════════════════════════
-- PART 1: Product Database
-- ═══════════════════════════════════════════════════════════
USE [B2BCoffee_ProductDB];
GO

-- Only insert if not already seeded
IF NOT EXISTS (SELECT 1 FROM Products WHERE Id = 'A1B2C3D4-1111-4AAA-BBBB-000000000001')
BEGIN
    INSERT INTO Products (Id, Name, Description, SKU, Price, DiscountedPrice, Origin, Category, RoastLevel, WeightInGrams, ImageUrl, IsActive, IsFeatured, MinimumOrderQuantity, CreatedAt, UpdatedAt, CreatedBy)
    VALUES
    ('A1B2C3D4-1111-4AAA-BBBB-000000000001', 'Ethiopian Yirgacheffe',       'Bright, floral Ethiopian single origin with notes of jasmine, bergamot, and citrus. Grown at 1,800m elevation in the birthplace of coffee.',          'ETH-YIR-01',  850,  799,  'Ethiopia',     2, 1, 250, '/assets/images/products/ethiopian-yirgacheffe.png', 1, 1, 5,  GETUTCDATE(), GETUTCDATE(), '00000000-0000-0000-0000-000000000001'),
    ('A1B2C3D4-2222-4AAA-BBBB-000000000002', 'Colombian Supremo',           'Rich and balanced Colombian beans with caramel sweetness, medium body, and a clean nutty finish. Supremo grade, screen 17+.',                        'COL-SUP-01',  720,  NULL, 'Colombia',     2, 2, 500, '/assets/images/products/colombian-supremo.png', 1, 1, 5,  GETUTCDATE(), GETUTCDATE(), '00000000-0000-0000-0000-000000000001'),
    ('A1B2C3D4-3333-4AAA-BBBB-000000000003', 'Italian Espresso Classico',   'Traditional dark-roasted espresso blend of Brazilian and Robusta beans. Bold, syrupy body with intense cocoa and smoky undertones.',                  'ESP-ITA-01',  680,  NULL, 'Italy/Brazil', 1, 4, 250, '/assets/images/products/italian-espresso.png', 1, 1, 10, GETUTCDATE(), GETUTCDATE(), '00000000-0000-0000-0000-000000000001'),
    ('A1B2C3D4-4444-4AAA-BBBB-000000000004', 'Sumatra Mandheling',          'Full-bodied Indonesian coffee with earthy, herbal complexity. Low acidity with a lingering dark chocolate and cedar finish. Wet-hulled process.',      'SUM-MAN-01',  920,  880,  'Indonesia',    2, 4, 250, '', 1, 0, 5,  GETUTCDATE(), GETUTCDATE(), '00000000-0000-0000-0000-000000000001'),
    ('A1B2C3D4-5555-4AAA-BBBB-000000000005', 'Guatemala Antigua',           'Refined Guatemalan coffee with spicy, chocolaty notes and a velvety texture. Shade-grown at volcanic highlands with rich volcanic soil.',              'GUA-ANT-01',  780,  NULL, 'Guatemala',    2, 2, 500, '/assets/images/products/guatemala-antigua.png', 1, 1, 5,  GETUTCDATE(), GETUTCDATE(), '00000000-0000-0000-0000-000000000001'),
    ('A1B2C3D4-6666-4AAA-BBBB-000000000006', 'Swiss Water Decaf Blend',     'Chemically-free Swiss Water Process decaf with all the flavor, none of the caffeine. Smooth, mellow, and perfect for evening cups.',                   'DEC-SWS-01',  650,  NULL, 'Brazil/Peru',  3, 2, 250, '', 1, 0, 10, GETUTCDATE(), GETUTCDATE(), '00000000-0000-0000-0000-000000000001'),
    ('A1B2C3D4-7777-4AAA-BBBB-000000000007', 'Kenya AA Peaberry',           'Exceptional Kenyan peaberry with wine-like acidity, blackcurrant, and grapefruit tones. SL28 varietal from Nyeri County, washed process.',           'KEN-PEA-01',  1050, 980,  'Kenya',        2, 1, 250, '/assets/images/products/kenya-aa.png', 1, 1, 3,  GETUTCDATE(), GETUTCDATE(), '00000000-0000-0000-0000-000000000001'),
    ('A1B2C3D4-8888-4AAA-BBBB-000000000008', 'Cold Brew Concentrate',       'Pre-ground coarse blend optimized for 12-24hr cold extraction. Yields a smooth, low-acid concentrate with natural sweetness and chocolate notes.',     'CLD-BRW-01',  550,  499,  'Brazil',       4, 3, 500, '', 1, 0, 10, GETUTCDATE(), GETUTCDATE(), '00000000-0000-0000-0000-000000000001'),
    ('A1B2C3D4-9999-4AAA-BBBB-000000000009', 'Costa Rica Tarrazu',          'Bright and clean Costa Rican beans from the famed Tarrazu highlands. Honey-processed with notes of apricot, brown sugar, and a silky mouthfeel.',     'CRC-TAR-01',  890,  NULL, 'Costa Rica',   2, 2, 250, '', 1, 0, 5,  GETUTCDATE(), GETUTCDATE(), '00000000-0000-0000-0000-000000000001'),
    ('A1B2C3D4-AAAA-4AAA-BBBB-000000000010', 'Midnight Mocha Blend',        'A luxurious house blend of Ethiopian and Brazilian beans with rich cocoa, toasted almond, and a whisper of vanilla. Perfect for lattes and cappuccinos.','BLD-MOC-01',  690,  650,  'Ethiopia/Brazil', 1, 3, 500, '', 1, 1, 5, GETUTCDATE(), GETUTCDATE(), '00000000-0000-0000-0000-000000000001');

    PRINT '✓ 10 products seeded into ProductDB';
END
ELSE
BEGIN
    PRINT '→ Products already seeded, skipping...';
END
GO

-- ═══════════════════════════════════════════════════════════
-- PART 2: Inventory Database
-- ═══════════════════════════════════════════════════════════
USE [B2BCoffee_InventoryDB];
GO

IF NOT EXISTS (SELECT 1 FROM InventoryItems WHERE ProductId = 'A1B2C3D4-1111-4AAA-BBBB-000000000001')
BEGIN
    INSERT INTO InventoryItems (Id, ProductId, ProductName, SKU, QuantityAvailable, ReservedQuantity, LowStockThreshold, UpdatedAt, CreatedAt)
    VALUES
    (NEWID(), 'A1B2C3D4-1111-4AAA-BBBB-000000000001', 'Ethiopian Yirgacheffe',     'ETH-YIR-01', 150, 0, 20, GETUTCDATE(), GETUTCDATE()),
    (NEWID(), 'A1B2C3D4-2222-4AAA-BBBB-000000000002', 'Colombian Supremo',         'COL-SUP-01', 200, 0, 25, GETUTCDATE(), GETUTCDATE()),
    (NEWID(), 'A1B2C3D4-3333-4AAA-BBBB-000000000003', 'Italian Espresso Classico', 'ESP-ITA-01', 300, 0, 30, GETUTCDATE(), GETUTCDATE()),
    (NEWID(), 'A1B2C3D4-4444-4AAA-BBBB-000000000004', 'Sumatra Mandheling',        'SUM-MAN-01', 100, 0, 15, GETUTCDATE(), GETUTCDATE()),
    (NEWID(), 'A1B2C3D4-5555-4AAA-BBBB-000000000005', 'Guatemala Antigua',         'GUA-ANT-01', 180, 0, 20, GETUTCDATE(), GETUTCDATE()),
    (NEWID(), 'A1B2C3D4-6666-4AAA-BBBB-000000000006', 'Swiss Water Decaf Blend',   'DEC-SWS-01',  80, 0, 15, GETUTCDATE(), GETUTCDATE()),
    (NEWID(), 'A1B2C3D4-7777-4AAA-BBBB-000000000007', 'Kenya AA Peaberry',         'KEN-PEA-01',  60, 0, 10, GETUTCDATE(), GETUTCDATE()),
    (NEWID(), 'A1B2C3D4-8888-4AAA-BBBB-000000000008', 'Cold Brew Concentrate',     'CLD-BRW-01', 250, 0, 30, GETUTCDATE(), GETUTCDATE()),
    (NEWID(), 'A1B2C3D4-9999-4AAA-BBBB-000000000009', 'Costa Rica Tarrazu',        'CRC-TAR-01', 120, 0, 15, GETUTCDATE(), GETUTCDATE()),
    (NEWID(), 'A1B2C3D4-AAAA-4AAA-BBBB-000000000010', 'Midnight Mocha Blend',      'BLD-MOC-01', 220, 0, 25, GETUTCDATE(), GETUTCDATE());

    PRINT '✓ 10 inventory items seeded into InventoryDB';
END
ELSE
BEGIN
    PRINT '→ Inventory already seeded, skipping...';
END
GO

PRINT '';
PRINT '═══════════════════════════════════════════════';
PRINT '  EMBER & BEAN — Seed Complete!';
PRINT '  10 products + 10 inventory records created.';
PRINT '═══════════════════════════════════════════════';
