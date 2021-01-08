using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.IO;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using System;
using System.Collections;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace Task_5.Controllers
{
    public class HomeController : Controller
    {

        private const string filePath = null;
        // Specify your bucket region (an example region is shown).  
        private static readonly string bucketName = ConfigurationManager.AppSettings["BucketName"];
        private static readonly RegionEndpoint bucketRegion = RegionEndpoint.USEast1;
        private static readonly string accesskey = ConfigurationManager.AppSettings["AWSAccessKey"];
        private static readonly string secretkey = ConfigurationManager.AppSettings["AWSSecretKey"];
        private static readonly string sessiontoken = ConfigurationManager.AppSettings["AWSSessionToken"];


        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        //[HttpGet]
        //public ActionResult S3Bucket()
        //{

        //    return View();
        //}

        [HttpGet]
        public ActionResult S3Bucket()
        {
            var sessionCredentials = new SessionAWSCredentials(accesskey, secretkey, sessiontoken);
            var s3Client = new AmazonS3Client(sessionCredentials, bucketRegion);


            S3DirectoryInfo di = new S3DirectoryInfo(s3Client, bucketName);
            IS3FileSystemInfo[] files = di.GetFileSystemInfos();

            ArrayList fileNames = new ArrayList();

            foreach (S3FileInfo file in files)
            {
                Debug.WriteLine($"{file.Name}");

                fileNames.Add(file.Name);
            }

            ViewBag.File = fileNames;


            return View();
        }



        [HttpPost]
        public ActionResult S3Bucket(HttpPostedFileBase file)
        {
            var sessionCredentials = new SessionAWSCredentials(accesskey, secretkey, sessiontoken);
            var s3Client = new AmazonS3Client(sessionCredentials, bucketRegion);

            var fileTransferUtility = new TransferUtility(s3Client);
            try
            {
                if (file == null)
                {
                    ViewBag.Message = "Please Select a file";
                }
                else if (file.ContentLength > 0)
                {
                    //var filePath = Path.Combine(Server.MapPath("~/Images"), Path.GetFileName(file.FileName));
                    var fileTransferUtilityRequest = new TransferUtilityUploadRequest
                    {
                        BucketName = bucketName,
                        InputStream = file.InputStream,
                        StorageClass = S3StorageClass.StandardInfrequentAccess,
                        PartSize = 6291456, // 6 MB.  
                        Key = file.FileName,
                        CannedACL = S3CannedACL.PublicRead
                    };
                    fileTransferUtilityRequest.Metadata.Add("param1", "Value1");
                    fileTransferUtilityRequest.Metadata.Add("param2", "Value2");
                    fileTransferUtility.Upload(fileTransferUtilityRequest);
                    fileTransferUtility.Dispose();

                    ViewBag.Message = "File Uploaded Successfully!!";
                }





            }

            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null &&
                    (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId")
                    ||
                    amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                    ViewBag.Message = "Check the provided AWS Credentials.";
                }
                else
                {
                    ViewBag.Message = "Error occurred: " + amazonS3Exception.Message;
                }
            }


            S3DirectoryInfo di = new S3DirectoryInfo(s3Client, bucketName);
            IS3FileSystemInfo[] files = di.GetFileSystemInfos();

            ArrayList fileNames = new ArrayList();

            foreach (S3FileInfo fileInfo in files)
            {
                Debug.WriteLine($"{fileInfo.Name}");

                fileNames.Add(fileInfo.Name);
            }

            ViewBag.File = fileNames;





            return View();
        }



    }
}