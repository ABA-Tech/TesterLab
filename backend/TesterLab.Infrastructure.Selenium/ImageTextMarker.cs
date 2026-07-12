using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Processing;
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
        string tesseractPath = GetTesseractPath();

        EnsureTesseractAvailable(tesseractPath);

        string tempBase = Path.Combine(
            Path.GetTempPath(),
            Guid.NewGuid().ToString());

        RunTesseract(
            tesseractPath,
            imagePath,
            tempBase);

        string tsvFile = tempBase + ".tsv";

        try
        {
            if (!File.Exists(tsvFile))
                throw new Exception("Le fichier TSV OCR n'a pas été généré.");

            using var image = Image.Load(imagePath);

            // -----------------------------
            // Watermark
            // -----------------------------
            if (!string.IsNullOrWhiteSpace(watermarkText))
            {
                // Récupère la première police disponible sur le système (Windows, Linux ou Mac)
                var fontFamily = SystemFonts.Families.FirstOrDefault();
                
                if (fontFamily != null)
                {
                    Font font = fontFamily.CreateFont(72, FontStyle.Bold);

                    var options = new RichTextOptions(font)
                    {
                        Origin = new PointF(
                            image.Width / 2f,
                            image.Height / 2f),

                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    };

                    image.Mutate(ctx =>
                        ctx.DrawText(
                            options,
                            watermarkText,
                            Color.Gray.WithAlpha(0.4f)));
                }
                else
                {
                    Console.WriteLine("Attention: Aucune police installée sur le système pour le filigrane.");
                }
            }

            // -----------------------------
            // Lecture OCR
            // -----------------------------
            List<OcrWord> words = ReadWordsFromTsv(tsvFile);

            // -----------------------------
            // Recherche mot ou phrase
            // -----------------------------
            var matches = FindText(words, textToFind);

            foreach (var match in matches)
            {
                DrawRectangle(image, match);
            }

            image.Save(outputPath);
        }
        finally
        {
            // Garantit que le fichier temporaire est supprimé même si une erreur survient
            if (File.Exists(tsvFile))
            {
                File.Delete(tsvFile);
            }
        }
    }

    private static List<OcrWord> ReadWordsFromTsv(string tsvFile)
    {
        var result = new List<OcrWord>();

        foreach (string line in File.ReadLines(tsvFile).Skip(1))
        {
            string[] cols = line.Split('\t');

            if (cols.Length < 12)
                continue;

            string text = cols[11].Trim();

            if (string.IsNullOrWhiteSpace(text))
                continue;

            result.Add(new OcrWord
            {
                Text = text,
                Left = int.Parse(cols[6], CultureInfo.InvariantCulture),
                Top = int.Parse(cols[7], CultureInfo.InvariantCulture),
                Width = int.Parse(cols[8], CultureInfo.InvariantCulture),
                Height = int.Parse(cols[9], CultureInfo.InvariantCulture),
                Block = cols[2],
                Paragraph = cols[3],
                Line = cols[4]
            });
        }

        return result;
    }

    private static List<List<OcrWord>> FindText(List<OcrWord> words, string search)
    {
        var results = new List<List<OcrWord>>();
        string normalizedSearch = search.Trim().ToLowerInvariant();

        var orderedWords = words
            .OrderBy(x => x.Top)
            .ThenBy(x => x.Left)
            .ToList();

        for (int i = 0; i < orderedWords.Count; i++)
        {
            string current = "";
            var matchedWords = new List<OcrWord>();

            for (int j = i; j < orderedWords.Count; j++)
            {
                current += " " + orderedWords[j].Text;
                matchedWords.Add(orderedWords[j]);

                if (current.Trim().Contains(normalizedSearch, StringComparison.OrdinalIgnoreCase))
                {
                    results.Add(new List<OcrWord>(matchedWords));
                    break;
                }

                if (current.Length > normalizedSearch.Length + 30)
                    break;
            }
        }

        return results;
    }

    private static void DrawRectangle(Image image, List<OcrWord> words)
    {
        // Conservation de ta logique : un grand rectangle autour de la phrase complète
        const float padding = 5;

        float x1 = words.Min(x => x.Left);
        float y1 = words.Min(x => x.Top);
        float x2 = words.Max(x => x.Left + x.Width);
        float y2 = words.Max(x => x.Top + x.Height);

        RectangleF rectangle = new RectangleF(
            x1 - padding,
            y1 - padding,
            (x2 - x1) + padding * 2,
            (y2 - y1) + padding * 2);

        image.Mutate(ctx => ctx.Draw(Color.Red, 3, rectangle));
    }

    private static string GetTesseractPath()
    {
        if (OperatingSystem.IsWindows())
        {
            string[] paths =
            {
                @"C:\Program Files\Tesseract-OCR\tesseract.exe",
                @"C:\Program Files (x86)\Tesseract-OCR\tesseract.exe"
            };

            foreach (var path in paths)
            {
                if (File.Exists(path))
                    return path;
            }

            return "tesseract.exe";
        }

        if (OperatingSystem.IsMacOS())
        {
            string[] paths =
            {
                "/usr/local/bin/tesseract",
                "/opt/homebrew/bin/tesseract"
            };

            foreach (var path in paths)
            {
                if (File.Exists(path))
                    return path;
            }
        }

        // Linux / Docker / Render
        return "/usr/bin/tesseract";
    }

    private static void EnsureTesseractAvailable(string path)
    {
        try
        {
            var psi = new ProcessStartInfo
            {
                FileName = path,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            
            // Utilisation de ArgumentList pour plus de sécurité
            psi.ArgumentList.Add("--version");

            using var process = Process.Start(psi);
            process!.WaitForExit();

            if (process.ExitCode != 0)
                throw new Exception();
        }
        catch
        {
            throw new Exception(
                $"Tesseract OCR introuvable.\n" +
                $"Chemin testé : {path}");
        }
    }

    private static void RunTesseract(
        string tesseractPath,
        string imagePath,
        string outputBase)
    {
        var psi = new ProcessStartInfo
        {
            FileName = tesseractPath,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        // Remplacement de l'interpolation par ArgumentList (protège contre les espaces dans les noms de fichiers)
        psi.ArgumentList.Add(imagePath);
        psi.ArgumentList.Add(outputBase);
        psi.ArgumentList.Add("-l");
        psi.ArgumentList.Add("fra+eng");
        psi.ArgumentList.Add("tsv");

        using var process = Process.Start(psi);

        string error = process!.StandardError.ReadToEnd();
        process.WaitForExit();

        if (process.ExitCode != 0)
        {
            throw new Exception($"Erreur Tesseract : {error}");
        }
    }
}

public class OcrWord
{
    public string Text { get; set; } = "";
    public int Left { get; set; }
    public int Top { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public string Block { get; set; } = "";
    public string Paragraph { get; set; } = "";
    public string Line { get; set; } = "";
}