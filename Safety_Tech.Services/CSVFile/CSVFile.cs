using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Safety_Tech.Models.Models;
using System.Drawing;
using System.Drawing.Imaging;
using System.ComponentModel;
using OfficeOpenXml;
using LicenseContext = OfficeOpenXml.LicenseContext;
using ClosedXML.Excel;
using Microsoft.Extensions.Configuration;

namespace Safety_Tech.Services.CSVFile
{
    /// <summary>
    /// Represents a service for processing CSV files containing object detection data.
    /// </summary>
    public class CSVFile : ICSVFile
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IConfiguration _configuration;
        /// <summary>
        /// Initializes a new instance of the <see cref="CSVFile"/> class.
        /// </summary>
        /// <param name="scopeFactory">The service scope factory.</param>
        public CSVFile(IServiceScopeFactory scopeFactory, IConfiguration configuration)
        {
            _scopeFactory = scopeFactory;
            _configuration = configuration;
        }

        /// <summary>
        /// Processes CSV files from the specified source folder and saves the processed data to the specified folder.
        /// </summary>
        /// <param name="sourceFolderPath">The path to the source folder containing CSV files.</param>
        /// <param name="processedFolderPath">The path to the folder where processed files will be saved.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
        /// <returns>The total number of saved object detections.</returns>
        /// <exception cref="ArgumentException">Thrown when source or processed folder paths are invalid.</exception>
        public async Task<int> ProcessCsvFilesAsync(string sourceFolderPath, string processedFolderPath, CancellationToken cancellationToken = default)
        {
            var totalSaved = 0;
            try
            {
                if (string.IsNullOrWhiteSpace(sourceFolderPath)) throw new ArgumentException("Source folder path is required.");
                if (string.IsNullOrWhiteSpace(processedFolderPath)) throw new ArgumentException("Processed folder path is required.");

                if (!Directory.Exists(sourceFolderPath)) return 0;
                Directory.CreateDirectory(processedFolderPath);

                var csvFiles = Directory.EnumerateFiles(sourceFolderPath, "*.*", SearchOption.TopDirectoryOnly)
                            .Where(f => f.EndsWith(".csv", StringComparison.OrdinalIgnoreCase)
                                     || f.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase));



                foreach (var file in csvFiles)
                {
                    var items = await ReadFileAsync(file, cancellationToken);

                    if (items.Count > 0)
                    {
                        try
                        {
                            // Prepare output image folder alongside processed folder
                            var processedImagesFolder = Path.Combine(processedFolderPath, "Images");
                            Directory.CreateDirectory(processedImagesFolder);

                            // Render and update image paths
                            var imagesFolder = _configuration["FolderPaths:ImageFolder"];
                            var wwwRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                            var incidentFolder = Path.Combine(wwwRootPath, "IncidentImages");
                            // Ensure processed folder exists
                            if (!Directory.Exists(incidentFolder))
                            {
                                Directory.CreateDirectory(incidentFolder);
                            }

                            foreach (var item in items)
                            {
                                var imageName = Path.GetFileName(item.Image); // just the file name
                                if (!string.IsNullOrWhiteSpace(imageName))
                                {
                                    var destPath = Path.Combine(incidentFolder, imageName);

                                    // Copy image into wwwroot/IncidentImages (overwrite if exists)
                                    if (!File.Exists(destPath))
                                    {
                                        // if image is coming from another source path
                                        var sourcePath = Path.Combine(imagesFolder, imageName);

                                        if (File.Exists(sourcePath))
                                        {
                                            File.Copy(sourcePath, destPath, true);
                                        }
                                    }

                                    //Store relative path in DB
                                    item.Image = $"/IncidentImages/{imageName}";
                                }
                            }

                            using var scope = _scopeFactory.CreateScope();
                            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDataContext>();
                            await dbContext.Incidents.AddRangeAsync(items, cancellationToken);
                            totalSaved += await dbContext.SaveChangesAsync(cancellationToken);

                            var destination = Path.Combine(processedFolderPath, Path.GetFileName(file));
                            if (File.Exists(destination))
                            {
                                File.Delete(destination);
                            }
                            File.Move(file, destination);
                            return totalSaved;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }
                    }


                }


            }
            catch (Exception ex)
            {

            }

            return totalSaved;
        }

        /// <summary>
        /// Reads a CSV file and converts its content into a list of <see cref="Incidents"/> objects.
        /// </summary>
        /// <param name="filePath">The path to the CSV file.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
        /// <returns>A list of <see cref="Incidents"/> objects.</returns>
        private static async Task<List<Incidents>> ReadFileAsync(string filePath, CancellationToken cancellationToken)
        {
            var results = new List<Incidents>();
            var extension = Path.GetExtension(filePath).ToLower();

            if (extension == ".csv")
            {
                using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                using var reader = new StreamReader(stream, Encoding.UTF8);

                // Read header
                var headerLine = await reader.ReadLineAsync();
                if (headerLine == null) return results;

                while (!reader.EndOfStream)
                {
                    var line = await reader.ReadLineAsync();
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    var columns = line.Split(',');

                    if (columns.Length < 7) continue;

                    int no = int.TryParse(columns[0], out var tempNo) ? tempNo : 0;
                    DateTime timestamp = DateTime.TryParse(columns[3], out var tempDate) ? tempDate : DateTime.MinValue;

                    results.Add(new Incidents
                    {
                        No = no,
                        CameraName = columns[1]?.Trim() ?? string.Empty,
                        CameraId = columns[2]?.Trim() ?? string.Empty,
                        Timestamp = timestamp,
                        TrackId = columns[4]?.Trim() ?? string.Empty,
                        MissingLabels = columns[5]?.Trim() ?? string.Empty,
                        ViolationType = columns[6]?.Trim() ?? string.Empty,

                        Image = columns[7]?.Trim() ?? string.Empty,
                        //XMin = ParseDouble(columns[8]?.Trim()),
                        //YMin = ParseDouble(columns[9]?.Trim()),
                        //XMax = ParseDouble(columns[10]?.Trim()),
                        //YMax = ParseDouble(columns[11]?.Trim()),

                    });
                }
            }
            else if (extension == ".xlsx")
            {

                using var workbook = new XLWorkbook(filePath);
                var worksheet = workbook.Worksheets.First();

                int lastRow = worksheet.LastRowUsed().RowNumber();

                for (int row = 2; row <= lastRow; row++) // assuming row 1 is header
                {
                    int no = int.TryParse(worksheet.Cell(row, 1).GetValue<string>(), out var tempNo) ? tempNo : 0;
                    DateTime timestamp = DateTime.TryParse(worksheet.Cell(row, 4).GetValue<string>(), out var tempDate) ? tempDate : DateTime.MinValue;

                    results.Add(new Incidents
                    {
                        No = no,
                        CameraName = worksheet.Cell(row, 2).GetValue<string>()?.Trim() ?? string.Empty,
                        CameraId = worksheet.Cell(row, 3).GetValue<string>()?.Trim() ?? string.Empty,
                        Timestamp = timestamp,
                        TrackId = worksheet.Cell(row, 5).GetValue<string>()?.Trim() ?? string.Empty,
                        MissingLabels = worksheet.Cell(row, 6).GetValue<string>()?.Trim() ?? string.Empty,
                        ViolationType = worksheet.Cell(row, 7).GetValue<string>()?.Trim() ?? string.Empty,
                        Image = worksheet.Cell(row, 8).GetValue<string>()?.Trim() ?? string.Empty,
                    });
                }

                return results;
            }
            else
            {
                throw new InvalidOperationException("Unsupported file type. Only CSV and XLSX are supported.");
            }

            return results;
        }

        /// <summary>
        /// Draws a bounding box on the image and saves it to the specified output folder.
        /// </summary>
        /// <param name="sourceImagePath">The path to the source image.</param>
        /// <param name="xMin">The minimum x-coordinate of the bounding box.</param>
        /// <param name="yMin">The minimum y-coordinate of the bounding box.</param>
        /// <param name="xMax">The maximum x-coordinate of the bounding box.</param>
        /// <param name="yMax">The maximum y-coordinate of the bounding box.</param>
        /// <param name="outputFolder">The folder where the processed image will be saved.</param>
        /// <returns>The path to the saved image file.</returns>
        private static string DrawBoundingBoxAndSave(string sourceImagePath, double xMin, double yMin, double xMax, double yMax, string outputFolder)
        {
            try
            {
                if (!File.Exists(sourceImagePath)) return string.Empty;
                Directory.CreateDirectory(outputFolder);

                var fileName = Path.GetFileNameWithoutExtension(sourceImagePath);
                var ext = Path.GetExtension(sourceImagePath);
                var destFile = Path.Combine(outputFolder, fileName + "_boxed" + ext);

                using var bmp = new Bitmap(sourceImagePath);
                using var g = Graphics.FromImage(bmp);

                var left = Math.Max(0, Math.Min(bmp.Width - 1, xMin));
                var top = Math.Max(0, Math.Min(bmp.Height - 1, yMin));
                var right = Math.Max(0, Math.Min(bmp.Width - 1, xMax));
                var bottom = Math.Max(0, Math.Min(bmp.Height - 1, yMax));

                var rect = new Rectangle(
                    x: (int)Math.Round(Math.Min(left, right)),
                    y: (int)Math.Round(Math.Min(top, bottom)),
                    width: (int)Math.Round(Math.Abs(right - left)),
                    height: (int)Math.Round(Math.Abs(bottom - top))
                );

                using var pen = new Pen(Color.Red, 3);
                g.DrawRectangle(pen, rect);

                var format = GetImageFormatFromExtension(ext);
                if (format != null)
                {
                    bmp.Save(destFile, format);
                }
                else
                {
                    bmp.Save(destFile);
                }
                // Additionally save to wwwroot/processImage for web access
                try
                {
                    var wwwrootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "processImage");
                    Directory.CreateDirectory(wwwrootPath);

                    var wwwrootFile = Path.Combine(wwwrootPath, fileName + "_boxed" + ext);

                    if (format != null)
                        bmp.Save(wwwrootFile, format);
                    else
                        bmp.Save(wwwrootFile);

                }
                catch
                {
                    // ignore wwwroot save errors, keep your existing flow safe
                }

                return destFile;
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the image format based on the file extension.
        /// </summary>
        /// <param name="extension">The file extension.</param>
        /// <returns>The corresponding <see cref="ImageFormat"/> or null if not recognized.</returns>
        private static ImageFormat? GetImageFormatFromExtension(string extension)
        {
            switch ((extension ?? string.Empty).ToLowerInvariant())
            {
                case ".jpg":
                case ".jpeg":
                    return ImageFormat.Jpeg;
                case ".png":
                    return ImageFormat.Png;
                case ".bmp":
                    return ImageFormat.Bmp;
                case ".gif":
                    return ImageFormat.Gif;
                case ".tiff":
                case ".tif":
                    return ImageFormat.Tiff;
                default:
                    return null;
            }
        }

        /// <summary>
        /// Splits a CSV line into an array of strings.
        /// </summary>
        /// <param name="line">The CSV line to split.</param>
        /// <returns>An array of strings representing the columns.</returns>
        private static string[] SplitCsvLine(string line)
        {
            // Simple CSV split; assumes values don't include commas inside quotes.
            return line.Split(',');
        }

        /// <summary>
        /// Parses a string into a double value.
        /// </summary>
        /// <param name="value">The string value to parse.</param>
        /// <returns>The parsed double value or 0 if parsing fails.</returns>
        private static double ParseDouble(string value)
        {
            if (double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var d))
            {
                return d;
            }
            if (double.TryParse(value, out d))
            {
                return d;
            }
            return 0d;
        }
    }
}
