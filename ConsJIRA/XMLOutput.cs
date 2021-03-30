using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using Atlassian.Jira;

namespace ConsJIRA
{
    // В проекте присутствуют различные версии
    public class XMLProject{
        public String ProjectCode;
        public List<XMLVersion> Versions;
        public XMLProject(String projectCode, List<XMLVersion> versions)
        {
            ProjectCode = projectCode;
            Versions = versions;
        }
        public XMLProject(){ }
    }
    // В каждой версии есть список выполненных задач
    public class XMLVersion{
        public String Ver;
        public List<XMLIssue> Issues;
        public XMLVersion(String ver, List<XMLIssue> issues)
        {
            Ver = ver;
            Issues = issues;
        }
        public XMLVersion() { }
    }
    public class XMLIssue{
        public String Key;
        public String Summary;
        public String Assignee;
        public String Reporter;

        public XMLIssue(String key, String summary, String assignee, String reporter)
        {
            Key = key;
            Summary = summary;
            Assignee = assignee;
            Reporter = reporter;
        }
        public XMLIssue() { }
    }

    //Класс, для работы с данными
    public class XMLOutput{
        public XMLProject Fields;

        public XMLOutput(String projectCode, List<XMLVersion> versions)
        {
            Fields = new XMLProject(projectCode, versions);
        }
        public void WriteXML(String FileName)
        {
            XmlSerializer ser = new XmlSerializer(typeof(XMLProject));
            TextWriter writer = new StreamWriter(FileName);
            ser.Serialize(writer, Fields);
            writer.Close();
        }
    }
}
