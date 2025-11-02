using OpenQA.Selenium;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

public static class ScreenshotAnnotator
{
    public static string CaptureWithHighlight(IWebDriver driver, IWebElement element, string filePath, string filename)
    {
        // Capture d’écran complète
        Screenshot screenshot = ((ITakesScreenshot)driver).GetScreenshot();

        // Crée le dossier si besoin
        Directory.CreateDirectory(Path.GetDirectoryName(filePath));

        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        var fileName = $"{filename}_{timestamp}.png";
        string tempPath = Path.Combine(Path.GetDirectoryName(filePath), fileName);
        screenshot.SaveAsFile(tempPath);

        // Infos sur l’élément
        var location = element.Location;
        var size = element.Size;

        // Dessiner le cadre et la coche
        using (var img = Image.FromFile(tempPath))
        using (var graphics = Graphics.FromImage(img))
        {
            using (var pen = new Pen(Color.Red, 3))
            {
                graphics.DrawRectangle(pen, location.X, location.Y, size.Width, size.Height);
            }

            // (Optionnel) coche “✅”
            using (Font font = new Font("Segoe UI Emoji", 28))
            {
                graphics.DrawString("✅", font, Brushes.Green, location.X + size.Width - 25, location.Y - 30);
            }

            img.Save(filePath, ImageFormat.Png);
        }

        // Nettoyage du fichier temporaire
        File.Delete(tempPath);
        return $"/screenshots/{fileName}";
    }
}
