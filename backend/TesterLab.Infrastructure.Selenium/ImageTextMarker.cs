using Tesseract;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.Fonts;
using System.Diagnostics;
using System.Globalization;

namespace TesterLab.Infrastructure.Selenium;

public class ImageTextMarker
{
    public static void MarkTextInImage(
        string imagePath,
        string outputPath,
        string textToFind,
        string watermarkText = "")
    {
        string tempBase = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

        var psi = new ProcessStartInfo
        {
            FileName = "tesseract",
            Arguments = $"\"{imagePath}\" \"{tempBase}\" -l fra+eng tsv",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(psi)!;

        process.WaitForExit();

        if (process.ExitCode != 0)
        {
            throw new Exception(process.StandardError.ReadToEnd());
        }

        string tsvFile = tempBase + ".tsv";

        using var image = Image.Load(imagePath);

        //----------------------------------------
        // Watermark
        //----------------------------------------

        if (!string.IsNullOrWhiteSpace(watermarkText))
        {
            Font font = SystemFonts.CreateFont("Arial", 72, FontStyle.Bold);

            var options = new RichTextOptions(font)
            {
                Origin = new PointF(image.Width / 2f, image.Height / 2f),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            image.Mutate(ctx =>
                ctx.DrawText(options,
                    watermarkText,
                    Color.Gray.WithAlpha(0.4f)));
        }

        //----------------------------------------
        // Lecture du TSV
        //----------------------------------------

        foreach (string line in File.ReadLines(tsvFile).Skip(1))
        {
            string[] cols = line.Split('\t');

            if (cols.Length < 12)
                continue;

            string word = cols[11];

            if (string.IsNullOrWhiteSpace(word))
                continue;

            if (!word.Contains(textToFind,
                StringComparison.OrdinalIgnoreCase))
                continue;

            float left = float.Parse(cols[6], CultureInfo.InvariantCulture);
            float top = float.Parse(cols[7], CultureInfo.InvariantCulture);
            float width = float.Parse(cols[8], CultureInfo.InvariantCulture);
            float height = float.Parse(cols[9], CultureInfo.InvariantCulture);

            const float padding = 5;

            RectangleF rect = new RectangleF(
                Math.Max(0, left - padding),
                Math.Max(0, top - padding),
                width + padding * 2,
                height + padding * 2);

            image.Mutate(ctx =>
                ctx.Draw(Color.Red, 3, rect));
        }

        image.Save(outputPath);

        File.Delete(tsvFile);
    }
}