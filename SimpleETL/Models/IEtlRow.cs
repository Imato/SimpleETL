using System.Collections.Generic;

namespace Imato.SimpleETL
{
    public interface IEtlRow
    {
        IEtlDataFlow Flow { get; }

        /// <summary>
        /// Get column value by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        object? this[int id] { get; set; }

        /// <summary>
        /// Get column value by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        object? this[string name] { get; set; }

        /// <summary>
        /// Columns count in row
        /// </summary>
        int ColumnsCount { get; }

        /// <summary>
        /// Find column id by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        int ColumnId(string name);

        /// <summary>
        /// Find column name by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        string ColumnName(int id);

        /// <summary>
        /// Get column values
        /// </summary>
        /// <returns></returns>
        IEnumerable<object> GetValues();

        /// <summary>
        /// Has column name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        bool HasColumn(string name);

        /// <summary>
        /// Delete all columns
        /// </summary>
        void Clear();

        /// <summary>
        /// Create new row
        /// </summary>
        /// <returns></returns>
        EtlRow Copy();
    }
}