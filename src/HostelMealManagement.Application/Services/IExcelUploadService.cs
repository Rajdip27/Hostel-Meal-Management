using ExcelDataReader;
using HostelMealManagement.Application.Attributes;
using System.Data;
using System.Reflection;

namespace HostelMealManagement.Application.Services;
/// <summary>
/// Provides functionality to read data from an Excel file stream and map each row to an instance of a specified model
/// type.
/// </summary>
/// <remarks>The mapping assumes that Excel columns correspond to the properties of the model type. Only public
/// properties with setters are populated. The caller is responsible for disposing the provided stream.</remarks>
public interface IExcelUploadService
{
    /// <summary>
    /// Reads data from an Excel file stream and maps each row to an instance of the specified model type.
    /// </summary>
    /// <remarks>The mapping assumes that the Excel columns correspond to the properties of the model type.
    /// Only public properties with setters are populated. The caller is responsible for disposing the provided
    /// stream.</remarks>
    /// <typeparam name="T">The type of model to map each Excel row to. Must have a parameterless constructor.</typeparam>
    /// <param name="fileStream">The stream containing the Excel file to read. The stream must be readable and positioned at the beginning of the
    /// file.</param>
    /// <returns>A list of model instances representing the rows in the Excel file. Returns an empty list if the file contains no
    /// data.</returns>
    List<T> ReadExcelToModel<T>(Stream fileStream) where T : new();
}
/// <summary>
/// Provides functionality to read Excel files and map their contents to strongly-typed model objects.
/// </summary>
/// <remarks>ExcelUploadService enables importing data from Excel files by converting worksheet rows into
/// instances of a specified model type. The service uses property mapping based on column names, supporting custom
/// mapping via the ExcelColumnAttribute. This class is typically used in scenarios where bulk data import from Excel is
/// required, such as administrative dashboards or data migration tools.</remarks>
public class ExcelUploadService : IExcelUploadService
{
    // Reads an Excel file from the provided stream and maps its data to a list of model objects of type T.
    public List<T> ReadExcelToModel<T>(Stream fileStream) where T : new()
    {
        try
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            using (var reader = ExcelReaderFactory.CreateReader(fileStream))
            {
                // Configure the reader to use the first row as headers
                var result = reader.AsDataSet(new ExcelDataSetConfiguration
                {
                    ConfigureDataTable = _ => new ExcelDataTableConfiguration
                    {
                        UseHeaderRow = true
                    }
                });
                /// Get the first table (worksheet)
                var table = result.Tables[0];
                /// Get the properties of T
                var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                var list = new List<T>();
                /// Iterate through each row in the DataTable
                foreach (DataRow row in table.Rows)
                {
                    // Create a new instance of T
                    var obj = new T();

                    foreach (var prop in properties)
                    {
                        // Get attribute if exists
                        var attr = prop.GetCustomAttribute<ExcelColumnAttribute>();
                        string columnName = attr?.ColumnName ?? prop.Name;

                        if (table.Columns.Contains(columnName) && row[columnName] != DBNull.Value)
                        {
                            try
                            {
                                /// Convert the value to the property type and set it
                                var safeValue = Convert.ChangeType(row[columnName], Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
                                prop.SetValue(obj, safeValue);
                            }
                            catch(Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                                // Optional: Log conversion errors
                            }
                        }
                    }
                    list.Add(obj);
                }
                return list;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }
}

