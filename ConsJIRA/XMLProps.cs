using System;
using System.Xml.Serialization;
using System.IO;

namespace ConsJIRA
{
    //Поля с настройками
    public class XMLPropsFields{
        //Путь к файлу с настройками
        public String XMLPropsFileName = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\config.xml";

        //Путь к файлу вывода
        public String XMLOutputFileName = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\output.xml";

        public String JiraURL = "http://jira.dev.nis.edu.kz/";
        public String JiraUser = "";
        public String JiraPass = "";
    }

    //Класс, для работы с данными
    public class Props{
        public XMLPropsFields Fields;

        public Props(){
            Fields = new XMLPropsFields();
        }

        public void WriteXML(){
            XmlSerializer ser = new XmlSerializer(typeof(XMLPropsFields));
            TextWriter writer = new StreamWriter(Fields.XMLPropsFileName);
            ser.Serialize(writer, Fields);
            writer.Close();
        }

        public void ReadXML(){
            if (CheckXML()){
                XmlSerializer ser = new XmlSerializer(typeof(XMLPropsFields));
                TextReader reader = new StreamReader(Fields.XMLPropsFileName);
                Fields = ser.Deserialize(reader) as XMLPropsFields;
                reader.Close();
            }
        }

        //Проверка наличия файла
        public bool CheckXML()
        {
            return File.Exists(Fields.XMLPropsFileName);
        }
    }
}
