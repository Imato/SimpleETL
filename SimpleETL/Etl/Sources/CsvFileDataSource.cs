using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Imato.SimpleETL
{
    public class CsvFileDataSource : DataSource
    {
        private readonly string _filesFolder;
        private List<string>? _columns;
        private readonly string _delimeter;
        private readonly bool _hasColumnNames;
        private DateTime _lastDate;
        private readonly Func<string, bool>? _fileSearchPredicate;
        private readonly string? _columnsSearchRe;

        public string LastFileName { get; private set; }

        public CsvFileDataSource(string filesFolder,
            string delimiter,
            bool hasColumnNames,
            DateTime lastDate,
            IList<string>? columns = null,
            Func<string, bool>? fileSearchPredicate = null,
            string? columnsSearchRe = null)
        {
            _filesFolder = filesFolder ?? throw new ArgumentNullException(nameof(filesFolder));
            _delimeter = delimiter ?? throw new ArgumentNullException(nameof(delimiter));

            _hasColumnNames = hasColumnNames;
            _lastDate = lastDate;
            _fileSearchPredicate = fileSearchPredicate;
            _columnsSearchRe = columnsSearchRe;

            if (columns != null)
                _columns = new List<string>(columns);
        }

        public override IEnumerable<IEtlRow> GetData()
        {
            Log($"Get CSV files data source from folder {_filesFolder}");

            if (!Directory.Exists(_filesFolder))
                Error($"Directory {_filesFolder} not exists!");
            else
            {
                var files = Directory.EnumerateFiles(_filesFolder, "*.*", SearchOption.AllDirectories)
                    .Where(f => File.GetLastWriteTime(f) > _lastDate);

                if (_fileSearchPredicate != null)
                    files = files.Where(f => _fileSearchPredicate(f));

                files = files.OrderBy(f => File.GetLastWriteTime(f));

                string[] fColumns = null;

                foreach (var file in files)
                {
                    Log($"Open file {file}");

                    foreach (var fRow in File.ReadAllLines(file))
                    {
                        if (!string.IsNullOrEmpty(fRow))
                        {
                            fColumns = null;

                            if (_columnsSearchRe != null)
                            {
                                var re = new Regex(_columnsSearchRe);
                                var matches = re.Matches(fRow);
                                if (matches != null && matches.Count > 0)
                                {
                                    fColumns = new string[matches[0].Groups.Count - 1];

                                    for (int i = 1; i < matches[0].Groups.Count; i++)
                                    {
                                        fColumns[i - 1] = matches[0].Groups[i].Value;
                                    }
                                }
                            }
                            else
                                fColumns = fRow.Split(_delimeter);

                            if (_columns == null)
                            {
                                _columns = new List<string>();

                                for (int i = 0; i < fColumns.Length; i++)
                                {
                                    var cName = fColumns[i].Trim();

                                    if (_hasColumnNames && !string.IsNullOrEmpty(cName))
                                        _columns.Add(cName);
                                    else
                                        _columns.Add($"Column_{i}");
                                }

                                if (_hasColumnNames)
                                    continue;
                            }

                            if (fColumns != null && fColumns.Length > 0)
                            {
                                var row = CreateRow();

                                for (int i = 0; i < _columns.Count; i++)
                                {
                                    if (i < fColumns.Length)
                                        row[_columns[i]] = fColumns[i];
                                    else
                                        row[_columns[i]] = null;
                                }

                                // Add file name
                                row["FileName"] = file;

                                RowAffected++;

                                yield return row;
                            }
                        }
                    }

                    Log($"Proceed  {RowAffected} rows from file {file}");
                    LastFileName = file;
                    LastDate = File.GetLastWriteTime(file);

                    RowAffected = 0;
                }
            }
        }
    }
}