﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace WcfServiceHosting
{
    // ПРИМЕЧАНИЕ. Команду "Переименовать" в меню "Рефакторинг" можно использовать для одновременного изменения имени интерфейса "IService1" в коде и файле конфигурации.
    [ServiceContract]
    public interface IService1
    {

        [OperationContract]
        string RegisterUser(string name, string password);

        [OperationContract]
        IEnumerable<CurrentUser> GetAllUsers();

        [OperationContract]
        bool AddFile(int userId, string name, string description, string path);

        [OperationContract]
        bool LogIn(string name, string password);

        [OperationContract]
        void CopyFileToFolder(string sourceFile, string fileName, string hostingPath, string userName);

        [OperationContract]
        int GetUserIdByName(string userName);

        [OperationContract]
        IEnumerable<UserFile> GetUserFilesByUserId(int userId);
    }


   
    [DataContract]
    public class UserFile
    {
        [DataMember]
        [Key]      
        public int UserFileId { get; set; }

        [DataMember]       
        public string UserFileName { get; set; }

        [DataMember]       
        public string UserFileDescription { get; set; }

        [DataMember]        
        public string UserFilePath { get; set; }

        [DataMember]
        public int CurrentUserId { get; set; }

        [DataMember]
        public CurrentUser CurrentUser { get; set; }
    }

    [DataContract]
    public class CurrentUser
    {
        [DataMember]
        [Key]
        public int CurrentUserId { get; set; }

        [DataMember]
        [Required]
        public string CurrentUserName { get; set; }

        [DataMember]
        [Required]
        public string CurrentUserPassword { get; set; }

        [DataMember]
        public List<UserFile> UserFile { get; set; }

    }

}
