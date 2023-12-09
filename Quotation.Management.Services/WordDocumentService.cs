using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Office.Word;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Quotation.Management.Contracts;
using Quotation.Management.Contracts.Services;
using System.Linq;

namespace Quotation.Management.Services
{
    public class WordDocumentService : IWordDocumentService
    {
        public byte[] CreateWordDocument(dynamic headerData,TableData data,decimal totalAmout)
        {
            //var stream = new MemoryStream();
            using (MemoryStream stream = new())
            {
                using (var document = WordprocessingDocument.Create(stream, WordprocessingDocumentType.Document, true))
                {
                    var mainPart = document.AddMainDocumentPart();
                    mainPart.Document = new Document();
                    var body = mainPart.Document.AppendChild(new Body());

                    body.Append(new Paragraph(new Run(new Break())));


                    body.Append(MakeHaderTable(headerData));
                    // Add a line break
                    body.Append(new Paragraph(new Run(new Break())));


                    Paragraph para = new Paragraph();
                    Run run = new Run(new Text("Equipment Lines"));
                    ParagraphProperties paragraphProperties = new ParagraphProperties();
                    Justification justification = new Justification() { Val = JustificationValues.Left };
                    paragraphProperties.Append(justification);
                    para.Append(paragraphProperties);
                    para.Append(run);
                    body.Append(para);

                    body.Append(MakeTable(data));
                    // Add a right-aligned sentence
                    para = new Paragraph();
                    run = new Run(new Text("Total Amount:"+ totalAmout.ToString("#,##0.##")));
                    paragraphProperties = new ParagraphProperties();
                    justification = new Justification() { Val = JustificationValues.Right };
                    paragraphProperties.Append(justification);
                    para.Append(paragraphProperties);
                    para.Append(run);
                    body.Append(para);

                    body.Append(new Paragraph(new Run(new Break())));

                    AddBulletPoints(body, new List<string> { "Scope of Supply1", "Scope of Supply2", "Scope of Supply3" });
                   
                    document.Save();
                }

            
                return stream.ToArray();
            }
            //stream.Position = 0;
            
        }
        private Table MakeTable(TableData data)
        {
            var table = new Table();
            // Set table properties
            TableProperties tblProps = new TableProperties(
                new TableWidth { Type = TableWidthUnitValues.Pct, Width = "5000" },// 5000/50 = 100% of the page width
                new TableBorders(
                    new TopBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12, Color = "CCCCCC" },
                    new BottomBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12, Color = "CCCCCC" },
                    new LeftBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12, Color = "CCCCCC" },
                    new RightBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12, Color = "CCCCCC" },
                    new InsideHorizontalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12, Color = "CCCCCC" },
                    new InsideVerticalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12, Color = "CCCCCC" }
                )
            );
            table.Append(tblProps);

            //table.AppendChild<TableProperties>(tblProps);




            // Add headers
            var headerRow = new TableRow();

            foreach (var header in data.Headers)
            {
                var cell = new TableCell();
                cell.Append(new TableCellProperties(
                    new TableCellBorders(
                        new TopBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12, Color = "CCCCCC" },
                        new BottomBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12, Color = "CCCCCC" }),
                    new Shading { Fill = "BDD7EE" } // Light blue shading
                ));
                Paragraph paragraph = new Paragraph(new Run(new Text(header)));
                paragraph.ParagraphProperties = new ParagraphProperties(new Justification() { Val = JustificationValues.Center });
                cell.Append(paragraph);
                headerRow.Append(cell);
            }
            table.Append(headerRow);

            // Add rows
            foreach (var row in data.Rows)
            {
                var tableRow = new TableRow();
                foreach (var cellValue in row)
                {
                    var tc = new TableCell();
                    tc.Append(new TableCellProperties(
                        new TableCellWidth { Type = TableWidthUnitValues.Auto }));
                    Paragraph paragraph = new Paragraph(new Run(new Text(cellValue)));
                    paragraph.ParagraphProperties = new ParagraphProperties(new Justification() { Val = JustificationValues.Center });
                    tc.Append(paragraph);
                    tableRow.Append(tc);
                }
                table.Append(tableRow);
            }
            return table;
        }


        private Table MakeHaderTable(dynamic data)
        {
            var table = new Table();
            // Set table properties
            TableProperties tblProps = new TableProperties(
                new TableWidth { Type = TableWidthUnitValues.Pct, Width = "5000" },// 5000/50 = 100% of the page width
                new TableBorders(
                    new TopBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12, Color = "CCCCCC" },
                    new BottomBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12, Color = "CCCCCC" },
                    new LeftBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12, Color = "CCCCCC" },
                    new RightBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12, Color = "CCCCCC" },
                    new InsideHorizontalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12, Color = "CCCCCC" },
                    new InsideVerticalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12, Color = "CCCCCC" }
                )
            );
            table.Append(tblProps);
                    
            table.Append(MakeHeaderTableRow(new List<string> { "quotationNum","customerName"},new List<string> { "Quotation","Customer"},data));

            return table;

        }

        private void AddBulletPoints(Body body, List<string> bulletPoints)
        {
            foreach (var point in bulletPoints)
            {
                var p = new Paragraph(new Run(new Text(point)));
                p.ParagraphProperties = new ParagraphProperties(
                    new NumberingProperties(
                        new NumberingLevelReference { Val = 0 },
                        new NumberingId { Val = 1 }
                    )
                );
                body.Append(p);
            }
        }
        private T getValueFromDyanmicObject<T>(dynamic data,string key)
        {
            return (T)data.GetType().GetProperty(key).GetValue(data, null);
        }
        private TableRow MakeHeaderTableRow(List<string> keys, List<string> display, dynamic data)
        {
            var tableRow = new TableRow();

            for (int i = 0; i < keys.Count; i++)
            {
                var tc = new TableCell();
                tc.Append(new TableCellProperties(
                    new TableCellWidth { Type = TableWidthUnitValues.Auto }));
                Run run = new Run(new Text(display[i]));
                run.RunProperties = new RunProperties(new FontSize { Val = "28" }); // Example: 14-point font size
                Paragraph paragraph = new Paragraph(run);
                paragraph.ParagraphProperties = new ParagraphProperties(new Justification() { Val = JustificationValues.Center });
                tc.Append(paragraph);

                var tc2 = new TableCell();
                tc2.Append(new TableCellProperties(
                    new TableCellWidth { Type = TableWidthUnitValues.Auto }));
                Run run2 = new Run(new Text(getValueFromDyanmicObject<string>(data, keys[i])));
                run2.RunProperties = new RunProperties(new FontSize { Val = "28" }); // Example: 14-point font size
                Paragraph paragraph2 = new Paragraph(run2);
                paragraph2.ParagraphProperties = new ParagraphProperties(new Justification() { Val = JustificationValues.Center });
                tc2.Append(paragraph2);

                tableRow.Append(tc);
                tableRow.Append(tc2);
            }
            return tableRow;
        }
    }

   
}
