using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Threading;

namespace CodeReuser
{
    public class VisualStudioWorkItemHelper
    {

        public Uri TeamUri { get; private set; }

        public string ProjectName { get; private set; }

        private readonly Uri DefaultTeamUri = new Uri("https://msazure.visualstudio.com/DefaultCollection");

        public WorkItemTrackingHttpClient VssWorkItemTrackingHttpClient { get; private set; }

        private Wiql wiqlInstance = null;

        private FileInfo wiqFile;

        public VisualStudioWorkItemHelper(string query)
        {
            var project = string.Empty;
            Uri uri = null;

            this.wiqlInstance = VisualStudioUtils.CreateWiql(query, out uri, out project);

            this.TeamUri = uri ?? DefaultTeamUri;
            this.ProjectName = project;

            this.VssWorkItemTrackingHttpClient = VisualStudioHttpClientPool.GetVssHttpClient<WorkItemTrackingHttpClient>(this.TeamUri);
        }

        public VisualStudioWorkItemHelper(FileInfo wiqfile)
        {
            string currnetExecutableDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            this.wiqFile = wiqfile;

            if (!this.wiqFile.Exists)
            {
                throw new FileNotFoundException("Failed to load Query file.", this.wiqFile.FullName);
            }

            var project = string.Empty;
            Uri uri = null;

            this.wiqlInstance = VisualStudioUtils.CreateWiql(this.wiqFile, out uri, out project);

            this.TeamUri = uri ?? DefaultTeamUri;
            this.ProjectName = project;

            this.VssWorkItemTrackingHttpClient = VisualStudioHttpClientPool.GetVssHttpClient<WorkItemTrackingHttpClient>(this.TeamUri);
        }

        public IEnumerable<WorkItem> QueryWorkItemSummary(IEnumerable<string> fileds = null, WorkItemExpand expandOption = WorkItemExpand.All)
        {
            string[] columnSequence = null;
            return QueryWorkItemSummary(out columnSequence, fileds, expandOption);
        }

        public IEnumerable<WorkItem> QueryWorkItemSummary(out string[] columnSequence, IEnumerable<string> fileds = null, WorkItemExpand expandOption = WorkItemExpand.All)
        {
            columnSequence = new string[] { };
            var workItems = Enumerable.Empty<WorkItem>();

            if (this.wiqlInstance != null)
            {
                var result = this.VssWorkItemTrackingHttpClient.QueryByWiqlAsync(this.wiqlInstance, this.ProjectName).Result;


                Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "Found {0} record(s) from VSO.", result.WorkItems.Count()));

                if (result.WorkItems.Count() > 0)
                {
                    var desiredColumns = result.Columns.Select(c => c.ReferenceName).ToList();

                    // If any asked fields missing in the result columns, need expend the fields for further query
                    if (fileds != null)
                    {
                        var additionalColumns = fileds.Except(result.Columns.Select(c => c.ReferenceName)).ToList();

                        if (additionalColumns.Count > 0)
                        {
                            desiredColumns.AddRange(additionalColumns);
                            expandOption = WorkItemExpand.Fields;
                        }
                    }

                    columnSequence = desiredColumns.ToArray();

                    //// Note that if the field is not set with any value, the workItems returned value will not contain the field even
                    //// Fixing this with filling in empty string
                    workItems = this.VssWorkItemTrackingHttpClient.GetWorkItemsAsync(result.WorkItems.Select(w => w.Id),
                        expandOption == WorkItemExpand.None ? (fileds == null ? result.Columns.Select(c => c.ReferenceName) : result.Columns.Where(c => fileds.Contains(c.ReferenceName)).Select(c => c.ReferenceName))
                        : (expandOption == WorkItemExpand.Fields ? fileds : null), expand: expandOption).Result;

                    foreach (var wi in workItems)
                    {
                        var missingFields = desiredColumns.Except(wi.Fields.Select(w => w.Key));

                        foreach (var missingField in missingFields)
                        {
                            wi.Fields.Add(missingField, string.Empty);
                        }
                    }

                    //Sorting the result
                    var sortColumns = result.SortColumns.ToArray();
                    IOrderedEnumerable<WorkItem> orderedResult = null;

                    if (sortColumns.Length > 0)
                    {
                        for (int i = 0; i < sortColumns.Length; i++)
                        {
                            var sorting = sortColumns[i];

                            if (i == 0)
                            {
                                if (sorting.Descending)
                                {
                                    orderedResult = workItems.OrderByDescending(wi => wi.Fields.GetValueOrDefault(sorting.Field.ReferenceName, string.Empty));
                                }
                                else
                                {
                                    orderedResult = workItems.OrderBy(wi => wi.Fields.GetValueOrDefault(sorting.Field.ReferenceName, string.Empty));
                                }
                            }
                            else
                            {
                                if (sorting.Descending)
                                {
                                    orderedResult = orderedResult.ThenByDescending(wi => wi.Fields.GetValueOrDefault(sorting.Field.ReferenceName, string.Empty));
                                }
                                else
                                {
                                    orderedResult = orderedResult.ThenBy(wi => wi.Fields.GetValueOrDefault(sorting.Field.ReferenceName, string.Empty));
                                }
                            }
                        }

                        workItems = orderedResult;
                    }
                }
            }

            return workItems;
        }

        public IEnumerable<WorkItem> QueryWorkItems(out string[] columnSequence, IEnumerable<string> fileds = null, string vsoFiledNamePrefixOverride = "System")
        {
            //// If the given fields is null, honor the fields specified in the wiql string
            //// If the given fileds is not null, pick the given fileds that specified in the wiql string
            var workItems = QueryWorkItemSummary(out columnSequence, fileds, WorkItemExpand.None);

            if (workItems != null)
            {
                foreach (var item in workItems)
                {
                    var itemViewingUrl = string.Format(CultureInfo.InvariantCulture, "{0}/{1}/_workitems?id={2}", this.TeamUri, this.ProjectName, item.Id);
                    item.Fields.Add(VisualStudioUtils.GetCustomVsoFiledRefName("Url", vsoFiledNamePrefixOverride), itemViewingUrl);

                    var itemId = item.Id.HasValue ? item.Id.ToString() : string.Empty;
                    item.Fields.Add(VisualStudioUtils.GetCustomVsoFiledRefName("Id", vsoFiledNamePrefixOverride), itemId);

                    var assignedTo = VisualStudioUtils.GetAliasFromVsoWorkItemField(VisualStudioUtils.GetWorkItemFieldValue(item, "System.AssignedTo"));
                    item.Fields.Add(VisualStudioUtils.GetCustomVsoFiledRefName("AssignedTo", vsoFiledNamePrefixOverride), assignedTo);
                }
            }

            return workItems;
        }
    }
}
