using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading;
using System.Web.ModelBinding;

namespace WcfServiceHosting
{
    // ПРИМЕЧАНИЕ. Команду "Переименовать" в меню "Рефакторинг" можно использовать для одновременного изменения имени класса "Service1" в коде, SVC-файле и файле конфигурации.
    // ПРИМЕЧАНИЕ. Чтобы запустить клиент проверки WCF для тестирования службы, выберите элементы Service1.svc или Service1.svc.cs в обозревателе решений и начните отладку.
    public class Service1 : IService1
    {
        HostingContext db = new HostingContext();

        public bool AddFile(int userId, string filename, string description, string path)
        {
            try
            {
                CheckIfFileExist(userId, filename);

                UserFile file = new UserFile
                {
                    CurrentUserId = userId,
                    UserFileName = filename,
                    UserFileDescription = description,
                    UserFilePath = path
                };
                db.UserFiles.Add(file);
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public bool CheckIfFileExist(int userId, string fileName)
        {

            // Check if current file has been added already!!!"
            List<UserFile> files = db.UserFiles.Where(x => x.CurrentUserId == userId).ToList()
                                               .Where(z => z.UserFileName.ToLower() == fileName.ToLower()).ToList();

            if (files.Any())
            {
                return true;
            }
            else

                return false;

            //  try
            //  {
            //return true;
            // throw new Exception("Current file !!!");
            //  }

            //  catch
            //{
            //return false;
            //throw new Exception("Current file has been added already!!!");

            // }
        }


        public IEnumerable<CurrentUser> GetAllUsers()
        {
            List<CurrentUser> users = new List<CurrentUser>();
            users = db.CurrentUsers.ToList();
            return users;
        }

        public bool LogIn(string name, string password)
        {

            if (CheckIfUserExist(name))
            {
                return UserAuthorise(name, password);
            }
            else
                return UserAuthorise(name, password);
        }

        public string RegisterUser(string name, string password)
        {
          
            try
            {
                //if (name == "")
                //{
                //throw new FaultException("Fields must be filled");
                //}

                Thread.Sleep(1000);
                if (!CheckIfUserExist(name) )
                {
                    CurrentUser user = new CurrentUser { CurrentUserName = name, CurrentUserPassword = password };
                    db.CurrentUsers.Add(user);
                    db.SaveChanges();
                    return name;
                }
                else
                {
                    return null;
                }

           }
            catch (Exception ex)
           {
                throw new FaultException("Fields must be filled");
          //      throw new Exception("Fields must be filled");
            }
          
            
        }

        public bool CheckIfUserExist(string name)
        {
            List<CurrentUser> user = db.CurrentUsers.Where(x => x.CurrentUserName.ToUpper() == name.ToUpper()).ToList();

            if (user.Any())
            {
                return true;
            }
            else
                return false;
        }

        public bool UserAuthorise(string name, string password)
        {
            List<CurrentUser> user = db.CurrentUsers.Where(x => x.CurrentUserName.ToUpper() == name.ToUpper()).ToList()
                           .Where(z => z.CurrentUserPassword == password).ToList();
            if (user.Any())
            {
                return true;
            }
            else
                return false;
        }

        public void CopyFileToFolder(string sourceFile, string fileName, string hostingPath, string userName)
        {
            //string targetPath = @"C:\Users\Public\TestFolder\SubDir";
            string targetPath = hostingPath + "\\" + userName;
            string destFile = Path.Combine(targetPath, fileName);

            // To copy a folder's contents to a new location:
            // Create a new target folder, if necessary.
            if (!Directory.Exists(targetPath))
            {
                Directory.CreateDirectory(targetPath);
            }

            // To copy a file to another location and 
            // overwrite the destination file if it already exists.

            File.Copy(sourceFile, destFile, true);
        }

        public int GetUserIdByName(string userName)
        {
           CurrentUser user  = db.CurrentUsers.FirstOrDefault(x => x.CurrentUserName == userName);
            return user.CurrentUserId;
        }

        public IEnumerable<UserFilesDTO> GetUserFilesByUserId(int userId)
        {
            List<UserFilesDTO> files = db.UserFiles.Where(x=>x.CurrentUserId == userId)
                                                   .Select(n => new UserFilesDTO
            {
                Id = n.UserFileId.ToString(),
                Description = n.UserFileDescription,
                Name = n.UserFileName
            }).ToList();

            return files;
        }

        public void UpdateFileInfo(UserFilesDTO fileInfo)
        {
           
        }
    }
}
