using LiftOps_BackEnd.Application.Interfaces.Installation;
using LiftOps_BackEnd.Domain.Entities.Installation;
using LiftOps_BackEnd.Domain.Entities.Maintenance;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LiftOps_BackEnd.Infrastructure.Services.Installation
{
    public class PdfGeneratorService : IPdfGenerator
    {
        public PdfGeneratorService()
        {
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public async Task<string> GenerateStageReportAsync(InstallationStage stage)
        {
            // Ensure directory exists
            var folderName = $"wwwroot/reports/{stage.Elevator.ProjectId}/{stage.ElevatorId}";
            var fileName = $"stage{stage.StageNumber}.pdf";
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), folderName);
            
            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
            }

            var filePath = Path.Combine(fullPath, fileName);

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    page.Header()
                        .Text($"Installation Stage {stage.StageNumber} Report")
                        .SemiBold().FontSize(20).FontColor(Colors.Red.Medium);

                    page.Content().PaddingVertical(1, Unit.Centimetre).Column(x =>
                    {
                        x.Item().Text($"Project: {stage.Elevator.Project.Customer?.ProjectNumber}");
                        x.Item().Text($"Customer: {stage.Elevator.Project.Customer?.Name}");
                        x.Item().Text($"Elevator Type: {stage.Elevator.ElevatorType}");
                        x.Item().Text($"Date: {DateTime.Now:d}");
                        x.Item().Text($"Stage Status: {stage.Status}");
                        
                        x.Spacing(20);

                        x.Item().Text("Parts Used:").Bold();
                        foreach (var part in stage.RequiredParts)
                        {
                            x.Item().Text($"- {part.InventoryItem?.Name ?? "Unknown Part"} (Qty: {part.Quantity})");
                        }

                        x.Spacing(20);
                        x.Item().Text($"Supply Cost: {stage.SupplyCost:C}");
                        x.Item().Text($"Notes: {stage.Notes}");
                    });

                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Page ");
                            x.CurrentPageNumber();
                        });
                });
            })
            .GeneratePdf(filePath);

            // Return relative path for storage
            return Path.Combine(folderName, fileName);
        }

        public async Task<byte[]> GenerateMaintenanceVisitReportAsync(MaintenanceVisit visit)
        {
            var logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/liftops.png");

            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(1, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily(Fonts.Arial));

                    // Fixed Header on every page
                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("LiftOps Elevators & Escalators").FontSize(16).SemiBold();
                            col.Item().Text("كولينز للمصاعد والسلالم الكهربائية").FontSize(14).FontColor(Colors.Red.Medium);
                        });

                        if (File.Exists(logoPath))
                        {
                            row.ConstantItem(60).Image(logoPath);
                        }

                        row.RelativeItem().AlignRight().Column(col =>
                        {
                            col.Item().Text("Inspection Report").FontSize(20).SemiBold().FontColor(Colors.Red.Medium);
                            col.Item().Text($"Date: {visit.VisitDate:dd/MM/yyyy}").FontSize(10);
                        });
                    });

                    page.Content().PaddingVertical(10).Column(col =>
                    {
                        // Visit Information
                        col.Item().PaddingBottom(10).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(80);
                                columns.RelativeColumn();
                                columns.ConstantColumn(80);
                                columns.RelativeColumn();
                            });

                            table.Cell().Element(LabelCellStyle).Text("Client Name");
                            table.Cell().Element(ValueCellStyle).Text(visit.MaintenanceElevator?.Contract?.Customer?.Name ?? "N/A");
                            
                            table.Cell().Element(LabelCellStyle).Text("Project No");
                            table.Cell().Element(ValueCellStyle).Text(visit.MaintenanceElevator?.Contract?.ProjectNumber ?? "N/A");

                            table.Cell().Element(LabelCellStyle).Text("Address");
                            table.Cell().Element(ValueCellStyle).Text(visit.MaintenanceElevator?.Contract?.Customer?.Address ?? "N/A");

                            table.Cell().Element(LabelCellStyle).Text("Technician");
                            table.Cell().Element(ValueCellStyle).Text(visit.Technician?.Name ?? "N/A");
                        });

                        // Maintenance Notes
                        if (!string.IsNullOrWhiteSpace(visit.Notes))
                        {
                            col.Item().PaddingBottom(10).Column(c =>
                            {
                                c.Item().Text("Maintenance Notes").SemiBold().FontSize(12);
                                c.Item().Background(Colors.Grey.Lighten4).Padding(5).Text(visit.Notes);
                            });
                        }

                        // Payment Notes
                        if (!string.IsNullOrWhiteSpace(visit.PaymentNotes))
                        {
                            col.Item().PaddingBottom(10).Column(c =>
                            {
                                c.Item().Text("Payment Notes").SemiBold().FontSize(12);
                                c.Item().BorderLeft(3).BorderColor(Colors.Green.Medium).Background(Colors.Grey.Lighten5).Padding(5).Text(visit.PaymentNotes);
                            });
                        }

                        // Checklist Items Table
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(30);
                                columns.RelativeColumn(3);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(2);
                            });

                            // Table Header (Repeats on every page)
                            table.Header(header =>
                            {
                                header.Cell().Element(TableHeaderStyle).Text("#");
                                header.Cell().Element(TableHeaderStyle).Text("Item Name");
                                header.Cell().Element(TableHeaderStyle).Text("Status");
                                header.Cell().Element(TableHeaderStyle).Text("Count");
                                header.Cell().Element(TableHeaderStyle).Text("Percentage");
                                header.Cell().Element(TableHeaderStyle).Text("Notes");
                            });

                            int index = 1;
                            foreach (var item in visit.ChecklistItems.OrderBy(i => i.ChecklistItem?.Order ?? 0))
                            {
                                table.Cell().Element(TableCellStyle).AlignCenter().Text(index++.ToString());
                                table.Cell().Element(TableCellStyle).Text(item.ChecklistItem?.Title ?? "N/A");
                                
                                var statusText = item.Status ?? (item.IsCompleted ? "Good" : "N/A");
                                var statusColor = statusText.ToLower() switch
                                {
                                    "good" => Colors.Green.Medium,
                                    "bad" => Colors.Red.Medium,
                                    "medium" => Colors.Orange.Medium,
                                    _ => Colors.Black
                                };
                                table.Cell().Element(TableCellStyle).AlignCenter().Text(statusText).FontColor(statusColor).Bold();
                                
                                table.Cell().Element(TableCellStyle).AlignCenter().Text(item.Count?.ToString() ?? "-");
                                table.Cell().Element(TableCellStyle).AlignCenter().Text(item.Percentage.HasValue ? $"{item.Percentage:F2}%" : "-");
                                table.Cell().Element(TableCellStyle).Text(item.Notes ?? "-");
                            }
                        });

                        // Spare Parts (Only if exist)
                        if (visit.SpareParts != null && visit.SpareParts.Any())
                        {
                            col.Item().PaddingTop(20).Column(c =>
                            {
                                c.Item().Text("Spare Parts").SemiBold().FontSize(14).FontColor(Colors.Red.Medium);
                                c.Item().Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn(3);
                                        columns.RelativeColumn(1);
                                        columns.RelativeColumn(1);
                                        columns.RelativeColumn(1);
                                        columns.RelativeColumn(1);
                                    });

                                    table.Header(header =>
                                    {
                                        header.Cell().Element(TableHeaderStyle).Text("Item");
                                        header.Cell().Element(TableHeaderStyle).Text("Quantity");
                                        header.Cell().Element(TableHeaderStyle).Text("Unit Price");
                                        header.Cell().Element(TableHeaderStyle).Text("Total");
                                        header.Cell().Element(TableHeaderStyle).Text("Paid");
                                    });

                                    foreach (var part in visit.SpareParts)
                                    {
                                        table.Cell().Element(TableCellStyle).Text(part.InventoryItem?.Name ?? "N/A");
                                        table.Cell().Element(TableCellStyle).AlignCenter().Text(part.Quantity.ToString());
                                        table.Cell().Element(TableCellStyle).AlignRight().Text($"{part.PriceAtTimeOfUsage:C}");
                                        table.Cell().Element(TableCellStyle).AlignRight().Text($"{part.Quantity * part.PriceAtTimeOfUsage:C}");
                                        table.Cell().Element(TableCellStyle).AlignCenter().Text(part.IsPaid ? "Yes" : "No").FontColor(part.IsPaid ? Colors.Green.Medium : Colors.Orange.Medium).Bold();
                                    }

                                    table.Footer(footer =>
                                    {
                                        var total = visit.SpareParts.Sum(p => p.Quantity * p.PriceAtTimeOfUsage);
                                        footer.Cell().ColumnSpan(3).Element(TableCellStyle).AlignRight().Text("Total:").Bold();
                                        footer.Cell().Element(TableCellStyle).AlignRight().Text($"{total:C}").Bold();
                                        footer.Cell().Element(TableCellStyle);
                                    });
                                });
                            });
                        }
                    });

                    // Fixed Footer on every page
                    page.Footer().Column(col =>
                    {
                        col.Item().PaddingVertical(5).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text("LiftOps Elevators & Escalators").FontSize(8).SemiBold();
                                c.Item().Text("Customer Service: 01022223207 | Email: info@liftops.com").FontSize(8);
                            });

                            row.RelativeItem().AlignRight().Text(x =>
                            {
                                x.Span("Page ");
                                x.CurrentPageNumber();
                                x.Span(" of ");
                                x.TotalPages();
                            });
                        });
                    });
                });
            }).GeneratePdf();
        }

        // Helper styles
        private static IContainer LabelCellStyle(IContainer container) => container.Border(0.5f).BorderColor(Colors.Grey.Lighten1).Background(Colors.Grey.Lighten4).Padding(5).AlignMiddle().DefaultTextStyle(x => x.SemiBold());
        private static IContainer ValueCellStyle(IContainer container) => container.Border(0.5f).BorderColor(Colors.Grey.Lighten1).Padding(5).AlignMiddle();
        private static IContainer TableHeaderStyle(IContainer container) => container.Border(0.5f).BorderColor(Colors.Grey.Lighten1).Background(Colors.Grey.Lighten3).Padding(5).AlignCenter().AlignMiddle().DefaultTextStyle(x => x.SemiBold());
        private static IContainer TableCellStyle(IContainer container) => container.Border(0.5f).BorderColor(Colors.Grey.Lighten1).Padding(5).AlignMiddle();
    }
}
