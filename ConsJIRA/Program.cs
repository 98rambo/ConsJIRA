using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Atlassian.Jira;

namespace ConsJIRA
{
    class Program
    {
        private static Jira _jiraConn;
        public static String XMLOutputFileName;
        static void Main(string[] args)
        {
            LoopMenu();
        }
        //Установка соединения
        private static void CreateConnect(){
            Props props = new Props();
            String jiraUser, jiraPass = "";
            if (!props.CheckXML()){
                Console.Write("Добро пожаловать!\nВведите логин от JIRA: ");
                jiraUser = Console.ReadLine();
                Console.Write("Введите пароль: ");
                jiraPass = Console.ReadLine();
                
                props.Fields.JiraPass = jiraPass;
                props.Fields.JiraUser = jiraUser;
                props.WriteXML();
                
                Console.Clear();
            }
            else{
                props.ReadXML();
                jiraUser = props.Fields.JiraUser;
                jiraPass = props.Fields.JiraPass;
                XMLOutputFileName = props.Fields.XMLOutputFileName;
            }
            
            try{
                _jiraConn = Jira.CreateRestClient(props.Fields.JiraURL, jiraUser, jiraPass);
            }
            catch(Exception e){
                Console.WriteLine("Проверьте вводные данные, либо подключение к VPN!\nТекст ошибки" + e.Message);
            }
            finally{
                Console.WriteLine("Подключение прошло успешно!");
            }
        }

        //Показывает список проектов
        static async Task ShowProjects(){
            
            var projects = await _jiraConn.Projects.GetProjectsAsync();

            int keyLength = projects.Select(x => x.Key.Length).Max();
            int nameLength = projects.Select(x => x.Name.Length).Max();

            System.Console.WriteLine("| {0,-" + keyLength + "} | {1,-" + nameLength + "} |\n", "Код", "Наименование проекта");

            foreach (var proj in projects){
                System.Console.WriteLine("| {0,-" + keyLength + "} | {1,-" + nameLength + "} |", proj.Key, proj.Name);
            }
        }


        static async Task WriteFile(){
            Console.WriteLine("Укажите название проекта");
            String ProjectKey = Console.ReadLine();
            Console.WriteLine("Укажите количество версий, на которые нужно сформировать изменения");
            int VersionCount;
            while (!int.TryParse(Console.ReadLine(), out VersionCount))
            {
                Console.WriteLine("Ошибка ввода! Введите целое число");
            }

            //Получение версий проекта
            var Project = await _jiraConn.Projects.GetProjectAsync(ProjectKey);
            var Versions = await Project.GetVersionsAsync();
            Versions = Versions.Select(x => x).OrderByDescending(x => x.ReleasedDate).Take(VersionCount);
            //Issue в проекте
            var Issues = _jiraConn.Issues.Queryable.Select(i => i).Where(i => i.Project  == ProjectKey).OrderByDescending(i => i.Created);

            List<XMLVersion> xmlVersions = new List<XMLVersion>();

            foreach (var Version in Versions){
                var VersionIssues = Issues.Select(i=>i).Where(i => i.FixVersions == Version.Name);
                List<XMLIssue> xmlIssues = new List<XMLIssue>();
                foreach (var Issue in VersionIssues){
                    xmlIssues.Add(new XMLIssue(Issue.Key.ToString(), Issue.Summary, Issue.Assignee, Issue.Reporter));
                }

                xmlVersions.Add(new XMLVersion(Version.Name, xmlIssues));
            }

            XMLOutput xmlOutput = new XMLOutput(ProjectKey, xmlVersions);
            try{
                xmlOutput.WriteXML(XMLOutputFileName);
            }
            catch (Exception e){
                Console.WriteLine(e);
                throw;
            }
            finally{
                Console.Clear();
                Console.WriteLine("Запись в файл прошла успешно! Файл находится в: " + XMLOutputFileName);
            }
        }

        static void ChoiseFolder()
        {
            Console.WriteLine("Укажите каталог, куда будет сохраняться файл output.xml");
            String dir = Console.ReadLine();
            if (Directory.Exists(dir))
            {
                XMLOutputFileName = dir + "\\output.xml";
            }
        }

        static void LoopMenu(){
            CreateConnect();
            ShowProjects().Wait();
            bool Exit = false;
            while (!Exit){
                Console.WriteLine("Выберите вариант действия\n1)Вывести список изменений в файл\n2)Поменять путь для выходного файла\n3)Выйти из программы");
                String UserPick = Console.ReadLine();

                switch (UserPick){
                    case "1":
                        Console.Clear();
                        WriteFile().Wait();
                        break;
                    case "2":
                        Console.Clear();
                        ChoiseFolder();
                        break;
                    default:
                        Exit = true; 
                        break;
                }
                ShowProjects().Wait();
            }
        }


    }
}
