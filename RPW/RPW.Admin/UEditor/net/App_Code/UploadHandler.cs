using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

/// <summary>
/// UploadHandler 的摘要说明
/// </summary>
public class UploadHandler : Handler
{
    //记录日志
    private static ILog logger = LogManager.GetLogger(typeof(UploadHandler));

    public UploadConfig UploadConfig { get; private set; }
    public UploadResult Result { get; private set; }

    public UploadHandler(HttpContext context, UploadConfig config)
        : base(context)
    {
        this.UploadConfig = config;
        this.Result = new UploadResult() { State = UploadState.Unknown };
    }

    public override void Process()
    {
        byte[] uploadFileBytes = null;
        string uploadFileName = null;

        if (UploadConfig.Base64)
        {
            uploadFileName = UploadConfig.Base64Filename;
            uploadFileBytes = Convert.FromBase64String(Request[UploadConfig.UploadFieldName]);
        }
        else
        {
            var file = Request.Files[UploadConfig.UploadFieldName];
            uploadFileName = file.FileName;           //分析：拿到文件的上传的文件名

            if (!CheckFileType(uploadFileName))       //分析：检查文件的类型，是否是允许的类型
            {
                Result.State = UploadState.TypeNotAllow;
                WriteResult();
                return;
            }
            if (!CheckFileSize(file.ContentLength))   //分析：拿到文件的上传的大小是否允许
            {
                Result.State = UploadState.SizeLimitExceed;
                WriteResult();
                return;
            }

            uploadFileBytes = new byte[file.ContentLength];   //分析：将文件读取到声明的字节数组中uploadFileBytes
            try
            {
                file.InputStream.Read(uploadFileBytes, 0, file.ContentLength);
            }
            catch (Exception)
            {
                Result.State = UploadState.NetworkError;     //：分析：不能读取到数组中，返回错误结果
                WriteResult();
            }
        }

        Result.OriginFileName = uploadFileName;  //分析：走到了这里，说明，已经将文件读取到了二进制的字节数组中了。接着拿到文件的文件名字，开始执行，上传操作；


        #region 上传到“又拍云”的文件夹中

        DateTime today = DateTime.Today;
        //文具文件的 文件流计算出来md5值，再加上文件扩展名，最终得到要保存到云中文件的名字
        string upYunFileName = RPWCommonts.RPCommonHelper.CalcMD5(uploadFileBytes) + Path.GetExtension(uploadFileName);
        //指定 保存到云中的路径信息
        string upYunFilePath = "/upload/" + today.Year + "/" + today.Month + "/" + today.Day + "/" + uploadFileName;
        //执行上传，上传过程可能会出错，于是，尝试捕获！
        try
        {

            UpYun upyun = new UpYun("xclnet123", "xclnet1234", "xcl123456");
            /// 上传文件
            //upyun.setContentMD5(UpYun.md5_file("..\\..\\test.jpeg"));//发现又拍云使用的是，根据文件名计算md5值
            //我想的是 通过文件的内容，就算出 md5 值
            upyun.setContentMD5(RPWCommonts.RPCommonHelper.CalcMD5(uploadFileBytes));
            //写入到 云，并返回结果
            bool uploadResult = upyun.writeFile(upYunFilePath, uploadFileBytes, true);//路径，哪个字节流
            if (uploadResult)
            {
                //返回用户访问 云中的地址，就是img 标签中的src属性，对应的那个连接...
                Result.Url = "http://xclnet123.b0.upaiyun.com" + upYunFilePath;
                Result.State = UploadState.Success;
            }
            else
            {
                Result.State = UploadState.FileAccessError;
                Result.ErrorMessage = "上传到又拍云失败！";
                logger.Error("上传文件到又拍云失败！");
            }

        }
        catch (Exception e)
        {
            Result.State = UploadState.FileAccessError;
            Result.ErrorMessage = e.Message;
            logger.Error("上传文件失败，发生异常：" + e);
        }
        finally
        {
            WriteResult();
        }
        #endregion

        #region 上传到后台的文件夹中UEditor的upload文件夹
        /*
        var savePath = PathFormatter.Format(uploadFileName, UploadConfig.PathFormat);//路径格式；上传路径的虚拟路径
        var localPath = Server.MapPath(savePath);        //将虚拟路径，进行，转化为物理路径
        try
        {
            if (!Directory.Exists(Path.GetDirectoryName(localPath)))  //这里开始尝试，上传操作了，分析结果：上传到 云，就是在这里进行处理
            {
                Directory.CreateDirectory(Path.GetDirectoryName(localPath));
            }
            File.WriteAllBytes(localPath, uploadFileBytes);
            Result.Url = savePath;
            Result.State = UploadState.Success;
        }
        catch (Exception e)
        {
            Result.State = UploadState.FileAccessError;
            Result.ErrorMessage = e.Message;
        }
        finally
        {
            WriteResult();
        }
         * **********/

        #endregion
    }

    private void WriteResult()
    {
        this.WriteJson(new
        {
            state = GetStateMessage(Result.State),
            url = Result.Url,
            title = Result.OriginFileName,
            original = Result.OriginFileName,
            error = Result.ErrorMessage
        });
    }

    private string GetStateMessage(UploadState state)
    {
        switch (state)
        {
            case UploadState.Success:
                return "SUCCESS";
            case UploadState.FileAccessError:
                return "文件访问出错，请检查写入权限";
            case UploadState.SizeLimitExceed:
                return "文件大小超出服务器限制";
            case UploadState.TypeNotAllow:
                return "不允许的文件格式";
            case UploadState.NetworkError:
                return "网络错误";
        }
        return "未知错误";
    }

    private bool CheckFileType(string filename)
    {
        var fileExtension = Path.GetExtension(filename).ToLower();
        return UploadConfig.AllowExtensions.Select(x => x.ToLower()).Contains(fileExtension);
    }

    private bool CheckFileSize(int size)
    {
        return size < UploadConfig.SizeLimit;
    }
}

public class UploadConfig
{
    /// <summary>
    /// 文件命名规则
    /// </summary>
    public string PathFormat { get; set; }

    /// <summary>
    /// 上传表单域名称
    /// </summary>
    public string UploadFieldName { get; set; }

    /// <summary>
    /// 上传大小限制
    /// </summary>
    public int SizeLimit { get; set; }

    /// <summary>
    /// 上传允许的文件格式
    /// </summary>
    public string[] AllowExtensions { get; set; }

    /// <summary>
    /// 文件是否以 Base64 的形式上传
    /// </summary>
    public bool Base64 { get; set; }

    /// <summary>
    /// Base64 字符串所表示的文件名
    /// </summary>
    public string Base64Filename { get; set; }
}

public class UploadResult
{
    public UploadState State { get; set; }//上传结果
    public string Url { get; set; }//上传地址
    public string OriginFileName { get; set; }//原文件的名字

    public string ErrorMessage { get; set; }//错误信息
}

public enum UploadState
{
    Success = 0,
    SizeLimitExceed = -1,
    TypeNotAllow = -2,
    FileAccessError = -3,
    NetworkError = -4,
    Unknown = 1,
}

