using System;
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
        [WebInvoke(
            Method = "POST", 
            UriTemplate = "User/Register",
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        string RegisterUser(string name, string password);

        [OperationContract]
        [WebInvoke(
            Method = "POST",
            UriTemplate = "File/Copy",
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        void CopyFileToFolder(string sourceFile, string fileName, string hostingPath, string userName);

        [OperationContract]
        [WebGet(UriTemplate = "AllUsers", ResponseFormat = WebMessageFormat.Json)]
        IEnumerable<CurrentUser> GetAllUsers();

        [OperationContract]
        [WebInvoke(
            Method = "POST",
            UriTemplate = "File/Add",
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        bool AddFile(int userId, string name, string description, string path);

        [OperationContract]
        [WebGet(
            UriTemplate = "User/LogIn/{name}/{password}",
            ResponseFormat = WebMessageFormat.Json)]      
        bool LogIn(string name, string password);

        [OperationContract]
        [WebGet(
            UriTemplate = "UserIdByName/{userName}",
            ResponseFormat = WebMessageFormat.Json)]
        int GetUserIdByName(string userName);

        [OperationContract]
        [WebGet(
            UriTemplate = "UserFilesByUserId/{userId}",
            ResponseFormat =WebMessageFormat.Json)]
        IEnumerable<UserFilesDTO> GetUserFilesByUserId(string userId);

        [OperationContract]
        [WebInvoke(UriTemplate = "FileInfo/Update",Method ="POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        bool UpdateFileInfo(UserFilesDTO fileInfo, string hostingPath);

        [OperationContract]
        [WebInvoke(
            UriTemplate = "File/Delete/{fileName}/{userName}",
            Method = "Delete",
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        string DeleteFileByName(string fileName, string userName);
    }

    [DataContract]
    public class UserFilesDTO
    {
        [DataMember]
        public string Id { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Description { get; set; }
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
       // public string _message { get; set; }
        
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
