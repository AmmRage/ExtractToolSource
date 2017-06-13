using Ravioli.ArchiveInterface.Scanning;

    using System;
    using System.Collections.Generic;
    using System.Threading;

namespace Ravioli.Scanner
{
    public class ScanStatistics
    {
        private Dictionary<PerceivedType, PerceivedTypeStatsItem> perceivedTypeStats = new Dictionary<PerceivedType, PerceivedTypeStatsItem>();
        private Dictionary<string, ScanStatisticsItem> stats = new Dictionary<string, ScanStatisticsItem>();

        public event EventHandler Cleared;

        public event EventHandler Updated;

        public void Clear()
        {
            this.stats.Clear();
            this.perceivedTypeStats.Clear();
            if (this.Cleared != null)
            {
                this.Cleared(this, EventArgs.Empty);
            }
        }

        public bool ContainsStatisticsItem(ScanStatisticsItem item)
        {
            return this.stats.ContainsKey(item.TypeName);
        }

        public void DeleteStatisticsItem(ScanStatisticsItem item)
        {
            if (this.stats.ContainsKey(item.TypeName))
            {
                if (this.perceivedTypeStats.ContainsKey(item.PerceivedType))
                {
                    PerceivedTypeStatsItem item2 = this.perceivedTypeStats[item.PerceivedType];
                    item2.Count -= this.stats[item.TypeName].Count;
                    item2.Bytes -= this.stats[item.TypeName].Bytes;
                    if (item2.Count == 0)
                    {
                        this.perceivedTypeStats.Remove(item.PerceivedType);
                    }
                }
                this.stats.Remove(item.TypeName);
                if (this.Updated != null)
                {
                    this.Updated(this, EventArgs.Empty);
                }
            }
        }

        public PerceivedTypeStatsItem[] GetPerceivedTypeStatsItems()
        {
            PerceivedTypeStatsItem[] itemArray = new PerceivedTypeStatsItem[this.perceivedTypeStats.Count];
            int num = 0;
            foreach (KeyValuePair<PerceivedType, PerceivedTypeStatsItem> pair in this.perceivedTypeStats)
            {
                itemArray[num++] = new PerceivedTypeStatsItem(pair.Value.PerceivedType, pair.Value.Count, pair.Value.Bytes);
            }
            return itemArray;
        }

        public ScanStatisticsItem[] GetStatisticsItems()
        {
            ScanStatisticsItem[] itemArray = new ScanStatisticsItem[this.stats.Count];
            int num = 0;
            foreach (KeyValuePair<string, ScanStatisticsItem> pair in this.stats)
            {
                itemArray[num++] = new ScanStatisticsItem(pair.Key, pair.Value.PerceivedType, pair.Value.Count, pair.Value.Bytes);
            }
            return itemArray;
        }

        public void SetStatisticsItem(ScanStatisticsItem item)
        {
            if (!this.stats.ContainsKey(item.TypeName))
            {
                this.stats.Add(item.TypeName, new ScanStatisticsItem(item.TypeName, item.PerceivedType, item.Count, item.Bytes));
            }
            else
            {
                ScanStatisticsItem item2 = this.stats[item.TypeName];
                item2.Count = item.Count;
                item2.Bytes = item.Bytes;
            }
            if (!this.perceivedTypeStats.ContainsKey(item.PerceivedType))
            {
                this.perceivedTypeStats.Add(item.PerceivedType, new PerceivedTypeStatsItem(item.PerceivedType, item.Count, item.Bytes));
            }
            else
            {
                PerceivedTypeStatsItem item3 = this.perceivedTypeStats[item.PerceivedType];
                item3.Count += item.Count;
                item3.Bytes += item.Bytes;
            }
            if (this.Updated != null)
            {
                this.Updated(this, EventArgs.Empty);
            }
        }

        public void Update(ScanDirectoryEntry entry)
        {
            this.UpdateInternal(entry);
            if (this.Updated != null)
            {
                this.Updated(this, EventArgs.Empty);
            }
        }

        public void Update(ScanDirectoryEntry[] entries)
        {
            foreach (ScanDirectoryEntry entry in entries)
            {
                this.UpdateInternal(entry);
            }
            if (this.Updated != null)
            {
                this.Updated(this, EventArgs.Empty);
            }
        }

        private void UpdateInternal(ScanDirectoryEntry entry)
        {
            if (!this.stats.ContainsKey(entry.TypeName))
            {
                this.stats.Add(entry.TypeName, new ScanStatisticsItem(entry.TypeName, entry.PerceivedType, 1, entry.Length));
            }
            else
            {
                ScanStatisticsItem item = this.stats[entry.TypeName];
                item.Count++;
                item.Bytes += entry.Length;
            }
            if (!this.perceivedTypeStats.ContainsKey(entry.PerceivedType))
            {
                this.perceivedTypeStats.Add(entry.PerceivedType, new PerceivedTypeStatsItem(entry.PerceivedType, 1, entry.Length));
            }
            else
            {
                PerceivedTypeStatsItem item2 = this.perceivedTypeStats[entry.PerceivedType];
                item2.Count++;
                item2.Bytes += entry.Length;
            }
        }
    }
}

