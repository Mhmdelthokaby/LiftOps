-- MT-011 legacy path migration script
-- Prefixes existing report/file path values with the default company id.

DECLARE @DefaultCompanyId NVARCHAR(36) = '00000000-0000-0000-0000-000000000000';

-- Stage PDF paths under wwwroot/reports/*
UPDATE InstallationStages
SET PdfPath = REPLACE(PdfPath, 'wwwroot/reports/', CONCAT('wwwroot/reports/', @DefaultCompanyId, '/'))
WHERE PdfPath IS NOT NULL
  AND PdfPath LIKE 'wwwroot/reports/%'
  AND PdfPath NOT LIKE CONCAT('wwwroot/reports/', @DefaultCompanyId, '/%');

-- Offer, quotation and attachment paths
UPDATE Offers
SET OfferPdfPath = CONCAT(@DefaultCompanyId, '/', OfferPdfPath)
WHERE OfferPdfPath IS NOT NULL
  AND OfferPdfPath <> ''
  AND OfferPdfPath NOT LIKE CONCAT(@DefaultCompanyId, '/%');

UPDATE Quotations
SET AttachmentPath = CONCAT(@DefaultCompanyId, '/', AttachmentPath)
WHERE AttachmentPath IS NOT NULL
  AND AttachmentPath <> ''
  AND AttachmentPath NOT LIKE CONCAT(@DefaultCompanyId, '/%');

UPDATE QuotationAttachments
SET FilePath = CONCAT(@DefaultCompanyId, '/', FilePath)
WHERE FilePath IS NOT NULL
  AND FilePath <> ''
  AND FilePath NOT LIKE CONCAT(@DefaultCompanyId, '/%');
