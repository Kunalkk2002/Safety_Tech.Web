using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Safety_Tech.Services.CSVFile
{
    public interface ICSVFile
    {
        Task<int> ProcessCsvFilesAsync(string sourceFolderPath, string processedFolderPath, CancellationToken cancellationToken = default);
    }
}
