using Qiniu.Storage;
using Qiniu.Storage.Model;
using Qiniu.Util;
using System.IO;

namespace Gemstar.BSPMS.Common.Tools
{
    public static class QiniuHelper
    {
        public static string GetUpToken(string bucket, string access_key, string secret_key,bool isView=true)
        {
            Mac mac = new Mac(access_key, secret_key);

            PutPolicy putPolicy = new PutPolicy();
            putPolicy.Scope = bucket;
            putPolicy.SetExpires(3600);
            putPolicy.DeleteAfterDays = 1000;
            string token = Auth.createUploadToken(putPolicy, mac);
            if (!isView)
                return token;
            return "{\"uptoken\":\"" + token + "\"}";
        }

        public static bool ImgDelete(string bucket, string filename, string access_key, string secret_key)
        {
            Mac mac = new Mac(access_key, secret_key);
            BucketManager bm = new BucketManager(mac);
            var result = bm.delete(bucket, filename);
            return result.ResponseInfo.isOk();
        }
        public static void UploadFile(Stream stream,string key,string token)
        {
            UploadManager um = new UploadManager();
            stream.Seek(0, SeekOrigin.Begin);
            var option = UploadOptions.defaultOptions();
            um.uploadStream(stream, key, token, option, null);
        }
        public static void UploadFile(string bucket, string access_key, string secret_key,byte[] data,string saveKey)
        {
            Mac mac = new Mac(access_key, secret_key);
            PutPolicy putPolicy = new PutPolicy();
            putPolicy.Scope = bucket;
            putPolicy.SetExpires(3600);
            putPolicy.DeleteAfterDays = 100;
            string token = Auth.createUploadToken(putPolicy, mac);
            FormUploader fu = new FormUploader();
            var option = UploadOptions.defaultOptions();
            fu.uploadData(data, saveKey, token, option, null);
        }
        public static bool isexistfile(string bucket, string filename, string access_key, string secret_key)
        {
            Mac mac = new Mac(access_key, secret_key);
            BucketManager bm = new BucketManager(mac);
            StatResult result = bm.stat(bucket, filename);
            return result.ResponseInfo.isOk();
        }


    }
}

