using System.Text;
using System.Text.Json;
using System.Drawing;
using System.IO;
using ESC_POS_USB_NET.Enums;
using ESC_POS_USB_NET.Printer;

namespace Wokcito
{
    class EscPosWrapper
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                return;
            }

            string printerName = args[0];
            string commandsJson = args[1];

            /**
             * https://github.com/mtmsuhail/ESC-POS-USB-NET/issues/10
             */
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            try
            {
                Printer printer = new Printer(printerName);
                var commands = JsonSerializer.Deserialize<Command[]>(commandsJson);

                if (commands != null)
                {
                    foreach (var cmd in commands)
                    {
                        ExecuteCommand(printer, cmd);
                    }
                }

                printer.PrintDocument();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al imprimir: {ex.Message}");
            }
        }

        private static void ExecuteCommand(Printer printer, Command command)
        {
            switch (command.Type.ToLower())
            {
                // === TEXTO ===
                case "append":
                    printer.Append(command.Text ?? "");
                    break;

                case "appendwithoutlf":
                    printer.AppendWithoutLf(command.Text ?? "");
                    break;

                // === MODOS DE FUENTE ===
                case "boldmode":
                    if (!string.IsNullOrEmpty(command.Text))
                        printer.BoldMode(command.Text);
                    else if (command.State.HasValue)
                        printer.BoldMode(command.State.Value ? PrinterModeState.On : PrinterModeState.Off);
                    break;

                case "underlinemode":
                    if (!string.IsNullOrEmpty(command.Text))
                        printer.UnderlineMode(command.Text);
                    else if (command.State.HasValue)
                        printer.UnderlineMode(command.State.Value ? PrinterModeState.On : PrinterModeState.Off);
                    break;

                case "expandedmode":
                    if (!string.IsNullOrEmpty(command.Text))
                        printer.ExpandedMode(command.Text);
                    else if (command.State.HasValue)
                        printer.ExpandedMode(command.State.Value ? PrinterModeState.On : PrinterModeState.Off);
                    break;

                case "condensedmode":
                    if (!string.IsNullOrEmpty(command.Text))
                        printer.CondensedMode(command.Text);
                    else if (command.State.HasValue)
                        printer.CondensedMode(command.State.Value ? PrinterModeState.On : PrinterModeState.Off);
                    break;

                // === ANCHO DE FUENTE ===
                case "doublewidth2":
                    printer.DoubleWidth2();
                    break;

                case "doublewidth3":
                    printer.DoubleWidth3();
                    break;

                case "normalwidth":
                    printer.NormalWidth();
                    break;

                // === ALINEACIÓN ===
                case "alignleft":
                    printer.AlignLeft();
                    break;

                case "alignright":
                    printer.AlignRight();
                    break;

                case "aligncenter":
                    printer.AlignCenter();
                    break;

                // === FUENTES ===
                case "font":
                    if (!string.IsNullOrEmpty(command.Text) && command.FontType.HasValue)
                        printer.Font(command.Text, (Fonts)command.FontType.Value);
                    break;

                // === SEPARADORES ===
                case "separator":
                    if (command.Character.HasValue)
                        printer.Separator(command.Character.Value);
                    else
                        printer.Separator();
                    break;

                // === ALTURA DE LÍNEA ===
                case "setlineheight":
                    if (command.Height.HasValue)
                        printer.SetLineHeight(command.Height.Value);
                    break;

                case "normallineheight":
                    printer.NormalLineHeight();
                    break;

                // === SALTOS DE LÍNEA ===
                case "newline":
                    printer.NewLine();
                    break;

                case "newlines":
                    if (command.Lines.HasValue)
                        printer.NewLines(command.Lines.Value);
                    break;

                // === CORTE DE PAPEL ===
                case "fullpapercut":
                    printer.FullPaperCut();
                    break;

                case "partialpapercut":
                    printer.PartialPaperCut();
                    break;

                // === CAJÓN ===
                case "opendrawer":
                    printer.OpenDrawer();
                    break;

                // === CÓDIGOS DE BARRAS ===
                case "code128":
                    if (!string.IsNullOrEmpty(command.Text))
                    {
                        var position = command.Position.HasValue ? (Positions)command.Position.Value : Positions.NotPrint;
                        printer.Code128(command.Text, position);
                    }
                    break;

                case "code39":
                    if (!string.IsNullOrEmpty(command.Text))
                    {
                        var position = command.Position.HasValue ? (Positions)command.Position.Value : Positions.NotPrint;
                        printer.Code39(command.Text, position);
                    }
                    break;

                case "ean13":
                    if (!string.IsNullOrEmpty(command.Text))
                    {
                        var position = command.Position.HasValue ? (Positions)command.Position.Value : Positions.NotPrint;
                        printer.Ean13(command.Text, position);
                    }
                    break;

                // === CÓDIGOS QR ===
                case "qrcode":
                    if (!string.IsNullOrEmpty(command.Text))
                    {
                        if (command.QrSize.HasValue)
                            printer.QrCode(command.Text, (QrCodeSize)command.QrSize.Value);
                        else
                            printer.QrCode(command.Text);
                    }
                    break;

                // === IMAGEN ===
                case "image":
                    if (!string.IsNullOrEmpty(command.ImagePath) && File.Exists(command.ImagePath))
                    {
                        using (var bitmap = new Bitmap(command.ImagePath))
                        {
                            printer.Image(bitmap);
                        }
                    }
                    break;

                // === UTILIDADES ===
                case "clear":
                    printer.Clear();
                    break;

                case "initializeprint":
                    printer.InitializePrint();
                    break;

                case "autotest":
                    printer.AutoTest();
                    break;

                case "testprinter":
                    printer.TestPrinter();
                    break;

                default:
                    Console.WriteLine($"Comando no reconocido: {command.Type}");
                    break;
            }
        }
    }

    public class Command
    {
        public string Type { get; set; } = "";
        public string? Text { get; set; }
        public bool? State { get; set; }
        public char? Character { get; set; }
        public byte? Height { get; set; }
        public int? Lines { get; set; }
        public int? FontType { get; set; }
        public int? Position { get; set; }
        public int? QrSize { get; set; }
        public string? ImagePath { get; set; }
    }
}
