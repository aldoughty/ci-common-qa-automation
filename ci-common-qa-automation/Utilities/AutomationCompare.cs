using System.Data;

namespace ci_common_qa_automation.Utilities
{
    public static class AutomationCompare
    {
        public static DataTable DataTableDifferences(DataTable source, DataTable target, string? ignoreExtrasIn)
        {
            ignoreExtrasIn = ignoreExtrasIn ?? "";
            //Create Empty Table
            DataTable differences = new("Difference");
            DataTable sourceSingleColumn = new("SourceSingleColumn");
            DataTable targetSingleColumn = new("TargetSingleColumn");
            sourceSingleColumn.Columns.Add("Data", typeof(string));
            targetSingleColumn.Columns.Add("Data", typeof(string));

            //Translate to a single column
            sourceSingleColumn.BeginLoadData();
            foreach (DataRow row in source.Rows)
            {
                string data = "";
                foreach (DataColumn column in source.Columns)
                {
                    data = data + row[column].ToString()!.Trim();
                }
                string[] stuff = new string[1] { data };
                sourceSingleColumn.LoadDataRow(stuff, true);
            }
            sourceSingleColumn.EndLoadData();
            targetSingleColumn.BeginLoadData();
            foreach (DataRow row in target.Rows)
            {
                string data = "";
                foreach (DataColumn column in target.Columns)
                {
                    data = data + row[column].ToString().Trim();
                }
                string[] stuff = new string[1] { data };
                targetSingleColumn.LoadDataRow(stuff, true);
            }
            targetSingleColumn.EndLoadData();

            using (DataSet ds = new())
            {
                //Add tables
                ds.Tables.AddRange(new DataTable[] { sourceSingleColumn.Copy(), targetSingleColumn.Copy() });

                //Get Columns for DataRelation
                DataColumn[] firstColumns = new DataColumn[ds.Tables[0].Columns.Count];

                for (int i = 0; i < firstColumns.Length; i++)
                {
                    firstColumns[i] = ds.Tables[0].Columns[i];
                }

                DataColumn[] secondColumns = new DataColumn[ds.Tables[1].Columns.Count];

                for (int i = 0; i < secondColumns.Length; i++)
                {
                    secondColumns[i] = ds.Tables[1].Columns[i];
                }

                //Create DataRelation
                DataRelation r1 = new(string.Empty, firstColumns, secondColumns, false);
                DataRelation r2 = new(string.Empty, secondColumns, firstColumns, false);

                ds.Relations.Add(r1);
                ds.Relations.Add(r2);

                //Create columns for return table
                differences.Columns.Add("Source/Target", typeof(string));
                for (int i = 0; i < source.Columns.Count; i++)
                {
                    if (source.Columns[i].ColumnName.Equals(target.Columns[i].ColumnName)) { differences.Columns.Add(source.Columns[i].ColumnName, typeof(string)); }
                    else { differences.Columns.Add(source.Columns[i].ColumnName + "_" + target.Columns[i].ColumnName, typeof(string)); }
                }

                //If First Row not in Second, Add to return table.
                differences.BeginLoadData();
                if (!ignoreExtrasIn.ToLower().Equals("source"))
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        DataRow[] childrows = ds.Tables[0].Rows[i].GetChildRows(r1);
                        if (childrows != null && childrows.Length != 0) continue;
                        List<object> row = source.Rows[i].ItemArray.ToList();
                        row.Insert(0, "Source");
                        differences.LoadDataRow(row.ToArray(), true);
                    }
                }

                //If First Row not in Second, Add to return table.
                if (!ignoreExtrasIn.ToLower().Equals("target"))
                {
                    for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                    {
                        DataRow[] childrows = ds.Tables[1].Rows[i].GetChildRows(r2);
                        if (childrows != null && childrows.Length != 0) continue;
                        List<object> row = target.Rows[i].ItemArray.ToList();
                        row.Insert(0, "Target");
                        differences.LoadDataRow(row.ToArray(), true);
                    }
                }
                differences.EndLoadData();
            }
            return differences;
        }
        public static DataTable DataTableDifferencesWithUniqueId(Serilog.Core.Logger logger, DataTable source, DataTable target, int uniqueIdPosition, bool ignoreCase)
        {
            //Create Empty Table
            DataTable differences = new DataTable("Difference");
            if (source.Columns.Count == 0) { logger.Warning("Source Table Columns are empty for Comparison with Key"); return differences; }
            if (target.Columns.Count == 0) { logger.Warning("Target Table Columns are empty for Comparison with Key"); return differences; }
            string sourceKeyColumnName = source.Columns[uniqueIdPosition - 1].ColumnName;
            string targetKeyColumnName = target.Columns[uniqueIdPosition - 1].ColumnName;

            differences.Columns.Add("Source/Target", typeof(string));
            for (int x = 0; x < source.Columns.Count; x++)
            {
                if (source.Columns[x].ColumnName.Equals(target.Columns[x].ColumnName)) { differences.Columns.Add(source.Columns[x].ColumnName, typeof(string)); }
                else { differences.Columns.Add(source.Columns[x].ColumnName + "_" + target.Columns[x].ColumnName, typeof(string)); }
            }
            Dictionary<string, string> sourceHT;
            Dictionary<string, string> targetTT;
            if (ignoreCase)
            {
                StringComparer comparer = StringComparer.OrdinalIgnoreCase;
                sourceHT = new Dictionary<string, string>(comparer);
                targetTT = new Dictionary<string, string>(comparer);
            }
            else
            {
                sourceHT = new Dictionary<string, string>();
                targetTT = new Dictionary<string, string>();
            }
            //Translate to a single column
            //sourceSingleColumn.BeginLoadData();
            foreach (DataRow row in source.Rows)
            {
                string data = "";
                foreach (DataColumn column in source.Columns)
                {
                    if (!column.ColumnName.ToString().Equals(sourceKeyColumnName)) { data = data + row[column].ToString().Trim(); }
                }
                Guid outGuidId;
                Guid.TryParse(row[sourceKeyColumnName].ToString().Trim(), out outGuidId);
                string key;
                if (outGuidId == Guid.Empty) { key = row[sourceKeyColumnName].ToString().Trim(); }
                else { key = outGuidId.ToString("X"); }
                //string key = row[sourceKeyColumnName].ToString().Trim();
                if (sourceHT.Keys.Contains(key)) { logger.Error("Source Key Already Exists: {@Key}", key); }
                sourceHT.Add(key, data);
            }
            foreach (DataRow row in target.Rows)
            {
                string data = "";
                foreach (DataColumn column in target.Columns)
                {
                    if (!column.ColumnName.ToString().Equals(targetKeyColumnName)) { data = data + row[column].ToString().Trim(); }
                }
                Guid outGuidId;
                Guid.TryParse(row[targetKeyColumnName].ToString().Trim(), out outGuidId);
                string key;
                if (outGuidId == Guid.Empty) { key = row[targetKeyColumnName].ToString().Trim(); }
                else { key = outGuidId.ToString("X"); }
                //string key = row[targetKeyColumnName].ToString().Trim();    
                if (targetTT.Keys.Contains(key)) { logger.Error("Target Key Already Exists: {@Key}",key); }
                targetTT.Add(key, data);
            }

            //If First Row not in Second, Add to return table.
            int i = 0;
            differences.BeginLoadData();
            foreach (var key in sourceHT.Keys)
            {
                if (!targetTT.ContainsKey(key))
                {
                    List<object> row = source.Rows[i].ItemArray.ToList();
                    row.Insert(0, "Source");
                    differences.LoadDataRow(row.ToArray(), true);
                }
                else
                {
                    if (ignoreCase)
                    {
                        if (!sourceHT[key].ToString().ToLower().Equals(targetTT[key].ToString().ToLower()))
                        {
                            List<object> row = source.Rows[i].ItemArray.ToList();
                            row.Insert(0, "Source");
                            differences.LoadDataRow(row.ToArray(), true);
                        }
                    }
                    else
                    {
                        if (!sourceHT[key].ToString().Equals(targetTT[key].ToString()))
                        {
                            List<object> row = source.Rows[i].ItemArray.ToList();
                            row.Insert(0, "Source");
                            differences.LoadDataRow(row.ToArray(), true);
                        }
                    }
                }
                i = i + 1;
            }
            i = 0;
            //If First Row not in Second, Add to return table.
            foreach (var key in targetTT.Keys)
            {
                if (!sourceHT.ContainsKey(key))
                {
                    List<object> row = target.Rows[i].ItemArray.ToList();
                    row.Insert(0, "Target");
                    differences.LoadDataRow(row.ToArray(), true);
                }
                else
                {
                    if (ignoreCase)
                    {
                        if (!targetTT[key].ToString().ToLower().Equals(sourceHT[key].ToString().ToLower()))
                        {
                            List<object> row = target.Rows[i].ItemArray.ToList();
                            row.Insert(0, "Target");
                            differences.LoadDataRow(row.ToArray(), true);
                        }
                    }
                    else
                    {
                        if (!targetTT[key].ToString().Equals(sourceHT[key].ToString()))
                        {
                            List<object> row = target.Rows[i].ItemArray.ToList();
                            row.Insert(0, "Target");
                            differences.LoadDataRow(row.ToArray(), true);
                        }
                    }
                }
                i = i + 1;
            }

            return differences;
        }
    }
}
