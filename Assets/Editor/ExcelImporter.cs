using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Table;

public class ExcelImporter
{
    public struct Table {
        readonly ExcelTable _source;
        
        public int RowCount { get { return _source.Address.Rows + (_source.ShowTotal ? -2 : -1); } }
        public int ColumnCount { get { return _source.Columns.Count; } }

        // TODO: Investigate why built-in ExcelTableColumnCollection[name] fails.
        readonly Dictionary<string, ExcelTableColumn> _columnLookup;        

        public Table(ExcelTable source) {
            _source = source;
            _columnLookup = new Dictionary<string, ExcelTableColumn>(source.Columns.Count);
            foreach (var column in source.Columns) {
                _columnLookup.Add(column.Name, column);
            }
        }

        public T GetValue<T>(int row, string columnName) {

            if (row > RowCount) {
                Debug.LogError($"Tried to access row {row} of table {_source.Name} which has only {RowCount} rows.");
                return default;
            }

            if (!_columnLookup.TryGetValue(columnName, out ExcelTableColumn column)) {
                Debug.LogError($"Cannot find column named {columnName} in table {_source.Name}.");
                string info = "Valid columns are...";
                foreach (var label in _source.Columns) {
                    info += " " + label.Name;
                    if (label.Name == columnName) info += "!";
                }
                Debug.Log(info);
                return default;
            }

            var start = _source.Address.Start;
            return _source.WorkSheet.GetValue<T>(start.Row + row, start.Column + column.Position);
        }
    }

    public struct Range {
        readonly ExcelNamedRange _source;
        readonly ExcelWorksheet _sheet;

        public int RowCount { get { return _source.Rows; } }
        public int ColumnCount { get { return _source.Columns; } }

        public Range(ExcelWorksheet sheet, ExcelNamedRange source) {
            _sheet = sheet;
            _source = source;
        }

        public T GetValue<T>(int row, int column) {
            var start = _source.Start;            
            return _sheet.GetValue<T>(start.Row + row-1, start.Column + column-1);
        }
    }

    ExcelPackage _package;

    Dictionary<string, Table> _tables = new Dictionary<string, Table>();
    Dictionary<string, Range> _namedRanges = new Dictionary<string, Range>();

    public ExcelImporter(string filePath) {
        var path = Application.dataPath + "/" + filePath;
        try {
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
                _package = new ExcelPackage();
                _package.Load(stream);
            }

            var workbook = _package.Workbook;

            foreach (var sheet in workbook.Worksheets) {
                
                foreach (var table in sheet.Tables) {
                    _tables.Add(table.Name, new Table(table));
                }               
            }

            foreach (var range in workbook.Names) {
                var sheetName = range.FullAddress.Substring(0, range.FullAddress.IndexOf('!'));
                if (sheetName.StartsWith("'")) {
                    sheetName = sheetName.Substring(1, sheetName.Length - 2);
                }
                var sheet = workbook.Worksheets[sheetName];
                
                _namedRanges.Add(range.Name, new Range(sheet, range));
            }
        } catch (System.Exception e) {
            Debug.LogError($"Error importing Excel file from {path}: {e.Message}");
        }
    }

    public bool TryGetTable(string name, out Table table) {
        if (_tables.TryGetValue(name, out table)) {
            return true;
        }
        return false;
    }

    public bool TryGetNamedRange(string name, out Range range) {
        if (_namedRanges.TryGetValue(name, out range)) {
            return true;
        }
        return false;
    }
}
