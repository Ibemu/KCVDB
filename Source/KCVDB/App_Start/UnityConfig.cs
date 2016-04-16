using Microsoft.Practices.Unity;
using System.Web.Http;
using Unity.WebApi;
using KCVDB.Services;
using KCVDB.Services.BlobStorage;
using Microsoft.WindowsAzure.Storage;
using System.Configuration;

namespace KCVDB
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();

			// register all your components with the container here
			// it is NOT necessary to register your controllers

			// e.g. container.RegisterType<ITestService, TestService>();

			// ƒT[ƒrƒX‚ğ“o˜^

			// ‚±‚Ìˆ—ŠÂ‹«‚²‚Æ‚ÉU‚è•ª‚¯‚Å‚«‚é‚æ‚¤‚ÈƒNƒ‰ƒXì‚Á‚Ä‚»‚±‚É”C‚¹‚½‚¢
#if DEBUG
			var storageAccount = CloudStorageAccount.DevelopmentStorageAccount;
#else
			var storageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings[Constants.BlobStorage.ApiDataStorageKey].ConnectionString);
#endif
			var blobClient = storageAccount.CreateCloudBlobClient();
			var blobContainer = blobClient.GetContainerReference(Constants.BlobStorage.ApiDataBlobContainerName);

			container.RegisterType<IApiDataWriter>(
				new ContainerControlledLifetimeManager(),
				new InjectionFactory(c => new AzureBlobApiDataWriter(blobContainer)));
            
            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}