using System;
using System.Globalization;
using System.IO;
using System.Xml;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace CodeReuser
{
    internal class VisualStudioUtils
    {
        internal static string GetCustomVsoFiledRefName(string fieldRefNameBase, string vsoFiledNamePrefixOverride)
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}.{1}", vsoFiledNamePrefixOverride, fieldRefNameBase);
        }

        internal static string GetAliasFromVsoWorkItemField(string rawFiledValue)
        {
            string assignedTo = string.Empty;

            if (!string.IsNullOrWhiteSpace(rawFiledValue) && rawFiledValue.Contains("<") && rawFiledValue.Contains("@"))
            {
                var parts = rawFiledValue.Split(new char[] { '<', '@', '>' });

                if (parts.Length > 1)
                {
                    assignedTo = parts[1];
                }
            }

            return assignedTo;
        }

        internal static string GetWorkItemFieldValue(WorkItem item, string fieldName)
        {
            object value = null;

            if (item != null && item.Fields.ContainsKey(fieldName))
            {
                value = item.Fields[fieldName];

                if (value != null)
                {
                    if (value.GetType() == typeof(DateTime))
                    {
                        return string.Format(CultureInfo.InvariantCulture, "{0:yyyy-MM-dd}", value);
                    }
                    else
                    {
                        return value.ToString();
                    }
                }
            }

            return string.Empty;
        }
        internal static Wiql CreateWiql(string queryString, out Uri baseUri, out string project)
        {
            Console.WriteLine(queryString);

            using (var xmlReader = XmlReader.Create(new StringReader(queryString)))
            {
                return CreateWiql(xmlReader, out baseUri, out project);
            }
        }

        internal static Wiql CreateWiql(FileInfo queryFile, out Uri baseUri, out string project)
        {
            Console.WriteLine(queryFile.FullName);

            using (var xmlReader = XmlReader.Create(queryFile.FullName))
            {
                return CreateWiql(xmlReader, out baseUri, out project);
            }
        }

        private static Wiql CreateWiql(XmlReader reader, out Uri baseUri, out string project)
        {
            reader.ReadToFollowing("TeamFoundationServer");
            string connection = reader.ReadString();

            if (!string.IsNullOrWhiteSpace(connection))
            {
                baseUri = new Uri(connection);
            }
            else
            {
                baseUri = null;
            }

            reader.ReadToFollowing("TeamProject");
            project = reader.ReadString();

            reader.ReadToFollowing("Wiql");
            string query = reader.ReadString();

            if (!string.IsNullOrWhiteSpace(query))
            {
                return new Wiql() { Query = query };
            }
            else
            {
                return null;
            }
        }

    }
}
