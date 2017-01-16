using System;
using System.Collections.Generic;
using System.Data.Entity;
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
            CurrentUser user =  db.CurrentUsers.Find(userId);
            try
            {


                if (!CheckIfFileExist(userId, filename))
                {
                    UserFile file = new UserFile
                    {
                        CurrentUserId = userId,
                        UserFileName = filename,
                        UserFileDescription = description,
                        UserFilePath = path,
                        CurrentUser = user

                    };
                    db.UserFiles.Add(file);
                    db.SaveChanges();
                }
                
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
                
                return null;
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

        public bool UpdateFileInfo(UserFilesDTO fileInfo, string hostingPath)
        {
           try
            {
               
                int fileId = Convert.ToInt32(fileInfo.Id);
                UserFile oldFileInfo = db.UserFiles.FirstOrDefault(x => x.UserFileId == fileId);
                oldFileInfo.CurrentUser = db.CurrentUsers.Find(oldFileInfo.CurrentUserId);
                string userName = oldFileInfo.CurrentUser.CurrentUserName;

                if (oldFileInfo.UserFileName != fileInfo.Name)
                {
                    string newFilePath = hostingPath + "\\" +userName +"\\" + fileInfo.Name;
                    File.Move(oldFileInfo.UserFilePath, newFilePath);
                    oldFileInfo.UserFilePath = newFilePath;
                }

                oldFileInfo.UserFileName = fileInfo.Name;
                oldFileInfo.UserFileDescription = fileInfo.Description;
               
                db.Entry(oldFileInfo).State = EntityState.Modified;
                db.SaveChanges();

                return true;
          }
            catch(Exception ex)
            {
                return false;
              
            }
            
        }

        public string DeleteFileByName(string fileName, string userName)
        {
            int userId =  GetUserIdByName(userName);
            UserFile file = db.UserFiles.Where(x => x.CurrentUserId == userId).FirstOrDefault(z=>z.UserFileName.ToUpper() == fileName.ToUpper());

            try
            {
                string filePath = file.UserFilePath;

                if (System.IO.File.Exists(filePath))
                {
                    File.Delete(filePath);

                    db.UserFiles.Remove(file);
                    db.SaveChanges();
                    return "File deleted. Success";

                }
                else
                {
                    return "Invalid file name: file does not exist";
                }
            }
            catch
            {
                
                return "Invalid file name: file does not exist";
            }
           

           
        }
    }
}
